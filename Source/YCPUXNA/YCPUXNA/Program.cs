using System;
using System.Runtime.InteropServices;
using Ypsilon;

namespace YCPUXNA
{
    class Program
    {
        static string[] s_DefaultArgs = new string[] { "-emu", "../../Tests/testconsole.asm" };
        const string errNoArguments = "YCPUXNA: No input specified. Select an option:\n" +
            "    1. Assemble default 'testconsole.asm' file.\n    2. Disassemble default 'testconsole.asm.bin' file.\n" + 
            "    3. Run emulator!\n    4. Run assembly tests.\n    5. Exit.";

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
                            args = new string[] { "-asm", "../../Tests/testconsole.asm" };
                            waitForKey = false;
                            break;
                        case '2':
                            args = new string[] { "-disasm", "../../Tests/testconsole.asm.bin" };
                            waitForKey = false;
                            break;
                        case '3':
                            args = new string[] { "-emu" };
                            waitForKey = false;
                            break;
                        case '4':
                            args = new string[] { "-test" };
                            waitForKey = false;
                            break;
                        case '5':
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
                    case "-test": // run assembly tests
                        StdConsole.StdOutWriteLine(Tests.Run());
                        StdConsole.StdInReadKey();
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
