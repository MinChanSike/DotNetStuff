﻿using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using More;

namespace More.Net
{
    public static class HiddenServerStreamHandlerExtension
    {
        public static void HandleBytes(this FrameAndHeartbeatReceiverFilter filter,
            FrameAndHeartbeatTestCommandHandler testHandler, Byte[] bytes)
        {
            filter.FilterTo(testHandler.HandleData, testHandler.HandleHeartbeat, bytes, 0, bytes.Length);
        }
    }

    public class FrameAndHeartbeatTestCommandHandler : IDataHandler
    {
        Int32 expectedHeartbeats;
        readonly List<Byte[]> nextExpectedFrames = new List<Byte[]>();


        public void AssertNoMoreExpectedCommandsOrHeartbeats()
        {
            if (expectedHeartbeats > 0)
                Assert.Fail("You are still expected {0} heartbeats", expectedHeartbeats);
            if (nextExpectedFrames.Count > 0)
                Assert.Fail("You expected all commads to be handled already but there's still {0} left", nextExpectedFrames.Count);
        }
        public void ExpectHeartbeats(Byte heartbeats)
        {
            expectedHeartbeats += heartbeats;
        }
        public void ExpectFrames(params Byte[][] expectedFrames)
        {
            for (int i = 0; i < expectedFrames.Length; i++)
            {
                nextExpectedFrames.Add(expectedFrames[i]);
            }
        }

        public void HandleHeartbeat()
        {
            if (expectedHeartbeats <= 0)
                Assert.Fail("You got a heartbeat but weren't expecting one");
            expectedHeartbeats--;
        }
        public void HandleData(Byte[] data)
        {
            HandleData(data, 0, data.Length);
        }
        public void HandleData(byte[] data, int offset, int length)
        {
            if (nextExpectedFrames.Count <= 0)
                Assert.Fail("You got a command but weren't expecting one");

            Byte[] expectedFrame = nextExpectedFrames[0];
            nextExpectedFrames.RemoveAt(0);

            for (int i = 0; i < expectedFrame.Length; i++)
            {
                if (expectedFrame[i] != data[i + offset])
                {
                    Assert.Fail("The command that was received was not expected");
                }
            }
        }
        public void Dispose()
        {
        }
    }
    [TestClass]
    public class TestFrameAndHeartbeatHandler
    {
        [TestMethod]
        public void TestHeartbeat()
        {
            FrameAndHeartbeatTestCommandHandler testCommandHandler = new FrameAndHeartbeatTestCommandHandler();
            FrameAndHeartbeatReceiverFilter filter = new FrameAndHeartbeatReceiverFilter();

            testCommandHandler.ExpectHeartbeats(1);
            filter.HandleBytes(testCommandHandler, FrameAndHeartbeatProtocol.HeartBeatPacket);
            testCommandHandler.AssertNoMoreExpectedCommandsOrHeartbeats();

            testCommandHandler.ExpectHeartbeats(2);
            filter.HandleBytes(testCommandHandler, new Byte[] { FrameAndHeartbeatProtocol.Heartbeat, FrameAndHeartbeatProtocol.Heartbeat });
            testCommandHandler.AssertNoMoreExpectedCommandsOrHeartbeats();
        }
        [TestMethod]
        public void TestMultipleHandleBytesPerCommand()
        {
            FrameAndHeartbeatTestCommandHandler testCommandHandler = new FrameAndHeartbeatTestCommandHandler();
            FrameAndHeartbeatReceiverFilter filter = new FrameAndHeartbeatReceiverFilter();

            filter.HandleBytes(testCommandHandler, new Byte[] { 0, 0, 1 });
            testCommandHandler.ExpectFrames(new Byte[] { 0x72 });
            filter.HandleBytes(testCommandHandler, new Byte[] { 0x72 });
            testCommandHandler.AssertNoMoreExpectedCommandsOrHeartbeats();

            filter.HandleBytes(testCommandHandler, new Byte[] { 0, 0 });
            filter.HandleBytes(testCommandHandler, new Byte[] { 2 });
            filter.HandleBytes(testCommandHandler, new Byte[] { 0x84 });
            testCommandHandler.ExpectFrames(new Byte[] { 0x84, 0xF0 });
            filter.HandleBytes(testCommandHandler, new Byte[] { 0xF0 });
            testCommandHandler.AssertNoMoreExpectedCommandsOrHeartbeats();

            filter.HandleBytes(testCommandHandler, new Byte[] { 0 });
            filter.HandleBytes(testCommandHandler, new Byte[] { 0, 10 });
            filter.HandleBytes(testCommandHandler, new Byte[] { 0x12, 0x34, 0x56, 0x78 });
            filter.HandleBytes(testCommandHandler, new Byte[] { 0x9A, 0xBC });
            testCommandHandler.ExpectFrames(new Byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0, 0x12, 0x34 });
            filter.HandleBytes(testCommandHandler, new Byte[] { 0xDE, 0xF0, 0x12, 0x34 });
            testCommandHandler.AssertNoMoreExpectedCommandsOrHeartbeats();
        }
        [TestMethod]
        public void TestMultipleCommandsPerHandleBytes()
        {
            FrameAndHeartbeatTestCommandHandler testCommandHandler = new FrameAndHeartbeatTestCommandHandler();
            FrameAndHeartbeatReceiverFilter filter = new FrameAndHeartbeatReceiverFilter();

            testCommandHandler.ExpectHeartbeats(3);
            testCommandHandler.ExpectFrames(new Byte[] { 0xA4 }, new Byte[] { 0x73, 0xF3, 0x29, 0x44 });
            filter.HandleBytes(testCommandHandler, new Byte[] { FrameAndHeartbeatProtocol.Heartbeat, 0, 0, 1, 0xA4, FrameAndHeartbeatProtocol.Heartbeat, 0, 0, 4, 0x73, 0xF3, 0x29, 0x44, FrameAndHeartbeatProtocol.Heartbeat });
            testCommandHandler.AssertNoMoreExpectedCommandsOrHeartbeats();
        }
        [TestMethod]
        public void TestOverlappingHandleBytePerCommand()
        {
            FrameAndHeartbeatTestCommandHandler testCommandHandler = new FrameAndHeartbeatTestCommandHandler();
            FrameAndHeartbeatReceiverFilter filter = new FrameAndHeartbeatReceiverFilter();

            testCommandHandler.ExpectHeartbeats(1);
            testCommandHandler.ExpectFrames(new Byte[] { 0xA4 });
            filter.HandleBytes(testCommandHandler, new Byte[] { FrameAndHeartbeatProtocol.Heartbeat, 0, 0, 1, 0xA4, 0 });
            testCommandHandler.AssertNoMoreExpectedCommandsOrHeartbeats();

            testCommandHandler.ExpectFrames(new Byte[] { 0x73, 0xF3, 0x29, 0x44 });
            filter.HandleBytes(testCommandHandler, new Byte[] { 0, 4, 0x73, 0xF3, 0x29, 0x44, 0, 0 });
            testCommandHandler.AssertNoMoreExpectedCommandsOrHeartbeats();

            testCommandHandler.ExpectFrames(new Byte[] { 0x43, 0xAB, 0x71 });
            filter.HandleBytes(testCommandHandler, new Byte[] { 3, 0x43, 0xAB, 0x71, 0, 0, 1 });
            testCommandHandler.AssertNoMoreExpectedCommandsOrHeartbeats();

            testCommandHandler.ExpectHeartbeats(2);
            testCommandHandler.ExpectFrames(new Byte[] { 0xF0 }, new Byte[] { }, new Byte[] { 0x12, 0x34 });
            filter.HandleBytes(testCommandHandler, new Byte[] { 0xF0, 0, 0, 0, FrameAndHeartbeatProtocol.Heartbeat, 0, 0, 2, 0x12, 0x34, FrameAndHeartbeatProtocol.Heartbeat });
            testCommandHandler.AssertNoMoreExpectedCommandsOrHeartbeats();
        }
    }
}