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
    private const int INTERRUPT_ENABLE = 1 << 0;

    private readonly ConcurrentQueue<byte> reader = new();
    private readonly SubscriberSocket subscriberSocket;

    private int deviceControl;

    public KeyboardReader(string inputAddress)
    {
        subscriberSocket = new SubscriberSocket();
        subscriberSocket.Connect(inputAddress);
        subscriberSocket.SubscribeToAnyTopic();
    }


    protected override bool InterruptEnable => (deviceControl & INTERRUPT_ENABLE) != 0;

    protected override int TickDelay => 100;


    public byte InputBuffer { get; private set; }


    public bool InputFlag { get; private set; }

    public override bool InterruptRequested => InputFlag && InterruptEnable;

    public void ClearInputFlag() => InputFlag = false;

    public void SetDeviceControl(int data)
    {
        deviceControl = data & INTERRUPT_ENABLE;
    }

    public void Clear()
    {
        SetDeviceControl(0b_000_000_000_011);

        reader.Clear();

        Ticks = 0;
    }

    public void Type(byte c)
    {
        reader.Enqueue(c);
    }

    public void Type(byte[] buffer)
    {
        foreach (byte b in buffer)
        {
            Type(b);
        }
    }

    public void MountPaperTape(byte[] chars)
    {
        if (chars is null)
        {
            throw new ArgumentNullException(nameof(chars));
        }

        reader.Clear();

        foreach (byte c in chars)
        {
            reader.Enqueue(c);
        }
    }

    public void RemovePaperTape()
    {
        reader.Clear();
    }

    protected override void HandleTick()
    {
        HandleInput();
    }

    private void SetInputFlag() => InputFlag = true;


    private void HandleInput()
    {
        if (InputFlag)
        {
            return;
        }

        while (subscriberSocket.TryReceiveFrameBytes(TimeSpan.Zero, out byte[] frame))
        {
            foreach (byte key in frame)
            {
                byte uppercaseByte = Convert.ToByte(char.ToUpperInvariant(Convert.ToChar(key)));

                reader.Enqueue(uppercaseByte);
            }
        }

        if (reader.TryDequeue(out byte b))
        {
            Log.Debug($"Input: {b.ToPrintableAscii()}");

            InputBuffer = b;

            SetInputFlag();
        }
    }
}