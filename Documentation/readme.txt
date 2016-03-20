If YCPUXNA fails to load, you may be missing a required dependancy. Please
make certain that you have these packages installed:

  * Microsoft .NET Framework 4 Client Profile
    https://www.microsoft.com/en-us/download/details.aspx?id=24872
    
  * Microsoft XNA Framework Redistributable 4.0
    https://www.microsoft.com/en-us/download/details.aspx?id=20914

When YCPUXNA.exe is run without any parameters, or when it is run from
YLauncher.exe, it boots into a menu where you can select to assemble a test
program (../Examples/testconsole.asm) or run the binary form of the same test
program (../Examples/testconsole.asm.bin).

You can assemble your own programs by running YCPUXNA from the console with
these arguments:

    ycpuxna -asm path_to_assembly_file.asm
    
You can load your own binary to the emulator's ROM with these arguments:

    ycpuxna -emu path_to_binary_file.bin
    