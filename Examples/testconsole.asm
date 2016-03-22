; TEST CONSOLE
; Tests segments and mmu, interrupts, etc.
; Expects Graphics Device @ bus index 1, Keyboard @ index 2, at least 128kb RAM.

.alignglobals 2                 ; align global labels to 16-bit boundaries
.include "testconsole.ivt.asm"  ; include int vector table & interrupt handlers

; === ResetInt =================================================================
ResetInt:
{
    ; After the reset interrupt, the processor state is as described in 2.G.
    ; set stack pointer to $0000.
    lod     r0, $0000
    sto     r0, sp
    
    ;set up devices, mmu, etc.
    jsr     Setup
    
    ; show the 'NYA ELEKTRISKA' screen
    jsr     ShowStartScreen
    jsr     ClearScreen
    
    ; enable clock interrupt - tick at 100hz
    lod     r0, 100
    hwq     $83
    
    ; use r5 as index to onscreen char, starting at y = 0, x = 0
    lod     r5, $0000
    
    Update:
        jsr     GetKeyboardEvents   ; R0 = number of events, 16bit events copied to $7002-$701F
        cmp     r0, 0
        beq     Update
    
        lod     r1, $2800            ; hi byte is color: yellow on blue
        lod     r6, $7002            ; get first char, written in $7002 (see GetKeyboardEvents)
        writeSingleChar:
            lod     r2, [r6]
            adi     r6, 2
            lod     r3, r2
            rnr     r3, 8
            and     r3, 0x000f
            cmp     r3, 3
            bne     writeSingleChar
            and     r2, 0x00ff
            bne     writeChar
            baw     writeSingleChar
        writeChar:
            orr     r2, r1
            sto     r2, ES[r5]
            adi     r5, 2
            dec     r0
            bne     writeSingleChar
        baw     Update
}

; === ClockInt =================================================================
ClockInt:
{
    psh     r0, r1, r2, r3, fl  ; save register contents    
    hwq     $80                 ; get RTC time. seconds in low 6 bits of R2.
    psh     r0, r1, r2          ; save RTC time.
    
    pop     r2
    psh     r2
    and     r2, $003f           ; r2 = hex seconds
    jsr     HexToDec            ; r0 = dec seconds
    lod     r1, $003c
    jsr     WriteDecToScreen
    
    pop     r2
    asr     r2, 8               ; r2 = hex minutes
    jsr     HexToDec            ; r0 = dec minutes
    lod     r1, $0036
    jsr     WriteDecToScreen
    
    lod     r0, $283A           ; r0 is yellow on blue colon
    sto     r0, ES[r1]
    
    pop     r2
    ;psh     r2
    and     r2, $001f
    jsr     HexToDec            ; r0 = dec hours
    lod     r1, $0030
    jsr     WriteDecToScreen
    
    lod     r0, $283A           ; r0 is yellow on blue colon
    sto     r0, ES[r1]
    
    pop     r0
    pop     r0, r1, r2, r3, fl
    rti
}

; === GetKeyboardEvents ========================================================
; gets all keyboard events, copies to $7000.
; returns: r0 is number of keyboard events.
GetKeyboardEvents:
{
    psh     r1, r2
    lod     r0, $0002
    lod     r1, $0001
    lod     r2, $7000
    hwq     $02
    
    lod     r0, [$7000]
    pop     r1, r2
    rts
}

; === Setup ====================================================================
; uses r0, r1, r2, r3.
Setup:
{
    ; r3 = calling address
    pop     r3
    
    ; set up devices
    lod     r0, $0001    ; set graphics adapter to LEM mode.
    lod     r1, $0000
    lod     r2, $0001
    hwq     $02
    lod     r0, $0002    ; reset keyboard, press events only.
    lod     r1, $0000
    lod     r2, $0000
    hwq     $02
    
    ; set up segment registers. (See 2.F.1.)
    lod     r0, $0000
    lod     r1, $0800
    psh     r1          ; ds = $0800 0000 (RAM @ $00000000, size = $8000)
    psh     r0
    add     r0, $0180
    psh     r1          ; ss = $0800 0180 (RAM @ $00018000, size = $8000)
    psh     r0
    lod     r0, $0000
    lod     r1, $8000
    psh     r1          ; is = $8000 0000 (ROM @ $00000000, size = $10000)
    psh     r0
    psh     r1          ; cs = $8000 0000 (ROM @ $00000000, size = $10000)
    psh     r0
    add     r1, $0101   ; es = $8101 0000 (device 1 @ $00000000, size = $1000)
    psh     r1           
    psh     r0
    lsg     es
    lsg     cs
    lsg     is
    lsg     ss
    lsg     ds
    
    ; enable mmu by setting 'm' bit in PS. See 2.A.2.
    lod     r0, ps
    orr     r0, 0x4000
    sto     r0, ps
    
    ; sp = $8000
    lod     r0, $8000
    sto     r0, sp
    
    ; push r3 (calling PC) to stack
    psh     r3
    
    rts
}

; === ShowStartScreen ==========================================================
; Draws the NE logo on r0 blue screen
ShowStartScreen:
{
    jsr ClearScreen
    
    ; write logo in center of screen.
    lod     r1, $8200            ; yellow on blue
    lod     r5, $00D6            ; location in video memory
    lod     r6, txtBootText
    lod     r3, 3                ; count of lines to draw
    writeLine:
        jsr WriteChars
        add     r5, 64           ; increment one line in video memory (32 words)
        dec     r3
        beq     LastLine
        bne     writeLine
    lastLine:
        add     r5, $3C          ; skip line, align left 4 chars
        jsr     WriteChars
    rts
    
    ; boot up txt - local to this routine
    txtBootText:
    .dat8 "  \\ |  ___", 0x00
    .dat8 "|\\ \\|  ___", 0x00
    .dat8 "| \\       ", 0x00
    .dat8 "NYA ELEKTRISKA", 0x00
}

; === ClearScreen ==============================================================
ClearScreen:
{
    lod     r1, $0220       ; space char with blue background
    lod     r2, 384         ; words to write
    lod     r5, $0000       ; start of video memory
    jsr     FillMemoryWords ; far call example: jsr.f FillMemoryWords, $80000000
    rts
}

; === WriteChars ===============================================================
; Writes r0 null-terminated string of 8-bit chars to video memory.
; in:   r1: color
; in:   r5: location to write to video memory.
; in:   r6: location to read 8-bit characters from. on exit, points to next byte after string.
WriteChars:
{
    psh     r0, r5              ; push r0 r5 to stack
    ssg     ds                  ; push DS register to stack
    ssg     cs
    lsg     ds                  ; DS = CS (ROM@$00000000) (reading from ROM).
    writeChar:                  ; copy chars from ds[r6] to es[r5] 
        lod.8   r0, DS[r6]      ; ... until ds[r6] == 0x00.
        beq     return
        orr     r0, r1
        sto     r0, ES[r5]
        inc     r6
        adi     r5, 2
        baw     writeChar
    return:
        inc     r6
        lsg     ds              ; pop ds register from stack
        pop     r0, r5          ; pop r0 r5 from stack
        rts
}

; === FillMemoryWords ==========================================================
; Fills range of memory with words of specified value.
; in:   r1 - word to fill memory with
; in:   r2 - count of words of memory to fill    
; in:   r5 - first address to write to in ES
FillMemoryWords:
{
    psh r2, r5                  ; save r2 and r5
    asl r2, 1                   ; r2 = count of bytes
    add r2, r5                  ; r2 = first address after all bytes are written
    copyWord:
        sto     r1, ES[r5]      ; save word in r1 to [r5], r5 = r5 + 2
        adi     r5, 2
        cmp     r5, r2          ; if r5
        bne     copyWord
    pop r2, r5                  ; restore r2 and r5
    rts
}

; === HexToDec =================================================================
; Converts two digit hex in r2 to three digit decimal in r0.
; Wipes out r2, r1, & r0.
HexToDec:
{
    lod     r0, $0000           ; r0 = 0
    lod     r1, $0064           ; r1 = 100
    getHundreds:
        cmp     r2, r1
        buf     getTensBegin    ; if r2 < 100 goto getTensBegin
        add     r0, $0100       ; else r0 += 100
        sub     r2, r1          ;   r2 -= 100
        baw     getHundreds
    getTensBegin:
        lod     r1, $000a       ; r1 = 10
    getTens:
        cmp     r2, r1
        buf     getOnes         ; if r2 < 10 goto getOnes
        add     r0, $0010       ; else r0 += 10
        sub     r2, r1
        baw     getTens
    getOnes:
        add     r0, r2
        rts
}

; === WriteDecToScreen =========================================================
; Writes two digit dec in r0 to ES at r1. Wipes out r0, r1 += 4.
WriteDecToScreen:
{
    psh     r0
    asr     r0, 4
    and     r0, $000f
    add     r0, $2830   ; yellow on blue, char $30 + r1
    sto     r0, ES[r1]
    adi     r1, 2
    pop     r0
    and     r0, $000f
    add     r0, $2830   ; yellow on blue, char $30 + r1
    sto     r0, ES[r1]
    adi     r1, 2
    rts
}