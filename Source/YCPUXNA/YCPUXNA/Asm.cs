using System.Collections.Generic;
using System.IO;
using Ypsilon.Assembler;

namespace YCPUXNA
{
    class Asm
    {
        const string errArguments = "yasm: Incorrect argument format. Stop.\n    {0}";
        const string errParam = "yasm: Unknown parameter: {0}";
        const string errEmptyInput = "yasm: Input assembly file does not exist or is empty.";
        const string errAssembling = "yasm: Error assembling input file: {0}";
        const string errWritingOutput = "yasm: Error writing machine code. Press any key to exit.";
        const string descAssembler = "yasm: Assembles assembly code into binary code for YCPU.\n    in:  {0}\n    out: {1}";
        const string descSuccess = "yasm: Input file successfully assembled.";
        const string descFileWrittenPressKey = "yasm: File written. Press any key to exit.";

        public void AssembleFromArgs(string[] args)
        {
            string inPath, outPath, error;
            string[] options;

            if (!tryReadArguments(args, out inPath, out outPath, out options, out error))
            {
                StdConsole.StdOutWriteLine(string.Format(errArguments, error));
                return;
            }
            else
            {
                // check for options?
            }

            StdConsole.StdOutWriteLine(string.Format(descAssembler, inPath, outPath));

            List<byte> machineCode;
            string errorMessage;

            if (TryAssemble(inPath, out machineCode, out errorMessage))
            {
                StdConsole.StdOutWriteLine(descSuccess);
                if (tryWriteMachineCode(machineCode, Path.GetDirectoryName(inPath), outPath))
                    StdConsole.StdOutWriteLine(descFileWrittenPressKey);
                else
                    StdConsole.StdOutWriteLine(errWritingOutput);
            }
            else
            {
                StdConsole.StdOutWriteLine(string.Format(errAssembling, errorMessage));
            }

            StdConsole.StdInReadKey();
        }

        private bool tryReadArguments(string[] args, out string inPath, out string outPath, out string[] options, out string error)
        {
            inPath = null;
            outPath = null;
            options = null;
            error = null;

            List<string> readOptions = new List<string>();

            for (int i = 1; i < args.Length; i++)
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

        private string getFileContents(string in_path)
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

        public bool TryAssemble(string pathToAsmFile, out List<byte> machineCode, out string errorMessage)
        {
            machineCode = null;
            errorMessage = null;

            string asmFileContents = getFileContents(pathToAsmFile);
            if (asmFileContents == null)
            {
                errorMessage = errEmptyInput;
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

        private bool tryWriteMachineCode(List<byte> machineCode, string directory, string filename)
        {
            // if filename is null or empty, default to "out.bin"
            filename = (filename == null || (filename.Trim() == string.Empty)) ?
                "out.bin" :
                Path.GetFileNameWithoutExtension(filename) + ".bin"; ;

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
    }
}
