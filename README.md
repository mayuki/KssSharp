KssSharp
========

A .NET Implementation of KSS(https://github.com/kneath/kss): A methodology for documenting CSS and generating styleguides

[![Build status](https://ci.appveyor.com/api/projects/status?id=7rvmhdf1el1dmqyv)](https://ci.appveyor.com/project/ksssharp)

.NET Library
------------
```cs
var styleguide = new Parser(new[] { "Asset/css" });

styleguide.Section("1.1");
// [KssSharp.StyleguideSection]

styleguide.Section("1.1").Description;
// "A button suitable for giving stars to someone."

styleguide.Section("1.1").Modifiers.First();
// [KssSharp.StyleguideModifier]

styleguide.Section("1.1").Modifiers.First().Name;
// ":hover"

styleguide.Section("1.1").Modifiers.First().ClassName;
// "pseudo-class-hover"

styleguide.Section("1.1").Modifiers.First().Description;
// "Subtle hover highlight"
```