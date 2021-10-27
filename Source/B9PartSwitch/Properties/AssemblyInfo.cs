using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("B9 Part Switch /L Unleashed")]
[assembly: AssemblyDescription("A part switching module for KSP")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(B9PartSwitch.LegalMamboJambo.Company)]
[assembly: AssemblyProduct(B9PartSwitch.LegalMamboJambo.Product)]
[assembly: AssemblyCopyright(B9PartSwitch.LegalMamboJambo.Copyright)]
[assembly: AssemblyTrademark(B9PartSwitch.LegalMamboJambo.Trademark)]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: Guid("011a01e9-0074-47d5-82f8-4259d3d927b7")]

//[assembly: AssemblyInformationalVersionAttribute("<%= git_version %>")]
[assembly: AssemblyVersion(B9PartSwitch.Version.Number)]
[assembly: AssemblyFileVersion(B9PartSwitch.Version.Number)]

[assembly: KSPAssembly("B9PartSwitch", B9PartSwitch.Version.major, B9PartSwitch.Version.minor, B9PartSwitch.Version.patch)]
[assembly: KSPAssemblyDependency("KSPe", 2, 2)]
[assembly: KSPAssemblyDependency("KSPe.UI", 2, 2)]