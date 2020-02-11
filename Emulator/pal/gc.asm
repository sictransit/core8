/PDP-8 PAL ASSEMBLY LANGUAGE PROGRAM FOR A MYSTERY GEOCACHE

		AIX1=10					/SETUP AIX1

		*20						/GLOBALS

MYDTA,	0053					/MYST DATA
MYMSK,	0001					/MYST MASK
MYCHR,	0000					/MYST CHARACTER

		*200					/LOCATE @ 0200

		CLA CLL					/CLEAR ACC, CLEAR LINK
		TAD MSG					/LOAD ADDRESS OF MYST MESSAGE
		DCA AIX1				/DUMP IN AUTOINDEX REGISTER #1

		CLA CLL					/CLEAR ACC, CLEAR LINK

LOOP,							/READ AND PRINT LOOP
		TAD I AIX1				/INCREMENT AIX1, THEN LOAD ACC FROM THAT LOCATION
		SNA						/AC != 0? SKIP 
		HLT						/EXIT 
		SPA						/POSITIVE AC? SKIP
		JMS TWK					/TWEAK!
		JMS TTYO				/TTY OUTPUT
		JMP LOOP				/LOOP

TWK,	0						/MYST SUB-ROUTINE
		DCA MYCHR				/SAVE CHAR IN ACC
		TAD MYDTA				/LOAD MYST DATA
		RAR						/ROTATE ACC RIGHT
		DCA MYDTA				/STORE TWEAK DATA
		TAD MYDTA				/STORE IN ACC
		AND MYMSK				/KEEP LSB
		TAD MYCHR				/ADD SAVED CHAR
		JMP I TWK				/RETURN	

TTYO,	0						/TTY OUTPUT SUB-ROUTINE
		TLS						/WRITE ACC TO TTY
		TSF						/TTY READY? SKIP 
		JMP .-1					/CHECK AGAIN
		CLA						/CLEAR ACC
		JMP I TTYO				/RETURN 

MSG,	.						/POINTER TO MYST STRING
		0103					/C	
		0117					/O
		0116					/N
		0107					/G
		0122					/R
		0101					/A
		0124					/T
		0125					/U
		0114					/L
		0101					/A
		0124					/T
		0111					/I
		0117					/O
		0116					/N
		0123					/S
		0041					/!
		0040					/ 
		0131					/Y
		0117					/O
		0125					/U
		0047					/'
		0126					/V
		0105					/E
		0040					/ 
		0123					/S
		0117					/O
		0114					/L
		0126					/V
		0105					/E
		0104					/D
		0040					/ 
		0111					/I
		0124					/T
		0041					/!
		0040					/ 
		0116					/N
		0117					/O
		0127					/W
		0040					/ 
		0107					/G
		0117					/O
		0040					/ 
		0107					/G
		0105					/E
		0124					/T
		0040					/ 
		0111					/I
		0124					/T
		0040					/ 
		0100					/@
		0040					/ 
		0116					/N
		0040					/ 
		0065					/5
		0071					/9
		0040					/ 
		0064					/4
		0071					/9
		0056					/.
		7060					/0
		7066					/6
		7061					/1
		0040					/ 
		0105					/E
		0040					/ 
		0060					/0
		0061					/1
		0067					/7
		0040					/ 
		0064					/4
		0062					/2
		0056					/.
		7065					/5
		7063					/3
		7070					/8
		0000					/<EOT>
$