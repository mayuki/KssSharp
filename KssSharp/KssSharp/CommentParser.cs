using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KssSharp
{
    /// <summary>
    /// Takes a file path of a text file and extracts comments from it.
    /// Currently accepts two formats:
    ///
    /// // Single line style.
    /// /* Multi-line style. */
    /// </summary>
    internal class CommentParser
    {
        private CommentParserOption _option;
        private String _filePath;
        private String _stringInput;
        private List<String> _blocks;
        private Boolean _parsed;

        /// <summary>
        /// Is this a single-line comment? // This style
        /// </summary>
        /// <param name="line">A String of one line of text.</param>
        /// <returns></returns>
        public static Boolean IsSingleLineComment(String line)
        {
            return Regex.IsMatch(line, @"^\s*//");
        }

        /// <summary>
        /// Is this the start of a multi-line comment? /* This style */
        /// </summary>
        /// <param name="line">A String of one line of text.</param>
        /// <returns></returns>
        public static Boolean IsStartMultiLineComment(String line)
        {
            return Regex.IsMatch(line, @"^\s*/\*");
        }

        /// <summary>
        /// Is this the end of a multi-line comment? /* This style */
        /// </summary>
        /// <param name="line">A String of one line of text.</param>
        /// <returns></returns>
        public static Boolean IsEndMultiLineComment(String line)
        {
            if (IsSingleLineComment(line))
                return false;
            return Regex.IsMatch(line, @".*\*/");
        }

        /// <summary>
        /// Removes comment identifiers for single-line comments.
        /// </summary>
        /// <param name="line">A String of one line of text.</param>
        /// <returns></returns>
        public static String ParseSingleLine(String line)
        {
            return Regex.Replace(line, @"\s*//", "")
                        .TrimEnd();
        }

        /// <summary>
        /// Remove comment identifiers for multi-line comments.
        /// </summary>
        /// <param name="line">A String of one line of text.</param>
        /// <returns></returns>
        public static String ParseMultiLine(String line)
        {
            return Regex.Replace(Regex.Replace(line, @"\s*/\*", ""), @"\*/", "")
                        .TrimEnd();
        }

        /// <summary>
        /// Initializes a new comment parser object. Does not parse on initialization.
        /// </summary>
        /// <param name="filePathOrStringInput">The location of the file to parse as a String, or the String itself.</param>
        /// <param name="options">Optional options object.</param>
        public CommentParser(String filePathOrStringInput, CommentParserOption options = null)
        {
            _option = options ?? new CommentParserOption();
            if (File.Exists(filePathOrStringInput))
            {
                _filePath = filePathOrStringInput;
            }
            else
            {
                _stringInput = filePathOrStringInput;
            }

            _blocks = new List<String>();
            _parsed = false;
        }

        /// <summary>
        /// The different sections of parsed comment text. A section is
        /// either a multi-line comment block's content, or consecutive lines of
        /// single-line comments.
        /// </summary>
        public IEnumerable<String> Blocks
        {
            get { return _parsed ? _blocks : ParseBlocks(); }
        }

        /// <summary>
        /// Parse the file or string for comment blocks and populate them into @blocks.
        /// </summary>
        /// <returns>Returns an Array of parsed comment Strings.</returns>
        private IEnumerable<String> ParseBlocks()
        {
            return ParseBlocksInput((_filePath != null)
                                        ? File.ReadAllText(_filePath, Encoding.UTF8) // the input is an existing file
                                        : _stringInput // filePath is null, we then expect the input to be a String
                                    );
        }

        private IEnumerable<String> ParseBlocksInput(String input)
        {
            String currentBlock = null;
            var insideSingleLineBlock = false;
            var insideMultiLineBlock = false;

            foreach (var line in input.Split('\n').Select(x => x.TrimEnd('\r')))
            {
                // Parse single-line style
                if (IsSingleLineComment(line))
                {
                    var parsed = ParseSingleLine(line);
                    if (insideSingleLineBlock)
                    {
                        currentBlock += "\n" + parsed;
                    }
                    else
                    {
                        currentBlock = parsed;
                        insideSingleLineBlock = true;
                    }
                }

                // Parse multi-line style
                if (IsStartMultiLineComment(line) || insideMultiLineBlock)
                {
                    var parsed = ParseMultiLine(line);
                    if (insideMultiLineBlock)
                    {
                        currentBlock += "\n" + parsed;
                    }
                    else
                    {
                        currentBlock = parsed;
                        insideMultiLineBlock = true;
                    }
                }

                // End a multi-line block if detected
                if (IsEndMultiLineComment(line))
                {
                    insideMultiLineBlock = false;
                }

                // Store the current block if we're done
                if (!(IsSingleLineComment(line) || insideMultiLineBlock))
                {
                    if (currentBlock != null)
                    {
                        _blocks.Add(Normalize(currentBlock));
                    }
                    insideSingleLineBlock = false;
                    currentBlock = null;
                }
            }

            _parsed = true;
            return _blocks;
        }

        /// <summary>
        /// Normalizes the comment block to ignore any consistent preceding
        /// whitespace. Consistent means the same amount of whitespace on every line
        /// of the comment block. Also strips any whitespace at the start and end of
        /// the whole block.
        /// </summary>
        /// <param name="textBlock"></param>
        /// <returns>Returns a String of normalized text.</returns>
        private String Normalize(String textBlock)
        {
            if (_option.PreserveWhitespace)
            {
                return textBlock;
            }

            // Strip out any preceding [whitespace]* that occur on every line. Not
            // the smartest, but I wonder if I care.
            textBlock = Regex.Replace(textBlock, @"^(\s*\*+)", "", RegexOptions.Multiline);

            // Strip consistent indenting by measuring first line's whitespace
            var indentSize = -1;
            var unindented = String.Join("\n", textBlock.Split('\n')
                .Select(line =>
                {
                    var precedingWhitespace = Regex.Match(line, @"^\s*").Groups[0].Length;
                    if (indentSize == -1)
                    {
                        indentSize = precedingWhitespace;
                    }
                    if (String.IsNullOrWhiteSpace(line))
                    {
                        return String.Empty;
                    }
                    else if (indentSize <= precedingWhitespace && indentSize > 0)
                    {
                        return line.Substring(indentSize);
                    }
                    return line;
                }));

            return unindented.Trim();
        }
    }

    public class CommentParserOption
    {
        /// <summary>
        /// Preserve the whitespace before/after comment markers (default:false).
        /// </summary>
        public Boolean PreserveWhitespace { get; set; }
    }
}
