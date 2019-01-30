using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace TestingProject.CSharpBasics
{
    public class DemoOfBasics
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public DemoOfBasics(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void NativeNumericTypes()
        {
            // SIGNED INTEGER TYPES

            Assert.Equal(127, sbyte.MaxValue);
            Assert.Equal(-128, sbyte.MinValue);
            Assert.Equal(TypeCode.SByte, ((sbyte) 1).GetTypeCode());
            Assert.Equal(1, sizeof(sbyte));

            Assert.Equal(32767, short.MaxValue);
            Assert.Equal(-32768, short.MinValue);
            Assert.Equal(TypeCode.Int16, ((short) 1).GetTypeCode());
            Assert.Equal(2, sizeof(short));


            Assert.Equal(2147483647, int.MaxValue);
            Assert.Equal(-2147483648, int.MinValue);
            Assert.Equal(TypeCode.Int32, 1.GetTypeCode());
            Assert.Equal(4, sizeof(int));


            Assert.Equal(9223372036854775807, long.MaxValue);
            Assert.Equal(-9223372036854775808, long.MinValue);
            Assert.Equal(TypeCode.Int64, 1L.GetTypeCode());
            Assert.Equal(8, sizeof(long));

            // UNSIGNED INTEGER TYPES

            Assert.Equal(255, byte.MaxValue);
            Assert.Equal(0, byte.MinValue);
            Assert.Equal(TypeCode.Byte, ((byte) 1).GetTypeCode());
            Assert.Equal(1, sizeof(byte));


            Assert.Equal(65535, ushort.MaxValue);
            Assert.Equal(0, ushort.MinValue);
            Assert.Equal(TypeCode.UInt16, ((ushort) 1).GetTypeCode());
            Assert.Equal(2, sizeof(ushort));


            Assert.Equal(4294967295, uint.MaxValue);
            Assert.Equal((uint) 0, uint.MinValue);
            Assert.Equal(TypeCode.UInt32, ((uint) 1).GetTypeCode());
            Assert.Equal(4, sizeof(uint));


            Assert.Equal(18446744073709551615, ulong.MaxValue);
            Assert.Equal((ulong) 0, ulong.MinValue);
            Assert.Equal(TypeCode.UInt64, ((ulong) 1).GetTypeCode());
            Assert.Equal(8, sizeof(ulong));

            // REAL NUMBERS

            // WTF?!? why is float.MaxValue different to 3.402823E+38f?
            // binary float.MaxValue: 00000000000000000000000011111111000000000000000000000000111111110000000000000000000000000111111100000000000000000000000001111111
            // binary 3.402823E+38f:  00000000000000000000000011111101000000000000000000000000111111110000000000000000000000000111111100000000000000000000000001111111

            _testOutputHelper.WriteLine(FloatToString(float.MaxValue));
            _testOutputHelper.WriteLine(FloatToString(3.402823E+38f));
            // Assert.Equal(3.402823E+38f, float.MaxValue, 1);
            // Assert.Equal(-3.402823E+38f, float.MinValue, 1);
            Assert.Equal(TypeCode.Single, 1.0f.GetTypeCode());
            Assert.Equal(4, sizeof(float));


            // WTF?!? why is 1.79769313486232E+308 not a valid double value?
            // Assert.Equal(1.79769313486232E+308, double.MaxValue);
            // Assert.Equal(-1.79769313486232E+308, double.MinValue);
            Assert.Equal(TypeCode.Double, 1.0d.GetTypeCode());
            Assert.Equal(8, sizeof(double));
            Assert.Equal(double.PositiveInfinity, 1d / 0d);
            Assert.Equal(double.NegativeInfinity, -1d / 0d);
            Assert.Equal(double.NegativeInfinity, 1d / -0d);
            Assert.Equal(double.PositiveInfinity, -1d / -0d);
            Assert.Equal(double.NaN, -0d / -0d);
            Assert.Equal(double.NaN, double.PositiveInfinity / double.PositiveInfinity);


            Assert.Equal(79228162514264337593543950335M, decimal.MaxValue);
            Assert.Equal(-79228162514264337593543950335M, decimal.MinValue);
            Assert.Equal(TypeCode.Decimal, 1.0m.GetTypeCode());
            Assert.Equal(8, sizeof(decimal));


            Assert.Equal(0.3M, 0.1M + 0.2M);
            Assert.NotEqual(0.3D, 0.1D + 0.2D);
        }

        [Fact]
        public void NumericLiterals()
        {
            Assert.Equal(123456789, 123_456_789);
            Assert.Equal(65_535, 0xffff);
            Assert.Equal(10, 0b1010);
        }

        private static string FloatToString(float value)
        {
            var stringBuilder = new StringBuilder();

            foreach (var b in BitConverter.GetBytes(value))
            {
                stringBuilder.Append(Convert.ToString(b, 2).PadLeft(32, '0'));
            }

            return stringBuilder.ToString();
        }


        [Fact]
        public void IntegralTypedExpressionsCanBeCheckedForOverflow()
        {
            Assert.Throws<OverflowException>(() => CheckedMultiply(int.MaxValue, int.MaxValue));
        }

        private static int CheckedMultiply(int x, int y)
        {
            return checked(x * y);
        }

        [Fact]
        public void LazyEvaluationForConditionalOperators()
        {
            Assert.False(ConditionA() && ConditionB());
        }

        [Fact]
        public void EagerEvaluationForConditionalOperators()
        {
            Assert.Throws<Exception>(() => ConditionA() & ConditionB());
        }

        private static bool ConditionA()
        {
            return false;
        }

        private static bool ConditionB()
        {
            throw new Exception();
        }

        [Fact]
        public void StringIndexedAccess()
        {
            const string s = "Test";
            var x = s[0];
            Assert.Equal('T', x);
            Assert.Equal(TypeCode.Char, x.GetTypeCode());
        }

        [Fact]
        public void StringInterpolation()
        {
            const int x = 42;
            Assert.Equal("The answer is 42", $"The answer is {x}");
        }

        [Fact]
        public void VerbatimStrings()
        {
            const string escapedString = "\\\\server\\shared\\x.html";
            const string verbatimString = @"\\server\shared\x.html";
            Assert.Equal(verbatimString, escapedString);

            const string multilineString = @"a
b";
            Assert.Equal(new[] {"a", "b"}, multilineString.Split("\r\n"));
        }

        [Fact]
        public void StringInterpolationWithVerbatimString()
        {
            const int x = 42;
            Assert.Equal("The answer is 42\\", $@"The answer is {x}\");
        }

        [Fact]
        public void StringComparison()
        {
            Assert.Equal(0, string.Compare("A", "A", System.StringComparison.Ordinal));
            Assert.Equal(1, string.Compare("B", "A", System.StringComparison.Ordinal));
            Assert.Equal(-1, string.Compare("A", "B", System.StringComparison.Ordinal));
        }

        [Fact]
        public void RectangularArraysVsJaggedArrays()
        {
            int[,] matrixA =
            {
                {0, 1, 2},
                {3, 4, 5},
                {6, 7, 8}
            };
            var matrixB = new int[3][];
            for (var i = 0; i < 3; i++)
            {
                matrixB[i] = new int[3];
                for (var j = 0; j < 3; j++)
                {
                    matrixB[i][j] = matrixA[i, j];
                }
            }
        }

        [Fact]
        public void PassingByValueVsByReference()
        {
            var x = 42;
            Assert.Equal(43, AddOneToValueParameter(x));
            Assert.Equal(42, x);
            Assert.Equal(43, AddOneToRefParameter(ref x));
            Assert.Equal(43, x);
        }

        private static int AddOneToValueParameter(int x)
        {
            x = x + 1;
            return x;
        }

        private static int AddOneToRefParameter(ref int x)
        {
            x = x + 1;
            return x;
        }

        [Fact]
        public void OutParameter()
        {
            var x = CreateTwoOutputs(out var y, out _);
            Assert.Equal(43, y);
            Assert.Equal(42, x);

            // this is one of the few cases where out parameters make sense
            var isValid = int.TryParse("100", out var result);
            Assert.True(isValid);
            Assert.Equal(100, result);
        }

        private static int CreateTwoOutputs(out int y, out bool toBeIgnored)
        {
            y = 43;
            toBeIgnored = false;
            return 42;
        }

        [Fact]
        public void ParamsModifierLikeVArgsInJava()
        {
            Assert.Equal(42, Sum(21, 21));
        }

        private static int Sum(params int[] values)
        {
            return values.Sum();
        }

        [Fact]
        public void OptionalParameters()
        {
            Assert.Equal(0, ReturnParameter());
            Assert.Equal(1, ReturnParameter(1));
        }

        private static int ReturnParameter(int x = 0)
        {
            return x;
        }

        [Fact]
        public void NamedParameters()
        {
            Assert.Equal(60, ReturnSumOfParameters(10));
            Assert.Equal(57, ReturnSumOfParameters(10, c: 5));
        }

        private static int ReturnSumOfParameters(int a, int b = 42, int c = 8)
        {
            return a + b + c;
        }

        [Fact]
        public void SwitchByType()
        {
            Assert.Equal("I am an integer with value 5", DoSomethingThatMightBetterBeDoneWithPolymorphism(5));
            Assert.Equal("I am a true boolean!", DoSomethingThatMightBetterBeDoneWithPolymorphism(true));
            Assert.Equal("I am a string of length 4", DoSomethingThatMightBetterBeDoneWithPolymorphism("test"));
            Assert.Equal("Nulls can be handled!", DoSomethingThatMightBetterBeDoneWithPolymorphism(null));
            Assert.Equal("I don't know how to handle the value 'False'",
                DoSomethingThatMightBetterBeDoneWithPolymorphism(false));
        }

        private static string DoSomethingThatMightBetterBeDoneWithPolymorphism(object o)
        {
            switch (o)
            {
                case int i:
                    return $"I am an integer with value {i}";
                case bool b when b:
                    return "I am a true boolean!";
                case string s:
                    return $"I am a string of length {s.Length}";
                case null:
                    return "Nulls can be handled!";
                default:
                    return $"I don't know how to handle the value '{o}'";
            }
        }

        private struct MyStruct
        {
            public readonly int X;
            public readonly int Y;

            public MyStruct(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        [Fact]
        public void Structs()
        {
            var myStruct = new MyStruct(5, 6);
            Assert.Equal(5, myStruct.X);
            Assert.Equal(6, myStruct.Y);
            Assert.Equal(8, Marshal.SizeOf(typeof(MyStruct)));
        }

        [Fact]
        public void NullConditional()
        {
            Assert.Null(NullOrZero(true)?.ToString());
            Assert.Equal("0", NullOrZero(false)?.ToString());
        }

        [Fact]
        public void NullCoalescing()
        {
            Assert.Equal(1, NullOrZero(true) ?? 1);
            Assert.Equal(0, NullOrZero(false) ?? 1);
        }

        private static int? NullOrZero(bool setToNull)
        {
            return setToNull ? (int?) null : 0;
        }
    }
}