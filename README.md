# core8

My first attempt to write an emulator. Ever! 

This is work in progress and might even be finished eventually.

The following MAINDEC diagnostics have been tested and run successfully:
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

32 kW extended memory is still in early alpha, so related tests are failing spectacularly.
 
As a bonus, the emulator is stable enough to run [TINT8](https://github.com/PontusPih/TINT8), the PDP-8 version of a popular tile-matching puzzle video game.

## TODO

When I haven't refactored everything in a while, I might fix issues like `	Type 'MQRelay' owns disposable field(s) 'publisherSocket, subscriberSocket' but is not disposable.` For now, it is not a priority! I know how and when to use the `using {}` pattern.

