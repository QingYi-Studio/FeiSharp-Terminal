using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FeiSharpTerminal3._1.ThinkGeo
{
    internal interface INewNumber<T> where T : struct
    {
        public T Value { get; set; }
        public string ToString();
        public bool Equals(object? obj);
        private static double ModDoubleCalc(NewDouble x, NewDouble y, char op) { return 0; }
    }
    internal struct NewDouble : INewNumber<double>
    {
        public double Value { get; set; } = 0;

        public static implicit operator double(NewDouble x) => x.Value;
        public static implicit operator int(NewDouble x) => int.Parse(x.Value.ToString());
        public static implicit operator string(NewDouble x) => x.Value.ToString();
        public static implicit operator float(NewDouble x) => float.Parse(x.Value.ToString());
        public static implicit operator NewDouble(double x) => new(x);
        public static implicit operator NewDouble(int x) => new(double.Parse(x.ToString()));
        public static implicit operator NewDouble(float x) => new(double.Parse(x.ToString()));
        public static implicit operator NewDouble(string x) => new(double.Parse(x));
        public NewDouble(double init)
        {
            Value = init;
        }
        public static NewDouble operator +(NewDouble x, NewDouble y)
        {
            return new(ModDoubleCalc(x, y, '+'));
        }
        public static NewDouble operator -(NewDouble x, NewDouble y)
        {
            return new(ModDoubleCalc(x, y, '-'));
        }
        public static NewDouble operator *(NewDouble x, NewDouble y)
        {
            return new(ModDoubleCalc(x, y, '*'));
        }
        public static NewDouble operator /(NewDouble x, NewDouble y)
        {
            return new(ModDoubleCalc(x, y, '/'));
        }
        public override string ToString()
        {
            return this.Value.ToString();
        }
        public override bool Equals(object? obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj)+": Is a null object.");
            if (double.IsNaN(double.Parse(obj.ToString()))) throw new ArgumentException(nameof(obj) + ": Is Not A Number(NaN)");
            if(obj is double d) return this.Value == d;
            if(obj is NewDouble a) return this.Value == a.Value;
            return base.Equals(obj);
        }
        public string CreateNumberType(int numbers)
        {
            string a = "";
            for (int i = 1; i <= numbers; i++)
            {
                a += this.Value;
            }
            return a;
        }
        public string CreateNumberType(int numbers,int insertIndex,string value)
        {
            string a = "";
            for (int i = 1; i <= numbers; i++)
            {
                if(i == insertIndex)
                {
                    a += value;
                }
                a += this.Value;
            }
            return a;
        }
        public string CreateNumberType(int numbers, int insertIndex, char value)
        {
            string a = "";
            for (int i = 1; i <= numbers; i++)
            {
                if (i == insertIndex)
                {
                    a += value;
                }
                a += this.Value;
            }
            return a;
        }
        public int GetInteger()
        {
            return int.Parse(this.Value.ToString().Split('.')[0]);
        }
        public int GetIntegerNumbers()
        {
            return this.Value.ToString().Split('.')[0].Length;
        }
        public double GetScale()
        {
            try
            {
                return double.Parse(this.Value.ToString().Split('.')[1]);
            }
            catch
            {
                return 0;
            }
        }
        public int GetScaleNumbers()
        {
            try
            {
                return this.Value.ToString().Split('.')[1].Length;
            }
            catch{
                return 0;
            }
        }
        private static double ModDoubleCalc(NewDouble x, NewDouble y, char op)
        {
            if (x.Value.ToString().IndexOf('.') == -1 && y.Value.ToString().IndexOf('.') == -1)
            {
                return op switch { '+' => x.Value + y.Value, '-' => x.Value - y.Value, '*' => x.Value * y.Value, '/' => x.Value / y.Value, _ => throw new InvalidOperationException("Unvalid") };
            }
            int number = default(int);
            double max = Math.Max(x.Value, y.Value);
            if(x.Value.ToString().IndexOf('.') == -1 && y.Value.ToString().IndexOf('.') != -1) number = int.Parse("1" + new string('0', y.ToString().Split('.')[1].Length));
            if(y.Value.ToString().IndexOf('.') == -1 && x.Value.ToString().IndexOf('.') != -1) number = int.Parse("1" + new string('0', x.ToString().Split('.')[1].Length));
            number = int.Parse("1" + new string('0', max.ToString().Split('.')[1].Length));
            x.Value *= number;
            y.Value *= number;
            return op switch { '+' => (x.Value + y.Value) / number, '-' => (x.Value - y.Value) / number, '*' => x.Value * y.Value / number, '/' => x.Value / y.Value / number, _ => throw new InvalidOperationException("Unvalid") };
        }
    }
    internal interface IDpiFix
    {
        public static int Fixed(int y)
        {
            if(y % 2 == 0) return y;
                else return y++;
        }
        public static int Fixed(double y)
        {
            if (y % 2 == 0 && new NewDouble(y).GetScale() == 0) return int.Parse(y.ToString());
            else
            {
                if(new NewDouble(y).GetScale() != 0 && y % 2 != 0)
                {
                    return new NewDouble(y).GetInteger() + 1;
                }
                else
                {
                    if(new NewDouble(y).GetScale() != 0 && y % 2 == 0)
                    {
                        return new NewDouble(y).GetInteger();
                    }
                    else if (new NewDouble(y).GetScale() == 0 && y % 2 != 0)
                    {
                        return Fixed(int.Parse(y.ToString()));
                    }
                }
            }
            throw new Exception("If you touch this exception, you are the best-clever human!(No edit source code file.)");
        }
    }
}
