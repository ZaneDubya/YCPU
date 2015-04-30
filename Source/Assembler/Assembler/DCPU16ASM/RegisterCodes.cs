/*
 * DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
*/

namespace YCPU.Assembler.DCPU16ASM
{
    /// <summary>
    /// DCPU-16 Register Codes
    /// 
    /// </summary>
    public enum dcpuRegisterCodes : ushort
    {
        // Basic register code, used to read value from register
        // ie SET A, X

        /// <summary>
        /// A register
        /// </summary>
        A = 0x00,
        /// <summary>
        /// B Register
        /// </summary>
        B = 0x01,
        /// <summary>
        /// C Register
        /// </summary>
        C = 0x02,
        /// <summary>
        /// X Register
        /// </summary>
        X = 0x03,
        /// <summary>
        /// Y Register
        /// </summary>
        Y = 0x04,
        /// <summary>
        /// Z Register
        /// </summary>
        Z = 0x05,
        /// <summary>
        /// I Register
        /// </summary>
        I = 0x06,
        /// <summary>
        /// J Register
        /// </summary>
        J = 0x07,

        /// <summary>
        /// Memory reference at address in register A
        /// </summary>
        A_Mem = 0x08,
        /// <summary>
        /// Memory reference at address in register B
        /// </summary>
        B_Mem = 0x09,
        /// <summary>
        /// Memory reference at address in register C
        /// </summary>
        C_Mem = 0x0A,
        /// <summary>
        /// Memory reference at address in register X
        /// </summary>
        X_Mem = 0x0B,
        /// <summary>
        /// Memory reference at address in register Y
        /// </summary>
        Y_Mem = 0x0C,
        /// <summary>
        /// Memory reference at address in register Z
        /// </summary>
        Z_Mem = 0x0D,
        /// <summary>
        /// Memory reference at address in register I
        /// </summary>
        I_Mem = 0x0E,
        /// <summary>
        /// Memory reference at address in register J
        /// </summary>
        J_Mem = 0x0F,

        /// <summary>
        /// Memory reference at address in register A +
        /// literal offset stored in next word
        /// </summary>
        A_NextWord = 0x10,
        /// <summary>
        /// Memory reference at address in register B +
        /// literal offset stored in next word
        /// </summary>
        B_NextWord = 0x11,
        /// <summary>
        /// Memory reference at address in register C +
        /// literal offset stored in next word
        /// </summary>
        C_NextWord = 0x12,
        /// <summary>
        /// Memory reference at address in register X +
        /// literal offset stored in next word
        /// </summary>
        X_NextWord = 0x13,
        /// <summary>
        /// Memory reference at address in register Y +
        /// literal offset stored in next word
        /// </summary>
        Y_NextWord = 0x14,
        /// <summary>
        /// Memory reference at address in register Z +
        /// literal offset stored in next word
        /// </summary>
        Z_NextWord = 0x15,
        /// <summary>
        /// Memory reference at address in register I +
        /// literal offset stored in next word
        /// </summary>
        I_NextWord = 0x16,
        /// <summary>
        /// Memory reference at address in register J +
        /// literal offset stored in next word
        /// </summary>
        J_NextWord = 0x17,

        /// <summary>
        /// POP Macro register
        /// Simulates Memory[SP++] 
        /// </summary>
        POP = 0x18,
        /// <summary>
        /// PEEK Macro register
        /// Reads value from Memory[SP]
        /// </summary>
        PEEK = 0x19,
        /// <summary>
        /// PUSH Macro register
        /// Simulates Memory[--SP] 
        /// </summary>
        PUSH = 0x1A,

        /// <summary>
        /// Stack Pointer
        /// </summary>
        SP = 0x1B,
        /// <summary>
        /// Program Counter
        /// </summary>
        PC = 0x1C,
        /// <summary>
        /// Overflow register
        /// </summary>
        O = 0x1D,

        /// <summary>
        /// Literal Memory address reference, where literal value
        /// is stored in the next word. 
        /// </summary>
        NextWord_Literal_Mem = 0x1E,    // IE for "SET A, [0x1000]" B register will be 0x1E and we assume the next word (PC++)'s value is to reference a memory location

        /// <summary>
        /// Literal Value is stored in next word. 
        /// NOTE that literal values that are less than 0x1F are directly coded in the opWord, and are decoded via
        /// the math -> 0x20 + literal. 
        /// </summary>
        NextWord_Literal_Value = 0x1F

    }
}
