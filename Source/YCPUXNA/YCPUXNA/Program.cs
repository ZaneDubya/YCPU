using System;
using System.Runtime.InteropServices;

namespace YCPUXNA
{
    class Program
    {
        static string[] s_DefaultArgs = new string[] { "-asm", "../../Tests/rain.asm" };
        const string errNoArguments = "YCPUXNA: No input specified.";

        // default entry point
        static void Main(string[] args)
        {
            ShowConsoleWindow();

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
                    Emu e = new Emu();
                    e.Run();
                    break;
                default:
                    // do nothing;
                    break;
            }

            HideConsoleWindow();
        }

        public static void ShowConsoleWindow()
        {
            var handle = GetConsoleWindow();

            if (handle == IntPtr.Zero)
            {
                AllocConsole();
            }
            else
            {
                ShowWindow(handle, SW_SHOW);
            }
        }

        public static void HideConsoleWindow()
        {
            var handle = GetConsoleWindow();

            ShowWindow(handle, SW_HIDE);
        }

        public static void StdOutWriteLine(string line)
        {
            Console.WriteLine(line);
        }

        public static ConsoleKeyInfo StdInReadKey()
        {
            return Console.ReadKey();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
    }


}
