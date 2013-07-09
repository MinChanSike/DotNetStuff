//
// This file is autogenerated using the NpcClientGenerator.
// To use the NpcClientGenerator, there must be an NpcServer running.
// Once the server is running, you can run the  NpcClientGenerator to generate this file.
//
// This file was generated with the following command:
//     NpcClientGenerator.exe localhost 81 --methods --xmlcomments
//
using System;
using System.Net;
using System.Net.Sockets;

using HP.Libraries;
using HP.Libraries.Npc;

namespace TestNamespace
{
    /// <summary>The NpcClient wrapper for Device</summary>
    public class Device : INpcClient
    {
        /// <summary>The NpcClient interface to perform npc calls</summary>
        public INpcClientCaller npcClientCaller;
        /// <summary>The empty constructor</summary>
        public Device(){}
        /// <summary>The constructor with an NpcClient</summary>
        public Device(INpcClientCaller npcClientCaller)
        {
            this.npcClientCaller = npcClientCaller;
        }
        /// <summary>The constructor with an endpoint</summary>
        public Device(EndPoint endPoint)
        {
            this.npcClientCaller = new NpcClient(endPoint);
        }
        /// <summary>The constructor with a socket</summary>
        public Device(Socket socket)
        {
            this.npcClientCaller = new NpcClient(socket);
        }
        /// <summary>SosMethodDefinitions</summary>
        public readonly SosMethodDefinition[] methodDefinitions = new SosMethodDefinition[] {
            new SosMethodDefinition("SetId","Void","UInt32","id"),
            new SosMethodDefinition("GetId","UInt32"),
            new SosMethodDefinition("SetName","Void","String","name"),
            new SosMethodDefinition("GetName","String"),
            new SosMethodDefinition("SetVersion","Void","Byte[]","version"),
            new SosMethodDefinition("GetVersion","Byte[]"),
            new SosMethodDefinition("SaveStrings","Void","String[]","someStrings"),
            new SosMethodDefinition("GetStrings","String[]"),
            new SosMethodDefinition("Overloaded","Void"),
            new SosMethodDefinition("Overloaded","Void","Int32","value"),
            new SosMethodDefinition("Overloaded","Void","Int32","value","Boolean","value2"),
            new SosMethodDefinition("ThrowAnException","Void","String","message"),
            new SosMethodDefinition("GetDate","System.DateTime"),
            new SosMethodDefinition("ToNormalDate","Void"),
            new SosMethodDefinition("OverrideDate","Void","Byte","month","Byte","day","UInt16","year"),
            new SosMethodDefinition("OverrideDate","Void","System.DateTime","date"),
        };
        /// <summary>Retrieve remote object and enum types from server and verify they are correct</summary>
        public void UpdateAndVerifyEnumAndObjectTypes()
        {
            npcClientCaller.UpdateAndVerifyEnumAndObjectTypes();
        }
        /// <summary>Verify that expected method definitions are the same</summary>
        /// <param name="forceMethodUpdateFromServer">True if you would like to update method defintions from server whether or not they have been cached</param>
        public void VerifyMethodDefinitions(Boolean forceMethodUpdateFromServer)
        {
            npcClientCaller.VerifyMethodDefinitions(forceMethodUpdateFromServer, methodDefinitions);
        }
        /// <summary>Dispose the class</summary>
        public void Dispose()
        {
            npcClientCaller.Dispose();
        }
        /// <summary>The SetId method</summary>
        /// <param name="id">The id parameter of type UInt32</param>
        public void SetId(UInt32 id)
        {
            npcClientCaller.Call(typeof(void), "TestNamespace.Device.SetId", id);
        }
        /// <summary>The GetId method</summary>
        /// <returns>Return type is UInt32</returns>
        public UInt32 GetId()
        {
            return (UInt32)npcClientCaller.Call(typeof(UInt32), "TestNamespace.Device.GetId");
        }
        /// <summary>The SetName method</summary>
        /// <param name="name">The name parameter of type String</param>
        public void SetName(String name)
        {
            npcClientCaller.Call(typeof(void), "TestNamespace.Device.SetName", name);
        }
        /// <summary>The GetName method</summary>
        /// <returns>Return type is String</returns>
        public String GetName()
        {
            return (String)npcClientCaller.Call(typeof(String), "TestNamespace.Device.GetName");
        }
        /// <summary>The SetVersion method</summary>
        /// <param name="version">The version parameter of type Byte[]</param>
        public void SetVersion(Byte[] version)
        {
            npcClientCaller.Call(typeof(void), "TestNamespace.Device.SetVersion", version);
        }
        /// <summary>The GetVersion method</summary>
        /// <returns>Return type is Byte[]</returns>
        public Byte[] GetVersion()
        {
            return (Byte[])npcClientCaller.Call(typeof(Byte[]), "TestNamespace.Device.GetVersion");
        }
        /// <summary>The SaveStrings method</summary>
        /// <param name="someStrings">The someStrings parameter of type String[]</param>
        public void SaveStrings(String[] someStrings)
        {
            npcClientCaller.Call(typeof(void), "TestNamespace.Device.SaveStrings", someStrings);
        }
        /// <summary>The GetStrings method</summary>
        /// <returns>Return type is String[]</returns>
        public String[] GetStrings()
        {
            return (String[])npcClientCaller.Call(typeof(String[]), "TestNamespace.Device.GetStrings");
        }
        /// <summary>The Overloaded method</summary>
        public void Overloaded()
        {
            npcClientCaller.Call(typeof(void), "TestNamespace.Device.Overloaded");
        }
        /// <summary>The Overloaded method</summary>
        /// <param name="value">The value parameter of type Int32</param>
        public void Overloaded(Int32 value)
        {
            npcClientCaller.Call(typeof(void), "TestNamespace.Device.Overloaded", value);
        }
        /// <summary>The Overloaded method</summary>
        /// <param name="value">The value parameter of type Int32</param>
        /// <param name="value2">The value2 parameter of type Boolean</param>
        public void Overloaded(Int32 value, Boolean value2)
        {
            npcClientCaller.Call(typeof(void), "TestNamespace.Device.Overloaded", value, value2);
        }
        /// <summary>The ThrowAnException method</summary>
        /// <param name="message">The message parameter of type String</param>
        public void ThrowAnException(String message)
        {
            npcClientCaller.Call(typeof(void), "TestNamespace.Device.ThrowAnException", message);
        }
        /// <summary>The GetDate method</summary>
        /// <returns>Return type is System.DateTime</returns>
        public System.DateTime GetDate()
        {
            return (System.DateTime)npcClientCaller.Call(typeof(System.DateTime), "TestNamespace.Device.GetDate");
        }
        /// <summary>The ToNormalDate method</summary>
        public void ToNormalDate()
        {
            npcClientCaller.Call(typeof(void), "TestNamespace.Device.ToNormalDate");
        }
        /// <summary>The OverrideDate method</summary>
        /// <param name="month">The month parameter of type Byte</param>
        /// <param name="day">The day parameter of type Byte</param>
        /// <param name="year">The year parameter of type UInt16</param>
        public void OverrideDate(Byte month, Byte day, UInt16 year)
        {
            npcClientCaller.Call(typeof(void), "TestNamespace.Device.OverrideDate", month, day, year);
        }
        /// <summary>The OverrideDate method</summary>
        /// <param name="date">The date parameter of type System.DateTime</param>
        public void OverrideDate(System.DateTime date)
        {
            npcClientCaller.Call(typeof(void), "TestNamespace.Device.OverrideDate", date);
        }
    }
}
    /// <summary>The NpcClient wrapper for UsbImpl</summary>
    public class UsbImpl : INpcClient
    {
        /// <summary>The NpcClient interface to perform npc calls</summary>
        public INpcClientCaller npcClientCaller;
        /// <summary>The empty constructor</summary>
        public UsbImpl(){}
        /// <summary>The constructor with an NpcClient</summary>
        public UsbImpl(INpcClientCaller npcClientCaller)
        {
            this.npcClientCaller = npcClientCaller;
        }
        /// <summary>The constructor with an endpoint</summary>
        public UsbImpl(EndPoint endPoint)
        {
            this.npcClientCaller = new NpcClient(endPoint);
        }
        /// <summary>The constructor with a socket</summary>
        public UsbImpl(Socket socket)
        {
            this.npcClientCaller = new NpcClient(socket);
        }
        /// <summary>SosMethodDefinitions</summary>
        public readonly SosMethodDefinition[] methodDefinitions = new SosMethodDefinition[] {
            new SosMethodDefinition("GetDevices","UsbDeviceInformation[]"),
            new SosMethodDefinition("AddUsbDevice","Void","UsbDeviceInformation","device"),
            new SosMethodDefinition("RemoveUsbDevice","Void","UsbDeviceInformation","device"),
            new SosMethodDefinition("ClearAllUsbDevices","Void"),
            new SosMethodDefinition("UsbDeviceIsPresent","Boolean","UsbDeviceInformation","device"),
            new SosMethodDefinition("UsbSwitchGetState","UsbSwitchState"),
            new SosMethodDefinition("UsbSwitchStateChange","Void","UsbSwitchState","state"),
        };
        /// <summary>Retrieve remote object and enum types from server and verify they are correct</summary>
        public void UpdateAndVerifyEnumAndObjectTypes()
        {
            npcClientCaller.UpdateAndVerifyEnumAndObjectTypes();
        }
        /// <summary>Verify that expected method definitions are the same</summary>
        /// <param name="forceMethodUpdateFromServer">True if you would like to update method defintions from server whether or not they have been cached</param>
        public void VerifyMethodDefinitions(Boolean forceMethodUpdateFromServer)
        {
            npcClientCaller.VerifyMethodDefinitions(forceMethodUpdateFromServer, methodDefinitions);
        }
        /// <summary>Dispose the class</summary>
        public void Dispose()
        {
            npcClientCaller.Dispose();
        }
        /// <summary>The GetDevices method</summary>
        /// <returns>Return type is UsbDeviceInformation[]</returns>
        public UsbDeviceInformation[] GetDevices()
        {
            return (UsbDeviceInformation[])npcClientCaller.Call(typeof(UsbDeviceInformation[]), "UsbImpl.GetDevices");
        }
        /// <summary>The AddUsbDevice method</summary>
        /// <param name="device">The device parameter of type UsbDeviceInformation</param>
        public void AddUsbDevice(UsbDeviceInformation device)
        {
            npcClientCaller.Call(typeof(void), "UsbImpl.AddUsbDevice", device);
        }
        /// <summary>The RemoveUsbDevice method</summary>
        /// <param name="device">The device parameter of type UsbDeviceInformation</param>
        public void RemoveUsbDevice(UsbDeviceInformation device)
        {
            npcClientCaller.Call(typeof(void), "UsbImpl.RemoveUsbDevice", device);
        }
        /// <summary>The ClearAllUsbDevices method</summary>
        public void ClearAllUsbDevices()
        {
            npcClientCaller.Call(typeof(void), "UsbImpl.ClearAllUsbDevices");
        }
        /// <summary>The UsbDeviceIsPresent method</summary>
        /// <param name="device">The device parameter of type UsbDeviceInformation</param>
        /// <returns>Return type is Boolean</returns>
        public Boolean UsbDeviceIsPresent(UsbDeviceInformation device)
        {
            return (Boolean)npcClientCaller.Call(typeof(Boolean), "UsbImpl.UsbDeviceIsPresent", device);
        }
        /// <summary>The UsbSwitchGetState method</summary>
        /// <returns>Return type is UsbSwitchState</returns>
        public UsbSwitchState UsbSwitchGetState()
        {
            return (UsbSwitchState)npcClientCaller.Call(typeof(UsbSwitchState), "UsbImpl.UsbSwitchGetState");
        }
        /// <summary>The UsbSwitchStateChange method</summary>
        /// <param name="state">The state parameter of type UsbSwitchState</param>
        public void UsbSwitchStateChange(UsbSwitchState state)
        {
            npcClientCaller.Call(typeof(void), "UsbImpl.UsbSwitchStateChange", state);
        }
    }
    /// <summary>The NpcClient wrapper for StaticClass</summary>
    public class StaticClass : INpcClient
    {
        /// <summary>The NpcClient interface to perform npc calls</summary>
        public INpcClientCaller npcClientCaller;
        /// <summary>The empty constructor</summary>
        public StaticClass(){}
        /// <summary>The constructor with an NpcClient</summary>
        public StaticClass(INpcClientCaller npcClientCaller)
        {
            this.npcClientCaller = npcClientCaller;
        }
        /// <summary>The constructor with an endpoint</summary>
        public StaticClass(EndPoint endPoint)
        {
            this.npcClientCaller = new NpcClient(endPoint);
        }
        /// <summary>The constructor with a socket</summary>
        public StaticClass(Socket socket)
        {
            this.npcClientCaller = new NpcClient(socket);
        }
        /// <summary>SosMethodDefinitions</summary>
        public readonly SosMethodDefinition[] methodDefinitions = new SosMethodDefinition[] {
            new SosMethodDefinition("BlankCall","Void"),
            new SosMethodDefinition("SetInt32","Void","Int32","value"),
            new SosMethodDefinition("GetInt32","Int32"),
        };
        /// <summary>Retrieve remote object and enum types from server and verify they are correct</summary>
        public void UpdateAndVerifyEnumAndObjectTypes()
        {
            npcClientCaller.UpdateAndVerifyEnumAndObjectTypes();
        }
        /// <summary>Verify that expected method definitions are the same</summary>
        /// <param name="forceMethodUpdateFromServer">True if you would like to update method defintions from server whether or not they have been cached</param>
        public void VerifyMethodDefinitions(Boolean forceMethodUpdateFromServer)
        {
            npcClientCaller.VerifyMethodDefinitions(forceMethodUpdateFromServer, methodDefinitions);
        }
        /// <summary>Dispose the class</summary>
        public void Dispose()
        {
            npcClientCaller.Dispose();
        }
        /// <summary>The BlankCall method</summary>
        public void BlankCall()
        {
            npcClientCaller.Call(typeof(void), "StaticClass.BlankCall");
        }
        /// <summary>The SetInt32 method</summary>
        /// <param name="value">The value parameter of type Int32</param>
        public void SetInt32(Int32 value)
        {
            npcClientCaller.Call(typeof(void), "StaticClass.SetInt32", value);
        }
        /// <summary>The GetInt32 method</summary>
        /// <returns>Return type is Int32</returns>
        public Int32 GetInt32()
        {
            return (Int32)npcClientCaller.Call(typeof(Int32), "StaticClass.GetInt32");
        }
    }