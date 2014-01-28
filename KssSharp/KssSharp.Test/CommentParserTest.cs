using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KssSharp.Test
{
    /// <summary>
    /// kss/test/comment_parser_test.rb
    /// </summary>
    [TestClass]
    public class CommentParserTest
    {
        private IEnumerable<String> _parsedComments;

        [TestInitialize]
        public void Setup()
        {
            _parsedComments = new CommentParser(@"Fixtures\comments.txt").Blocks;
        }

        [TestMethod]
        public void SingleLineCommentSyntax()
        {
            CommentParser.IsSingleLineComment("// yuuuup").IsTrue();
            CommentParser.IsSingleLineComment("!nooooope").IsFalse();
        }

        [TestMethod]
        public void StartOfMultiLineCommentSyntax()
        {
            CommentParser.IsStartMultiLineComment("/* yuuuup").IsTrue();
            CommentParser.IsStartMultiLineComment("nooooope").IsFalse();
        }

        [TestMethod]
        public void EndOfMultiLineCommentSyntax()
        {
            CommentParser.IsEndMultiLineComment(" yuuuuup */").IsTrue();
            CommentParser.IsEndMultiLineComment("noooooope").IsFalse();
        }

        [TestMethod]
        public void ParseSingleLineCommentSyntax()
        {
            CommentParser.ParseSingleLine("// yuuuuup").Is(" yuuuuup");
        }

        [TestMethod]
        public void ParseMultiLineCommentSyntax()
        {
            CommentParser.ParseMultiLine("/* yuuuup */").Is(" yuuuup");
        }

        [TestMethod]
        public void FindsSingleLineCommentStyles()
        {
            var expected = @"
This comment block has comment identifiers on every line.

Fun fact: this is Kyle's favorite comment syntax!
".Replace("\r\n", "\n");
            _parsedComments.Contains(expected.Trim()).IsTrue();
        }

        [TestMethod]
        public void FindsMultiLineCommentStyles()
        {
            var expected = @"
This comment block is a block-style comment syntax.

There's only two identifier across multiple lines.
".Replace("\r\n", "\n");
            _parsedComments.Contains(expected.Trim()).IsTrue();

            expected = @"
This is another common multi-line comment style.

It has stars at the begining of every line.
".Replace("\r\n", "\n");
            _parsedComments.Contains(expected.Trim()).IsTrue();
        }

        [TestMethod]
        public void HandlesMixedStyles()
        {
            _parsedComments.Contains("This comment has a /* comment */ identifier inside of it!").IsTrue();
            _parsedComments.Contains("Look at my //cool// comment art!").IsTrue();
        }

        [TestMethod]
        public void HandlesIndentedComments()
        {
            _parsedComments.Contains("Indented single-line comment.").IsTrue();
            _parsedComments.Contains("Indented block comment.").IsTrue();
        }
    }
}
