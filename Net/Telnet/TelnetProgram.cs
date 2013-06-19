﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using More;

namespace More.Net
{
    public class TelnetWindowSize
    {
        public readonly UInt16 width, height;
        public TelnetWindowSize(UInt16 width, UInt16 height)
        {
            this.width = width;
            this.height = height;
        }

        public static TelnetWindowSize Parse(String str)
        {
            Int32 xIndex = str.IndexOf('x');

            if (xIndex <= 0)
            {
                throw new FormatException(String.Format("Could not parse '{0}' as a telnet window size, {1}",
                    str, (xIndex == 0) ? "expected a number before 'x'" : "could not find the 'x' character"));
            }
            if (xIndex >= str.Length - 1)
            {
                throw new FormatException(String.Format("Could not parse '{0}' as a telnet window size, expected a number after 'x'", str));
            }

            UInt16 width = UInt16.Parse(str.Remove(xIndex));
            UInt16 height = UInt16.Parse(str.Substring(xIndex + 1));

            return new TelnetWindowSize(width, height);
        }
    }
    public class TelnetOptions : CLParser
    {
        public readonly CLGenericArgument<UInt16> port;
        public readonly CLGenericArgument<TelnetWindowSize> windowSize;
        public readonly CLSwitch wantServerEcho;

        public TelnetOptions()
        {
            port = new CLGenericArgument<UInt16>(UInt16.Parse, 'p', "port");
            port.SetDefault(23);
            Add(port);

            windowSize = new CLGenericArgument<TelnetWindowSize>(TelnetWindowSize.Parse, 'w', "Telnet Window Size");
            windowSize.SetDefault(null);
            Add(windowSize);

            wantServerEcho = new CLSwitch('e', "Want Server To Echo", "Tries to negotiate with the server to make the server echo");
            Add(wantServerEcho);
        }

        public override void PrintUsageHeader()
        {
            Console.WriteLine("Telnet [options] [host]");
        }
    }

    public class TelnetProgram
    {
        static Int32 Main(string[] args)
        {
            TelnetOptions optionsParser = new TelnetOptions();
            List<String> nonOptionArgs = optionsParser.Parse(args);

            if (nonOptionArgs.Count != 1)
            {
                Console.WriteLine("Expected 1 argument but got {0}", nonOptionArgs.Count);
                optionsParser.PrintUsage();
                return -1;
            }

            
            ISocketConnector connector = null;
            EndPoint serverEndPoint = null;
            if (nonOptionArgs.Count == 1)
            {
                serverEndPoint = ConnectorParser.Parse(nonOptionArgs[0], out connector);
            }

            TelnetClient client = new TelnetClient(optionsParser.wantServerEcho.set);


            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (serverEndPoint != null)
            {
                if (connector == null) socket.Connect(serverEndPoint); else connector.Connect(socket, serverEndPoint);
            }

            client.Run(socket);

            return 0;
        }
    }
}