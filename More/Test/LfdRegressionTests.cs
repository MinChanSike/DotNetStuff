﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace More
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class RegressionTests
    {
        [TestMethod]
        public void GenericTestMethod()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("");
            builder.AppendLine("LineWithNoValues \t  ");
            builder.AppendLine("Line1Fields \t field1  \t\t");
            builder.AppendLine("Line2Fields \t field1  \t\tfield2  ");
            builder.AppendLine("LineFieldWithWhitespace \t\t  \t \"this ; is a long field\"  \t");
            builder.AppendLine("LineWithEmptyFields \t\t\"\"  \"\"  \t");

            using (LfdReader reader = new LfdReader(new StringReader(builder.ToString())))
            {
                LfdLine line = reader.ReadLine();

                Assert.AreEqual("LineWithNoValues", line.id);

                line = reader.ReadLine();
                Assert.AreEqual("Line1Fields", line.id);
                Assert.AreEqual("field1", line.fields[0]);

                line = reader.ReadLine();
                Assert.AreEqual("Line2Fields", line.id);
                Assert.AreEqual("field1", line.fields[0]);
                Assert.AreEqual("field2", line.fields[1]);

                line = reader.ReadLine();
                Assert.AreEqual("LineFieldWithWhitespace", line.id);
                Assert.AreEqual("this ; is a long field", line.fields[0]);

                line = reader.ReadLine();
                Assert.AreEqual("LineWithEmptyFields", line.id);
                Assert.AreEqual(String.Empty, line.fields[0]);
                Assert.AreEqual("", line.fields[1]);

                Assert.IsNull(reader.ReadLine());
            }
            //
            // Test using line parser
            //
            OffsetLineParser lineParser = new OffsetLineParser(new Buf(0, 1));
            Byte[] linesAsBytes = Encoding.UTF8.GetBytes(builder.ToString());
            lineParser.Add(linesAsBytes, linesAsBytes.Length);

            List<String> fields = new List<String>();
            Byte[] lineBuffer;
            Int32 lineOffset = 0, lineLength = 0;

            lineBuffer = lineParser.GetLine(ref lineOffset, ref lineLength);
            LfdLine.ParseLine(fields, lineBuffer, lineOffset, lineOffset + lineLength);
            Assert.AreEqual(0, fields.Count);
            fields.Clear();

            lineBuffer = lineParser.GetLine(ref lineOffset, ref lineLength);
            LfdLine.ParseLine(fields, lineBuffer, lineOffset, lineOffset + lineLength);
            Assert.AreEqual("LineWithNoValues", fields[0]);
            fields.Clear();

            lineBuffer = lineParser.GetLine(ref lineOffset, ref lineLength);
            LfdLine.ParseLine(fields, lineBuffer, lineOffset, lineOffset + lineLength);
            Assert.AreEqual("Line1Fields", fields[0]);
            Assert.AreEqual("field1", fields[1]);
            fields.Clear();

            lineBuffer = lineParser.GetLine(ref lineOffset, ref lineLength);
            LfdLine.ParseLine(fields, lineBuffer, lineOffset, lineOffset + lineLength);
            Assert.AreEqual("Line2Fields", fields[0]);
            Assert.AreEqual("field1", fields[1]);
            Assert.AreEqual("field2", fields[2]);
            fields.Clear();

            lineBuffer = lineParser.GetLine(ref lineOffset, ref lineLength);
            LfdLine.ParseLine(fields, lineBuffer, lineOffset, lineOffset + lineLength);
            Assert.AreEqual("LineFieldWithWhitespace", fields[0]);
            Assert.AreEqual("this ; is a long field", fields[1]);
            fields.Clear();

            lineBuffer = lineParser.GetLine(ref lineOffset, ref lineLength);
            LfdLine.ParseLine(fields, lineBuffer, lineOffset, lineOffset + lineLength);
            Assert.AreEqual("LineWithEmptyFields", fields[0]);
            fields.Clear();
        }

        [TestMethod]
        public void TestEmptyLines()
        {
            using (LfdReader reader = new LfdReader(new StringReader("\r\n\r\n\n\n\r\n\n\n\n    \t\t\t\n \t\n\f\f\f\t\n\f\t\n\r\r\r")))
            {
                Assert.IsNull(reader.ReadLine());
            }
        }

        [TestMethod]
        public void TestQuotes()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("LineFieldWithWhitespace \t\t  \t \"this ; is a long field\"  \t");
            builder.AppendLine("LineWithEmptyFields \t\t\"\"  \"\"  \t");
            builder.AppendLine("LineWithEscapedQuotes \t\t\" hello \\\"person\\\" in quotes \" \"\\\"\"");

            using (LfdReader reader = new LfdReader(new StringReader(builder.ToString())))
            {
                LfdLine line;

                line = reader.ReadLine();
                Assert.AreEqual("LineFieldWithWhitespace", line.id);
                Assert.AreEqual("this ; is a long field", line.fields[0]);

                line = reader.ReadLine();
                Assert.AreEqual("LineWithEmptyFields", line.id);
                Assert.AreEqual(String.Empty, line.fields[0]);
                Assert.AreEqual("", line.fields[1]);

                line = reader.ReadLine();
                Assert.AreEqual("LineWithEscapedQuotes", line.id);
                Assert.AreEqual(" hello \"person\" in quotes ", line.fields[0]);
                Assert.AreEqual("\"", line.fields[1]);

                Assert.IsNull(reader.ReadLine());
            }
        }

        [TestMethod]
        public void TestLineContinuations()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(@"line1 \          ");
            builder.AppendLine(@"continuationofline1 \           ");
            builder.AppendLine(@"another continuation   ");
            builder.AppendLine(@"\");
            builder.AppendLine(@"");

            using (LfdReader reader = new LfdReader(new StringReader(builder.ToString())))
            {
                LfdLine line;

                line = reader.ReadLine();
                Assert.AreEqual("line1", line.id);
                Assert.AreEqual("continuationofline1", line.fields[0]);
                Assert.AreEqual("another", line.fields[1]);
                Assert.AreEqual("continuation", line.fields[2]);

                Assert.IsNull(reader.ReadLine());
            }
        }

        [TestMethod]
        public void TestContextChanges()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Parent1Context    \t\t{  \t\t");
            builder.AppendLine("Child1");
            builder.AppendLine("}");
            builder.AppendLine("NoParent");
            builder.AppendLine("");
            builder.AppendLine("FirstParentOfTree{");
            builder.AppendLine("   Child");
            builder.AppendLine("   SecondParentOfTree {");
            builder.AppendLine("      Child");
            builder.AppendLine("   }");
            builder.AppendLine("}");

            using (LfdReader reader = new LfdReader(new StringReader(builder.ToString())))
            {
                LfdLine line;

                line = reader.ReadLine();
                Assert.AreEqual("Parent1Context", line.id);

                line = reader.ReadLine();
                Assert.AreEqual("Child1", line.id);
                Assert.AreEqual("Parent1Context", line.parent.id);

                line = reader.ReadLine();
                Assert.AreEqual("NoParent", line.id);

                line = reader.ReadLine();
                Assert.AreEqual("FirstParentOfTree", line.id);

                line = reader.ReadLine();
                Assert.AreEqual("Child", line.id);
                Assert.AreEqual("FirstParentOfTree", line.parent.id);

                line = reader.ReadLine();
                Assert.AreEqual("SecondParentOfTree", line.id);
                Assert.AreEqual("FirstParentOfTree", line.parent.id);

                line = reader.ReadLine();
                Assert.AreEqual("Child", line.id);
                Assert.AreEqual("SecondParentOfTree", line.parent.id);
                Assert.AreEqual("FirstParentOfTree", line.parent.parent.id);

                line = reader.ReadLine();
                Assert.AreEqual(null, line);

                Assert.IsNull(reader.ReadLine());
            }
        }

        [TestMethod]
        public void TestEscapes()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("LineThatIsContinued field1 field2 \\");
            builder.AppendLine("field3 field4 \\    \t");
            builder.AppendLine("   field5");
            builder.AppendLine("AnotherLine \\{field1 field2");

            using (LfdReader reader = new LfdReader(new StringReader(builder.ToString())))
            {
                LfdLine line;

                line = reader.ReadLine();
                Assert.AreEqual("LineThatIsContinued", line.id);
                Assert.AreEqual("field1", line.fields[0]);
                Assert.AreEqual("field2", line.fields[1]);
                Assert.AreEqual("field3", line.fields[2]);
                Assert.AreEqual("field4", line.fields[3]);
                Assert.AreEqual("field5", line.fields[4]);
                
                line = reader.ReadLine();
                Assert.AreEqual("AnotherLine", line.id);
                Assert.AreEqual("{field1", line.fields[0]);
                Assert.AreEqual("field2", line.fields[1]);

                Assert.IsNull(reader.ReadLine());
            }
        }

        [TestMethod]
        public void TestExceptions()
        {
            String[] badStrings = new String[] {
                "}",
                "\"",
                "{",
                @"line ""no ending quote",
                @"line ""no ending quote\""",
                @" \ can't have data here",
                @"line id \ can't",
                @"line \"" hello \\n""",
            };

            for (int i = 0; i < badStrings.Length; i++)
            {
                try
                {
                    new LfdReader(new StringReader(badStrings[i])).ReadLine();
                    Assert.Fail(String.Format("Expected FormatException on \"{0}\" but didn't get one", badStrings[i]));
                }
                catch (FormatException e) { Console.WriteLine("Caught expected FormatException: {0}", e.Message); }
            }
        }
    }
}
