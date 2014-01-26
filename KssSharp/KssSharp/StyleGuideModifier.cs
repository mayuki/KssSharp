using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KssSharp
{
    /// <summary>
    /// Represents a style modifier. Usually a class name or a
    /// pseudo-class such as :hover. See the spec on The Modifiers Section for
    /// more information.
    /// </summary>
    [DebuggerDisplay("Modifier: Name={Name}, Description={Description}")]
    public class StyleGuideModifier
    {
        /// <summary>
        /// Returns the modifier name String.
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// Returns the description String for a Modifier.
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Initialize a new Modifier.
        /// </summary>
        /// <param name="name">The name String of the modifier.</param>
        /// <param name="description">The description String of the modifier.</param>
        public StyleGuideModifier(String name, String description)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// The modifier name as a CSS class. For pseudo-classes, a
        /// generated class name is returned. Useful for generating styleguides.
        ///
        /// Examples
        ///
        ///   :hover => "pseudo-class-hover"
        ///   sexy-button => "sexy-button"
        ///
        /// Returns a CSS class String.

        /// </summary>
        public String ClassName
        {
            get { return Name.Replace('.', ' ').Replace(":", " pseudo-class-").Trim(); }
        }
    }
}
