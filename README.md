VSCoverageGenerator
===============

Generate code coverage reports using Visual Studio Performance Tools.

Building
--------
### Requirements
 * Visual Studio 2013
 * Microsoft.VisualStudio.Coverage.Analysis.dll

### Build
    > build.bat

To build with earlier versions of Visual Studio, change **%VS120COMNTOOLS%** in **build.bat** to your version. e.g. **%VS110COMNTOOLS%** for Visual Studio 2012.


Running
--------
### Requirements
  * Microsoft.VisualStudio.Coverage.Analysis.dll
  * Microsoft.VisualStudio.Coverage.Symbols.dll

### Command Line
    > VSCoverageGenerator --help
    Usage: VSCoverageGenerator.exe <options> <executable> <arguments>
    Generate a code coverage report for a given executable.
    
    Options:
      -t, --tools=VALUE          Performace Tools version to use.
      -e, --exclude=VALUE        Exclude symbols from coverage.
      -o, --output=VALUE         Output directory for coverage report (Default: 'coverage').
      -h, --help                 Show this help message and exit.
    
    Performance Tools:
      Visual Studio 2013

    Examples:
      VSCoverageGenerator.exe TestApp.exe
      VSCoverageGenerator.exe -t "Visual Studio 2013" TestApp.exe
      VSCoverageGenerator.exe -t "Visual Studio 2013" -e std::* -e boost::* TestApp.exe

FAQ
--------
### Where do I get the required DLLs?
The required DLLs<sup>[1]</sup> ship with Visual Studio Premium / Ultimate.

They can be found in: **C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\PrivateAssemblies\\**

<sub>[1]: Microsoft.VisualStudio.Coverage.Analysis.dll, Microsoft.VisualStudio.Coverage.Symbols.dll</sub>
