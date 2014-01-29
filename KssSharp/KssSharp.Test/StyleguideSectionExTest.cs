using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KssSharp.Test
{
    [TestClass]
    public class StyleguideSectionExTest
    {
        private readonly string CommentText = @"
# Form Button

Your standard form button.

:hover    - Highlights when hovering.
:disabled - Dims the button when disabled.
.primary  - Indicates button is the primary action.
.smaller  - A smaller button

Markup: <button class=""{$modifiers}"">Button Content1</button>
        <button class=""{$modifiers}"">Button Content2</button>

Styleguide 2.1.1.
".Replace("\r\n", "\n").Trim();
        private StyleguideSection _section;

        [TestInitialize]
        public void Setup()
        {
            _section = new StyleguideSection(CommentText, "example.css");
        }

        [TestMethod]
        public void ParsesMarkup()
        {
            _section.Markup.Is("<button class=\"{$modifiers}\">Button Content1</button>\n        <button class=\"{$modifiers}\">Button Content2</button>");
        }

        [TestMethod]
        public void ParsesNoMarkup()
        {
            _section = new StyleguideSection(CommentText.Replace("Markup", "______"), "example.css");
            _section.Markup.Is("");
        }

        #region Original StyleguideSectionTest
        [TestMethod]
        public void ParsesDescription()
        {
            _section.Description.Is("# Form Button\n\nYour standard form button.");
        }

        [TestMethod]
        public void ParsesModifiers()
        {
            _section.Modifiers.Count().Is(4);
        }

        [TestMethod]
        public void ParsesModifierName()
        {
            _section.Modifiers.First().Name.Is(":hover");
        }

        [TestMethod]
        public void ParsesModifierDescription()
        {
            _section.Modifiers.First().Description.Is("Highlights when hovering.");
        }

        [TestMethod]
        public void ParsesStyleguideReference()
        {
            _section.Section.Is("2.1.1");
        }

        [TestMethod]
        public void ParsesWordPhrasesAsStyleguideReferences()
        {
            _section = new StyleguideSection(CommentText.Replace("2.1.1", "Buttons - Truly Lime"), "example.css");
            _section.Section.Is("Buttons - Truly Lime");
        }
        #endregion
    }
}
