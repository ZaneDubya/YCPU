; TEST CONSOLE
; Expects Graphics Device in bus index 1 and Keyboard in bus index 2

; === interrupt table =========================================================
.dat16 Start,  0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000
.dat16 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000


; === WriteChars ==============================================================
; Writes a string of chars to video memory.
; in:   b: color
; in:   c: count of chars to write.
; in:   x: location to write to video memory.
; in:   y: location to read 8-bit characters from.
WriteChars:
.scope
    psh a, c, x, y              ; push a c x  to the stack
    add c, y                    ; c = y + c
    _writeChar:                 ; copy c chars from y to x
        lod.8   a, [y+]
        orr     a, b
        sto     a, [x+]
        cmp     c, y
        bne     _writeChar
    pop a, c, x, y              ; pop a c x y from the stack
    rts
.scend

; === FillMemoryWords =========================================================
; Fills range of memory with words of specified value.
; in:   b - word to fill memory with
; in:   c - count of memory to fill    
; in:   x - first address to write to
FillMemoryWords:
.scope
    psh c, x
    asl c, 1
    add c, x
    _copyWord:
        sto     b, [x+]
        cmp     x, c
        bne     _copyWord
    pop c, x
    rts
.scend

; === Start ===================================================================
Start:
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
    
    ; set mmu to load graphics adaptor memory in bank $08
    lod     a, $10      ; mmu cache index 16 (bank $08, word 0)
    lod     b, $1000    ; device 01, bank 0
    mmw     a, b
    inc     a
    lod     b, $0000    ; high bits of device 01, no features
    mmw     a, b

    ; enable mmu
    tsr     a, ps
    orr     a, 0x4000   ; mmu bit is 0x4000
    trs     a, ps
    
    ; clear screen
    lod     b, $0220    ; space char with blue background
    lod     c, 384      ; words to write
    lod     x, $8000    ; start of video memory
    jsr     FillMemoryWords
    
    ; write logo in center of screen.
    lod     b, $8280            ; yellow on blue
    lod     x, $80D6            ; location in video memory
    lod     y, txtBootText
    lod     i, 3                ; count of lines to draw
    lod     c, 10
    _writeLine:
        jsr WriteChars
        add     y, 10           ; increment ten chars in txtBootText
        add     x, 64           ; increment one line in video memory (32 words)
        dec     i
        beq     _LastLine
        bne     _writeLine
    _lastLine:
        add     x, $3C ; skip line, left 4 chars
        add     c, 4
        jsr     WriteChars
    _infiniteLoop:
        jmp _infiniteLoop
.scend

; boot up txt
txtBootText:
.dat8 "  \ |  ___"
.dat8 "|\ \|  ___" 
.dat8 "| \       "
.dat8 "NYA ELEKTRISKA"