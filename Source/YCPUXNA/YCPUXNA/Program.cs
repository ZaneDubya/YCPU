using System;
using System.Runtime.InteropServices;

namespace YCPUXNA
{
    class Program
    {
        static string[] s_DefaultArgs = new string[] { "-emu", "../../Tests/rain.asm" };
        const string errNoArguments = "YCPUXNA: No input specified.";

        // default entry point
        static void Main(string[] args)
        {
            StdConsole.ShowConsoleWindow();

            if (args.Length == 0)
            {
#if DEBUG
                args = s_DefaultArgs;
#else
                StdOutWriteLine(errNoArguments);
                return;
#endif
            }

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

            StdConsole.HideConsoleWindow();
        }
    }
}
