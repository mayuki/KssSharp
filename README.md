KssSharp
========
[![Build status](https://ci.appveyor.com/api/projects/status?id=7rvmhdf1el1dmqyv)](https://ci.appveyor.com/project/ksssharp)

A .NET(C#) Implementation of KSS( https://github.com/kneath/kss ): A methodology for documenting CSS and generating styleguides


Installation
------------

Search and Install the package "KssSharp" in the NuGet Package Manager, or run the following command in the Package Manager Console.
```
Install-Package KssSharp
```

https://www.nuget.org/packages/KssSharp/


Requirements
------------
- .NET Framework 4.5 or later


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

License
-------
The MIT License (MIT)


Copyright (c) 2014 Mayuki Sawatari


Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:


The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.


THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
