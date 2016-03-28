/* =================================================================
 * YCPUAssembler
 * Copyright (c) 2014 ZaneDubya
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
 * =============================================================== */

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Ypsilon.Assembler
{
    partial class Parser
    {
        private void ParseData8(string line, ParserState state)
        {
            List<string> dataFields = new List<string>();

            bool inDoubleQuote = false, inSingleQuote = false, isEscaped = false;
            string field = string.Empty;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '\"')
                {
                    if (inDoubleQuote)
                    {
                        inDoubleQuote = false;
                        dataFields.Add("\"" + field);
                        field = string.Empty;
                    }
                    else
                    {
                        inDoubleQuote = true;
                    }
                }
                else if (c == '\'')
                {
                    if (inSingleQuote)
                    {
                        inSingleQuote = false;
                        dataFields.Add("\"" + field);
                        field = string.Empty;
                    }
                    else
                    {
                        inSingleQuote = true;
                    }
                }
                else if (c == '\\')
                {
                    if (inDoubleQuote || inSingleQuote)
                    {
                        if (isEscaped)
                        {
                            field += "\\";
                            isEscaped = false;
                        }
                        else
                        {
                            isEscaped = true;
                        }
                    }
                    else
                    {
                        throw new Exception("Illegal placement of backslash character");
                    }
                }
                else if (c == ',')
                {
                    if (inDoubleQuote || inSingleQuote)
                    {
                        field += ",";
                    }
                    else
                    {
                        if (field != string.Empty)
                        {
                            dataFields.Add(field);
                            field = string.Empty;
                        }
                    }
                }
                else
                {
                    if (isEscaped)
                    {
                        switch (char.ToLowerInvariant(c))
                        {
                            case '0':
                                field += (char)(0x00);
                                break;
                            case 'a':
                                field += (char)(0x07);
                                break;
                            case 'b':
                                field += (char)(0x08);
                                break;
                            case 'f':
                                field += (char)(0x0C);
                                break;
                            case 'n':
                                field += (char)(0x0A);
                                break;
                            case 'r':
                                field += (char)(0x0D);
                                break;
                            case 't':
                                field += (char)(0x09);
                                break;
                            case 'v':
                                field += (char)(0x0B);
                                break;
                            case '\'':
                                field += (char)(0x27);
                                break;
                            case '\"':
                                field += (char)(0x22);
                                break;
                            case 'x':
                                if (i + 3 < line.Length)
                                {
                                    char x0 = line[i + 1], x1 = line[i + 2];
                                    if (IsHex(x0) && IsHex(x1))
                                    {
                                        int hex = Int32.Parse(x0 + x1.ToString(), NumberStyles.AllowHexSpecifier);
                                        field += (char)hex;
                                        i += 2;
                                    }
                                    else
                                    {
                                        throw new Exception(x0 + x1 + " is not a hexidecimal character");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Insufficient line length to hold anticipated hex char");
                                }
                                break;
                            default:
                                throw new Exception($"Unknown escaped character \\{c}");
                        }
                        isEscaped = false;
                    }
                    else
                    {
                        field += line[i];
                    }
                }
            }

            if (field.Trim() != string.Empty)
            {
                dataFields.Add(field.Trim());
            }

            GenerateMachineCodeForDataFields(dataFields, DataFieldTypes.Int8, state);
        }

        private void ParseData16(string line, ParserState state)
        {
            List<string> dataFields = new List<string>();

            List<string> linesplit = Common.SplitString(line, ",");

            for (int i = 0; i < linesplit.Count; i++)
            {
                string field = linesplit[i];

                if (field.Trim() == string.Empty) continue;
                if (dataFields.Count == 0)
                {
                    dataFields.Add(field);
                }
                else
                {
                    int count = 0;
                    int last = -1;
                    string lastStr = dataFields[dataFields.Count - 1];

                    while ((last = lastStr.IndexOf('\"', last + 1)) != -1)
                    {
                        count++;
                    }

                    if (count == 1)
                    {
                        dataFields[dataFields.Count - 1] += "," + field;
                    }
                    else
                    {
                        dataFields.Add(field);
                    }
                }
            }

            GenerateMachineCodeForDataFields(dataFields, DataFieldTypes.Int16, state);
        }

        private bool IsHex(char c)
        {
            bool isHex = ((c >= '0' && c <= '9') ||
                         (c >= 'a' && c <= 'f') ||
                         (c >= 'A' && c <= 'F'));
            return isHex;
        }

        private void GenerateMachineCodeForDataFields(IList<string> dataFields, DataFieldTypes dataType, ParserState state)
        {
            for (int j = 0; j < dataFields.Count; j++)
            {
                string field = dataFields[j];

                if (field == string.Empty)
                    continue;

                if (field.IndexOf('\"') == 0)
                {
                    for (int i = 1; i < field.Length; i++)
                    {
                        char c = field[i];
                        switch (dataType)
                        {
                            case DataFieldTypes.Int8:
                                state.Code.Add((byte)c);
                                break;
                            case DataFieldTypes.Int16:
                                state.Code.Add((byte)(c & 0x00ff));
                                state.Code.Add((byte)((ushort)(c & 0xff00) >> 8));
                                break;
                        }
                    }
                }
                else
                {
                    string valField = field.Trim();
                    if (valField == string.Empty)
                        continue;

                    uint val;

                    if (valField.Substring(0, 1) == "$")
                    {
                        val = Convert.ToUInt32(valField.Substring(1, valField.Length - 1), 16);
                    }
                    else if (valField.Length >= 2 && valField.Substring(0, 2) == "0x")
                    {
                        val = Convert.ToUInt32(valField.Substring(2, valField.Length - 2), 16);
                    }
                    else if (uint.TryParse(valField, out val))
                    {
                        val = uint.Parse(valField); // do this twice? just to have something in the if statement...
                    }
                    else
                    {
                        state.DataFields.Add((ushort)state.Code.Count, valField);
                    }

                    switch (dataType)
                    {
                        case DataFieldTypes.Int8:
                            if (val > byte.MaxValue)
                                throw new Exception($"Included byte value '{field}' cannot be expressed in an 8-bit value");
                            state.Code.Add((byte)val);
                            break;
                        case DataFieldTypes.Int16:
                            if (val > ushort.MaxValue)
                                throw new Exception($"Included ushort value '{field}' cannot be expressed in a 16-bit value");
                            state.Code.Add((byte)((ushort)val & 0x00ff));
                            state.Code.Add((byte)((ushort)(val & 0xff00) >> 8));
                            break;
                    }
                }
            }
        }
    }
}
