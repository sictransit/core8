﻿using Core8.Extensions;
using Core8.Model;
using Core8.Model.Interfaces;
using NetMQ;
using NetMQ.Sockets;
using Serilog;
using System;
using System.Collections.Concurrent;

namespace Core8.Peripherals.Teletype;

public class KeyboardReader : IODevice, IKeyboardReader
{
    private readonly SubscriberSocket subscriberSocket;

    private const int INTERRUPT_ENABLE = 1 << 0;

    private int deviceControl;

    private readonly ConcurrentQueue<byte> reader = new();


    protected override bool InterruptEnable => (deviceControl & INTERRUPT_ENABLE) != 0;


    public byte InputBuffer { get; private set; }


    public bool InputFlag { get; private set; }

    public override bool InterruptRequested => InputFlag && InterruptEnable;

    public KeyboardReader(string inputAddress)
    {
        subscriberSocket = new SubscriberSocket();
        subscriberSocket.Connect(inputAddress);
        subscriberSocket.SubscribeToAnyTopic();
    }

    public void ClearInputFlag() => InputFlag = false;

    protected override int TickDelay => 100;

    public void SetDeviceControl(int data)
    {
        deviceControl = data & INTERRUPT_ENABLE;
    }

    public void Clear()
    {
        SetDeviceControl(0b_000_000_000_011);

        //TODO: Clear Output as well?
        reader.Clear();

        Ticks = 0;
    }

    public void Type(byte c)
    {
        reader.Enqueue(c);
    }

    public void Type(byte[] buffer)
    {
        foreach (var b in buffer)
        {
            Type(b);
        }
    }

    protected override void HandleTick()
    {
        HandleInput();
    }

    public void MountPaperTape(byte[] chars)
    {
        if (chars is null)
        {
            throw new ArgumentNullException(nameof(chars));
        }

        reader.Clear();

        foreach (var c in chars)
        {
            reader.Enqueue(c);
        }
    }

    public void RemovePaperTape()
    {
        reader.Clear();
    }

    private void SetInputFlag() => InputFlag = true;


    private void HandleInput()
    {
        if (InputFlag)
        {
            return;
        }

        while (subscriberSocket.TryReceiveFrameBytes(TimeSpan.Zero, out var frame))
        {
            foreach (var key in frame)
            {
                var uppercaseByte = Convert.ToByte(char.ToUpperInvariant(Convert.ToChar(key)));

                reader.Enqueue(uppercaseByte);
            }
        }

        if (reader.TryDequeue(out var b))
        {
            Log.Debug($"Input: {b.ToPrintableAscii()}");

            InputBuffer = b;

            SetInputFlag();
        }
    }

}