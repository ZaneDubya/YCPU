using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YCPU.Assembler.DCPU16ASM
{
    partial class Parser
    {
        public virtual ParsedOpcode ParseParam(string param)
        {
            var ParsedOpcode = new ParsedOpcode();

            var clearedParameter = param.Replace(" ", string.Empty).Trim();

            if (this.m_RegisterDictionary.ContainsKey(clearedParameter))
            {
                ParsedOpcode.Word = (ushort)this.m_RegisterDictionary[clearedParameter];
            }
            else
            {
                if ((clearedParameter.StartsWith("[") && clearedParameter.EndsWith("]")) || (clearedParameter.StartsWith("(") && clearedParameter.EndsWith(")")))
                {
                    clearedParameter = clearedParameter.Substring(1, clearedParameter.Length - 2).Replace(" ", string.Empty);

                    if (clearedParameter.Contains("+"))
                    {
                        ParsedOpcode = ParseMemoryAddressPlusRegisterParameter(ParsedOpcode, clearedParameter);
                    }
                    else
                    {
                        ParsedOpcode = ParseMemoryAddressParameter(ParsedOpcode, clearedParameter);
                    }
                }
                else
                {
                    ParseLiteralParameter(ParsedOpcode, clearedParameter);
                }
            }

            return ParsedOpcode;
        }

        public virtual ParsedOpcode ParseMemoryAddressPlusRegisterParameter(ParsedOpcode ParsedOpcode, string clearedParameter)
        {
            var psplit = clearedParameter.Split('+');
            if (psplit.Length < 2)
            {
                throw new Exception(string.Format("malformated memory reference '{0}'", clearedParameter));
            }

            var addressValue = "[+" + psplit[1] + "]";
            if (!this.m_RegisterDictionary.ContainsKey(addressValue))
            {
                throw new Exception(string.Format("Invalid register reference in '{0}'", clearedParameter));
            }

            ParsedOpcode.Word = (ushort)this.m_RegisterDictionary[addressValue];
            ParsedOpcode.UsesNextWord = true;

            if (psplit[0].StartsWith("\'") && psplit[0].EndsWith("\'") && psplit[0].Length == 3)
            {
                var val = (ushort)psplit[0][1];
                ParsedOpcode.NextWord = val;
            }
            else if (psplit[0].Contains("0x"))
            {
                ushort val = Convert.ToUInt16(psplit[0].Trim(), 16);
                ParsedOpcode.NextWord = val;
            }
            else if (psplit[0].Trim().All(x => char.IsDigit(x)))
            {
                var val = Convert.ToUInt16(psplit[0].Trim(), 10);
                ParsedOpcode.NextWord = val;
            }
            else
            {
                ParsedOpcode.UsesNextWord = true;
                ParsedOpcode.LabelName = psplit[0].Trim();
            }

            return ParsedOpcode;
        }

        public virtual ParsedOpcode ParseMemoryAddressParameter(ParsedOpcode ParsedOpcode, string clearedParameter)
        {
            ParsedOpcode.Word = (ushort)dcpuRegisterCodes.NextWord_Literal_Mem;
            ParsedOpcode.UsesNextWord = true;

            if (clearedParameter.StartsWith("\'") && clearedParameter.EndsWith("\'") && clearedParameter.Length == 5)
            {
                ushort val = clearedParameter[1];
                ParsedOpcode.NextWord = val;
            }
            else if (clearedParameter.Contains("0x"))
            {
                ushort val = Convert.ToUInt16(clearedParameter.Trim(), 16);
                ParsedOpcode.NextWord = val;
            }
            else if (clearedParameter.Trim().All(x => char.IsDigit(x)))
            {
                ushort val = Convert.ToUInt16(clearedParameter.Trim(), 10);
                ParsedOpcode.NextWord = val;
            }
            else
            {
                ParsedOpcode.UsesNextWord = true;
                ParsedOpcode.LabelName = clearedParameter.Trim();
            }

            return ParsedOpcode;
        }

        public virtual ParsedOpcode ParseLiteralParameter(ParsedOpcode ParsedOpcode, string clearedParameter)
        {
            ushort literalValue;

            if (clearedParameter.StartsWith("\'") && clearedParameter.EndsWith("\'") && clearedParameter.Length == 3)
            {
                literalValue = clearedParameter[1];
            }
            else if (clearedParameter.Contains("0x"))
            {
                literalValue = Convert.ToUInt16(clearedParameter, 16);
            }
            else if (clearedParameter.Trim().All(x => char.IsDigit(x)))
            {
                literalValue = Convert.ToUInt16(clearedParameter, 10);
            }
            else
            {
                ParsedOpcode.Word = (ushort)dcpuRegisterCodes.NextWord_Literal_Value;
                ParsedOpcode.UsesNextWord = true;
                ParsedOpcode.LabelName = clearedParameter;
                return ParsedOpcode;
            }

            ushort maxValue = 0x1F;

            if (literalValue < maxValue)
            {
                ParsedOpcode.Word = 0x20;
                ParsedOpcode.Word += literalValue;
            }
            else
            {
                ParsedOpcode.Word = (ushort)dcpuRegisterCodes.NextWord_Literal_Value;
                ParsedOpcode.UsesNextWord = true;
                ParsedOpcode.NextWord = literalValue;
            }

            return ParsedOpcode;
        }
    }
}
