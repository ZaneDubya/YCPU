using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ypsilon
{
    class Program
    {
        static void Main(string[] args)
        {
            string inPath, outPath, error;
            string[] options;

            if (args.Length == 0)
            {
#if DEBUG
                args = new string[1] { "../../../../Tests/bld/AsmTstGn-1.asm" };
#else
                Console.WriteLine(errNoArguments);
                return;
#endif
            }

            

            if (!tryReadArguments(args, out inPath, out outPath, out options, out error))
            {
                Console.WriteLine(string.Format(errArguments, error));
                return;
            }
            else
            {
                // check for options?
            }

            Console.WriteLine(string.Format(descAssembler, inPath, outPath));

            byte[] machineCode;
            AssemblerResult result = tryAssemble(inPath, out machineCode);
            Console.WriteLine(AssemblerResultMessages[(int)result]);

            if (tryWriteMachineCode(machineCode, Path.GetDirectoryName(inPath), outPath))
                Console.WriteLine("File successfully written. Press any key to continue.");
            else
                Console.WriteLine("Error while writing machine code. Press any key to continue.");

            Console.ReadKey();
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

        private static string[] getFileContents(string in_path)
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

            string[] lines = in_code.Split('\n');
            return lines;
        }

        private static AssemblerResult tryAssemble(string in_path, out byte[] machineCode)
        {
            machineCode = null;

            string[] lines = getFileContents(in_path);
            if (lines == null)
                return AssemblerResult.EmptyDocument;

            Assembler.Parser parser = new Assembler.Parser();
            machineCode = parser.Parse(lines, Path.GetDirectoryName(in_path));

            if (machineCode == null)
            {
                Console.WriteLine(parser.MessageOutput);
                return AssemblerResult.ParseError;
            }

            return AssemblerResult.Success;
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

        private enum AssemblerResult
        {
            Success,
            EmptyDocument,
            ParseError
        }

        private static string[] AssemblerResultMessages = new string[3]
        {
            "Successfully compiled input file.",
            "Nothing to compile.",
            "Parser error."
        };

        const string errNoArguments = "yasm: No input specified.";
        const string errArguments = "yasm: Incorrect argument format. Stop.\n    {0}";
        const string errParam = "yasm: Unknown parameter: {0}";
        const string descAssembler = "yasm: Assembles assembly code into binary code for YCPU.\n    in:  {0}\n    out: {1}";
    }
}
