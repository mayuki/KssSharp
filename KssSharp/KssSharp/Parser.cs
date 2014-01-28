using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KssSharp
{
    /// <summary>
    /// The main KSS parser. Takes a directory full of SASS / SCSS / CSS
    /// files and parses the KSS within them.
    /// </summary>
    public class Parser
    {
        internal static readonly Regex StyleguidePattern = new Regex("(?<!No )Styleguide [a-zA-Z0-9]", RegexOptions.IgnoreCase);

        /// <summary>
        /// Returns a hash of Sections.
        /// </summary>
        public IDictionary<String, StyleguideSection> Sections { get; private set; }

        /// <summary>
        /// Initializes a new parser based on a directory of files or kss strings.
        /// Scans within the directory recursively or the strings for any comment blocks
        /// that look like KSS.
        /// </summary>
        /// <param name="pathsOrStrings">Each path String where style files are located, or each String containing KSS.</param>
        public Parser(params String[] pathsOrStrings)
            : this(pathsOrStrings as IEnumerable<String>)
        {
        }

        /// <summary>
        /// Initializes a new parser based on a directory of files or kss strings.
        /// Scans within the directory recursively or the strings for any comment blocks
        /// that look like KSS.
        /// </summary>
        /// <param name="pathsOrStrings">Each path String where style files are located, or each String containing KSS.</param>
        public Parser(IEnumerable<String> pathsOrStrings)
        {
            Sections = new Dictionary<String, StyleguideSection>();
            foreach (var pathOrString in pathsOrStrings)
            {
                if (Directory.Exists(pathOrString))
                {
                    // argument is a path
                    var path = pathOrString;
                    var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
                        .Where(x => Regex.IsMatch(x, ".(css|less|sass|scss)"));
                    foreach (var file in files)
                    {
                        var parser = new CommentParser(file);
                        foreach (var commentBlock in parser.Blocks.Where(IsKssBlock))
                        {
                            AddSection(commentBlock, file);
                        }
                    }
                }
                else
                {
                    // argument is a KSS string
                    var kssString = pathOrString;
                    var parser = new CommentParser(kssString);
                    foreach (var commentBlock in parser.Blocks.Where(IsKssBlock))
                    {
                        AddSection(commentBlock);
                    }
                }
            }
        }

        private void AddSection(String commentText, String fileName = "")
        {
            var baseName = Path.GetFileName(fileName);
            var section = new StyleguideSection(commentText, baseName);
            Sections[section.Section] = section;
        }

        /// <summary>
        /// Takes a cleaned (no comment syntax like // or /* */) comment
        /// block and determines whether it is a KSS documentation block.
        /// </summary>
        /// <param name="cleanedComment"></param>
        /// <returns>Returns a boolean indicating whether the block conforms to KSS.</returns>
        private static Boolean IsKssBlock(String cleanedComment)
        {
            var possibleReference = cleanedComment.Split(new[] {"\n\n"}, StringSplitOptions.None).Last();
            return StyleguidePattern.IsMatch(possibleReference);
        }

        /// <summary>
        /// Finds the Section for a given styleguide reference.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns>Returns a Section for a reference, or a blank Section if none found.</returns>
        public StyleguideSection Section(String reference)
        {
            return Sections.ContainsKey(reference) ? Sections[reference] : new StyleguideSection();
        }
    }
}
