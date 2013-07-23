﻿using System;
using System.Text;

namespace More
{
    public class FixedLengthByteArrayReflector : ClassFieldReflector
    {
        readonly UInt32 fixedLength;
        public FixedLengthByteArrayReflector(Type classThatHasThisField, String fieldName, UInt32 fixedLength)
            : base(classThatHasThisField, fieldName, typeof(Byte[]))
        {
            this.fixedLength = fixedLength;
        }
        public override UInt32 FixedSerializationLength()
        {
            return fixedLength;
        }
        public override UInt32 SerializationLength(Object instance)
        {
            return fixedLength;
        }
        public override UInt32 Serialize(Object instance, Byte[] array, UInt32 offset)
        {
            Byte[] objBytes = (Byte[])fieldInfo.GetValue(instance);
            Array.Copy(objBytes, 0, array, offset, fixedLength);
            return offset + fixedLength;
        }
        public override UInt32 Deserialize(Object instance, Byte[] array, UInt32 offset, UInt32 offsetLimit)
        {
            Byte[] value = new Byte[fixedLength];
            Array.Copy(array, offset, value, 0, fixedLength);
            fieldInfo.SetValue(instance, value);
            return offset + fixedLength;
        }
        public override void DataString(object instance, StringBuilder builder)
        {
            Byte[] objBytes = (Byte[])fieldInfo.GetValue(instance);
            builder.Append('[');
            for (int i = 0; i < objBytes.Length; i++)
            {
                if (i > 0) builder.Append(',');
                builder.Append(objBytes[i].ToString());
            }
            builder.Append(']');
        }
        public override void DataSmallString(object instance, StringBuilder builder)
        {
            builder.Append(String.Format("[{0} bytes]", fixedLength));
        }
    }
    public class ByteArrayReflector : ClassFieldReflector
    {
        readonly Byte arraySizeByteCount;

        public ByteArrayReflector(Type classThatHasThisField, String fieldName, Byte arraySizeByteCount)
            : base(classThatHasThisField, fieldName, typeof(Byte[]))
        {
            this.arraySizeByteCount = arraySizeByteCount;
        }
        public override UInt32 FixedSerializationLength()
        {
            return UInt32.MaxValue;
        }
        public override UInt32 SerializationLength(Object instance)
        {
            Object obj = fieldInfo.GetValue(instance);
            if (obj == null) return 1;
            return ((UInt32)((Byte[])obj).Length) + 1;
        }
        public override UInt32 Serialize(Object instance, Byte[] array, UInt32 offset)
        {
            Object obj = fieldInfo.GetValue(instance);
            if (obj == null)
            {
                array[offset] = 0;
                return offset + 1;
            }

            Byte[] objBytes = (Byte[])obj;
            if(objBytes.Length > 255) throw new InvalidOperationException(String.Format("A byte length array has a max size of 255 but your array is {0}", objBytes.Length));

            array[offset] = (Byte)(objBytes.Length);
            Array.Copy(objBytes, 0, array, offset + 1, objBytes.Length);
            return offset + (UInt32)objBytes.Length + 1;
        }
        Byte GetLength(Object instance)
        {
            Object obj = fieldInfo.GetValue(instance);
            if (obj == null) return 0;
            return (Byte)(((Byte[])obj).Length);
        }
        public override UInt32 Deserialize(Object instance, Byte[] array, UInt32 offset, UInt32 offsetLimit)
        {
            Byte length = array[offset];
            if(length == 0)
            {
                fieldInfo.SetValue(instance, null);
                return offset + 1;
            }

            Byte[] value = new Byte[length];
            Array.Copy(array, offset + 1, value, 0, length);
            fieldInfo.SetValue(instance, value);

            return offset + length + 1;
        }
        public override void DataString(object instance, StringBuilder builder)
        {
            Object obj = fieldInfo.GetValue(instance);
            if (obj == null) builder.Append("[]");
            Byte[] objBytes = (Byte[])obj;
            builder.Append('[');
            for(int i = 0; i < objBytes.Length; i++)
            {
                if(i > 0) builder.Append(',');
                builder.Append(objBytes[i].ToString());
            }
            builder.Append(']');
        }
        public override void DataSmallString(object instance, StringBuilder builder)
        {
            builder.Append(String.Format("[{0} bytes]", GetLength(instance)));
        }
    }

    public class FixedLengthElementArrayReflector<ElementType> : ClassFieldReflector
    {
        readonly Byte arraySizeByteCount;
        readonly FixedLengthInstanceSerializer<ElementType> elementSerializer;
        readonly UInt32 fixedElementSerializationLength;

        public FixedLengthElementArrayReflector(Type classThatHasThisField, String fieldName, Byte arraySizeByteCount,
            FixedLengthInstanceSerializer<ElementType> elementSerializer)
            : base(classThatHasThisField, fieldName, typeof(ElementType[]))
        {
            this.arraySizeByteCount = arraySizeByteCount;
            this.elementSerializer = elementSerializer;
            this.fixedElementSerializationLength = elementSerializer.FixedSerializationLength();
        }
        public override UInt32 FixedSerializationLength()
        {
            return UInt32.MaxValue;
        }
        public override UInt32 SerializationLength(Object instance)
        {
            Object valueAsObject = fieldInfo.GetValue(instance);
            if (valueAsObject == null) return arraySizeByteCount;

            Array valueAsArray = (Array)valueAsObject;
            return arraySizeByteCount +
                (UInt32)valueAsArray.Length * fixedElementSerializationLength;
        }
        public override UInt32 Serialize(object instance, byte[] array, UInt32 offset)
        {
            Object valueAsObject = fieldInfo.GetValue(instance);
            if (valueAsObject == null)
            {
                array.BigEndianSetUInt32Subtype(offset, 0, arraySizeByteCount);
                return offset + arraySizeByteCount;
            }
            
            ElementType[] valueAsArray = (ElementType[])valueAsObject;

            array.BigEndianSetUInt32Subtype(offset, (UInt32)valueAsArray.Length, arraySizeByteCount);
            offset += arraySizeByteCount;

            for (int i = 0; i < valueAsArray.Length; i++)
            {
                elementSerializer.Serialize(array, offset, valueAsArray[i]);
                offset += fixedElementSerializationLength;
            }

            return offset;
        }
        public override UInt32 Deserialize(object instance, byte[] array, UInt32 offset, UInt32 offsetLimit)
        {
            UInt32 length = array.BigEndianReadUInt32Subtype(offset, arraySizeByteCount);
            offset += arraySizeByteCount;

            if (length <= 0)
            {
                fieldInfo.SetValue(instance, null);
            }
            else
            {
                ElementType[] values = new ElementType[length];
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = elementSerializer.FixedLengthDeserialize(array, offset);
                    offset += fixedElementSerializationLength;
                }
            }

            return offset;
        }
        public override void DataString(Object instance, StringBuilder builder)
        {
            Object valueAsObject = fieldInfo.GetValue(instance);
            if (valueAsObject == null)
            {
                builder.Append("null");
                return;
            }

            ElementType[] valueAsArray = (ElementType[])valueAsObject;
            builder.Append('[');
            for (int i = 0; i < valueAsArray.Length; i++)
            {
                elementSerializer.DataString(valueAsArray[i], builder);
            }
            builder.Append(']');
        }
        public override void DataSmallString(Object instance, StringBuilder builder)
        {
            Object valueAsObject = fieldInfo.GetValue(instance);
            if (valueAsObject == null)
            {
                builder.Append("null");
                return;
            }

            ElementType[] valueAsArray = (ElementType[])valueAsObject;
            builder.Append(String.Format("[{0} elements]", valueAsArray.Length));
        }
    }

    public class DynamicSizeElementArrayReflector<ElementType> : ClassFieldReflector
    {
        readonly Byte arraySizeByteCount;
        readonly IInstanceSerializer<ElementType> elementSerializer;

        public DynamicSizeElementArrayReflector(Type classThatHasThisField, String fieldName, Byte arraySizeByteCount,
            IInstanceSerializer<ElementType> elementSerializer)
            : base(classThatHasThisField, fieldName, typeof(ElementType[]))
        {
            this.arraySizeByteCount = arraySizeByteCount;
            this.elementSerializer = elementSerializer;
        }
        public override UInt32 FixedSerializationLength()
        {
            return UInt32.MaxValue;
        }
        public override UInt32 SerializationLength(Object instance)
        {
            Object valueAsObject = fieldInfo.GetValue(instance);
            if (valueAsObject == null) return arraySizeByteCount;

            ElementType[] elements = (ElementType[])valueAsObject;
            UInt32 length = 0;
            for (int i = 0; i < elements.Length; i++)
            {
                length += elementSerializer.SerializationLength(elements[i]);
            }
            return arraySizeByteCount + length;
        }
        public override UInt32 Serialize(Object instance, Byte[] array, UInt32 offset)
        {
            Object valueAsObject = fieldInfo.GetValue(instance);
            if (valueAsObject == null)
            {
                array.BigEndianSetUInt32Subtype(offset, 0, arraySizeByteCount);
                return offset + arraySizeByteCount;
            }

            ElementType[] valueAsArray = (ElementType[])valueAsObject;

            array.BigEndianSetUInt32Subtype(offset, (UInt32)valueAsArray.Length, arraySizeByteCount);
            offset += arraySizeByteCount;

            for (int i = 0; i < valueAsArray.Length; i++)
            {
                offset = elementSerializer.Serialize(array, offset, valueAsArray[i]);
            }

            return offset;
        }
        public override UInt32 Deserialize(Object instance, Byte[] array, UInt32 offset, UInt32 offsetLimit)
        {
            UInt32 length = array.BigEndianReadUInt32Subtype(offset, arraySizeByteCount);
            offset += arraySizeByteCount;

            if (length <= 0)
            {
                fieldInfo.SetValue(instance, null);
            }
            else
            {
                ElementType[] values = new ElementType[length];
                for (int i = 0; i < values.Length; i++)
                {
                    offset = elementSerializer.Deserialize(array, offset, offsetLimit, out values[i]);
                }
            }

            return offset;
        }
        public override void DataSmallString(Object instance, StringBuilder builder)
        {
            Object valueAsObject = fieldInfo.GetValue(instance);
            if (valueAsObject == null)
            {
                builder.Append("null");
                return;
            }

            ElementType[] valueAsArray = (ElementType[])valueAsObject;
            builder.Append(String.Format("[{0} elements]", valueAsArray.Length));
        }
        public override void DataString(Object instance, StringBuilder builder)
        {
            Object valueAsObject = fieldInfo.GetValue(instance);
            if (valueAsObject == null)
            {
                builder.Append("null");
                return;
            }

            ElementType[] valueAsArray = (ElementType[])valueAsObject;
            builder.Append('[');
            for (int i = 0; i < valueAsArray.Length; i++)
            {
                elementSerializer.DataString(valueAsArray[i], builder);
            }
            builder.Append(']');
        }
    }
}