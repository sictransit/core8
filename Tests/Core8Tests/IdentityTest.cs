using Core8.Tests.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;

namespace Core8.Tests;

[TestClass]
public class IdentityTest : PDPTestsBase
{
    [TestMethod]
    public void TestIdentity()
    {
        PDP.Clear();

        // IDENT.bin is the MACHINE IDENTIFYING ROUTINE in KERMIT-12:

        // COPYRIGHT(C) 1989, 1990 BY THE TRUSTEES OF COLUMBIA UNIVERSITY IN THE CITY OF
        // NEW YORK.

        // PERMISSION IS GRANTED  TO ANY  INDIVIDUAL OR INSTITUTION TO COPY OR USE THIS
        // DOCUMENT AND THE  PROGRAM(S) DESCRIBED IN IT, EXCEPT FOR EXPLICITLY COMMERCIAL
        // PURPOSES.

        PDP.LoadPaperTape(File.ReadAllBytes(@"tapes\IDENT.bin"));

        PDP.Load8(0200);

        PDP.Continue();

        var acc = PDP.CPU.Registry.AC.Accumulator;

        var machine = acc switch
        {
            0 => "UNKNOWN (DCC112, MP-12?)",
            1 => "PDP-5 (THE INCOMPATIBLE ONE!)",
            2 => "PDP-8 (THE REAL ONE!)",
            3 => "PDP-8/S (THE SLOW ONE!)",
            4 => "LINC-8 (THE STRANGE ONE!)",
            5 => "PDP-8/I (THE ORANGE ONE!)",
            6 => "PDP-8/L (THE STRIPPED-DOWN ONE!)",
            7 => "PDP-12 (THE GREEN ONE! (BLUE?))",
            8 => "PDP-8/E (THE FAST ONE!)",
            9 => "PDP-8/A (THE WIDE ONE!)",
            10 => "6100 (THE MICRO ONE!)",
            11 => "6120 (THE HARRIS ONE!)",
            _ => throw new NotImplementedException(),
        };

        Trace.WriteLine(machine);

        Assert.AreEqual(8, acc);
    }
}
