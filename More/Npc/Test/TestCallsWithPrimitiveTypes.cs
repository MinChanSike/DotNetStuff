﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace More
{
    [NpcInterface]
    interface TestClassNpcInterface
    {
        void VoidCall();
        Boolean ReturnBoolean();
        Char ReturnChar();
        Int32 ReturnInt32();
        UInt32 ReturnUint32();
        Single ReturnSingle();
        Double ReturnDouble();
        String ReturnString();

        Boolean EchoBoolean(Boolean b);
        Char EchoChar(Char c);
        Int32 EchoInt32(Int32 i);
        UInt32 EchoUInt32(UInt32 u);
        Single EchoSingle(Single s);
        Double EchoDouble(Double d);
        String EchoString(String s);
    }
    class TestClass : TestClassNpcInterface
    {
        public void VoidCall() { }
        public Boolean ReturnBoolean() { return false; }
        public Char ReturnChar() { return '\0'; }
        public Int32 ReturnInt32() { return 0; }
        public UInt32 ReturnUint32() { return 0; }
        public Single ReturnSingle() { return 0; }
        public Double ReturnDouble() { return 0; }
        public String ReturnString() { return null; }

        public Boolean EchoBoolean(Boolean b) { return b; }
        public Char EchoChar(Char c) { return c; }
        public Int32 EchoInt32(Int32 i) { return i; }
        public UInt32 EchoUInt32(UInt32 u) { return u; }
        public Single EchoSingle(Single s) { return s; }
        public Double EchoDouble(Double d) { return d; }
        public String EchoString(String s) { return s; }
    }

    [TestClass]
    public class TestCallsWithPrimitiveTypes
    {
        [TestMethod]
        public void CallsWithPrimitiveTypes()
        {
            NpcReflector npcReflector = new NpcReflector(new Object[] { new TestClass() });
            npcReflector.PrintInformation(Console.Out);
            NpcReturnObject returnObject;

            //
            // Test Instance Calls
            //
            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.ReturnBoolean");
            Assert.AreEqual(typeof(Boolean), returnObject.type);
            Assert.AreEqual(false, (Boolean)returnObject.value);
            Assert.AreEqual(false, Boolean.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.ReturnChar");
            Assert.AreEqual(typeof(Char), returnObject.type);
            Assert.AreEqual('\0', (Char)returnObject.value);
            Assert.AreEqual(@"""\0""", returnObject.valueSosSerializationString);

            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.ReturnInt32");
            Assert.AreEqual(typeof(Int32), returnObject.type);
            Assert.AreEqual(0, (Int32)returnObject.value);
            Assert.AreEqual(0, Int32.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.ReturnUInt32");
            Assert.AreEqual(typeof(UInt32), returnObject.type);
            Assert.AreEqual(0U, (UInt32)returnObject.value);
            Assert.AreEqual(0U, UInt32.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.ReturnSingle");
            Assert.AreEqual(typeof(Single), returnObject.type);
            Assert.AreEqual(0.0, (Single)returnObject.value);
            Assert.AreEqual(0.0, Single.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.ReturnDouble");
            Assert.AreEqual(typeof(Double), returnObject.type);
            Assert.AreEqual(0.0, (Double)returnObject.value);
            Assert.AreEqual(0.0, Double.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.ReturnString");
            Assert.AreEqual(typeof(String), returnObject.type);
            Assert.AreEqual(null, (String)returnObject.value);
            Assert.AreEqual("null", returnObject.valueSosSerializationString);

            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.EchoBoolean", false);
            Assert.AreEqual(typeof(Boolean), returnObject.type);
            Assert.AreEqual(false, (Boolean)returnObject.value);
            Assert.AreEqual(false, Boolean.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.EchoBoolean", true);
            Assert.AreEqual(typeof(Boolean), returnObject.type);
            Assert.AreEqual(true, (Boolean)returnObject.value);
            Assert.AreEqual(true, Boolean.Parse(returnObject.valueSosSerializationString));


            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.EchoChar", 'a');
            Assert.AreEqual(typeof(Char), returnObject.type);
            Assert.AreEqual('a', (Char)returnObject.value);
            Assert.AreEqual("\"a\"", returnObject.valueSosSerializationString);

            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.EchoChar", 'x');
            Assert.AreEqual(typeof(Char), returnObject.type);
            Assert.AreEqual('x', (Char)returnObject.value);
            Assert.AreEqual("\"x\"", returnObject.valueSosSerializationString);

            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.EchoChar", '0');
            Assert.AreEqual(typeof(Char), returnObject.type);
            Assert.AreEqual('0', (Char)returnObject.value);
            Assert.AreEqual("\"0\"", returnObject.valueSosSerializationString);

            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.EchoChar", '9');
            Assert.AreEqual(typeof(Char), returnObject.type);
            Assert.AreEqual('9', (Char)returnObject.value);
            Assert.AreEqual("\"9\"", returnObject.valueSosSerializationString);


            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.EchoInt32", 0);
            Assert.AreEqual(typeof(Int32), returnObject.type);
            Assert.AreEqual(0, (Int32)returnObject.value);
            Assert.AreEqual(0, Int32.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.EchoInt32", 9876);
            Assert.AreEqual(typeof(Int32), returnObject.type);
            Assert.AreEqual(9876, (Int32)returnObject.value);
            Assert.AreEqual(9876, Int32.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.EchoInt32", -1);
            Assert.AreEqual(typeof(Int32), returnObject.type);
            Assert.AreEqual(-1, (Int32)returnObject.value);
            Assert.AreEqual(-1, Int32.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.EchoInt32", 2147483647);
            Assert.AreEqual(typeof(Int32), returnObject.type);
            Assert.AreEqual(2147483647, (Int32)returnObject.value);
            Assert.AreEqual(2147483647, Int32.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.EchoInt32", -2147483648);
            Assert.AreEqual(typeof(Int32), returnObject.type);
            Assert.AreEqual(-2147483648, (Int32)returnObject.value);
            Assert.AreEqual(-2147483648, Int32.Parse(returnObject.valueSosSerializationString));


            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.EchoUInt32", 0U);
            Assert.AreEqual(typeof(UInt32), returnObject.type);
            Assert.AreEqual(0U, (UInt32)returnObject.value);
            Assert.AreEqual(0U, UInt32.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.EchoUInt32", 9876U);
            Assert.AreEqual(typeof(UInt32), returnObject.type);
            Assert.AreEqual(9876U, (UInt32)returnObject.value);
            Assert.AreEqual(9876U, UInt32.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.EchoUInt32", 2147483647U);
            Assert.AreEqual(typeof(UInt32), returnObject.type);
            Assert.AreEqual(2147483647U, (UInt32)returnObject.value);
            Assert.AreEqual(2147483647U, UInt32.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.EchoUInt32", 4294967295U);
            Assert.AreEqual(typeof(UInt32), returnObject.type);
            Assert.AreEqual(4294967295U, (UInt32)returnObject.value);
            Assert.AreEqual(4294967295U, UInt32.Parse(returnObject.valueSosSerializationString));


            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.EchoSingle", -3.40282e+038f);
            Assert.AreEqual(typeof(Single), returnObject.type);
            Assert.AreEqual(-3.40282e+038f, (Single)returnObject.value);
            Assert.AreEqual(-3.40282e+038f, Single.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.EchoSingle", 3.40282e+038f);
            Assert.AreEqual(typeof(Single), returnObject.type);
            Assert.AreEqual(3.40282e+038f, (Single)returnObject.value);
            Assert.AreEqual(3.40282e+038f, Single.Parse(returnObject.valueSosSerializationString));


            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.EchoDouble", -1.79769e+308);
            Assert.AreEqual(typeof(Double), returnObject.type);
            Assert.AreEqual(-1.79769e+308, (Double)returnObject.value);
            Assert.AreEqual(-1.79769e+308, Double.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.EchoDouble", 1.79769e+308);
            Assert.AreEqual(typeof(Double), returnObject.type);
            Assert.AreEqual(1.79769e+308, (Double)returnObject.value);
            Assert.AreEqual(1.79769e+308, Double.Parse(returnObject.valueSosSerializationString));


            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.EchoString", new Object[] { null });
            Assert.AreEqual(typeof(String), returnObject.type);
            Assert.AreEqual(null, (String)returnObject.value);
            Assert.AreEqual("null", returnObject.valueSosSerializationString);

            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.EchoString", String.Empty);
            Assert.AreEqual(typeof(String), returnObject.type);
            Assert.AreEqual(String.Empty, (String)returnObject.value);
            Assert.AreEqual("\"\"", returnObject.valueSosSerializationString);

            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.EchoString", "a");
            Assert.AreEqual(typeof(String), returnObject.type);
            Assert.AreEqual("a", (String)returnObject.value);
            Assert.AreEqual("a", returnObject.valueSosSerializationString);

            returnObject = npcReflector.ExecuteWithObjects("More.TestClass.EchoString", "\"hello\"");
            Assert.AreEqual(typeof(String), returnObject.type);
            Assert.AreEqual("\"hello\"", (String)returnObject.value);
            Assert.AreEqual("\"\\\"hello\\\"\"", returnObject.valueSosSerializationString);
        }
        [TestMethod]
        public void CallsWithPrimitiveTypeStrings()
        {
            NpcReflector npcReflector = new NpcReflector(new Object[] { new TestClass() });
            npcReflector.PrintInformation(Console.Out);
            NpcReturnObject returnObject;

            //
            // Test Instance Calls
            //
            returnObject = npcReflector.ExecuteWithStrings("More.TestClass.EchoBoolean", "false");
            Assert.AreEqual(typeof(Boolean), returnObject.type);
            Assert.AreEqual(false, (Boolean)returnObject.value);
            Assert.AreEqual(false, Boolean.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithStrings("More.TestClass.EchoBoolean", "true");
            Assert.AreEqual(typeof(Boolean), returnObject.type);
            Assert.AreEqual(true, (Boolean)returnObject.value);
            Assert.AreEqual(true, Boolean.Parse(returnObject.valueSosSerializationString));


            returnObject = npcReflector.ExecuteWithStrings("More.TestClass.EchoChar", "a");
            Assert.AreEqual(typeof(Char), returnObject.type);
            Assert.AreEqual('a', (Char)returnObject.value);
            Assert.AreEqual("\"a\"", returnObject.valueSosSerializationString);

            returnObject = npcReflector.ExecuteWithStrings("More.TestClass.EchoChar", "x");
            Assert.AreEqual(typeof(Char), returnObject.type);
            Assert.AreEqual('x', (Char)returnObject.value);
            Assert.AreEqual("\"x\"", returnObject.valueSosSerializationString);

            returnObject = npcReflector.ExecuteWithStrings("More.TestClass.EchoChar", "0");
            Assert.AreEqual(typeof(Char), returnObject.type);
            Assert.AreEqual('0', (Char)returnObject.value);
            Assert.AreEqual("\"0\"", returnObject.valueSosSerializationString);

            returnObject = npcReflector.ExecuteWithStrings("More.TestClass.EchoChar", "\"9\"");
            Assert.AreEqual(typeof(Char), returnObject.type);
            Assert.AreEqual('9', (Char)returnObject.value);
            Assert.AreEqual("\"9\"", returnObject.valueSosSerializationString);


            returnObject = npcReflector.ExecuteWithStrings("More.TestClass.EchoInt32", "0");
            Assert.AreEqual(typeof(Int32), returnObject.type);
            Assert.AreEqual(0, (Int32)returnObject.value);
            Assert.AreEqual(0, Int32.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithStrings("More.TestClass.EchoInt32", "9876");
            Assert.AreEqual(typeof(Int32), returnObject.type);
            Assert.AreEqual(9876, (Int32)returnObject.value);
            Assert.AreEqual(9876, Int32.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithStrings("More.TestClass.EchoInt32", "-1");
            Assert.AreEqual(typeof(Int32), returnObject.type);
            Assert.AreEqual(-1, (Int32)returnObject.value);
            Assert.AreEqual(-1, Int32.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithStrings("More.TestClass.EchoInt32", "2147483647");
            Assert.AreEqual(typeof(Int32), returnObject.type);
            Assert.AreEqual(2147483647, (Int32)returnObject.value);
            Assert.AreEqual(2147483647, Int32.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithStrings("More.TestClass.EchoInt32", "-2147483648");
            Assert.AreEqual(typeof(Int32), returnObject.type);
            Assert.AreEqual(-2147483648, (Int32)returnObject.value);
            Assert.AreEqual(-2147483648, Int32.Parse(returnObject.valueSosSerializationString));


            returnObject = npcReflector.ExecuteWithStrings("More.TestClass.EchoUInt32", "0");
            Assert.AreEqual(typeof(UInt32), returnObject.type);
            Assert.AreEqual(0U, (UInt32)returnObject.value);
            Assert.AreEqual(0U, UInt32.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithStrings("More.TestClass.EchoUInt32", "9876");
            Assert.AreEqual(typeof(UInt32), returnObject.type);
            Assert.AreEqual(9876U, (UInt32)returnObject.value);
            Assert.AreEqual(9876U, UInt32.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithStrings("More.TestClass.EchoUInt32", "2147483647");
            Assert.AreEqual(typeof(UInt32), returnObject.type);
            Assert.AreEqual(2147483647U, (UInt32)returnObject.value);
            Assert.AreEqual(2147483647U, UInt32.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithStrings("More.TestClass.EchoUInt32", "4294967295");
            Assert.AreEqual(typeof(UInt32), returnObject.type);
            Assert.AreEqual(4294967295U, (UInt32)returnObject.value);
            Assert.AreEqual(4294967295U, UInt32.Parse(returnObject.valueSosSerializationString));


            returnObject = npcReflector.ExecuteWithStrings("More.TestClass.EchoSingle", "-3.40282e+038");
            Assert.AreEqual(typeof(Single), returnObject.type);
            Assert.AreEqual(-3.40282e+038f, (Single)returnObject.value);
            Assert.AreEqual(-3.40282e+038f, Single.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithStrings("More.TestClass.EchoSingle", "3.40282e+038");
            Assert.AreEqual(typeof(Single), returnObject.type);
            Assert.AreEqual(3.40282e+038f, (Single)returnObject.value);
            Assert.AreEqual(3.40282e+038f, Single.Parse(returnObject.valueSosSerializationString));


            returnObject = npcReflector.ExecuteWithStrings("More.TestClass.EchoDouble", "-1.79769e+308");
            Assert.AreEqual(typeof(Double), returnObject.type);
            Assert.AreEqual(-1.79769e+308, (Double)returnObject.value);
            Assert.AreEqual(-1.79769e+308, Double.Parse(returnObject.valueSosSerializationString));

            returnObject = npcReflector.ExecuteWithStrings("More.TestClass.EchoDouble", "1.79769e+308");
            Assert.AreEqual(typeof(Double), returnObject.type);
            Assert.AreEqual(1.79769e+308, (Double)returnObject.value);
            Assert.AreEqual(1.79769e+308, Double.Parse(returnObject.valueSosSerializationString));


            returnObject = npcReflector.ExecuteWithStrings("More.TestClass.EchoString", new String[] { "null" });
            Assert.AreEqual(typeof(String), returnObject.type);
            Assert.AreEqual(null, (String)returnObject.value);
            Assert.AreEqual("null", returnObject.valueSosSerializationString);

            returnObject = npcReflector.ExecuteWithStrings("More.TestClass.EchoString", new String[] { "\"\"" });
            Assert.AreEqual(typeof(String), returnObject.type);
            Assert.AreEqual(String.Empty, (String)returnObject.value);
            Assert.AreEqual("\"\"", returnObject.valueSosSerializationString);

            returnObject = npcReflector.ExecuteWithStrings("More.TestClass.EchoString", "\"null\"");
            Assert.AreEqual(typeof(String), returnObject.type);
            Assert.AreEqual("null", (String)returnObject.value);
            Assert.AreEqual("\"null\"", returnObject.valueSosSerializationString);

            returnObject = npcReflector.ExecuteWithStrings("More.TestClass.EchoString", "\"a\"");
            Assert.AreEqual(typeof(String), returnObject.type);
            Assert.AreEqual("a", (String)returnObject.value);
            Assert.AreEqual("a", returnObject.valueSosSerializationString);

            returnObject = npcReflector.ExecuteWithStrings("More.TestClass.EchoString", "\"hello\"");
            Assert.AreEqual(typeof(String), returnObject.type);
            Assert.AreEqual("hello", (String)returnObject.value);
            Assert.AreEqual("hello", returnObject.valueSosSerializationString);
        }
    }
}
