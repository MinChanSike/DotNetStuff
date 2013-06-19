﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

using More;

namespace More.Net
{
    public class NfsServerProgramOptions : CLParser
    {
        public CLGenericArgument<IPAddress> listenIPAddress;

        public CLGenericArgument<UInt16> debugListenPort;

        public CLGenericArgument<UInt16> npcListenPort;

        public CLSwitch dontLogCallsInternally;
        public CLSwitch dontLogCallsToConsole;
        public CLSwitch dontLogFileSystemCallsToConsole;
        public CLSwitch dontLogWarningsToConsole;
        public CLSwitch dontLogSerializationPerformanceToConsole;
        public CLSwitch dontLogServerEventsToConsole;
        public CLSwitch dontLogNpcEventsToConsole;

        public NfsServerProgramOptions()
        {
            listenIPAddress = new CLGenericArgument<IPAddress>(IPAddress.Parse, 'l', "Listen IP Address");
            listenIPAddress.SetDefault(IPAddress.Parse("0.0.0.0"));
            Add(listenIPAddress);


            //
            // Debug Server
            //
            debugListenPort = new CLGenericArgument<UInt16>(UInt16.Parse, 'd', "DebugListenPort", "The TCP port that the debug server will be listening to (If no port is specified, the debug server will not be running)");
            Add(debugListenPort);

            //
            // Npc Server
            //
            npcListenPort = new CLGenericArgument<UInt16>(UInt16.Parse, 'n', "NpcListenPort", "The TCP port that the NPC server will be listening to (If no port is specified, the NPC server will not be running)");
            Add(npcListenPort);


            dontLogCallsInternally = new CLSwitch("DontLogCallsInternally", "Log all RPC calls internally to be dumped upon request");
            Add(dontLogCallsInternally);

            dontLogCallsToConsole = new CLSwitch("DontLogCallsToConsole", "Log all RPC calls to the Console");
            Add(dontLogCallsToConsole);

            dontLogFileSystemCallsToConsole = new CLSwitch("DontLogFileSystemCalls", "Log all file system calls to Console");
            Add(dontLogFileSystemCallsToConsole);

            dontLogWarningsToConsole = new CLSwitch("DontLogWarningsToConsole", "Log all warnings to the Console");
            Add(dontLogWarningsToConsole);

            dontLogSerializationPerformanceToConsole = new CLSwitch("DontLogSerializationPerfToConsole", "Log serialization performance to Console");
            Add(dontLogSerializationPerformanceToConsole);

            dontLogServerEventsToConsole = new CLSwitch("DontLogServerEventsToConsole", "Log server events to Console");
            Add(dontLogServerEventsToConsole);

            dontLogNpcEventsToConsole = new CLSwitch("DontLogNpcEventsToConsole", "Log NPC events to Console");
            Add(dontLogNpcEventsToConsole);
        }

        public override void PrintUsageHeader()
        {
            Console.WriteLine("Usage: NfsServer.exe [options] <share-directory> <share-name>");
        }
    }

    class Program
    {
        static void Main(String[] args)
        {
            NfsServerProgramOptions options = new NfsServerProgramOptions();
            List<String> nonOptionArguments = options.Parse(args);


            if (nonOptionArguments.Count != 2)
            {
                options.ErrorAndUsage("Expected 2 non-option arguments but got '{0}'", nonOptionArguments.Count);
                return;
            }

            String shareDirectory = nonOptionArguments[0];
            String shareName = nonOptionArguments[1];
            
            //
            // Options not exposed via command line yet
            //
            Int32 mountListenPort = 59733;
            Int32 backlog = 4;

            UInt32 readSizeMax = 65536;
            UInt32 suggestedReadSizeMultiple = 4096;

            //
            // Listen IP Address
            //
            IPAddress listenIPAddress = options.listenIPAddress.ArgValue;

            //
            // Debug Server
            //
            IPEndPoint debugServerEndPoint = !options.debugListenPort.set ? null :
                new IPEndPoint(listenIPAddress, options.debugListenPort.ArgValue);

            //
            // Npc Server
            //
            IPEndPoint npcServerEndPoint = !options.npcListenPort.set ? null :
                new IPEndPoint(listenIPAddress, options.npcListenPort.ArgValue);
                    
            //
            // Logging Options
            //
            NfsServerLog.storePerformance                   = !options.dontLogCallsInternally.set;
            NfsServerLog.sharedFileSystemLogger             = options.dontLogFileSystemCallsToConsole.set          ? null : Console.Out;
            NfsServerLog.rpcCallLogger                      = options.dontLogCallsToConsole.set                    ? null : Console.Out;
            NfsServerLog.warningLogger                      = options.dontLogWarningsToConsole.set                 ? null : Console.Out;
            NfsServerLog.npcEventsLogger                    = options.dontLogNpcEventsToConsole.set                ? null : Console.Out;

            RpcPerformanceLog.rpcMessageSerializationLogger = options.dontLogSerializationPerformanceToConsole.set ? null : Console.Out;


            TextWriter selectServerEventLog                 = options.dontLogServerEventsToConsole.set             ? null : Console.Out;





            //
            // Permissions
            //
            Nfs3Procedure.ModeFlags defaultDirectoryPermissions =
                Nfs3Procedure.ModeFlags.OtherExecute | Nfs3Procedure.ModeFlags.OtherWrite | Nfs3Procedure.ModeFlags.OtherRead |
                Nfs3Procedure.ModeFlags.GroupExecute | Nfs3Procedure.ModeFlags.GroupWrite | Nfs3Procedure.ModeFlags.GroupRead |
                Nfs3Procedure.ModeFlags.OwnerExecute | Nfs3Procedure.ModeFlags.OwnerWrite | Nfs3Procedure.ModeFlags.OwnerRead;
            /*Nfs3Procedure.ModeFlags.SaveSwappedText | Nfs3Procedure.ModeFlags.SetUidOnExec | Nfs3Procedure.ModeFlags.SetGidOnExec;*/
            Nfs3Procedure.ModeFlags defaultFilePermissions =
                Nfs3Procedure.ModeFlags.OtherExecute | Nfs3Procedure.ModeFlags.OtherWrite | Nfs3Procedure.ModeFlags.OtherRead |
                Nfs3Procedure.ModeFlags.GroupExecute | Nfs3Procedure.ModeFlags.GroupWrite | Nfs3Procedure.ModeFlags.GroupRead |
                Nfs3Procedure.ModeFlags.OwnerExecute | Nfs3Procedure.ModeFlags.OwnerWrite | Nfs3Procedure.ModeFlags.OwnerRead;
            /*Nfs3Procedure.ModeFlags.SaveSwappedText | Nfs3Procedure.ModeFlags.SetUidOnExec | Nfs3Procedure.ModeFlags.SetGidOnExec;*/
            IPermissions permissions = new DumbPermissions(defaultDirectoryPermissions, defaultFilePermissions);

            ShareDirectory[] shareDirectories = new ShareDirectory[] {
                new ShareDirectory(shareDirectory, shareName),
            };

            IFileIDsAndHandlesDictionary fileIDDictionary = new FreeStackFileIDDictionary(512, 512, 4096, 1024);

            SharedFileSystem sharedFileSystem = new SharedFileSystem(fileIDDictionary, permissions, shareDirectories);

            try
            {
                new RpcServicesManager().Run(
                    selectServerEventLog,
                    debugServerEndPoint,
                    npcServerEndPoint,
                    listenIPAddress,
                    backlog, sharedFileSystem,
                    Ports.PortMap, mountListenPort, Ports.Nfs,
                    readSizeMax, suggestedReadSizeMultiple);
            }
            finally
            {
                if (NfsServerLog.storePerformance == true) NfsServerLog.PrintNfsCalls(Console.Out);
            }
        }
    }
}

