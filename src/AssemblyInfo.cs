// CivOne
//
// To the extent possible under law, the person who associated CC0 with
// CivOne has waived all copyright and related or neighboring rights
// to CivOne.
//
// You should have received a copy of the CC0 legalcode along with this
// work. If not, see <http://creativecommons.org/publicdomain/zero/1.0/>.

using System.Reflection;

// For the FileVersion, CivOne uses Semantic Versioning 2.0.0 (see: http://semver.org/)
// This generates compiler warning CS7035. We don't want to see that warning.
#pragma warning disable 7035
[assembly: AssemblyCopyright("CC0 Creative Commons")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyFileVersion("0.1.0-alpha.1-dev")]
[assembly: AssemblyProduct("CivOne")]
[assembly: AssemblyVersion("0.1.0")]
#pragma warning restore 7035