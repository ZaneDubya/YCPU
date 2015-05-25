; TEST CONSOLE
; Expects Graphics Device in bus index 1 and Keyboard in bus index 2

; === Interrupt table =========================================================
.dat16 Start,  0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000
.dat16 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000

; === Start ===================================================================
Start:
.scope
    ;set up devices, mmu, etc.
    jsr     Setup
    ; show the 'NYA ELEKTRISKA' screen
    jsr     ShowStartScreen
    
    lod     x, $8000
    _checkForKB:
        jsr     GetKeyboardEvents
        cmp     a, 0
        beq     _checkForKB
    
        lod     b, $2800            ; yellow on blue
        lod     y, $7002            ; get first char
        _writeSingleChar:
            lod     c, [y+]
            and     c, 0x00ff
            cmp     c, 0x0D
            bne     _writeChar
            baw     _writeSingleChar
        _writeChar:
            orr     c, b
            sto     c, [x+]
            dec     a
            bne     _writeSingleChar
        baw     _checkForKB
.scend

; === GetKeyboardEvents =======================================================
; gets all keyboard events, copies to $7000.
; returns: a is number of keyboard events.
GetKeyboardEvents:
.scope
    psh     b, c
    lod     a, $0002
    lod     b, $0001
    lod     c, $7000
    hwq     $02
    
    lod     a, [$7000]
    pop     b, c
    rts
.scend

; === Setup ===================================================================
Setup:
.scope
    ; set up devices
    lod     a, $0001    ; set graphics adapter to LEM mode.
    lod     b, $0000
    lod     c, $0001
    hwq     $02
    lod     a, $0002    ; reset keyboard, press events only.
    lod     b, $0000
    lod     c, $0000
    hwq     $02
    
    ; set mmu to load graphics adaptor memory in supervisor bank $02
    lod     a, $0C      ; mmu cache index 8 (bank $02, word 0)
    lod     b, $0100    ; device 01, bank 0
    mmw     a, b
    inc     a
    lod     b, $0000    ; no protection enabled for this bank.
    mmw     a, b

    ; set mmu bank 0 to cpu rom.
    lod     a, $08
    lod     b, $0080
    mmw     a, b
    inc     a
    lod     b, $0000
    mmw     a, b
    
    ; enable mmu
    lod     a, ps
    orr     a, 0x4000   ; mmu bit is 0x4000
    sto     a, ps
    rts
.scend

; === ShowStartScreen =========================================================
; Draws the NE logo on a blue screen
ShowStartScreen:
    ; clear screen
    lod     b, $0220    ; space char with blue background
    lod     c, 384      ; words to write
    lod     x, $8000    ; start of video memory
    jsr     FillMemoryWords
    
    ; write logo in center of screen.
    lod     b, $8200            ; yellow on blue
    lod     x, $80D6            ; location in video memory
    lod     y, txtBootText
    lod     i, 3                ; count of lines to draw
    _writeLine:
        jsr WriteChars
        add     x, 64           ; increment one line in video memory (32 words)
        dec     i
        beq     _LastLine
        bne     _writeLine
    _lastLine:
        add     x, $3C ; skip line, left 4 chars
        add     c, 4
        jsr     WriteChars
    rts

; === WriteChars ==============================================================
; Writes a null-terminated string of 8-bit chars to video memory.
; in:   b: color
; in:   x: location to write to video memory.
; in:   y: location to read 8-bit characters from. on exit, points to next byte after string.
WriteChars:
.scope
    psh a, x                    ; push a x  to the stack
    _writeChar:                 ; copy c chars from y to x
        lod.8   a, [y+]
        beq _return
        orr     a, b
        sto     a, [x+]
        baw     _writeChar
    _return:
        pop a, x                 ; pop a x from the stack
        rts
.scend

; === FillMemoryWords =========================================================
; Fills range of memory with words of specified value.
; in:   b - word to fill memory with
; in:   c - count of words of memory to fill    
; in:   x - first address to write to
FillMemoryWords:
.scope
    psh c, x                    ; save c and x
    asl c, 1                    ; c = count of bytes
    add c, x                    ; c = first address after all bytes are written
    _copyWord:
        sto     b, [x+]         ; save word in b to [x], x = x + 2
        cmp     x, c            ; if x
        bne     _copyWord
    pop c, x                    ; restore c and x
    rts
.scend

; boot up txt
txtBootText:
.dat8 "  \\ |  ___", 0x00
.dat8 "|\\ \\|  ___", 0x00
.dat8 "| \\       ", 0x00
.dat8 "NYA ELEKTRISKA", 0x00