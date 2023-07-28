# core8 - A PDP-8/E emulator in C#

This is my first attempt to write an emulator. Ever! 

It is work-in-progress and might even be finished eventually.

# TDD - MAINDEC

The following MAINDEC diagnostics have been tested and run successfully. In fact that's how I implemented the emulator. Run the tests, fix the code, ... Loop until the emulator passes for a real machine.
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
 
As a bonus, the emulator is stable enough to run [TINT8](https://github.com/PontusPih/TINT8), the PDP-8 version of a popular tile-matching puzzle video game.

# Terminal server

In lack of a proper teletype, I implemented a terminal server of sorts. You can connect to it using e.g. [Putty](https://en.wikipedia.org/wiki/PuTTY).

# ASR33 style teletype + SVG punch

There is a working teletype (device 03/04) implementation, including an SVG paper tape punch. It is quite an anachronism, but I needed it for a [reason](https://www.geocaching.com/geocache/GC96VGE).

Of course, the SVG punch can also read SVG paper tapes. 

<img width="787" alt="image" src="https://github.com/sictransit/core8/assets/4610247/6fa1db81-ae80-4cee-94da-d3539c732fd1">

# Work in Progress

## RX8E/RX01

The implementation of a floppy controller + drive is very much work-in-progress. 8-bit mode is not implemented, nor is deleted sectors. Error register handling is probably also defunct. 

It is at least able to boot OS/8 from a disk image. 

<img width="331" alt="image" src="https://github.com/sictransit/core8/assets/4610247/4c1bb56b-1b49-4b16-b517-398859422e50">

## RK8E

Not started, but when the RX floppies started working, I realized I'll need support for the fixed platters as well. Probably a top TODO.

# TODO

When I haven't refactored everything in a while, I might fix issues like `	Type 'MQRelay' owns disposable field(s) 'publisherSocket, subscriberSocket' but is not disposable.` For now, it is not a priority! I know how and when to use the `using {}` pattern.

