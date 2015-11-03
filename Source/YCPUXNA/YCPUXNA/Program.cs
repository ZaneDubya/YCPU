using System;
using System.Runtime.InteropServices;

namespace YCPUXNA
{
    class Program
    {
        static string[] s_DefaultArgs = new string[] { "-emu", "../../Tests/rain.asm" };
        const string errNoArguments = "YCPUXNA: No input specified. Select an option:\n" + 
            "    1. Assemble default 'rain.asm' file.\n    2. Disassemble default 'rain.asm.bin' file.\n" + 
            "    3. Run emulator!\n    4. Exit.";

        // default entry point
        static void Main(string[] args)
        {
            StdConsole.ShowConsoleWindow();

            if (args.Length == 0)
            {
                StdConsole.StdOutWriteLine(errNoArguments);
                bool waitForKey = true;
                while (waitForKey)
                {
                    ConsoleKeyInfo cki = StdConsole.StdInReadKey();
                    switch (cki.KeyChar)
                    {
                        case '1':
                            args = new string[] { "-asm", "../../Tests/rain.asm" };
                            waitForKey = false;
                            break;
                        case '2':
                            args = new string[] { "-disasm", "../../Tests/rain.asm.bin" };
                            waitForKey = false;
                            break;
                        case '3':
                            args = new string[] { "-emu" };
                            waitForKey = false;
                            break;
                        case '4':
                            waitForKey = false;
                            break;
                    }
                }
            }

            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "-asm": // run assembler
                        Asm asm = new Asm();
                        asm.AssembleFromArgs(args);
                        break;
                    case "-disasm": // run disassembler
                        Dsm disasm = new Dsm();
                        disasm.TryDisassemble(args);
                        break;
                    case "-emu": // run emulator!
                        StdConsole.StdOutWriteLine("Starting emulator...");
                        Emu e = new Emu();
                        StdConsole.HideConsoleWindow();
                        e.Run();
                        break;
                    default:
                        // do nothing;
                        break;
                }
            }

            StdConsole.HideConsoleWindow();
            return;
        }
    }
}
