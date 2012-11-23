﻿using System;
using System.Net.Sockets;
using System.Net;

namespace Marler.NetworkTools
{
    public class RpcTcpClientConnection : IDisposable
    {
        public const Int32 lowPrivelegedPort  = 900;
        public const Int32 highPrivelegedPort = 1010;

        public readonly TcpSocket socket;
        public readonly RpcProgramHeader programHeader;

        private RpcCredentials credentials;
        private RpcVerifier verifier;

        UInt32 nextTransactionID;

        public RpcTcpClientConnection(TcpSocket socket, RpcProgramHeader programHeader,
            RpcCredentials credentials, RpcVerifier verifier)
        {
            this.socket = socket;
            this.programHeader = programHeader;

            this.credentials = credentials;
            this.verifier = verifier;

            this.nextTransactionID = 0;
        }
        public void CallBlockingTcp(RpcProcedure procedure, ByteBuffer buffer)
        {
            UInt32 transmissionID = nextTransactionID++;

            RpcMessage callMessage = new RpcMessage(transmissionID, new RpcCall(programHeader,
                procedure.procedureNumber, credentials, verifier));
            callMessage.SendTcp(socket, buffer, procedure.requestSerializer);

            Int32 contentOffset, contentMaxOffset;
            RpcMessage replyMessage = new RpcMessage(socket, buffer, out contentOffset, out contentMaxOffset);

            if (replyMessage.messageType != RpcMessageType.Reply)
                throw new InvalidOperationException(String.Format("Received an Rpc call from '{0}' but expected an rpc reply", socket.RemoteEndPoint));

            if (replyMessage.transmissionID != transmissionID)
                throw new InvalidOperationException(String.Format("Expected reply with transmission id {0} but got {1}", transmissionID, replyMessage.transmissionID));

            RpcReply reply = replyMessage.reply;
            RpcCallFailedException.VerifySuccessfulReply(callMessage.call, reply);

            Int32 offset = procedure.responseSerializer.Deserialize(buffer.array, contentOffset, contentMaxOffset);

            if (offset != contentMaxOffset)
                throw new InvalidOperationException(String.Format("Deserialization of rpc message '{0}' as the following '{1}' resulted in an offset of {2}, but the record had {3} bytes",
                    reply.ToNiceString(), procedure.responseSerializer.ToNiceString(), offset, contentMaxOffset));
        }
        public void BindToPrivelegedPort()
        {
            Boolean success = false;
            int localMountSocketPort = lowPrivelegedPort;
            for (localMountSocketPort = lowPrivelegedPort; localMountSocketPort < highPrivelegedPort; localMountSocketPort++)
            {
                try
                {
                    socket.Bind(new IPEndPoint(IPAddress.Any, localMountSocketPort));
                    success = true;
                    break;
                }
                catch (SocketException e)
                {
                    Console.WriteLine("[Warning] Failed to bind to port {0}: {1}", localMountSocketPort, e.Message);
                }
            }

            if (!success)
            {
                //Console.WriteLine("[FatalError] Could not bind to any priveleged ports");
                throw new InvalidOperationException(String.Format("Could not bind to any priveleged ports"));
            }

            Console.WriteLine("Successfully bound Socket to priveleged local port {0}", localMountSocketPort);
        }
        public void Dispose()
        {
            if (socket != null)
            {
                if (socket.Connected)
                {
                    try { socket.Shutdown(SocketShutdown.Both); }
                    catch (Exception) { }
                }
                socket.Close();
            }
        }

    }
}