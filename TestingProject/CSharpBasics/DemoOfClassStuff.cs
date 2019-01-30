using System;
using System.Text;
using Xunit;

namespace TestingProject.CSharpBasics
{
    public class ManyFeaturesDemo
    {
        // constants are static fields and are evaluated at compile time. they are substituted where ever used
        // constants can only be of type numeric, bool, char, string or enum
        private const string DefaultName = "King Kong";

        // static readonly properties are initialized at runtime.
        // if a static constructor exists, static field initializers are run just before the static constructor.
        // otherwise, the field initializers are executed just prior to type usage or anytime early at the will of the runtime.
        public static readonly DateTime CurrentDate = new DateTime(2019, 1, 26);

        // static constructors must not have parameters and are called only one time during runtime (triggered by
        // instantiating the type or accessing a static member of the type).
        static ManyFeaturesDemo()
        {
            CurrentDate = CurrentDate.AddDays(1);
        }

        private readonly int _x;
        internal readonly bool State = true;
        public string MyName = GetSomeName();
        private decimal _money;

        public decimal Money
        {
            get => _money;
            set => _money = value >= 0 ? value : 0;
        }

        public long Version { get; set; }
        public long Age { get; set; } = 123;
        public long CreationTime { get; private set; } = 42;

        private readonly string[] _words = "Hello World".Split();

        public string this[int wordNumber]
        {
            get => _words[wordNumber];
            set => _words[wordNumber] = value;
        }

        public char this[int wordNumber, int character] => _words[wordNumber][character];

        public ManyFeaturesDemo()
        {
        }

        public void ChangeCreationTime()
        {
            CreationTime = 43;
        }

        private ManyFeaturesDemo(int x)
        {
            _x = x;
        }

        // Expression bodied member can be used for constructor with single expression
        public ManyFeaturesDemo(bool state) => State = state;

        public ManyFeaturesDemo(int x, bool state) : this(x) // constructor ManyFeaturesDemo(int x) is called first
        {
            State = state;
        }

        private static string GetSomeName()
        {
            return DefaultName;
        }

        // Expression bodied method can be used for method with single expression
        public static int ExpressionBodiedMethod(int i) => i * 2;

        public static int LocalMethod(int i)
        {
            int Cube(int value) => value * value * value;

            return Cube(i);
        }

        public void Deconstruct(out int x, out bool state)
        {
            x = _x;
            state = State;
        }

        public static int DestroyedObjects;

        ~ManyFeaturesDemo()
        {
            DestroyedObjects++;
        }
    }

    public class UnusableClass
    {
        static UnusableClass()
        {
            throw new Exception();
        }
    }

    // static classes can only have static members and cannot be subclassed.
    public static class StaticClass
    {
        public static string Hello = "Hello";

        public static string SayHello()
        {
            return Hello;
        }
    }
    
    

    public class DemoOfClassStuff
    {
        [Fact]
        public void EmptyConstructor()
        {
            var obj = new ManyFeaturesDemo();
            Assert.Equal("King Kong", obj.MyName);
            obj.MyName = "Test";
            Assert.Equal("Test", obj.MyName);
        }

        [Fact]
        public void ExpressionBodiedMethod()
        {
            Assert.Equal(6, ManyFeaturesDemo.ExpressionBodiedMethod(3));
        }

        [Fact]
        public void LocalMethod()
        {
            Assert.Equal(27, ManyFeaturesDemo.LocalMethod(3));
        }

        [Fact]
        public void ExpressionBodiedConstructor()
        {
            var obj = new ManyFeaturesDemo(true);
            Assert.True(obj.State);
        }

        [Fact]
        public void DeconstructWorksLikeInJavascript()
        {
            var obj = new ManyFeaturesDemo(5, true);
            var (x, state) = obj;
            Assert.Equal(5, x);
            Assert.True(state);
        }

        [Fact]
        public void ObjectInitializers()
        {
            var obj = new ManyFeaturesDemo(true)
            {
                MyName = "Peter"
            };
            Assert.Equal("Peter", obj.MyName);
        }

        [Fact]
        public void Properties()
        {
            var obj = new ManyFeaturesDemo {Money = 5m};
            Assert.Equal(5m, obj.Money);
            obj.Money = -1;
            Assert.Equal(0m, obj.Money);
        }

        [Fact]
        public void AutoProperties()
        {
            var obj = new ManyFeaturesDemo();
            Assert.Equal(0, obj.Version);
            obj.Version = 43;
            Assert.Equal(43, obj.Version);
        }

        [Fact]
        public void PropertyInitializer()
        {
            var obj = new ManyFeaturesDemo();
            Assert.Equal(123, obj.Age);
            obj.Age = 43;
            Assert.Equal(43, obj.Age);
        }

        [Fact]
        public void PropertiesWithPrivateSetter()
        {
            var obj = new ManyFeaturesDemo();
            Assert.Equal(42, obj.CreationTime);
            obj.ChangeCreationTime();
            Assert.Equal(43, obj.CreationTime);
        }

        [Fact]
        public void Indexers()
        {
            var obj = new ManyFeaturesDemo();
            Assert.Equal("World", obj[1]);
            obj[1] = "Indexer";
            Assert.Equal("Indexer", obj[1]);
        }

        [Fact]
        public void IndexersWithMultipleArguments()
        {
            var obj = new ManyFeaturesDemo();
            Assert.Equal('r', obj[1, 2]);
        }

        [Fact]
        public void IndexAccessCanBeNullCoalesced()
        {
            Assert.Null(GetNullString()?[0]);
        }

        private static string GetNullString()
        {
            return null;
        }

        [Fact]
        public void Finalizers()
        {
            long sumOfAges = 0;
            for (var i = 0; i < 100; i++)
            {
                sumOfAges += new ManyFeaturesDemo().Age;
            }

            GC.Collect();
            Assert.Equal(123 * 100, sumOfAges);
            Assert.True(ManyFeaturesDemo.DestroyedObjects > 0);
        }

        [Fact]
        public void StaticFieldsAndConstructors()
        {
            Assert.Equal(new DateTime(2019, 1, 27), ManyFeaturesDemo.CurrentDate);
        }

        [Fact]
        public void StaticClassesCanOnlyHaveStaticMembers()
        {
            Assert.Equal("Hello", StaticClass.Hello);
            Assert.Equal("Hello", StaticClass.SayHello());
        }

        [Fact]
        public void ExceptionsInStaticConstructorMakeClassUnusable()
        {
            Assert.Throws<TypeInitializationException>(() => new UnusableClass());
        }

        [Fact]
        public void NameOf()
        {
            // ReSharper disable once LocalNameCapturedOnly
            const string x = "test";
            Assert.Equal("x.Count", nameof(x) + "." + nameof(x.Length));
            Assert.Equal("StringBuilder.Length", nameof(StringBuilder) + "." + nameof(StringBuilder.Length));
        }
    }
}