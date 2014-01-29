using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KssSharp
{
    /// <summary>
    /// Represents a styleguide section. Each section describes one UI
    /// element. A Section can be thought of as the collection of the description,
    /// modifiers, and styleguide reference.
    /// </summary>
    [DebuggerDisplay("Section: Section={Section}, FileName={FileName}")]
    public class StyleguideSection
    {
        private Lazy<String[]> _commentSections;
        private Lazy<String> _section;

        /// <summary>
        /// Returns the raw comment text for the section, not including comment syntax (such as // or /* */).
        /// </summary>
        public String Raw { get; private set; }

        /// <summary>
        /// Returns the filename where this section is found.
        /// </summary>
        public String FileName { get; private set; }

        /// <summary>
        /// Initialize a new Section
        /// </summary>
        public StyleguideSection()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initialize a new Section
        /// </summary>
        /// <param name="commentText">The raw comment String, minus any comment syntax.</param>
        /// <param name="fileName">The filename as a String.</param>
        public StyleguideSection(String commentText, String fileName)
        {
            Raw = commentText;
            FileName = fileName;

            _commentSections = new Lazy<String[]>(() => (Raw != null)
                                                            ? Raw.Split(new [] { "\n\n" }, StringSplitOptions.None)
                                                            : new String[0]);
            _section = new Lazy<String>(() =>
                                        {
                                            var cleaned = SectionComment.Trim().TrimEnd('.');
                                            return Regex.Match(cleaned, @"Styleguide (.+)", RegexOptions.IgnoreCase).Groups[1].Value;
                                        });
        }

        /// <summary>
        /// Splits up the raw comment text into comment sections that represent
        /// description, modifiers, etc.
        /// 
        /// Returns an Array of comment Strings.
        /// </summary>
        public IEnumerable<String> CommentSections
        {
            get { return _commentSections.Value; }
        }

        /// <summary>
        /// The styleguide section for which this comment block references.
        /// Returns the section reference String (ex: "2.1.8").
        /// </summary>
        public String Section
        {
            get { return _section.Value; }
        }

        /// <summary>
        /// The description section of a styleguide comment block.
        /// Returns the description String.
        /// </summary>
        public String Description
        {
            get
            {
                return String.Join("\n\n",
                    CommentSections.Where(section => !(section == SectionComment || section == ModifiersComment || section == MarkupComment))); // unless
            }
        }

        /// <summary>
        /// The modifiers section of a styleguide comment block.
        /// Returns an Array of Modifiers.
        /// </summary>
        public IEnumerable<StyleguideModifier> Modifiers
        {
            get
            {
                var lastIndent = -1;
                var modifiers = new List<StyleguideModifier>();

                if (ModifiersComment == null)
                {
                    return modifiers;
                }

                foreach (var line in ModifiersComment.Split('\n').Where(x => !String.IsNullOrWhiteSpace(x)))
                {
                    var indent = Regex.Match(line, @"^\s*").Groups[0].Length;

                    if (lastIndent != -1 && indent > lastIndent)
                    {
                        modifiers.Last().Description += Regex.Replace(line, " +", " ");
                    }
                    else
                    {
                        // modifier, desc
                        var parts = line.Split(new[] {" - "}, 2, StringSplitOptions.None);
                        if (parts.Length == 2)
                        {
                            modifiers.Add(new StyleguideModifier(parts[0].Trim(), parts[1].Trim()));
                        }
                    }

                    lastIndent = indent;
                }

                return modifiers;
            }
        }
        /// <summary>
        /// The markup section of a styleguide comment block.
        /// Returns the description String.
        /// </summary>
        public String Markup
        {
            get
            {
                var markupComment = MarkupComment;
                return String.IsNullOrWhiteSpace(markupComment) ? "" : markupComment.Substring("Markup: ".Length);
            }
        }

        private String MarkupComment
        {
            get { return CommentSections.FirstOrDefault(x => x.StartsWith("Markup: ", StringComparison.OrdinalIgnoreCase)) ?? ""; }
        }

        private String SectionComment
        {
            get { return CommentSections.FirstOrDefault(Parser.StyleguidePattern.IsMatch) ?? ""; }
        }

        private String ModifiersComment
        {
            get { return CommentSections.Skip(1).LastOrDefault(section => section != SectionComment && section != MarkupComment); }
        }
    }
}
