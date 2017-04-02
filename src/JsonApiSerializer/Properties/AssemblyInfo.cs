using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
 [assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyTitle("JsonApiSerializer")]
[assembly: AssemblyDescription(@"JsonApiSerializer supports configurationless serializing and deserializing objects into the json:api format (http://jsonapi.org).")]
[assembly: AssemblyCompany("Codecutout")]
[assembly: AssemblyProduct("JsonApiSerializer")]
[assembly: AssemblyCopyright("Copyright © 2017")]

[assembly: ComVisible(false)]
[assembly: Guid("59D7D795-82B5-48F2-92D5-70F346EB18E7")]

[assembly: AssemblyVersion("0.9.0")]
[assembly: AssemblyFileVersion("0.9.0")]
[assembly: InternalsVisibleTo("JsonApiSerializer.Test")]

