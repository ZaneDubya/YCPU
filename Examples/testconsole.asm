; TEST CONSOLE
; Tests segments and mmu, interrupts, etc.
; Expects Graphics Device @ bus index 1, Keyboard @ index 2, at least 128kb RAM.

.alignglobals 2                 ; align global labels to 16-bit boundaries
.alias  KeyboardData    $7000
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
    
    ; draw the 'NYA ELEKTRISKA' screen, then clear
    jsr     ShowStartScreen
    jsr     ClearScreen
    ; draw top bar on screen
    lod     r1, $2820       ; space char with yellow background
    lod     r2, 32          ; words to write
    lod     r5, $0000       ; start of video memory
    jsr     FillMemoryWords ;
    
    ; enable clock interrupt - tick at 120hz
    lod     r0, 120
    hwq     $83
    
    ; use r5 as index to onscreen char, starting at y = 1, x = 0
    lod     r5, $0040
    
    Update:
        jsr     Getc                ; R0 = event, or 0x0000 if no event.
        beq     Update
        
        lod     r2, r0              ; r2 = event type
        lsr     r2, 8
        and     r2, 0x000f
        cmp     r2, 3
        bne     Update
        and     r0, 0x00ff
        orr     r0, 0x2800          ; yellow on blue.
        sto     r0, ES[r5]
        adi     r5, 2
        baw     Update
}

; === ClockInt =================================================================
ClockInt:
{
    psh     r0, r1, r2, r3, fl  ; save register contents    
    hwq     $80                 ; get RTC time. seconds in low 6 bits of R2.
    psh     r0, r1, r2          ; save RTC time.
    
    and     r2, $003f           ; r2 = hex seconds
    jsr     HexToDec            ; r0 = decimal seconds
    lod     r1, $003c
    jsr     WriteDecToScreen
    
    pop     r2
    asr     r2, 8               ; r2 = hex minutes
    jsr     HexToDec            ; r0 = decimal minutes
    lod     r1, $0036
    jsr     WriteDecToScreen
    
    lod     r0, $283A           ; r0 = yellow on blue colon
    sto     r0, ES[r1]
    
    pop     r2
    ;psh     r2
    and     r2, $001f
    jsr     HexToDec            ; r0 = decimal hours
    lod     r1, $0030
    jsr     WriteDecToScreen
    
    lod     r0, $283A           ; r0 = yellow on blue colon
    sto     r0, ES[r1]
    
    pop     r0
    pop     r0, r1, r2, r3, fl
    rti
}

; === Getc =====================================================================
; r0 should be index of file descriptor. right now, does not matter, we read
; only from keyboard. Returns ascii key code in r0.8, 0x00 if no key code.
Getc:
{
    psh     r1, r2
GetCAgain:
    lod.8   r1, [KeyboardData+0]    ; r1 = number of events in buffer
    lod.8   r2, [KeyboardData+1]    ; r2 = last handled event
    cmp     r1, r2                  ; if r1 == r2, get new events.
    beq     getKeyboardEvents
    inc     r2
    sto.8   r2, [KeyboardData+1]    
    asl     r2, 1
    lod     r0, [KeyboardData,r2]
return:
    pop     r1, r2
    rts
    ; === GetKeyboardEvents ====================
    ; gets all keyboard events, copies to $7000.
    ; returns: r0 is number of keyboard events.
    getKeyboardEvents:
        lod     r0, $0002
        lod     r1, $0001
        lod     r2, KeyboardData
        hwq     $02
        lod     r0, [KeyboardData]
        bne     GetCAgain       ; if events, then do Getc again
        baw     return          ; else return, r0 == 0x0000
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
    psh     r0, r2, r5              ; save r0, r2, r5
    asl     r2, 1                   ; r2 = count of bytes
    add     r2, r5                  ; r2 = first address after all bytes are written
    copyWordOneAtATime:
        sto     r1, ES[r5]          ; save word in r1 to [r5], r5 = r5 + 2
        adi     r5, 2
        cmp     r5, r2              ; if r5
        bne     copyWordOneAtATime
    pop     r0, r2, r5              ; restore r0, r2, r5
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
; Writes two digit dec in r0.8 to ES at r1. Wipes out r0, r1 += 4.
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