/PDP-8 PAL ASSEMBLY LANGUAGE PROGRAM FOR A MYSTERY GEOCACHE

        AIX1=10             /SETUP AIX1

        *20                 /GLOBALS

MTDATA, 6204                /MYST DATA
MTCHAR, 0000                /MYST CHARACTER

        *200                /LOCATE @ 0200

        CLA CLL             /CLEAR ACC, CLEAR LINK
        TAD MSG             /LOAD ADDRESS OF MYST MESSAGE
        DCA AIX1            /DUMP IT IN AUTOINDEX REGISTER #1

LOOP,                       /READ AND PRINT LOOP
        TAD I AIX1          /INCR AIX1, THEN LOAD ACC FROM IT
        SNA                 /AC != 0? SKIP! 
        HLT                 /WE'VE REACHED EOT, SO HALT 
        SPA                 /POSITIVE AC? SKIP!
        JMS MYST            /PERFORM MYST MAGIC
        JMS TTYO            /TTY OUTPUT
        JMP LOOP            /LOOP

MYST,   0                   /MYST SUB-ROUTINE
        DCA MTCHAR          /TEMP STORE CHAR 
        TAD MTDATA          /LOAD MYST DATA

        SMA                 /BIT 0 SET? TIME FOR MAGIC!
        JMP .+3             /APPARENTLY NOT
        ISZ MTCHAR          /DO MAGIC!
        ISZ MTCHAR          /MORE MAGIC!

        RAL                 /ROTATE LEFT
        SPA                 /BIT 0 CLEAR? NO MAGIC!
        ISZ MTCHAR          /DO MAGIC!

        RAL                 /ROTATE LEFT
        DCA MTDATA          /AND STORE

        TAD MTCHAR          /LOAD TEMP STORED CHAR
        JMP I MYST          /RETURN    

TTYO,   0                   /TTY OUTPUT SUB-ROUTINE
        TLS                 /WRITE ACC TO TTY
        TSF                 /TTY READY? SKIP! 
        JMP .-1             /CHECK AGAIN
        CLA                 /CLEAR ACC
        JMP I TTYO          /RETURN 

MSG,    .                   /POINTER TO MYST STRING
        0103                /C    
        0117                /O
        0116                /N
        0107                /G
        0122                /R
        0101                /A
        0124                /T
        0125                /U
        0114                /L
        0101                /A
        0124                /T
        0111                /I
        0117                /O
        0116                /N
        0123                /S
        0041                /!
        0040                / 
        0131                /Y
        0117                /O
        0125                /U
        0047                /'
        0126                /V
        0105                /E
        0040                / 
        0123                /S
        0117                /O
        0114                /L
        0126                /V
        0105                /E
        0104                /D
        0040                / 
        0111                /I
        0124                /T
        0041                /!
        0040                / 
        0116                /N
        0117                /O
        0127                /W
        0040                / 
        0107                /G
        0117                /O
        0040                / 
        0107                /G
        0105                /E
        0124                /T
        0040                / 
        0111                /I
        0124                /T
        0040                / 
        0100                /@
        0040                / 
        0116                /N
        0040                / 
        0065                /5
        0071                /9
        0040                / 
        0064                /4
        0071                /9
        0056                /.
        7060                /0
        7060                /0
        7060                /0
        0040                / 
        0105                /E
        0040                / 
        0060                /0
        0061                /1
        0067                /7
        0040                / 
        0064                /4
        0062                /2
        0056                /.
        7060                /0
        7060                /0
        7060                /0
        0000                /<EOT>
$