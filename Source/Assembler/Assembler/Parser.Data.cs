/* =================================================================
 * YCPUAssembler
 * Copyright (c) 2014 ZaneDubya
 * Based on DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
 * =============================================================== */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Ypsilon.Assembler
{
    partial class Parser
    {
        void ParseData8(string line, ParserState state)
        {
            List<string> dataFields = new List<string>();

            foreach (var field in line.Split(','))
            {
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

            GenerateMachineCodeForDataFields(dataFields, DataFieldTypes.Int8, state);
        }

        void ParseData16(string line, ParserState state)
        {
            List<string> dataFields = new List<string>();

            foreach (var field in line.Split(','))
            {
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

        private void GenerateMachineCodeForDataFields(IList<string> dataFields, DataFieldTypes dataType, ParserState state)
        {
            foreach (string data in dataFields)
            {
                string valStr = data.Trim();
                if (valStr.IndexOf('"') > -1)
                {
                    for (int i = 1; i < valStr.Length - 1; i++)
                    {
                        char c = valStr[i];
                        switch (dataType)
                        {
                            case DataFieldTypes.Int8:
                                state.Code.Add((byte)c);
                                break;
                            case DataFieldTypes.Int16:
                                state.Code.Add((byte)((ushort)c & 0x00ff));
                                state.Code.Add((byte)((ushort)(c & 0xff00) >> 8));
                                break;
                        }
                    }
                }
                else
                {
                    uint val = (uint)0;

                    if (valStr.Contains("0x") != false)
                    {
                        val = Convert.ToUInt32(valStr, 16);
                    }
                    else if (valStr.All(x => char.IsDigit(x)))
                    {
                        val = Convert.ToUInt32(valStr, 10);
                    }
                    else
                    {
                        state.DataFields.Add((ushort)state.Code.Count, valStr);
                    }

                    switch (dataType)
                    {
                        case DataFieldTypes.Int8:
                            if ((val > byte.MaxValue) || (val < byte.MinValue))
                                throw new Exception(string.Format("Included byte value '{0}' cannot be expressed in an 8-bit value.", data));
                            state.Code.Add((byte)val);
                            break;
                        case DataFieldTypes.Int16:
                            if ((val > ushort.MaxValue) || (val < ushort.MinValue))
                                throw new Exception(string.Format("Included ushort value '{0}' cannot be expressed in a 16-bit value.", data));
                            state.Code.Add((byte)((ushort)val & 0x00ff));
                            state.Code.Add((byte)((ushort)(val & 0xff00) >> 8));
                            break;
                    }
                }
            }
        }
    }
}
