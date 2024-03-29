﻿using Core8.Extensions;
using Core8.Model.Registers.Abstract;

namespace Core8.Model.Registers;

public class DataField : RegisterBase
{
    protected override string ShortName => "DF";

    public override void Set(int value) => Content = value & 0b_111;

    public override string ToString()
    {
        return $"[{ShortName}] {Content.ToOctalString(1)}";
    }
}
