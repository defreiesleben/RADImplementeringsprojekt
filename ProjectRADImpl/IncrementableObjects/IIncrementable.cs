using System;
using System.Collections.Generic;
using System.Text;

namespace RADImplementationProject
{
    public interface IIncrementable<T>
    {
        void SetValue(T v);
        T GetValue();
        void Increment(T v);
    }
}
