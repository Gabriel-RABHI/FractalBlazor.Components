using System;
using System.Collections.Generic;
using System.Text;

namespace FractalBlazor.Components.Forms.Contracts
{
    public interface IStateAction<TAction, TValue>
    {
        static abstract TAction Create(TValue newValue);
    }

    public interface IActionHandler<in TAction>
    {
        void Handle(TAction action);
    }
}
