/PDP-8 PAL ASSEMBLY LANGUAGE PROGRAM TO PRINT HELLO WORLD!

        *200                    /LOCATE PROGRAM STARTING AT ADDR 200
        AIX1=10                 /SETUP AUTOINDEX REGISTER 1

        CLA                     /CLEAR ACCUMULATOR
        CLL                     /CLEAR AC LINK
        TAD CHRSTR              /LOAD 1ST WRD OF CHRSTR (WHICH IS
                                /THE ADDR OF CHRSTR)
        DCA AIX1                /STORE THAT IN AUTOINDEX REG 1

LOOP,   NOP                     /TOP OF LOOP TO READ AND PRINT STRING
                                /I USE A NOP JUST TO MAKE IT EASIER TO
                                /INSERT CODE BELOW THE LABEL.
        TAD I AIX1              /INCR ADDR IN AIX1, THEN LOAD AC FROM THAT
        SNA                     /IF AC IS NOT ZERO, SKIP NEXT INSTRUCTION
        HLT                     /EXIT PROGRAM (BACK TO MONITOR)
        JMS TTYO                /CALL OUTPUT ROUTINE
        JMP LOOP                /REPEAT LOOP

TTYO,   0                       /TTY OUTPUT ROUTINE. THE FIRST WORD OF
                                /A SUBROUTINE MUST BE EMPTY (0) BECAUSE
                                /THE JMS INSTRUCTION INSERTS THE RETURN
                                /ADDR IN THIS WORD.
        TLS                     /WRITE AC TO THE OUTPUT DEVICE (TTY)
        TSF                     /IF TTY IS READY, SKIP NEXT INSTRUCTION.
        JMP .-1                 /TTY IS NOT READY, SO CHECK AGAIN
        CLA                     /CLEAR AC
        JMP I TTYO              /RETURN TO CALLER

CHRSTR, .                       /1ST WORD IS ADDR OF STRING
        110                     /H
        105                     /E
        114                     /L
        114                     /L
        117                     /O
        040                     /
        127                     /W
        117                     /O
        122                     /R
        114                     /L
        104                     /D
        041                     /!
        000                     /<EOT>
$