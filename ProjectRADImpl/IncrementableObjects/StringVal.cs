using System;
using System.Collections.Generic;
using System.Text;

namespace RADImplementationProject
{
    public class StringVal : IIncrementable<string>
    {
        string value = "";

        public StringVal(string val) { value = val; }


        public string GetValue() => value;

        public void Increment(string v)
        {
            value += v;
        }

        public void SetValue(string v)
        {
            value = v;
        }

        public static implicit operator StringVal(string v) => new StringVal(v);
    }
}
