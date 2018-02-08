﻿using System;

namespace Alligator.Solver
{
    public interface IPosition<TPly>
    {
        ulong Identifier { get; }
        bool IsOver { get; }
        bool HasWinner { get; }  
        bool IsQuiet { get; }       
        void Do(TPly ply);
        void Undo();
    }
}