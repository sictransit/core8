/PDP-8 PAL ASSEMBLY LANGUAGE PROGRAM FOR A MYSTERY GEOCACHE!

        *200                    /LOCATE @ 0200
        AIX1=10                 /SETUP AIX REGISTER 1
        AIX2=11                 /SETUP AIX REGISTER 2
        
        CLA                     /CLEAR ACC
        CLL                     /CLEAR LINK
        TAD MSG					/LOAD 1ST WRD OF MSG (ADDR OF MSG)
        DCA AIX1                /STORE IN AUTOINDEX REG 1

        CLA                     /CLEAR ACC
        CLL                     /CLEAR LINK
        TAD MASK                /LOAD MASK
        DCA AIX2                /STORE THAT IN AUTOINDEX REG 2

LOOP,   NOP                     /READ AND PRINT
        TAD I AIX1              /INCR AIX1, THEN LOAD ACC FROM THAT
        SNA                     /IF AC NOT ZERO, SKIP 
        HLT                     /EXIT 
        JMS TTYO                /TTY OUTPUT
        JMP LOOP                /REPEAT

TTYO,   0                       /TTY OUTPUT SUB-ROUTINE.
                                /RETURN ADDR GOES HERE!
        TLS                     /WRITE ACC TO TTY
        TSF                     /IF TTY IS READY, SKIP 
        JMP .-1                 /TTY IS NOT READY, SO CHECK AGAIN
        CLA                     /CLEAR ACC
        JMP I TTYO              /RETURN 

MASK,   1723                    /SUPER SECRET BIT MASK

MSG,	.                       /1ST WORD IS ADDR OF STRING
		0103       				/C
		0117       				/O
		0116       				/N
		0107       				/G
		0122       				/R
		0101       				/A
		0124       				/T
		0125       				/U
		0114       				/L
		0101       				/A
		0124       				/T
		0111       				/I
		0117       				/O
		0116       				/N
		0123       				/S
		0041       				/!
		0040       				/ 
		0131       				/Y
		0117       				/O
		0125       				/U
		0047       				/'
		0126       				/V
		0105       				/E
		0040       				/ 
		0123       				/S
		0117       				/O
		0114       				/L
		0126       				/V
		0105       				/E
		0104       				/D
		0040       				/ 
		0111       				/I
		0124       				/T
		0041       				/!
		0040       				/ 
		0116       				/N
		0117       				/O
		0127       				/W
		0040       				/ 
		0107       				/G
		0117       				/O
		0040       				/ 
		0107       				/G
		0105       				/E
		0124       				/T
		0040       				/ 
		0111       				/I
		0124       				/T
		0040       				/ 
		0100       				/@
		0040       				/ 
		0116       				/N
		0040       				/ 
		0065       				/5
		0071       				/9
		0040       				/ 
		0064       				/4
		0071       				/9
		0056       				/.
		1060       				/0
		1066       				/6
		1061       				/1
		0040       				/ 
		0105       				/E
		0040       				/ 
		0060       				/0
		0061       				/1
		0067       				/7
		0040       				/ 
		0064       				/4
		0062       				/2
		0056       				/.
		1065       				/5
		1063       				/3
		1070       				/8
        0000					/<EOT>
$