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
		103	       				/C
		117	       				/O
		116	       				/N
		107	       				/G
		122	       				/R
		101	       				/A
		124	       				/T
		125	       				/U
		114	       				/L
		101	       				/A
		124	       				/T
		111	       				/I
		117	       				/O
		116	       				/N
		123	       				/S
		041	       				/!
		040	       				/ 
		131	       				/Y
		117	       				/O
		125	       				/U
		047	       				/'
		126	       				/V
		105	       				/E
		040	       				/ 
		123	       				/S
		117	       				/O
		114	       				/L
		126	       				/V
		105	       				/E
		104	       				/D
		040	       				/ 
		111	       				/I
		124	       				/T
		041	       				/!
		040	       				/ 
		116	       				/N
		117	       				/O
		127	       				/W
		040	       				/ 
		107	       				/G
		117	       				/O
		040	       				/ 
		107	       				/G
		105	       				/E
		124	       				/T
		040	       				/ 
		111	       				/I
		124	       				/T
		040	       				/ 
		100	       				/@
		040	       				/ 
		116	       				/N
		040	       				/ 
		065	       				/5
		071	       				/9
		040	       				/ 
		064	       				/4
		071	       				/9
		056	       				/.
		060	       				/0
		066	       				/6
		061	       				/1
		040	       				/ 
		105	       				/E
		040	       				/ 
		060	       				/0
		061	       				/1
		067	       				/7
		040	       				/ 
		064	       				/4
		062	       				/2
		056	       				/.
		065	       				/5
		063	       				/3
		070	       				/8
        0000                    /<EOT>
$