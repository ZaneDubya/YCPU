; === Interrupt vector table ===================================================
; Described in 2.D.6.
.dat16  ResetInt,   ClockInt,   DivZeroFlt, DoubleFault
.dat16  StackFault, SegFault,   UnprivFault,UndefFault
.dat16  0x0000,     0x0000,     0x0000,     0x0000
.dat16  HWI,        BusRefresh, DebugQuery, SWI

; === DivZeroFlt ===============================================================
DivZeroFlt:
{
    lod     r6, txtFault
    jmp     BSOD
    txtFault:
    .dat8 "DivZeroFault", 0x00
}

; === DoubleFault ==============================================================
DoubleFault:
{
    lod     r6, txtFault
    jmp     BSOD
    txtFault:
    .dat8 "DoubleFault", 0x00
}

; === StackFault ===============================================================
StackFault:
{
    lod     r6, txtFault
    jmp     BSOD
    txtFault:
    .dat8 "StackFault", 0x00
}

; === SegFault =================================================================
SegFault:
{
    lod     r6, txtFault
    jmp     BSOD
    txtFault:
    .dat8 "SegFault", 0x00
}

; === UnprivFault ==============================================================
UnprivFault:
{
    lod     r6, txtFault
    jmp     BSOD
    txtFault:
    .dat8 "UnprivFault", 0x00
}

; === UndefFault ===============================================================
UndefFault:
{
    lod     r6, txtFault
    jmp     BSOD
    txtFault:
    .dat8 "UndefFault", 0x00
}

; === HWI ======================================================================
HWI:
{
    lod     r6, txtFault
    jmp     BSOD
    txtFault:
    .dat8 "HWI", 0x00
}

; === BusRefresh ===============================================================
BusRefresh:
{
    lod     r6, txtFault
    jmp     BSOD
    txtFault:
    .dat8 "BusRefresh", 0x00
}

; === DebugQuery ===============================================================
DebugQuery:
{
    lod     r6, txtFault
    jmp     BSOD
    txtFault:
    .dat8 "DebugQuery", 0x00
}

; === SWI ======================================================================
SWI:
{
    lod     r6, txtFault
    jmp     BSOD
    txtFault:
    .dat8 "SWI", 0x00
}

; === BSOD =====================================================================
BSOD:
{
    ; disable clock
    lod     r0, 0
    hwq     $83
    
    jsr ClearScreen
    
    ; write text at location in r6.
    lod     r1, $8200            ; yellow on blue
    lod     r5, $0000            ; location in video memory
    jsr     WriteChars
    
    HangAtBSOD:
        baw HangAtBSOD
}