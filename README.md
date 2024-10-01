# core8 - A PDP-8/E emulator in C#

This is my first attempt to write an emulator. Ever! It is work-in-progress and might even be finished eventually.

The emulator is stable enough to boot OS/8 and run [TINT8](https://github.com/PontusPih/TINT8), the PDP-8 version of a popular tile-matching puzzle video game. You can even play [ADVENT (Colossal Cave)](https://en.wikipedia.org/wiki/Colossal_Cave_Adventure) on it.

Have fun!

## Build

To build the Emulator: 

	PS c:\...\core8> dotnet build --configuration Release

This will build the Emulator in Release mode as well as the Terminal Server. Those are separate projects in the solution. The Terminal Server is a simple console application that listens on port 23 for incoming connections and should be started before the Emulator.

## Test

There are extensive tests for the emulator as described in the MAINDEC section below. To run them from the command line, use the following command:

	PS c:\...\core8> dotnet test --configuration Release

## Run

Please note that the Emuluator is a console application. It can do many things, but it does not have a GUI. Options are described below. Most users will probably hook up Putty to the terminal server.

Why not play game of ADVENT?

### Launch the Terminal Server
	
	PS c:\...\core8\TerminalServer\bin\Release\net8.0> .\TerminalServer.exe
	[20:57:23 INF] Server starting ...

Terminal server is now running and waiting for connections.

### Connect using Putty

	PS c:\bin\putty)> .\putty.exe -telnet -raw localhost 23
	WELCOME TO THE PDP-8 TERMINAL SERVER

Terminal Server shows:

	[21:01:15 INF] Connected: 127.0.0.1:61365

### Launch the Emulator

	PS c:\...\core8\Emulator\bin\Release\net8.0\> .Emulator.exe --advent

The Emulator will boot OS/8 and start ADVENT:

	WELCOME TO THE PDP-8 TERMINAL SERVER

	.R FRTS
	*ADVENT
	*$
	Welcome to Adventure!!  Would you like instructions?

	>

Type `Y` and hit `Enter` to get instructions. Have fun!
# Test-driven development - MAINDEC

The following MAINDEC diagnostics have been tested and run successfully. 

In fact that's how I implemented the emulator. Run the tests, fix the code, ... Loop until the emulator passes for a real machine.

 * `MAINDEC-8E-D0AB-PB`: Instruction Test Part 1 
 * `MAINDEC-8E-D0BB-PB`: Instruction Test Part 2 
 * `MAINDEC-8E-D0CC-PB`: Adder Test 
 * `MAINDEC-8E-D0DB-PB`: Random AND Test 
 * `MAINDEC-8E-D0EB-PB`: Random TAD Test 
 * `MAINDEC-8E-D0FC-PB`: Random ISZ Test 
 * `MAINDEC-8E-D0GC-PB`: Random DCA Test 
 * `MAINDEC-8E-D0HC-PB`: Random JMP Test 
 * `MAINDEC-8E-D0IB-PB`: Basic JMP-JMS Test 
 * `MAINDEC-8E-D0JC-PB`: Random JMP-JMS Test 
 * `MAINDEC-8E-D1EC-PB`: Memory Address Test 
 * `MAINDEC-8E-D1FB-PB`: Extended Memory Address Test
 * `MAINDEC-8E-D1HA-PB`: Memory Extension and Time Share Control Test

# Options

	Emulator 1.0.0
	Mikael Fredriksson <micke@sictransit.net>

	--tint               (Default: false) play TINT
	
	--palbart            (Default: c:\bin\palbart.exe) PALBART executable, required for assemble
	
	--assemble           PAL assembly file
	
	--load               load bin format paper tape
	
	--run                (Default: false) run the assembled file
	
	--tty                (Default: false) dump tty output to console
	
	--startingaddress    (Default: 200) starting address
	
	--dumpmemory         (Default: false) dump memory
	
	--os8                (Default: false) boot OS/8

	--kermit             (Default: false) boot Kermit utility disk

	--advent             (Default: false) run ADVENT
	
	--debug              (Default: false) debug mode
	
	--convert            convert ASCII string to octal words
	
	--punch              punch paper tape, i.e. copy bin image
	
	--help               Display this help screen.
	
	--version            Display version information.

# PALBART

There is support for an assembler, provided you have the required PALBART binary. It can be found, or built, if you need it.

# RIM/BIN loaders

RIM and BIN loaders are already loaded into "core" memory for you convenience.

# Terminal server

In lack of a proper teletype, I implemented a terminal server of sorts. You can connect to it using e.g. [Putty](https://en.wikipedia.org/wiki/PuTTY).

# ASR33 style teletype + SVG punch

There is a working teletype (device 03/04) implementation, including an SVG paper tape punch. It is quite an anachronism, but I needed it for a [reason](https://www.geocaching.com/geocache/GC96VGE).

Of course, the SVG punch can also read SVG paper tapes. 

<img width="787" alt="image" src="https://github.com/sictransit/core8/assets/4610247/6fa1db81-ae80-4cee-94da-d3539c732fd1">

# Line Printer

I've cloned the printer part of the teletype to implement a line printer. It was necessary to boot some RK05 disks.

# Work in Progress

## RX8E/RX01

The implementation of a floppy controller + drive is very much work-in-progress. 8-bit mode is not implemented, nor is deleted sectors. Error register handling is probably also defunct. 

It is at least able to boot OS/8 from a disk image. 

<img width="331" alt="image" src="https://github.com/sictransit/core8/assets/4610247/4c1bb56b-1b49-4b16-b517-398859422e50">

## RK8E/RK05

I realized I'll need support for the cartridge disks as well, at least to be able to load and boot some of the more interesing images out there. Now you can play [ADVENT](https://en.wikipedia.org/wiki/Colossal_Cave_Adventure) on the emulator, which was actually my goal from the beginning and why I started coding the emulator.

<img width="331" alt="image" src="https://github.com/sictransit/core8/assets/4610247/cdec18cb-15a1-4347-a2c4-318c20e09f96">


# TODO

When I haven't refactored everything in a while, I might fix issues like `Type 'MQRelay' owns disposable field(s) 'publisherSocket, subscriberSocket' but is not disposable.` For now, it is not a priority! I know how and when to use the `using {}` pattern.

