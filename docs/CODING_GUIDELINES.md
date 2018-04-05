# Coding guidelines

CivOne is written in C#. Since there's no predefined rules on how to write C#, everyone has their own preferences. It is expected that code written for CivOne will mimic existing code, and follow certain guidelines. A commit can be refused if the guidelines were ignored.
Coding guidelines are open for discussion, but rarely changed.

## Visual Studio Code is the prefered code editor

I will not stop you from using another code editor, but commits that add metadata (such as .vs folders) or make unnecessary 'upgrades' to the csproj and sln files will be refused.

## Code file structure

All .cs code files have a certain structure. Please follow this structure:

### Licence header

The CivOne licence header, refering to the CC0 website, and ending with a single empty line.
```
// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

```

### Usings

Usings are sorted alphabetically, with one exception: System usings first, then the rest. Usings block ends with a single empty line. Sample:
```
using System;
using System.IO;
using CivOne.Enums;
using CivOne.Graphics.Sprites;

```

### Namespaces

All code is placed under the CivOne.[directory] namespace. Namespace names are always PascalCase. The file ends without and empty line.
```
namespace CivOne.Sample
{
	[...]
}
```

### Classes, structs, enums and constructors/destructors

All classes, structs and enums must be placed in seperate files. Unless it's an inline class, struct or enum.
Always use the internal modifier for classes, structs and enums, unless the class implements a public interface or inherits a public class. Inline classes, structs and enums that are only used in their parent class should be private or protected if it can be used by inheriting classes. 

Variables, fields, methods and constructors in internal classes do not need to be internal, they can be made public. If the class is internal, the all public fields are automatically internal. This makes it easier to change a class from internal to public if needed.

The order of classes and structs is:
* Inline classes, structs and enums
* Constants (private, then protected, then internal, then public)
* Readonly fields (private, then protected, then internal, then public)
* Events (private, then protected, then internal, then public)
* Properties (private, then protected, then internal, then public)
	- Properties can be grouped with the variable that holds their value
* Methods (private, then protected, then internal, then public)
* Optional Singleton fields and properties
* Constructor
* Destructor
* Public Dispose method if the class implements IDisposable

Sample code file:
```
// [header]

using System;
using System.Drawing;
using CivOne.Enums;

namespace CivOne.Sample
{
	[SampleAttribute(true)]
	internal class Example : IDisposable
	{
		private struct Point3D
		{
			public int X, Y, Z;
		}
		
		private const int SAMPLE_CONSTANT = 0x47;
		public const string PUBLIC_CONSTANT = "Good example.";

		private readonly byte _neverChange = 74;

		public event EventHandler OnChange;

		private string _propertyValue;
		protected string ChangeValue
		{
			get => _propertyValue;
			set
			{
				if (value == _propertyValue) return;
				_propertyValue = value;
				OnChange?.Invoke(this, EventArgs.Empty);
			}
		}

		private int DoSomething(int input)
		{
			int output = input - 1;
			output *= 2;
			return output;
		}

		public int DoSomethingElse(int input) => DoSomething(input) * 3;

		private static Example _instance;
		public static Example Instance
		{
			get
			{
				if (_instance == null)
					_instance = new Instance();
				return _instance;
			}
		}

		private Example()
		{
		}

		~Example()
		{
		}

		public void Dispose()
		{
		}
	}
}
```

## Use tabs for indents, not spaces

This rule is enforced when using Visual Studio Code. If you want to use another code editor, please double check you tab usage before committing your code.

## Always use strongly typed declarations

This is a personal preference, I will not change this opinion. This project does not allow the usage of the var keyword. Any code code that uses the var keyword will be refused. Always use strongly typed declarations.
The argument for var is that complex Linq statements have complex type. My counter argument is: Don't write Linq statements that end in complex types: Cast your statement to a predefined struct, or find another way to achieve what you want.

## Do not waste space

* Never place an empty line after a bracket
* Files do not need to end with a new line
* A single empty line between fields, methods, properties, etc is enough

## Compact code, but readability is more important

This is a bit instinctive. Sometimes compact code makes the code hard to read. In these cases, it is better to seperate you code over multiple lines or statements.

## Naming

The following naming conventions are used:
* Constants use SCREAMING_CAPS
* Private variables use _underscoreCamelCase
* Local variables and method arguments use camelCase
* Classes, structs, enums, methods, properties and public variables use PascalCasing