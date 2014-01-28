using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KssSharp.Test
{
    /// <summary>
    /// kss/test/parser_test.rb
    /// </summary>
    [TestClass]
    public class ParserTest
    {
        private Parser _scssParsed;
        private Parser _sassParsed;
        private Parser _cssParsed;
        private Parser _lessParsed;
        private Parser _multipleParsed;

        [TestInitialize]
        public void Setup()
        {
            _scssParsed = new Parser(@"Fixtures\scss");
            _sassParsed = new Parser(@"Fixtures\sass");
            _cssParsed = new Parser(@"Fixtures\css");
            _lessParsed = new Parser(@"Fixtures\less");
            _multipleParsed = new Parser(@"Fixtures\scss", @"Fixtures\less");
        }

        [TestMethod]
        public void ParsesKssCommentsInScss()
        {
            _scssParsed.Section("2.1.1").Description.Is("Your standard form button.");
        }
        [TestMethod]
        public void ParsesKssKeysInScss()
        {
            _scssParsed.Section("Buttons.Big").Description.Is("A big button");
        }

        [TestMethod]
        public void ParsesKssCommentsInLess()
        {
            _lessParsed.Section("2.1.1").Description.Is("Your standard form button.");
        }
        [TestMethod]
        public void ParsesKssKeysInLess()
        {
            _lessParsed.Section("Buttons.Big").Description.Is("A big button");
        }

        [TestMethod]
        public void ParsesKssMultiLineCommentsInSass()
        {
            _sassParsed.Section("2.1.1").Description.Is("Your standard form button.");
        }
        [TestMethod]
        public void ParsesKssSingleLineCommentsInSass()
        {
            _sassParsed.Section("2.2.1").Description.Is("A button suitable for giving stars to someone.");
        }
        [TestMethod]
        public void ParsesKssKeysInSass()
        {
            _sassParsed.Section("Buttons.Big").Description.Is("A big button");
        }

        [TestMethod]
        public void ParsesKssCommentsInCss()
        {
            _cssParsed.Section("2.1.1").Description.Is("Your standard form button.");
        }
        [TestMethod]
        public void ParsesKssKeysInCss()
        {
            _cssParsed.Section("Buttons.Big").Description.Is("A big button");
        }
        [TestMethod]
        public void ParsesKssKeysWordPhasesInCss()
        {
            _cssParsed.Section("Buttons - Truly Lime").Description.Is("A button truly lime in color");
        }

        [TestMethod]
        public void ParsesNestedScssDocuments()
        {
            _scssParsed.Section("3.0.0").Description.Is("Your standard form element.");
            _scssParsed.Section("3.0.1").Description.Is("Your standard text input box.");
        }
        [TestMethod]
        public void ParsesNestedLessDocuments()
        {
            _lessParsed.Section("3.0.0").Description.Is("Your standard form element.");
            _lessParsed.Section("3.0.1").Description.Is("Your standard text input box.");
        }
        [TestMethod]
        public void ParsesNestedSassDocuments()
        {
            _sassParsed.Section("3.0.0").Description.Is("Your standard form element.");
            _sassParsed.Section("3.0.1").Description.Is("Your standard text input box.");
        }

        [TestMethod]
        public void PublicSectionsCount()
        {
            _cssParsed.Sections.Count.Is(5);
        }

        [TestMethod]
        public void ParseMultiplePaths()
        {
            _multipleParsed.Sections.Count.Is(7);
        }

        [TestMethod]
        public void ParseFromString()
        {
            var scssInput = @"
                // Your standard form element.
                //
                // Styleguide 3.0.0
                form {


                  // Your standard text input box.
                  //
                  // Styleguide 3.0.1
                  input[type=""text""] {
                    border: 1px solid #ccc;
                  }
                }
            ";
            new Parser(scssInput).Section("3.0.0").Description.Is("Your standard form element.");
            new Parser(scssInput).Section("3.0.1").Description.Is("Your standard text input box.");
        }

        [TestMethod]
        public void ParseWithNoStyleguideReferenceComment()
        {
            var scssInput = @"
                // Nothing here
                //
                // No styleguide reference.
                input[type=""text""] {
                  border: 1px solid #ccc;
                }
            ";
            new Parser(scssInput).IsNotNull();
        }
    }
}
