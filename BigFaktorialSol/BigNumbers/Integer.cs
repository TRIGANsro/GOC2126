using System.Text;

namespace BigNumbers
{
    public class Integer : IComparable<Integer>, IComparable, IEquatable<Integer>
    {
        private List<sbyte> digits = new ();
        public Integer(string number)
        {
            foreach (var digit in number.Reverse())
            {
                digits.Add((sbyte)(digit - '0'));
            }
        }
        public Integer(long number)
        {
            while (number > 0)
            {
                digits.Add((sbyte)(number % 10));
                number /= 10;
            }
        }
        public Integer(long number, int capacity)
        {
            digits.Capacity = capacity;
            while (number > 0)
            {
                digits.Add((sbyte)(number % 10));
                number /= 10;
            }
        }
        public Integer Add(Integer other)
        {
            var result = new Integer(0, Math.Max(digits.Count, other.digits.Count) + 1);
            int carry = 0;
            for (int i = 0; i < result.digits.Capacity; i++)
            {
                int sum = carry;
                if (i < digits.Count)
                {
                    sum += digits[i];
                }
                if (i < other.digits.Count)
                {
                    sum += other.digits[i];
                }
                result.digits.Add((sbyte)(sum % 10));
                carry = sum / 10;
            }
            return result;
        }
        public Integer Multiple(Integer other)
        {
            var result = new Integer(0, digits.Count + other.digits.Count);
            for (int i = 0; i < digits.Count; i++)
            {
                int carry = 0;
                for (int j = 0; j < other.digits.Count; j++)
                {
                    int product = digits[i] * other.digits[j] + carry;
                    if (i + j < result.digits.Count)
                    {
                        product += result.digits[i + j];
                    }
                    if (product >= 10)
                    {
                        carry = product / 10;
                        product %= 10;
                    }
                    else
                    {
                        carry = 0;
                    }
                    if (i + j < result.digits.Count)
                    {
                        result.digits[i + j] = (sbyte) product;
                    }
                    else
                    {
                        result.digits.Add((sbyte)product);
                    }
                }
                if (carry > 0)
                {
                    result.digits.Add((sbyte)carry);
                }
            }
            return result;
        }

        public bool Equals(Integer? other)
        {
            if (other is null)
            {
                return false;
            }
            if (digits.Count != other.digits.Count)
            {
                return false;
            }
            for (int i = 0; i < digits.Count; i++)
            {
                if (digits[i] != other.digits[i])
                {
                    return false;
                }
            }
            return true;
        }

        public int CompareTo(Integer? other)
        {
            if (other is null)
            {
                return 1;
            }
            if (digits.Count > other.digits.Count)
            {
                return 1;
            }
            if (digits.Count < other.digits.Count)
            {
                return -1;
            }
            for (int i = digits.Count - 1; i >= 0; i--)
            {
                if (digits[i] > other.digits[i])
                {
                    return 1;
                }
                if (digits[i] < other.digits[i])
                {
                    return -1;
                }
            }
            return 0;
        }

        public int CompareTo(object? obj)
        {
            if (obj is null)
            {
                return 1;
            }
            if (obj is Integer integer)
            {
                return CompareTo(integer);
            }
            throw new ArgumentException("Object is not an Integer");
        }

        public override bool Equals(object? obj)
        {
            if (obj is Integer integer)
            {
                return Equals(integer);
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                long sum = 0;
                foreach (var digit in digits)
                {
                    sum += digit;
                }

                return (int) sum;
            }
        }

        public static bool operator ==(Integer? a, Integer? b)
        {
            if (a is null)
            {
                return b is null;
            }
            return a.Equals(b);
        }

        public static bool operator !=(Integer? a, Integer? b)
        {
            return !(a == b);
        }

        public static bool operator <(Integer? a, Integer? b)
        {
            if (a is null)
            {
                return b is not null;
            }
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(Integer? a, Integer? b)
        {
            if (a is null)
            {
                return false;
            }
            return a.CompareTo(b) > 0;
        }

        public static bool operator <=(Integer? a, Integer? b)
        {
            if (a is null)
            {
                return true;
            }
            return a.CompareTo(b) <= 0;
        }

        public static bool operator >=(Integer? a, Integer? b)
        {
            if (a is null)
            {
                return b is null;
            }
            return a.CompareTo(b) >= 0;
        }


        public override string ToString()
        {
            //return new string(digits.Select(d => (char)(d + '0')).Reverse().ToArray());

            StringBuilder sb = new StringBuilder(digits.Count);

            for (int i = digits.Count - 1; i >= 0; i--)
            {
                sb.Append((char)(digits[i] + '0'));
            }

            return sb.ToString();
        }

        

        public static Integer operator +(Integer a, Integer b)
        {
            return a.Add(b);
        }

        public static Integer operator *(Integer a, Integer b)
        {
            return a.Multiple(b);
        }

        public static implicit operator Integer(long number)
        {
            return new Integer(number);
        }

        public static explicit operator long(Integer integer)
        {
            long result = 0;
            long multiplier = 1;
            foreach (var digit in integer.digits)
            {
                result += digit * multiplier;
                multiplier *= 10;
            }
            return result;
        }
    }
}
