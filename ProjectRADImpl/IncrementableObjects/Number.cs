using System;
using System.Collections.Generic;
using System.Text;

namespace RADImplementationProject
{
    public class Number : IIncrementable<int>
    {
        private int _value = 0;

        public Number(int v) { _value = v; }

        public int GetValue() => _value;

        public void Increment(int v)
        {
            _value += v;
        }

        public void SetValue(int v)
        {
            _value = v;
        }

        public static implicit operator Number(int v) => new Number(v);
    }
}
