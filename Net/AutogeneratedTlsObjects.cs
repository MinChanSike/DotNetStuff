// InputSha 2ED94D0CF7894C453386975086810ACD3301B970
//
// This file was autogenerated using the PdlCodeGenerator
//     GenerationDateTime : 7/22/2013 5:59:59 PM
//
using System;
using System.Text;

using More;

namespace More.Net.TlsCommand
{
    public enum CipherSuite {
        TlsRsaWitNullNull = 0x0000,
        TlsRsaWithNullMd5 = 0x0001,
        TlsRsaWithNullSha = 0x0002,
        TlsRsaExportWithRc4_40Md5 = 0x0003,
        TlsRsaWithAes128CbcSha = 0x002F,
        TlsRsaWithAes256CbcSha = 0x0035,
    }
    public enum CompressionMethod {
        Null = 0,
    }
    public class ProtocolVersion
    {
        public const UInt32 FixedSerializationLength = 2;

        static InstanceSerializer serializer = null;
        public static FixedLengthInstanceSerializer<ProtocolVersion> Serializer
        {
            get
            {
                if(serializer == null) serializer = new InstanceSerializer();
                return serializer;
            }
        }

        class InstanceSerializer : FixedLengthInstanceSerializer<ProtocolVersion>
        {
            public InstanceSerializer() {}
            public override UInt32 FixedSerializationLength() { return ProtocolVersion.FixedSerializationLength; }
            public override void FixedLengthSerialize(Byte[] bytes, UInt32 offset, ProtocolVersion instance)
            {
                bytes[offset] = instance.MajorVersion;
                offset += 1;
                bytes[offset] = instance.MinorVersion;
                offset += 1;
            }
            public override ProtocolVersion FixedLengthDeserialize(Byte[] bytes, UInt32 offset)
            {
                return new ProtocolVersion (
                    bytes[offset + 0], // MajorVersion
                    bytes[offset + 1] // MinorVersion
                );
            }
            public override void DataString(ProtocolVersion instance, StringBuilder builder)
            {
                builder.Append("ProtocolVersion:{");
                builder.Append(instance.MajorVersion);
                builder.Append(',');
                builder.Append(instance.MinorVersion);
                builder.Append("}");
            }
            public override void DataSmallString(ProtocolVersion instance, StringBuilder builder)
            {
                builder.Append("ProtocolVersion:{");
                builder.Append(instance.MajorVersion);
                builder.Append(',');
                builder.Append(instance.MinorVersion);
                builder.Append("}");
            }
        }

        public Byte MajorVersion;
        public Byte MinorVersion;
        private ProtocolVersion() { }
        public ProtocolVersion(Byte MajorVersion, Byte MinorVersion)
        {
            this.MajorVersion = MajorVersion;
            this.MinorVersion = MinorVersion;
        }
        public FixedLengthInstanceSerializerAdapter<ProtocolVersion> CreateSerializerAdapater()
        {
            return new FixedLengthInstanceSerializerAdapter<ProtocolVersion>(Serializer, this);
        }
    }
    public class Extension
    {
        static InstanceSerializer serializer = null;
        public static IInstanceSerializer<Extension> Serializer
        {
            get
            {
                if(serializer == null) serializer = new InstanceSerializer();
                return serializer;
            }
        }

        class InstanceSerializer : IInstanceSerializer<Extension>
        {
            public InstanceSerializer() {}
            public UInt32 SerializationLength(Extension instance)
            {
                UInt32 dynamicLengthPart = 0;
                dynamicLengthPart += instance.Content.SerializationLength();
                return 2 + dynamicLengthPart;
            }
            public UInt32 Serialize(Byte[] bytes, UInt32 offset, Extension instance)
            {
                UInt32 arrayLength;
                bytes.BigEndianSetUInt16(offset, instance.Type);
                offset += 2;
                offset = instance.Content.Serialize(bytes, offset);
                return offset;
            }
            public UInt32 Deserialize(Byte[] bytes, UInt32 offset, UInt32 offsetLimit, out Extension outInstance)
            {
                UInt32 arrayLength;
                Extension instance = new Extension();
                instance.Type = bytes.BigEndianReadUInt16(offset);
                offset += 2;
                // not implemented; //instance.Content;
                outInstance = instance;
                return offset;
            }
            public void DataString(Extension instance, StringBuilder builder)
            {
                builder.Append("Extension:{");
                builder.Append(instance.Type);
                builder.Append(',');
                instance.Content.DataString(builder);
                builder.Append("}");
            }
            public void DataSmallString(Extension instance, StringBuilder builder)
            {
                builder.Append("Extension:{");
                builder.Append(instance.Type);
                builder.Append(',');
                instance.Content.DataSmallString(builder);
                builder.Append("}");
            }
        }

        public UInt16 Type;
        public ISerializer Content;
        private Extension() { }
        public Extension(UInt16 Type, ISerializer Content)
        {
            this.Type = Type;
            this.Content = Content;
        }
        public InstanceSerializerAdapter<Extension> CreateSerializerAdapater()
        {
            return new InstanceSerializerAdapter<Extension>(Serializer, this);
        }
    }
    public class TlsRecord
    {
        static InstanceSerializer serializer = null;
        public static IInstanceSerializer<TlsRecord> Serializer
        {
            get
            {
                if(serializer == null) serializer = new InstanceSerializer();
                return serializer;
            }
        }

        class InstanceSerializer : IInstanceSerializer<TlsRecord>
        {
            public InstanceSerializer() {}
            public UInt32 SerializationLength(TlsRecord instance)
            {
                UInt32 dynamicLengthPart = 0;
                dynamicLengthPart += instance.Content.SerializationLength();
                return 3 + dynamicLengthPart;
            }
            public UInt32 Serialize(Byte[] bytes, UInt32 offset, TlsRecord instance)
            {
                UInt32 arrayLength;
                ByteEnumSerializer<ContentTypeEnum>.Instance.FixedLengthSerialize(bytes, offset, instance.ContentType);
                offset += 1;
                bytes[offset] = instance.MajorVersion;
                offset += 1;
                bytes[offset] = instance.MinorVersion;
                offset += 1;
                offset = instance.Content.Serialize(bytes, offset);
                return offset;
            }
            public UInt32 Deserialize(Byte[] bytes, UInt32 offset, UInt32 offsetLimit, out TlsRecord outInstance)
            {
                UInt32 arrayLength;
                TlsRecord instance = new TlsRecord();
                instance.ContentType = ByteEnumSerializer<ContentTypeEnum>.Instance.FixedLengthDeserialize(bytes, offset);
                offset += 1;
                instance.MajorVersion = bytes[offset];
                offset += 1;
                instance.MinorVersion = bytes[offset];
                offset += 1;
                // not implemented; //instance.Content;
                outInstance = instance;
                return offset;
            }
            public void DataString(TlsRecord instance, StringBuilder builder)
            {
                builder.Append("TlsRecord:{");
                builder.Append(instance.ContentType);
                builder.Append(',');
                builder.Append(instance.MajorVersion);
                builder.Append(',');
                builder.Append(instance.MinorVersion);
                builder.Append(',');
                instance.Content.DataString(builder);
                builder.Append("}");
            }
            public void DataSmallString(TlsRecord instance, StringBuilder builder)
            {
                builder.Append("TlsRecord:{");
                builder.Append(instance.ContentType);
                builder.Append(',');
                builder.Append(instance.MajorVersion);
                builder.Append(',');
                builder.Append(instance.MinorVersion);
                builder.Append(',');
                instance.Content.DataSmallString(builder);
                builder.Append("}");
            }
        }

        public enum ContentTypeEnum {
            ChangeCipherSpec = 20,
            Alert = 21,
            Handshake = 22,
            ApplicationData = 23,
        }

        public ContentTypeEnum ContentType;
        public Byte MajorVersion;
        public Byte MinorVersion;
        public ISerializer Content;
        private TlsRecord() { }
        public TlsRecord(ContentTypeEnum ContentType, Byte MajorVersion, Byte MinorVersion, ISerializer Content)
        {
            this.ContentType = ContentType;
            this.MajorVersion = MajorVersion;
            this.MinorVersion = MinorVersion;
            this.Content = Content;
        }
        public InstanceSerializerAdapter<TlsRecord> CreateSerializerAdapater()
        {
            return new InstanceSerializerAdapter<TlsRecord>(Serializer, this);
        }
    }
    public class TlsHandshakeRecord
    {
        static InstanceSerializer serializer = null;
        public static IInstanceSerializer<TlsHandshakeRecord> Serializer
        {
            get
            {
                if(serializer == null) serializer = new InstanceSerializer();
                return serializer;
            }
        }

        class InstanceSerializer : IInstanceSerializer<TlsHandshakeRecord>
        {
            public InstanceSerializer() {}
            public UInt32 SerializationLength(TlsHandshakeRecord instance)
            {
                UInt32 dynamicLengthPart = 0;
                dynamicLengthPart += instance.Content.SerializationLength();
                return 1 + dynamicLengthPart;
            }
            public UInt32 Serialize(Byte[] bytes, UInt32 offset, TlsHandshakeRecord instance)
            {
                UInt32 arrayLength;
                ByteEnumSerializer<TypeEnum>.Instance.FixedLengthSerialize(bytes, offset, instance.Type);
                offset += 1;
                offset = instance.Content.Serialize(bytes, offset);
                return offset;
            }
            public UInt32 Deserialize(Byte[] bytes, UInt32 offset, UInt32 offsetLimit, out TlsHandshakeRecord outInstance)
            {
                UInt32 arrayLength;
                TlsHandshakeRecord instance = new TlsHandshakeRecord();
                instance.Type = ByteEnumSerializer<TypeEnum>.Instance.FixedLengthDeserialize(bytes, offset);
                offset += 1;
                // not implemented; //instance.Content;
                outInstance = instance;
                return offset;
            }
            public void DataString(TlsHandshakeRecord instance, StringBuilder builder)
            {
                builder.Append("TlsHandshakeRecord:{");
                builder.Append(instance.Type);
                builder.Append(',');
                instance.Content.DataString(builder);
                builder.Append("}");
            }
            public void DataSmallString(TlsHandshakeRecord instance, StringBuilder builder)
            {
                builder.Append("TlsHandshakeRecord:{");
                builder.Append(instance.Type);
                builder.Append(',');
                instance.Content.DataSmallString(builder);
                builder.Append("}");
            }
        }

        public enum TypeEnum {
            HelloRequest = 0,
            ClientHello = 1,
            ServerHello = 2,
            Certificate = 11,
            ServerKeyExchange = 12,
            CertificateRequest = 13,
            ServerHelloDone = 14,
            CertificateVerify = 15,
            ClientKeyExchange = 16,
            Finished = 20,
        }

        public TypeEnum Type;
        public ISerializer Content;
        private TlsHandshakeRecord() { }
        public TlsHandshakeRecord(TypeEnum Type, ISerializer Content)
        {
            this.Type = Type;
            this.Content = Content;
        }
        public InstanceSerializerAdapter<TlsHandshakeRecord> CreateSerializerAdapater()
        {
            return new InstanceSerializerAdapter<TlsHandshakeRecord>(Serializer, this);
        }
    }
    public class ClientHello
    {
        static InstanceSerializer serializer = null;
        public static IInstanceSerializer<ClientHello> Serializer
        {
            get
            {
                if(serializer == null) serializer = new InstanceSerializer();
                return serializer;
            }
        }

        class InstanceSerializer : IInstanceSerializer<ClientHello>
        {
            public InstanceSerializer() {}
            public UInt32 SerializationLength(ClientHello instance)
            {
                UInt32 dynamicLengthPart = 0;
                if(instance.SessionID != null) dynamicLengthPart += (UInt32)instance.SessionID.Length * 1;
                if(instance.CipherSuites != null) dynamicLengthPart += (UInt32)instance.CipherSuites.Length * 2;
                if(instance.CompressionMethods != null) dynamicLengthPart += (UInt32)instance.CompressionMethods.Length * 1;
                if(instance.Extensions != null)
                {
                    for(int i = 0; i < instance.Extensions.Length; i++)
                    {
                        dynamicLengthPart += Extension.Serializer.SerializationLength(instance.Extensions[i]);
                    }
                }
                return 40 + dynamicLengthPart;
            }
            public UInt32 Serialize(Byte[] bytes, UInt32 offset, ClientHello instance)
            {
                UInt32 arrayLength;
                bytes[offset] = instance.MajorVersion;
                offset += 1;
                bytes[offset] = instance.MinorVersion;
                offset += 1;
                bytes.BigEndianSetUInt32(offset, instance.Time);
                offset += 4;
                for(UInt32 i = 0; i < 28; i++)
                {
                    bytes[offset] = instance.Random[i];
                    offset += 1;
                }
                arrayLength = (instance.SessionID == null) ? 0 : (UInt32)instance.SessionID.Length;
                bytes[offset] = (Byte)(Byte)arrayLength;
                offset += 1;
                for(UInt32 i = 0; i < arrayLength; i++)
                {
                    bytes[offset] = instance.SessionID[i];
                    offset += 1;
                }
                arrayLength = (instance.CipherSuites == null) ? 0 : (UInt32)instance.CipherSuites.Length;
                bytes.BigEndianSetUInt16(offset, (UInt16)arrayLength);
                offset += 2;
                for(UInt32 i = 0; i < arrayLength; i++)
                {
                    BigEndianUnsignedEnumSerializer<CipherSuite>.TwoByteInstance.FixedLengthSerialize(bytes, offset, instance.CipherSuites[i]);
                    offset += 2;
                }
                arrayLength = (instance.CompressionMethods == null) ? 0 : (UInt32)instance.CompressionMethods.Length;
                bytes[offset] = (Byte)(Byte)arrayLength;
                offset += 1;
                for(UInt32 i = 0; i < arrayLength; i++)
                {
                    ByteEnumSerializer<CompressionMethod>.Instance.FixedLengthSerialize(bytes, offset, instance.CompressionMethods[i]);
                    offset += 1;
                }
                arrayLength = (instance.Extensions == null) ? 0 : (UInt32)instance.Extensions.Length;
                bytes.BigEndianSetUInt16(offset, (UInt16)arrayLength);
                offset += 2;
                for(UInt32 i = 0; i < arrayLength; i++)
                {
                    offset = Extension.Serializer.Serialize(bytes, offset, instance.Extensions[i]);
                }
                return offset;
            }
            public UInt32 Deserialize(Byte[] bytes, UInt32 offset, UInt32 offsetLimit, out ClientHello outInstance)
            {
                UInt32 arrayLength;
                ClientHello instance = new ClientHello();
                instance.MajorVersion = bytes[offset];
                offset += 1;
                instance.MinorVersion = bytes[offset];
                offset += 1;
                instance.Time = bytes.BigEndianReadUInt32(offset);
                offset += 4;
                instance.Random = bytes.CreateSubArray(offset, 28);
                offset += 1 * 28;
                arrayLength = (Byte)bytes[offset];
                offset += 1;
                instance.SessionID = bytes.CreateSubArray(offset, arrayLength);
                offset += 1 * arrayLength;
                arrayLength = bytes.BigEndianReadUInt16(offset);
                offset += 2;
                instance.CipherSuites = BigEndianUnsignedEnumSerializer<CipherSuite>.TwoByteInstance.FixedLengthDeserializeArray(bytes, offset, arrayLength);
                offset += 2 * arrayLength;
                arrayLength = (Byte)bytes[offset];
                offset += 1;
                instance.CompressionMethods = ByteEnumSerializer<CompressionMethod>.Instance.FixedLengthDeserializeArray(bytes, offset, arrayLength);
                offset += 1 * arrayLength;
                arrayLength = bytes.BigEndianReadUInt16(offset);
                offset += 2;
// Dynamic Length Element Arrays are not yet implemented
                outInstance = instance;
                return offset;
            }
            public void DataString(ClientHello instance, StringBuilder builder)
            {
                builder.Append("ClientHello:{");
                builder.Append(instance.MajorVersion);
                builder.Append(',');
                builder.Append(instance.MinorVersion);
                builder.Append(',');
                builder.Append(instance.Time);
                builder.Append(',');
                // Arrays inside dynamic length objects not implemented
                builder.Append(',');
                // Arrays inside dynamic length objects not implemented
                builder.Append(',');
                // Arrays inside dynamic length objects not implemented
                builder.Append(',');
                // Arrays inside dynamic length objects not implemented
                builder.Append(',');
                // Arrays inside dynamic length objects not implemented
                builder.Append("}");
            }
            public void DataSmallString(ClientHello instance, StringBuilder builder)
            {
                builder.Append("ClientHello:{");
                builder.Append(instance.MajorVersion);
                builder.Append(',');
                builder.Append(instance.MinorVersion);
                builder.Append(',');
                builder.Append(instance.Time);
                builder.Append(',');
            // Arrays inside dynamic length objects not implemented
                builder.Append(',');
            // Arrays inside dynamic length objects not implemented
                builder.Append(',');
            // Arrays inside dynamic length objects not implemented
                builder.Append(',');
            // Arrays inside dynamic length objects not implemented
                builder.Append(',');
            // Arrays inside dynamic length objects not implemented
                builder.Append("}");
            }
        }

        public Byte MajorVersion;
        public Byte MinorVersion;
        public UInt32 Time;
        public Byte[] Random;
        public Byte[] SessionID;
        public CipherSuite[] CipherSuites;
        public CompressionMethod[] CompressionMethods;
        public Extension[] Extensions;
        private ClientHello() { }
        public ClientHello(Byte MajorVersion, Byte MinorVersion, UInt32 Time, Byte[] Random, Byte[] SessionID, CipherSuite[] CipherSuites, CompressionMethod[] CompressionMethods, Extension[] Extensions)
        {
            this.MajorVersion = MajorVersion;
            this.MinorVersion = MinorVersion;
            this.Time = Time;
            this.Random = Random;
            this.SessionID = SessionID;
            this.CipherSuites = CipherSuites;
            this.CompressionMethods = CompressionMethods;
            this.Extensions = Extensions;
        }
        public InstanceSerializerAdapter<ClientHello> CreateSerializerAdapater()
        {
            return new InstanceSerializerAdapter<ClientHello>(Serializer, this);
        }
    }
}