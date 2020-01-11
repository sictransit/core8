﻿namespace Core8.Model.Interfaces
{
    public interface ITeleprinter : IIODevice
    {
        string Printout { get; }

        void FormFeed();
    }
}
