The YCPU Specification
====

The YCPU Specification describes an imaginary 16-bit processor with a comprehensive instruction set and features that make it both easy to program for and easy to emulate. The [specification document](https://github.com/ZaneDubya/YCPU/blob/master/Documentation/ycpu.txt "YCPU Specification Document") is very thorough, with all opcodes, interrupts, and error states included in detail. The specification is released to the public domain, and anyone may use it for any purpose without permission or attribution.

Source Tree
----
**Assembler** - Assembles YASM files into binaries.  
**Documentation** - Specifications describing the YCPU and related hardware devices.  
**Emulator** - Emulates the YCPU using the Hardware and Platform libraries.  
**Hardware** - Class library, implentation of the YCPU and hardware devices.  
**Platform** - Class library, XNA platform for hosting YCPU on the Win32/.NET platform.  
**Resources** - Resource files (images, palettes, shaders) that are included in Platform.  
**Tests** - Example YASM files that can be assembled by the Assembler.

Thanks To
----
Tim "DensitY" Hancock ([DCPU-16 ASM.NET](https://github.com/densitynz/DCPU-16-ASM.NET), a C# DCPU emulator, included under the MIT License)  
"Aphid" ([ParallelTasks](http://http://paralleltasks.codeplex.com/), a threading library, included under the Microsoft Public License)  
The [NESDEV Community](http://www.nesdev.com) (help developing the specification)
