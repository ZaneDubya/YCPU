The YCPU Specification
====

The YCPU Specification describes a virtual 16-bit processor with a comprehensive instruction set and features that make it both easy to program for and easy to emulate. The [specification document](https://github.com/ZaneDubya/YCPU/blob/master/Documentation/ycpu.txt "YCPU Specification Document") is released to the public domain, and anyone may use it for any purpose without permission or attribution.

This repository also hosts software which assembles and disassembles YCPU programs, and emulates the YCPU platform. This software is written in C# and uses the XNA framework, and may be reused under the terms of the MIT license.

Source Tree
----
- Documentation - Specifications describing the YCPU and auxiliary emulated hardware devices.
- Resources - Binary resource files that are used by the YCPU's auxiliary emulated hardware devices.
  - Import - contains raw resource files - a palette and charset for the LEM display device.
  - Export - contains binary resource files - 'compiled' versions of files from Import folder.
  - ResourceBuilder - C# program that builds the resource files.
- Source - Source code for the emulator, assembler, and disassembler.
  - Libraries - Class library that describes the YCPU, auxiliary hardware, emulator, assembler, and disassembler.
    - Assembler - Assembles YASM files into binaries.  
    - Devices - Implementation of the auxiliary hardware devices. 
    - Hardware - Implementation of the YCPU.  
  - YCPUXNA - XNA platform for hosting YCPU emulator/assembler/disassember on the Win/.NET platform.  
- Tests - Example YASM files that can be assembled by the Assembler.

Thanks To:
----
Tim "DensitY" Hancock ([DCPU-16 ASM.NET](https://github.com/densitynz/DCPU-16-ASM.NET), a C# DCPU emulator, included under the MIT License)  
The [NESDEV Community](http://www.nesdev.com) (help developing the specification)
