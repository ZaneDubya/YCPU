using System.Collections.Generic;
using System.IO;
using Ypsilon.Assembler;

namespace YCPUXNA
{
    internal class Asm
    {
        private const string c_ErrArguments = "yasm: Incorrect argument format. Stop.\n    {0}";
        private const string c_ErrParam = "yasm: Unknown parameter: {0}";
        private const string c_ErrEmptyInput = "yasm: Input assembly file does not exist or is empty";
        private const string c_ErrAssembling = "yasm: Error assembling input file: {0}";
        private const string c_ErrWritingOutput = "yasm: Error writing machine code";
        private const string c_DescAssembler = "yasm: Assembles assembly code into binary code for YCPU.\n    in:  {0}\n    out: {1}";
        private const string c_DescSuccess = "yasm: Input file successfully assembled";
        private const string c_DescFileWrittenPressKey = "yasm: File written";

        public void AssembleFromArgs(string[] args)
        {
            string inPath, outPath, error;
            string[] options;

            if (!TryReadArguments(args, out inPath, out outPath, out options, out error))
            {
                StdConsole.StdOutWriteLine(string.Format(c_ErrArguments, error));
                return;
            }

            StdConsole.StdOutWriteLine(string.Format(c_DescAssembler, inPath, outPath));

            List<byte> machineCode;
            string errorMessage;

            if (TryAssemble(inPath, out machineCode, out errorMessage))
            {
                StdConsole.StdOutWriteLine(c_DescSuccess);
                if (TryWriteMachineCode(machineCode, Path.GetDirectoryName(inPath), outPath))
                    StdConsole.StdOutWriteLine(c_DescFileWrittenPressKey);
                else
                    StdConsole.StdOutWriteLine(c_ErrWritingOutput);
            }
            else
            {
                StdConsole.StdOutWriteLine(string.Format(c_ErrAssembling, errorMessage));
            }
        }

        private bool TryReadArguments(string[] args, out string inPath, out string outPath, out string[] options, out string error)
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
                        error = string.Format(c_ErrParam, args[i]);
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

            string inCode;
            using (StreamReader sr = new StreamReader(in_path))
            {
                inCode = sr.ReadToEnd().Trim();
            }

            if (inCode == string.Empty)
                return null;
            return inCode;
        }

        private bool TryAssemble(string pathToAsmFile, out List<byte> machineCode, out string errorMessage)
        {
            machineCode = null;
            errorMessage = null;

            string asmFileContents = getFileContents(pathToAsmFile);
            if (asmFileContents == null)
            {
                errorMessage = c_ErrEmptyInput;
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

        private bool TryWriteMachineCode(IEnumerable<byte> machineCode, string directory, string filename)
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
