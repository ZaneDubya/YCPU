/*
 * DCPU-16 ASM.NET
 * Copyright (c) 2012 Tim "DensitY" Hancock (densitynz@orcon.net.nz)
 * This code is licensed under the MIT License
*/

namespace YCPU.Assembler.DCPU16ASM
{
    /// <summary>
    /// DCPU-16 Op codes, Unless extra bytes are required op codes are generally stored
    /// bbbbbbaaaaaaoooo
    /// 
    /// O = 4bits for cpu OpCode
    /// A = 6bits for Dest Param
    /// B = 6bits for Source Param 
    /// 
    /// Depending on parm, up to 2 extra words may be required (meaning max instruction length is 3 words long)
    /// </summary>
    public enum dcpuOpCode : ushort
    {
        /// <summary>
        /// Signals non basic instruction
        /// </summary>
        NB_OP = 0x0,      // 

        /// <summary>
        /// Set instruciton          -> A = B 
        /// </summary>
        SET_OP = 0x1,      // 
        /// <summary>
        /// Add instruction          -> A = A + B
        /// </summary>
        ADD_OP = 0x2,      // 
        /// <summary>
        /// Subtract instruciton     -> A = A - B
        /// </summary>
        SUB_OP = 0x3,      // 
        /// <summary>
        /// Muliply  instruction     -> A = A * B
        /// </summary>
        MUL_OP = 0x4,      // 
        /// <summary>
        /// Divide instruction       -> A = A / B
        /// </summary>
        DIV_OP = 0x5,      // 
        /// <summary>
        /// Modulate instruction     -> A = A % B
        /// </summary>
        MOD_OP = 0x6,      // 
        /// <summary>
        /// Shift Left instruction   -> A = A << B
        /// </summary>
        SHL_OP = 0x7,      // 
        /// <summary>
        /// Shift right instruction  -> A = A >> B
        /// </summary>
        SHR_OP = 0x8,      // 
        /// <summary>
        /// Boolean AND instruction  -> A = A & B
        /// </summary>
        AND_OP = 0x9,      // 
        /// <summary>
        /// Boolean OR instruction   -> A = A | B
        /// </summary>
        BOR_OP = 0xA,      // 
        /// <summary>
        /// Boolean XOR instruction  -> A = A ^ B
        /// </summary>
        XOR_OP = 0xB,      // 
        /// <summary>
        /// Branch! if(A == B) run next instruction
        /// </summary>
        IFE_OP = 0xC,      // 
        /// <summary>
        /// Branch! if(A != B) run next instruction
        /// </summary>
        IFN_OP = 0xD,      // 
        /// <summary>
        /// Branch! if(A > B) run next instruction
        /// </summary>
        IFG_OP = 0xE,      // 
        /// <summary>
        /// Branch! if((A & B) != 0) run next instruction
        /// </summary>
        IFB_OP = 0xF,      // 

        // Non basic instructions
        // Encoded as follows
        // AAAAAAoooooo0000 
        // Basically unlike basic instructions, we lose a register spot and use that for the op code.
        // the old op code is zeroed out (which signals a non basic instruction). This means 
        // any non basic instruction, even if its something like derp X, Y will use 2 words (unlike a basic instruction in that case)

        /// <summary>
        /// Jump and Store Program counter in Stack. 
        /// </summary>
        JSR_OP = 0x10
    }
}
