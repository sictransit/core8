using Core8.Abstract;
using Core8.Model.Interfaces;
using NetMQ;
using NetMQ.Sockets;
using Serilog;
using System;
using System.Text;

namespace Core8
{
    public class Teleprinter : IODevice, ITeleprinter
    {
        private readonly StringBuilder paper = new StringBuilder();
        private readonly PublisherSocket publisherSocket;

        public Teleprinter(uint id) : base(id)
        {
            publisherSocket = new PublisherSocket();

            publisherSocket.Connect(@"tcp://127.0.0.1:17233");
        }

        public string Printout => paper.ToString();

        public override void Tick()
        {
            if (!IsFlagSet && Queue.TryDequeue(out var item))
            {
                var data = new[] { item };

                if (!publisherSocket.TrySendFrame(data))
                {
                    Log.Debug("Failed to send frame.");
                }


                var c = Encoding.ASCII.GetChars(data)[0];
                paper.Append(c);

                SetFlag();
            }
        }
    }
}
