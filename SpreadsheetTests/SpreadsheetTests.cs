using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        public void CreateTestSave(string filename, string version, Dictionary<string, string> cells)
        {
            using (var writer = XmlWriter.Create(filename))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", version);

                foreach ((string key, string contents) in cells)
                {
                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", key);
                    writer.WriteElementString("contents", contents);
                    writer.WriteEndElement();
                }


                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public bool IsValid(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*$");
        }

        public string Normalize(string name)
        {
            return name.ToUpper();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestInvalidNameContent()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.GetCellContents("1a");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestNullNameContent()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.GetCellContents(null);
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestCircularFormula()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("B1", "2");
            spreadsheet.SetContentsOfCell("A1", "=B1 + 2");
            spreadsheet.SetContentsOfCell("B1", "=A1");
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestDirectCircularFormula()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "=A1 + 2");
        }

        [TestMethod]
        public void TestCircularRevert()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "12");
            spreadsheet.SetContentsOfCell("B1", "=5 + A1");
            spreadsheet.SetContentsOfCell("C1", "=5 + B1");
            try
            {
                spreadsheet.SetContentsOfCell("B1", "=C1");
            }
            catch (CircularException)
            {
                Assert.AreEqual(new Formula("5 + A1"), spreadsheet.GetCellContents("B1"));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestFormulaNullName()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell(null, "=A1 + 2");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestStringNullName()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell(null, "");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestDoubleNullName()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell(null, "2.0");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestFormulaInvalidName()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("1a", "=A1 + 2");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestStringInvalidName()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("1a", "");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestDoubleInvalidName()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("1a", "2.0");
        }

        [TestMethod]
        public void TestIntToDouble()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            Assert.AreEqual(1, spreadsheet.SetContentsOfCell("A1", "20").Count);
            Assert.AreEqual(20.0, spreadsheet.GetCellContents("A1"));
        }

        [TestMethod]
        public void TestSetString()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            Assert.AreEqual(1, spreadsheet.SetContentsOfCell("A1", "asdf").Count);
            Assert.AreEqual("asdf", spreadsheet.GetCellContents("A1"));
        }

        [TestMethod]
        public void TestFormula()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            Assert.AreEqual(1, spreadsheet.SetContentsOfCell("A1", "=b1 + 2").Count);
            Assert.AreEqual(new Formula("b1 + 2"), spreadsheet.GetCellContents("A1"));
        }

        [TestMethod]
        public void TestDependentsFormula()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("a1", "10");
            spreadsheet.SetContentsOfCell("f1", "13");
            spreadsheet.SetContentsOfCell("b1", "=a1 + f1");
            spreadsheet.SetContentsOfCell("c1", "=b1 + 2");
            spreadsheet.SetContentsOfCell("d1", "=c1 + 2");
            var result = spreadsheet.SetContentsOfCell("a1", "=10 + 10");
            Assert.AreEqual("a1", result[0]);
            Assert.AreEqual("b1", result[1]);
            Assert.AreEqual("c1", result[2]);
            Assert.AreEqual("d1", result[3]);
        }

        [TestMethod]
        public void TestDependentsDouble()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("a1", "10");
            spreadsheet.SetContentsOfCell("f1", "13");
            spreadsheet.SetContentsOfCell("b1", "=a1 + f1");
            spreadsheet.SetContentsOfCell("c1", "=b1 + 2");
            spreadsheet.SetContentsOfCell("d1", "=c1 + 2");
            var result = spreadsheet.SetContentsOfCell("a1", "20");
            Assert.AreEqual("a1", result[0]);
            Assert.AreEqual("b1", result[1]);
            Assert.AreEqual("c1", result[2]);
            Assert.AreEqual("d1", result[3]);
            Assert.AreEqual(20.0, spreadsheet.GetCellValue("a1"));
            Assert.AreEqual(33.0, spreadsheet.GetCellValue("b1"));
            Assert.AreEqual(35.0, spreadsheet.GetCellValue("c1"));
            Assert.AreEqual(37.0, spreadsheet.GetCellValue("d1"));
        }

        [TestMethod]
        public void TestDependentsString()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("a1", "10");
            spreadsheet.SetContentsOfCell("f1", "13");
            spreadsheet.SetContentsOfCell("b1", "=a1 + f1");
            spreadsheet.SetContentsOfCell("c1", "=b1 + 2");
            spreadsheet.SetContentsOfCell("d1", "=c1 + 2");
            var result = spreadsheet.SetContentsOfCell("a1", "test");
            Assert.AreEqual("a1", result[0]);
            Assert.AreEqual("b1", result[1]);
            Assert.AreEqual("c1", result[2]);
            Assert.AreEqual("d1", result[3]);
            Assert.AreEqual("test", spreadsheet.GetCellValue("a1"));
            Assert.AreEqual(typeof(FormulaError), spreadsheet.GetCellValue("b1").GetType());
            Assert.AreEqual(typeof(FormulaError), spreadsheet.GetCellValue("c1").GetType());
            Assert.AreEqual(typeof(FormulaError), spreadsheet.GetCellValue("d1").GetType());
        }


        [TestMethod]
        public void TestEmptyContent()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            Assert.AreEqual("", spreadsheet.GetCellContents("a1"));
            Assert.AreEqual("", spreadsheet.GetCellContents("z1"));
        }

        [TestMethod]
        public void TestFormulaToDouble()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("a1", "10");
            spreadsheet.SetContentsOfCell("b1", "=a1 + f1");
            spreadsheet.SetContentsOfCell("c1", "=b1 + 2");
            spreadsheet.SetContentsOfCell("d1", "=c1 + 2");
            var result = spreadsheet.SetContentsOfCell("b1", "20");
            Assert.AreEqual("b1", result[0]);
            Assert.AreEqual("c1", result[1]);
            Assert.AreEqual("d1", result[2]);
            result = spreadsheet.SetContentsOfCell("a1", "20");
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("a1", result[0]);
        }

        [TestMethod]
        public void TestFormulaToString()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("a1", "10");
            spreadsheet.SetContentsOfCell("b1", "=a1 + f1");
            spreadsheet.SetContentsOfCell("c1", "=b1 + 2");
            spreadsheet.SetContentsOfCell("d1", "=c1 + 2");
            var result = spreadsheet.SetContentsOfCell("b1", "test");
            Assert.AreEqual("b1", result[0]);
            Assert.AreEqual("c1", result[1]);
            Assert.AreEqual("d1", result[2]);
            result = spreadsheet.SetContentsOfCell("a1", "20");
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("a1", result[0]);
        }

        [TestMethod]
        public void TestCircularException()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("a1", "10");
            spreadsheet.SetContentsOfCell("b1", "=a1");
            spreadsheet.SetContentsOfCell("c1", "=b1");
            try
            {
                spreadsheet.SetContentsOfCell("a1", "=c1");
            }
            catch (CircularException)
            {
                Assert.AreEqual(10.0, spreadsheet.GetCellValue("c1"));
                var result = spreadsheet.SetContentsOfCell("a1", "20");
                Assert.AreEqual(3, result.Count);
                Assert.AreEqual("a1", result[0]);
                Assert.AreEqual("b1", result[1]);
                Assert.AreEqual("c1", result[2]);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullContent()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("c1", null);
        }

        [TestMethod]
        public void TestEmptyString()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("c1", "");
            Assert.AreEqual(0, spreadsheet.GetNamesOfAllNonemptyCells().Count());
            spreadsheet.SetContentsOfCell("c1", "abc");
            Assert.AreEqual(1, spreadsheet.GetNamesOfAllNonemptyCells().Count());
            spreadsheet.SetContentsOfCell("c1", "");
            Assert.AreEqual(0, spreadsheet.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod]
        public void TestSave()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            Assert.IsFalse(spreadsheet.Changed);
            spreadsheet.SetContentsOfCell("c1", "20");
            spreadsheet.SetContentsOfCell("a1", "=20 + c1");
            Assert.IsTrue(spreadsheet.Changed);
            spreadsheet.Save("TestSave.xml");
            Assert.IsFalse(spreadsheet.Changed);
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestInvalidSave()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.Save(null);
        }

        [TestMethod]
        public void TestSavedXml()
        {
            CreateTestSave("TestSavedXml.xml", "default", new Dictionary<string, string>
            {
                {
                    "a1", "test"
                },
                {
                    "a2", "12"
                },
                {
                    "a3", "=2+a2"
                },
                {
                    "a4", "=a3"
                }
            });
            AbstractSpreadsheet spreadsheet = new Spreadsheet("TestSavedXml.xml", IsValid, s => s, "default");
            Assert.AreEqual("test", spreadsheet.GetCellValue("a1"));
            Assert.AreEqual(12.0, spreadsheet.GetCellValue("a2"));
            Assert.AreEqual(14.0, spreadsheet.GetCellValue("a3"));
            Assert.AreEqual(14.0, spreadsheet.GetCellValue("a4"));
        }

        [TestMethod]
        public void TestVersion()
        {
            CreateTestSave("TestVersion.xml", "default", new Dictionary<string, string>
            {
                {
                    "a1", "test"
                }
            });
            AbstractSpreadsheet spreadsheet = new Spreadsheet("TestVersion.xml", IsValid, s => s, "default");
            Assert.AreEqual("default", spreadsheet.Version);
        }

        [TestMethod]
        public void TestSavedVersion()
        {
            CreateTestSave("SavedVersion.xml", "versionTest", new Dictionary<string, string>
            {
                {
                    "a1", "test"
                }
            });
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            Assert.AreEqual("versionTest", spreadsheet.GetSavedVersion("SavedVersion.xml"));
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestSavedVersionError()
        {
            var settings = new XmlWriterSettings
            {
                Indent = true
            };
            using (var writer = XmlWriter.Create("versionError.xml", settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.GetSavedVersion("versionError.xml");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void InvalidFileNameSavedVersion()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.GetSavedVersion("/invalid/wrongPath.xml");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestInvalidVersion()
        {
            CreateTestSave("InvalidVersion.xml", "default", new Dictionary<string, string>
            {
                {
                    "a1", "test"
                }
            });
            AbstractSpreadsheet spreadsheet = new Spreadsheet("InvalidVersion.xml", IsValid, s => s, "1.0");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void SaveInvalidName()
        {
            CreateTestSave("InvalidName.xml", "default", new Dictionary<string, string>
            {
                {
                    "10", "test"
                }
            });
            AbstractSpreadsheet spreadsheet = new Spreadsheet("InvalidName.xml", IsValid, s => s, "default");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestSaveCircular()
        {
            CreateTestSave("SaveCircular.xml", "default", new Dictionary<string, string>
            {
                {
                    "a1", "=b1"
                },
                {
                    "b1", "=a1"
                }
            });
            AbstractSpreadsheet spreadsheet = new Spreadsheet("SaveCircular.xml", IsValid, s => s, "default");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestSaveInvalidContent()
        {
            CreateTestSave("InvalidContent.xml", "default", new Dictionary<string, string>
            {
                {
                    "a1", "=2a"
                }
            });
            AbstractSpreadsheet spreadsheet = new Spreadsheet("InvalidContent.xml", IsValid, s => s, "default");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestInvalidFile()
        {
            new Spreadsheet("/invalid/InvalidFile.xml", IsValid, s => s, "1.0");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestInvalidGetValue()
        {
            var spreadsheet = new Spreadsheet();
            spreadsheet.GetCellValue("123");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestNullGetValue()
        {
            var spreadsheet = new Spreadsheet();
            spreadsheet.GetCellValue(null);
        }

        [TestMethod]
        public void TestEmptyConstructor()
        {
            var spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("a2", "12");
            Assert.AreEqual("", spreadsheet.GetCellValue("A2"));
            Assert.AreEqual(12.0, spreadsheet.GetCellValue("a2"));
            spreadsheet.SetContentsOfCell("abc", "test");
            Assert.AreEqual("test", spreadsheet.GetCellValue("abc"));
        }

        public bool ValidTest(string name)
        {
            return Regex.IsMatch(name, @"[a-zA-Z]{1}");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestIsValid()
        {
            var spreadsheet = new Spreadsheet(ValidTest, Normalize, "saved");
            spreadsheet.SetContentsOfCell("ab1a", "test");
        }

        [TestMethod]
        public void TestSaveSpreadsheet()
        {
            var spreadsheet = new Spreadsheet(IsValid, Normalize, "saved");
            spreadsheet.SetContentsOfCell("a1", "12");
            spreadsheet.SetContentsOfCell("b1", "=a1+f1");
            spreadsheet.SetContentsOfCell("c1", "test");
            spreadsheet.Save("TestSaveSpreadsheet.xml");
            using (var reader = XmlReader.Create("TestSaveSpreadsheet.xml"))
            {
                reader.Read();
                Assert.IsTrue(reader.ReadToFollowing("spreadsheet"));
                Assert.AreEqual("saved", reader.GetAttribute("version"));
                Assert.IsTrue(reader.ReadToFollowing("cell"));
                Assert.IsTrue(reader.ReadToDescendant("name"));
                Assert.AreEqual("A1", reader.ReadElementContentAsString());
                Assert.AreEqual("12", reader.ReadElementContentAsString());

                Assert.IsTrue(reader.ReadToFollowing("cell"));
                Assert.IsTrue(reader.ReadToDescendant("name"));
                Assert.AreEqual("B1", reader.ReadElementContentAsString());
                Assert.AreEqual("=A1+F1", reader.ReadElementContentAsString());

                Assert.IsTrue(reader.ReadToFollowing("cell"));
                Assert.IsTrue(reader.ReadToDescendant("name"));
                Assert.AreEqual("C1", reader.ReadElementContentAsString());
                Assert.AreEqual("test", reader.ReadElementContentAsString());
            }
        }

        [TestMethod]
        public void TestSaveFromSelf()
        {
            var spreadsheet = new Spreadsheet(IsValid, Normalize, "saved");
            spreadsheet.SetContentsOfCell("a1", "12");
            spreadsheet.SetContentsOfCell("b1", "=a1+f1");
            spreadsheet.SetContentsOfCell("c1", "test");
            spreadsheet.SetContentsOfCell("d1", "=a1+2");
            spreadsheet.Save("TestSaveSpreadsheet.xml");
            var spreadsheet2 = new Spreadsheet("TestSaveSpreadsheet.xml", IsValid, Normalize, "saved");
            Assert.AreEqual(12.0, spreadsheet2.GetCellValue("a1"));
            Assert.AreEqual(typeof(FormulaError), spreadsheet2.GetCellValue("b1").GetType());
            Assert.AreEqual("test", spreadsheet2.GetCellValue("c1"));
            Assert.AreEqual(14.0, spreadsheet2.GetCellValue("d1"));
        }
    }
}