using System;
using System.Collections.Generic;
using System.IO;
using Ypsilon.Assembler;

namespace Ypsilon
{
    class Program
    {
        const string errNoArguments = "yasm: No input specified.";
        const string errArguments = "yasm: Incorrect argument format. Stop.\n    {0}";
        const string errParam = "yasm: Unknown parameter: {0}";
        const string descAssembler = "yasm: Assembles assembly code into binary code for YCPU.\n    in:  {0}\n    out: {1}";

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
#if DEBUG
                args = new string[] { 
                    /*"../../Tests/bld/AsmTstGn-0.asm",
                    "../../Tests/bld/AsmTstGn-1.asm",*/
                    "../../Tests/rain.asm"};
                for (int i = 0; i < args.Length; i++)
                {
                    compileFromArgs(new string[] { args[i] });
                }
                return;
#else
                StdOutWriteLine(errNoArguments);
                return;
#endif
            }
            else
            {
                compileFromArgs(args);
            }
        }

        private static void compileFromArgs(string[] args)
        {
            string inPath, outPath, error;
            string[] options;

            if (!tryReadArguments(args, out inPath, out outPath, out options, out error))
            {
                StdOutWriteLine(string.Format(errArguments, error));
                return;
            }
            else
            {
                // check for options?
            }

            StdOutWriteLine(string.Format(descAssembler, inPath, outPath));

            byte[] machineCode;
            string errorMessage;

            if (TryAssemble(inPath, out machineCode, out errorMessage))
            {
                StdOutWriteLine("Assembly file compiled.");
                if (tryWriteMachineCode(machineCode, Path.GetDirectoryName(inPath), outPath))
                    StdOutWriteLine("File written. Press any key to continue.");
                else
                    StdOutWriteLine("Error writing machine code. Press any key to continue.");
            }
            else
            {
                StdOutWriteLine(string.Format("Error compiling assembly file: {0}", errorMessage));
            }

            StdInReadKey();
        }

        private static bool tryReadArguments(string[] args, out string inPath, out string outPath, out string[] options, out string error)
        {
            inPath = null;
            outPath = null;
            options = null;
            error = null;

            List<string> readOptions = new List<string>();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == null || args[i].Length == 0)
                    continue;

                if (args[i][0] == '-') // option!
                {
                    // do nothing - we don't yet handle any options.
                }
                else
                {
                    if (inPath == null)
                        inPath = args[i];
                    else if (outPath == null)
                        outPath = args[i];
                    else
                    {
                        error = string.Format(errParam, args[i]);
                        return false;
                    }
                }
            }

            if (inPath == null)
                return false;

            if (outPath == null)
                outPath = inPath + ".bin";

            return true;
        }

        private static string getFileContents(string in_path)
        {
            if (!File.Exists(in_path))
            {
                return null;
            }

            string in_code = null;
            using (StreamReader sr = new StreamReader(in_path))
            {
                in_code = sr.ReadToEnd().Trim();
            }

            if (in_code == string.Empty)
                return null;
            return in_code;
        }

        public static bool TryAssemble(string pathToAsmFile, out byte[] machineCode, out string errorMessage)
        {
            machineCode = null;
            errorMessage = null; 

            string asmFileContents = getFileContents(pathToAsmFile);
            if (asmFileContents == null)
            {
                errorMessage = "Input assembly file does not exist or is empty.";
                return false;
            }

            Parser parser = new Parser();
            machineCode = parser.Parse(asmFileContents, Path.GetDirectoryName(pathToAsmFile));

            if (machineCode == null)
            {
                errorMessage = parser.ErrorMsg;
                return false;
            }

            return true;
        }

        private static bool tryWriteMachineCode(byte[] machineCode, string directory, string filename)
        {
            // if filename is null or empty, default to "out.bin"
            filename = (filename == null || (filename.Trim() == string.Empty)) ?
                "out.bin" : 
                Path.GetFileNameWithoutExtension(filename) + ".bin";;

            // make sure that directory, if not empty, is postfixed with a slash.
            if (directory != string.Empty)
            {
                if ((directory[directory.Length - 1] != '/') && (directory[directory.Length - 1] != '\\'))
                    directory += '\\';
            }

            string path = directory + filename;

            // delete the output file if it exists.
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch
            {
                return false;
            }

            // write the file.
            try
            {
                MemoryStream outfile = new MemoryStream();
                foreach (byte word in machineCode)
                {
                    outfile.WriteByte(word);
                }

                File.WriteAllBytes(directory + filename, outfile.ToArray());
                return true;
            }
            catch
            {
                return false;
            }

            
        }

        static void StdOutWriteLine(string line)
        {
            Console.WriteLine(line);
        }

        static ConsoleKeyInfo StdInReadKey()
        {
            return Console.ReadKey();
        }
    }
}
