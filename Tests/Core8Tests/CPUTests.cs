using Core8.Extensions;
using Core8.Model;
using Core8.Tests.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using System.Linq;

namespace Core8.Tests;

[TestClass]
public class CPUTests : PDPTestsBase
{
    [TestMethod]
    public void TestHelloWorld()
    {
        PDP.Load8(0200);

        PDP.Deposit8(7200);
        PDP.Deposit8(7100);
        PDP.Deposit8(1220);
        PDP.Deposit8(3010);
        PDP.Deposit8(7000);
        PDP.Deposit8(1410);
        PDP.Deposit8(7450);
        PDP.Deposit8(7402); //5577
        PDP.Deposit8(4212);
        PDP.Deposit8(5204);
        PDP.Deposit8(0000);
        PDP.Deposit8(6046);
        PDP.Deposit8(6041);
        PDP.Deposit8(5214);
        PDP.Deposit8(7200);
        PDP.Deposit8(5612);
        PDP.Deposit8(0220);
        PDP.Deposit8(0110);
        PDP.Deposit8(0105);
        PDP.Deposit8(0114);
        PDP.Deposit8(0114);
        PDP.Deposit8(0117);
        PDP.Deposit8(0040);
        PDP.Deposit8(0127);
        PDP.Deposit8(0117);
        PDP.Deposit8(0122);
        PDP.Deposit8(0114);
        PDP.Deposit8(0104);
        PDP.Deposit8(0041);
        PDP.Deposit8(0000);

        //pdp.Load8(0177);
        //pdp.Deposit8(7600);

        //PDP.DumpMemory();

        PDP.Clear();

        PDP.Load8(0200);
        PDP.Continue();

        Assert.AreEqual("HELLO WORLD!", PDP.CPU.PrinterPunch.Printout);

        Log.Information(PDP.CPU.PrinterPunch.Printout);
    }

    [TestMethod]
    public void TestBreakpointAtPC()
    {
        PDP.CPU.Memory.Clear();
        PDP.CPU.SetBreakpoint(new Breakpoint(10));

        PDP.Load8(0);
        PDP.Continue();

        Assert.AreEqual(8, PDP.CPU.Registry.PC.Address);
    }

    [TestMethod]
    public void TestBreakpointSingleStep()
    {
        PDP.CPU.Memory.Clear();
        PDP.CPU.SetBreakpoint(new Breakpoint());

        PDP.Load8(0);

        PDP.Continue();
        Assert.AreEqual(1, PDP.CPU.Registry.PC.Address);

        PDP.Continue();
        Assert.AreEqual(2, PDP.CPU.Registry.PC.Address);
    }

    [TestMethod]
    public void TestBreakpointDelay()
    {
        PDP.CPU.Memory.Clear();
        PDP.CPU.SetBreakpoint(new Breakpoint(10) { Delay = 8 });

        PDP.Load8(0);
        PDP.Continue();

        Assert.AreEqual(20.ToDecimal(), PDP.CPU.Registry.PC.Address);
    }

    [TestMethod]
    public void TestBreakpointChain()
    {
        PDP.CPU.Memory.Clear();

        var parent = new Breakpoint(10) { Delay = 8 };
        var child = new Breakpoint(cpu => cpu.Registry.AC.Accumulator == 0)
        {
            Parent = parent,
            Delay = 8
        };

        PDP.CPU.SetBreakpoint(parent);
        PDP.CPU.SetBreakpoint(child);

        PDP.Load8(0);
        PDP.Continue();

        Assert.AreEqual(20.ToDecimal(), PDP.CPU.Registry.PC.Address);
    }


    [TestMethod]
    public void TestBreakpointMaxHits()
    {
        PDP.CPU.Memory.Clear();

        var breaks = Enumerable.Range(1, 10).ToList();
        breaks.Add(7777.ToDecimal());

        PDP.CPU.SetBreakpoint(new Breakpoint(cpu => cpu.Instruction.Data == 0) { MaxHits = 10 });
        PDP.CPU.SetBreakpoint(new Breakpoint(7777));

        PDP.Load8(0);

        var hit = 0;

        while (hit < breaks.Count)
        {
            PDP.Continue();

            Assert.AreEqual(breaks[hit], PDP.CPU.Registry.PC.Content);

            hit++;
        }
    }
}
