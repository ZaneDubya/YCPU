using System;
using System.Runtime.InteropServices;
using Ypsilon;

namespace YCPUXNA
{
    internal class Program
    {
        private static string[] s_DefaultArgs = new string[] { "-emu", "../Examples/testconsole.asm" };

        private const string errNoArguments = "YCPUXNA: No input specified. Select an option:\n" +
            "    1. Assemble default 'testconsole.asm' file.\n" + /* 2. Disassemble default 'testconsole.asm.bin' file.\n" + */
            "    2. Run emulator!\n    3. Run assembly tests.\n    4. Exit.";

        // default entry point
        private static void Main(string[] args)
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
#if DEBUG
                            args = new string[] { "-asm", "../../Examples/testconsole.asm" };
#else
                            args = new string[] { "-asm", "../Examples/testconsole.asm" };
#endif
                            waitForKey = false;
                            break;
                        /*case '2':
                            args = new string[] { "-disasm", "../Examples/testconsole.asm.bin" };
                            waitForKey = false;
                            break;*/
                        case '2':
#if DEBUG
                            args = new string[] { "-emu", "../../Examples/testconsole.asm.bin" };
#else
                            args = new string[] { "-emu", "../Examples/testconsole.asm.bin" };
#endif
                            waitForKey = false;
                            break;
                        case '3':
                            args = new string[] { "-test" };
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
                        e.SetArgs(args);
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
