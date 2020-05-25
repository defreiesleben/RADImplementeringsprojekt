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
    public class NumberLong : IIncrementable<long>
    {
        private long _value = 0;

        public NumberLong(long v) { _value = v; }

        public long GetValue() => _value;

        public void Increment(long v)
        {
            _value += v;
        }

        public void SetValue(long v)
        {
            _value = v;
        }

        public static implicit operator NumberLong(long v) => new NumberLong(v);
    }
}
