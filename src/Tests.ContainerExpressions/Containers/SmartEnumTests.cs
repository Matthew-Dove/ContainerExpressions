using ContainerExpressions.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Tests.ContainerExpressions.Containers
{
    [TestClass]
    public class SmartEnumTests
    {
        class Bingo : SmartEnum
        {
            public static readonly Bingo One = new(1);
            public static readonly Bingo Two = new("not_set", "none", "Unknown");
            public static readonly Bingo Five = new(5);
            public static readonly Bingo Six = new();

            private Bingo() { }
            private Bingo(int value) : base(value) { }
            private Bingo(string name, params string[] aliases) : base(name, aliases) { }
        }

        class Weekend : SmartEnum
        {
            public static readonly Weekend None = new();
            public static readonly Weekend Saturday = new();
            public static readonly Weekend Sunday = new();

            private Weekend() { }
        }

        class Colour : SmartEnum
        {
            public static readonly Colour None = new(0);
            public static readonly Colour Purple = new(1);
            public static readonly Colour Red = new(2);
            public static readonly Colour Green = new(4);
            public static readonly Colour Teal = new(8);

            private Colour(int value) : base(value) { }
        }

        [TestInitialize]
        public void Initialize()
        {
            // Priming the pump.
            _ = SmartEnum<Bingo>.Init();
            _ = SmartEnum<Weekend>.Init();
            _ = SmartEnum<Colour>.Init();
        }

        [TestMethod]
        public void Init()
        {
            var init = SmartEnum<Bingo>.Init(FormatOptions.Original);

            Assert.AreEqual(4, init.Count);
            Assert.AreEqual(Bingo.One, init[Bingo.One].Value);
            Assert.AreEqual(Bingo.Two, init["not_set"].Value);

            Assert.AreEqual(2, init["not_set"].Aliases.Length);
            Assert.IsTrue(init["not_set"].Aliases.Contains("none"));
            Assert.IsTrue(init["not_set"].Aliases.Contains("Unknown"));
        }

        [TestMethod]
        public void MagicTrio()
        {
            var se1 = SmartEnum<Weekend>.FromObject(Weekend.Saturday).Value;
            var se2 = SmartEnum<Weekend>.FromName(Weekend.Saturday).Value;
            var se3 = SmartEnum<Weekend>.FromValue(Weekend.Saturday).Value;
            
            Assert.AreEqual(se1, se2);
            Assert.AreEqual(se2, se3);
            Assert.AreEqual(se3, se1);

            Assert.IsTrue(se1.Equals(Weekend.Saturday));
            Assert.IsTrue(se2 == Weekend.Saturday);
            Assert.IsTrue(se3 == se2);

            Assert.AreEqual((int)se1, (int)Weekend.Saturday);
            Assert.AreEqual(se2.ToString(FormatOptions.Original), Weekend.Saturday.ToString());
            Assert.AreEqual(se3.Objects[0], Weekend.Saturday);
        }

        [TestMethod]
        public void BaseSmartEnum()
        {
            var name = SmartEnum<Weekend>.GetNames(FormatOptions.Original)[Weekend.Saturday];

            var se = Weekend.Saturday;

            Assert.AreEqual(1, se);
            Assert.AreEqual(name, se);
            Assert.AreEqual(Weekend.Saturday, se);
            Assert.IsTrue(Weekend.Saturday == se);

            Assert.IsFalse(Weekend.Sunday == se);
        }

        [TestMethod]
        public void GetObjects()
        {
            var se = SmartEnum<Weekend>.GetObjects();

            Assert.AreEqual(3, se.Length);
            Assert.AreEqual(Weekend.None, se[0]);
            Assert.AreEqual(Weekend.Saturday, se[1]);
            Assert.AreEqual(Weekend.Sunday, se[2]);
        }

        [TestMethod]
        public void GetObjects_Flags()
        {
            var se = SmartEnum<Colour>.GetObjects();

            Assert.AreEqual(5, se.Length);
            Assert.AreEqual(Colour.None, se[0]);
            Assert.AreEqual(Colour.Purple, se[1]);
            Assert.AreEqual(Colour.Red, se[2]);
            Assert.AreEqual(Colour.Green, se[3]);
            Assert.AreEqual(Colour.Teal, se[4]);
        }

        [TestMethod]
        public void FromObject()
        {
            var se1 = SmartEnum<Weekend>.FromObject(Weekend.Sunday).Value;
            var se2 = SmartEnum<Weekend>.FromObject(Weekend.Sunday).Value;

            Assert.AreEqual(1, se1.Objects.Length);
            Assert.AreEqual(Weekend.Sunday, se1.Objects[0]);
            Assert.IsTrue(se1.Equals(Weekend.Sunday));
            Assert.IsTrue(se1 == Weekend.Sunday);

            Assert.AreEqual(se1, se2);
            Assert.IsTrue(se1.Equals(se2));
            Assert.IsTrue(se1 == se2);
            Assert.AreEqual((int)se1, se2);
            Assert.AreEqual((string)se1, se2);

            Assert.IsFalse(se1 == Weekend.Saturday);
            Assert.IsFalse(se1 == SmartEnum<Weekend>.FromObject(Weekend.Saturday));
        }

        [TestMethod]
        public void FromObjects()
        {
            var se1 = SmartEnum<Colour>.FromObjects(Colour.Red, Colour.Purple).Value;
            var se2 = SmartEnum<Colour>.FromObjects(Colour.Purple, Colour.Red).Value;

            Assert.AreEqual(2, se1.Objects.Length);
            Assert.AreEqual(Colour.Purple, se1.Objects[0]); // Order is based on the underlying value (purple is 1, red is 2).
            Assert.IsFalse(se1 == Colour.Purple); // A flags enum with many values will not match a single target.

            Assert.AreEqual(se1, se2); // Even though the objects are added in different positions, order is determined internally.
            Assert.IsTrue(se1.Equals(se2));
            Assert.IsTrue(se1 == se2);
            Assert.AreEqual((int)se1, se2);
            Assert.AreEqual((string)se1, se2);

            Assert.IsFalse(se1 == SmartEnum<Colour>.FromObjects(Colour.Red, Colour.Purple, Colour.Green));
        }

        [TestMethod]
        public void GetNames()
        {
            var names = SmartEnum<Weekend>.GetNames(FormatOptions.Original);

            Assert.AreEqual(3, names.Length);
            Assert.AreEqual(names[Weekend.None], Weekend.None);
            Assert.AreEqual(names[Weekend.Saturday], Weekend.Saturday);
            Assert.AreEqual(names[Weekend.Sunday], Weekend.Sunday);
        }

        [TestMethod]
        public void GetNames_Aliases()
        {
            var names = SmartEnum<Bingo>.GetNames(FormatOptions.Original);

            Assert.AreEqual(4, names.Length);
            Assert.AreEqual(names[0], Bingo.One);
            Assert.AreEqual(names[1], "not_set");
            Assert.AreEqual(names[2], Bingo.Five);
            Assert.AreEqual(names[3], Bingo.Six);
        }

        [TestMethod]
        public void FromName()
        {
            var se1 = SmartEnum<Weekend>.FromName(nameof(Weekend.Saturday));
            var se2 = SmartEnum<Weekend>.FromName(Weekend.Saturday.Name);

            Assert.AreEqual(se1, se2);
            Assert.IsTrue(se1.Value == Weekend.Saturday);
            Assert.AreEqual(1, se1.Value.Objects.Length);
        }

        [TestMethod]
        public void FromName_NameOverride()
        {
            var se1 = SmartEnum<Bingo>.FromName(nameof(Bingo.Two)); // Property name is not used, as a value for "Name" is provided.
            var se2 = SmartEnum<Bingo>.FromName("not_set");

            Assert.IsFalse(se1);
            Assert.IsTrue(se2);
            Assert.AreEqual(1, se2.Value.Objects.Length);
        }

        [TestMethod]
        public void FromName_Aliases()
        {
            var se1 = SmartEnum<Bingo>.FromName("None").Value;
            var se2 = SmartEnum<Bingo>.FromName("unknown").Value;

            Assert.AreEqual(1, se1.Objects.Length);
            Assert.AreEqual(1, se2.Objects.Length);
            Assert.AreEqual(se1, se2);
            Assert.AreEqual(se1, SmartEnum<Bingo>.FromName("Not_Set").Value);
        }

        [TestMethod]
        public void FromNames()
        {
            var se = SmartEnum<Colour>.FromNames("Green,purple ,  gold").Value; // Gold is ignored, case-insensitive, whitespace ignored.

            Assert.AreEqual(2, se.Objects.Length);
            Assert.IsTrue(se == SmartEnum<Colour>.FromObjects(Colour.Purple, Colour.Green));
        }

        [TestMethod]
        public void FromNames_Aliases()
        {
            var se1 = SmartEnum<Bingo>.FromNames("Not_Set,unknown , None").Value; // All three names map to the same smart enum.

            Assert.AreEqual(1, se1.Objects.Length); // But only one instance is returned.
        }

        [TestMethod]
        public void GetAliases_None()
        {
            var aliases = SmartEnum<Weekend>.GetAliases();

            Assert.AreEqual(0, aliases.Length);
        }

        [TestMethod]
        public void GetAliases_Some()
        {
            var aliases = SmartEnum<Bingo>.GetAliases();

            Assert.AreEqual(2, aliases.Length);
        }

        [TestMethod]
        public void GetValues()
        {
            var values = SmartEnum<Weekend>.GetValues(); // Values start from 0, and auto increment from there.

            Assert.AreEqual(3, values.Distinct().Count());

            Assert.IsTrue(values.Contains(Weekend.None));
            Assert.IsTrue(values.Contains(Weekend.Saturday));
            Assert.IsTrue(values.Contains(Weekend.Sunday));

            Assert.IsTrue(values.Contains(0));
            Assert.IsTrue(values.Contains(1));
            Assert.IsTrue(values.Contains(2));
        }

        [TestMethod]
        public void GetValues_Flags()
        {
            var values = SmartEnum<Colour>.GetValues(); // Values are manually set for each instance.

            Assert.AreEqual(5, values.Distinct().Count());

            Assert.IsTrue(values.Contains(Colour.None));
            Assert.IsTrue(values.Contains(Colour.Purple));
            Assert.IsTrue(values.Contains(Colour.Red));
            Assert.IsTrue(values.Contains(Colour.Green));
            Assert.IsTrue(values.Contains(Colour.Teal));

            Assert.IsTrue(values.Contains(0));
            Assert.IsTrue(values.Contains(1));
            Assert.IsTrue(values.Contains(2));
            Assert.IsTrue(values.Contains(4));
            Assert.IsTrue(values.Contains(8));
        }

        [TestMethod]
        public void GetValues_Jagged()
        {
            var values = SmartEnum<Bingo>.GetValues(); // Values jump all over the place, but continue auto incrementing from their previous value.

            Assert.AreEqual(4, values.Distinct().Count());

            Assert.IsTrue(values.Contains(Bingo.One));
            Assert.IsTrue(values.Contains(Bingo.Two));
            Assert.IsTrue(values.Contains(Bingo.Five));
            Assert.IsTrue(values.Contains(Bingo.Six));

            Assert.IsTrue(values.Contains(1));
            Assert.IsTrue(values.Contains(2));
            Assert.IsTrue(values.Contains(5));
            Assert.IsTrue(values.Contains(6));
        }

        [TestMethod]
        public void FromValue()
        {
            var se1 = SmartEnum<Weekend>.FromValue(Weekend.Saturday).Value;
            var se2 = SmartEnum<Weekend>.FromValue(1).Value;

            Assert.AreEqual(1, se1.Objects.Length);
            Assert.AreEqual(se1.Objects[0], Weekend.Saturday);
            Assert.AreEqual(se1, se2);
        }

        [TestMethod]
        public void FromValue_Flags()
        {
            var se1 = SmartEnum<Colour>.FromValue(Colour.Green).Value;
            var se2 = SmartEnum<Colour>.FromValue(4).Value;

            Assert.AreEqual(1, se1.Objects.Length);
            Assert.AreEqual(se1.Objects[0], Colour.Green);
            Assert.AreEqual(se1, se2);
        }

        [TestMethod]
        public void FromValue_Flags_NotFound()
        {
            var se1 = SmartEnum<Colour>.FromValue(Colour.Green | Colour.Teal);
            var se2 = SmartEnum<Colour>.FromValue(12);

            Assert.IsFalse(se1);
            Assert.IsFalse(se2);
        }

        [TestMethod]
        public void FromValues()
        {
            var se1 = SmartEnum<Colour>.FromValues(Colour.Green | Colour.Teal).Value;
            var se2 = SmartEnum<Colour>.FromValues(12).Value;

            Assert.AreEqual(2, se1.Objects.Length);
            Assert.IsTrue(se1.Objects.Contains(Colour.Green));
            Assert.IsTrue(se1.Objects.Contains(Colour.Teal));
            Assert.AreEqual(se1, se2);
        }

        [TestMethod]
        public void FromValues_None()
        {
            var se = SmartEnum<Colour>.FromValues(Colour.Green & Colour.Teal).Value;

            Assert.AreEqual(1, se.Objects.Length);
            Assert.AreEqual(Colour.None, se.Objects[0]);
            Assert.IsTrue(Colour.None == se);
        }

        [TestMethod]
        public void Parse_Name()
        {
            var name = Colour.Green.Name;

            var se = SmartEnum<Colour>.Parse(name).Value;

            Assert.IsTrue(se == Colour.Green);
        }

        [TestMethod]
        public void Parse_Names()
        {
            var names = Colour.Green + "," + Colour.Teal;

            var se = SmartEnum<Colour>.Parse(names).Value;

            Assert.AreEqual(SmartEnum<Colour>.FromObjects(Colour.Teal, Colour.Green), se);
        }

        [TestMethod]
        public void Parse_Names_Separator()
        {
            var separator = ';';
            var names = Colour.Green + separator.ToString() + Colour.Teal;

            var se = SmartEnum<Colour>.Parse(names, separator).Value;

            Assert.AreEqual(SmartEnum<Colour>.FromObjects(Colour.Teal, Colour.Green), se);
        }

        [TestMethod]
        public void Parse_Value()
        {
            var value = Colour.Green.Value;

            var se = SmartEnum<Colour>.Parse(value).Value;

            Assert.IsTrue(se == Colour.Green);
        }

        [TestMethod]
        public void Parse_Values()
        {
            var values = Colour.Green | Colour.Teal;

            var se = SmartEnum<Colour>.Parse(values).Value;

            Assert.AreEqual(SmartEnum<Colour>.FromObjects(Colour.Teal, Colour.Green), se);
        }

        [TestMethod]
        public void Parse_Object_Name()
        {
            var name = (object)Colour.Green.Name;

            var se = SmartEnum<Colour>.Parse(name).Value;

            Assert.IsTrue(se == Colour.Green);
        }

        [TestMethod]
        public void Parse_Object_Value()
        {
            var value = (object)Colour.Green.Value;

            var se = SmartEnum<Colour>.Parse(value).Value;

            Assert.IsTrue(se == Colour.Green);
        }

        [TestMethod]
        public void Parse_Object_Enum()
        {
            var obj = Colour.Green;

            var se = SmartEnum<Colour>.Parse(obj).Value;

            Assert.IsTrue(se == Colour.Green);
        }

        [TestMethod]
        public void TryParse_Names()
        {
            var names = Colour.Green + "," + Colour.Teal;

            var result = SmartEnum<Colour>.TryParse(names, out var se);

            Assert.IsTrue(result);
            Assert.AreEqual(SmartEnum<Colour>.FromObjects(Colour.Teal, Colour.Green), se);
        }

        [TestMethod]
        public void Equals_Equals()
        {
            var green = Colour.Green;
            var greenSE = SmartEnum<Colour>.FromObject(green).Value;
            var greenAndTeal = Colour.Green | Colour.Teal;
            var greenAndTealSE = SmartEnum<Colour>.FromValues(greenAndTeal).Value;

            Assert.IsTrue(Colour.Green.Equals(green));
            Assert.IsTrue(Colour.Green == green);
            Assert.IsTrue(greenSE.Equals(green));
            Assert.IsTrue(greenSE == green);
            Assert.IsTrue(greenSE == SmartEnum<Colour>.FromObject(green));

            Assert.IsTrue(greenAndTeal.Equals(greenAndTealSE));
            Assert.IsTrue(greenAndTeal == greenAndTealSE);
            Assert.IsTrue(greenAndTealSE == SmartEnum<Colour>.FromValues(greenAndTeal));
        }

        [TestMethod]
        public void AddFlags()
        {
            var se1 = SmartEnum<Colour>.FromValues(Colour.Red | Colour.Green).Value;
            var se2 = SmartEnum<Colour>.FromObjects(Colour.Green, Colour.Red).Value;
            var se3 = SmartEnum<Colour>.FromNames($"{Colour.Red},{Colour.Green}").Value;
            var se4 = SmartEnum<Colour>.Parse($" {Colour.Red} ;{Colour.Green};", ';').Value;

            // All flags input methods are equivalent.
            Assert.AreEqual(se1, se2);
            Assert.AreEqual(se2, se3);
            Assert.AreEqual(se3, se1);
            Assert.AreEqual(se4, se3);
            Assert.AreEqual(se3.ToString(), se4.ToString());

            var se1a = se1.AddFlag(Colour.Red).AddFlag(Colour.Green);
            var se1b = se1.AddFlags(Colour.Green, Colour.Red);
            var se1c = se1 | Colour.Red | Colour.Green;
            var se1d = se1.AddFlags(se1.Objects);

            // Adding the same flag(s) (for all methods) is idempotent.
            Assert.AreEqual(se1, se1a);
            Assert.AreEqual(se1a, se1b);
            Assert.AreEqual(se1b, se1c);
            Assert.AreEqual(se1c, se1d);

            var se2a = se1a.AddFlag(Colour.Purple);
            var se2b = se1b.AddFlags(Colour.Purple);
            var se2c = se1c | Colour.Purple;
            var se2d = se1d.AddFlags(se2c.Objects);

            // Adding a new flag(s) value is reflected (for all input methods).
            Assert.AreEqual(se2a, SmartEnum<Colour>.FromValues(Colour.Purple | Colour.Red | Colour.Green));
            Assert.AreEqual(se2a, se2b);
            Assert.AreEqual(se2b, se2c);
            Assert.AreEqual(se2c, se2d);
        }

        [TestMethod]
        public void HasFlags()
        {
            var se = SmartEnum<Colour>.FromObjects(Colour.Red, Colour.Green, Colour.Purple).Value;

            var hasSingleFlag = se.HasFlag(Colour.Red);
            var hasManyFlags1 = se.HasFlags(Colour.Red);
            var hasManyFlags2 = se.HasFlags(Colour.Red, Colour.Green);
            var hasManyFlags3 = se.HasFlags(Colour.Red, Colour.Purple);
            var hasManyFlags4 = se.HasFlags(Colour.Red, Colour.Green, Colour.Purple);
            var doesNotHaveFlag = se.HasFlag(Colour.Teal);

            Assert.IsTrue(hasSingleFlag);
            Assert.IsTrue(hasManyFlags1);
            Assert.IsTrue(hasManyFlags2);
            Assert.IsTrue(hasManyFlags3);
            Assert.IsTrue(hasManyFlags4);
            Assert.IsFalse(doesNotHaveFlag);

            var hasSingleFlagBitwise = se & Colour.Red;
            var hasManyFlagsBitwise1 = se & SmartEnum<Colour>.FromObjects(Colour.Red);
            var hasManyFlagsBitwise2 = se & SmartEnum<Colour>.FromObjects(Colour.Red, Colour.Green);
            var hasManyFlagsBitwise3 = se & SmartEnum<Colour>.FromObjects(Colour.Red, Colour.Purple);
            var hasManyFlagsBitwise4 = se & SmartEnum<Colour>.FromObjects(Colour.Red, Colour.Green, Colour.Purple);
            var doesNotHaveFlagBitwise = se & Colour.Teal;

            Assert.AreEqual(hasSingleFlagBitwise, Colour.Red);
            Assert.AreEqual(SmartEnum<Colour>.FromObjects(Colour.Red), hasManyFlagsBitwise1);
            Assert.AreEqual(SmartEnum<Colour>.FromObjects(Colour.Red, Colour.Green), hasManyFlagsBitwise2);
            Assert.AreEqual(SmartEnum<Colour>.FromObjects(Colour.Red, Colour.Purple), hasManyFlagsBitwise3);
            Assert.AreEqual(SmartEnum<Colour>.FromObjects(Colour.Red, Colour.Green, Colour.Purple), hasManyFlagsBitwise4);
            Assert.AreEqual(doesNotHaveFlagBitwise, Colour.None);
        }

        [TestMethod]
        public void RemoveFlags()
        {
            var se = SmartEnum<Colour>.FromObjects(Colour.Red, Colour.Purple, Colour.Teal).Value;

            var manyRemove = se.RemoveFlag(Colour.Red).RemoveFlag(Colour.Red);
            var manyRemoves = se.RemoveFlags(Colour.Red, Colour.Teal).RemoveFlags(Colour.Red, Colour.Teal);
            var removeNothing = se.RemoveFlag(Colour.Green);
            var removeSmartEnum = se.RemoveFlags(SmartEnum<Colour>.FromObjects(Colour.Purple, Colour.Teal));

            Assert.AreEqual(SmartEnum<Colour>.FromObjects(Colour.Purple, Colour.Teal), manyRemove);
            Assert.AreEqual(SmartEnum<Colour>.FromObjects(Colour.Purple), manyRemoves);
            Assert.AreEqual(se, removeNothing);
            Assert.IsTrue(removeSmartEnum == Colour.Red);
        }

        [TestMethod]
        public void ToggleFlags()
        {
            var se = SmartEnum<Colour>.FromObjects(Colour.Teal, Colour.Green).Value;

            var removeFlag = se.ToggleFlag(Colour.Green);
            var putFlagBack = removeFlag.ToggleFlag(Colour.Green);
            var addFlag = se.ToggleFlag(Colour.Purple);
            var sameSame = se.ToggleFlag(Colour.Teal).ToggleFlag(Colour.Teal);

            Assert.IsTrue(removeFlag == Colour.Teal);
            Assert.IsTrue(putFlagBack == se);
            Assert.IsTrue(addFlag == se.AddFlag(Colour.Purple));
            Assert.IsTrue(sameSame == se);

            var removeFlagBitwise = se ^ Colour.Green;
            var putFlagBackBitwise = removeFlagBitwise ^ Colour.Green;
            var addFlagBitwise = se ^ Colour.Purple;
            var sameSameBitwise = se ^ Colour.Teal ^ Colour.Teal;

            Assert.IsTrue(removeFlagBitwise == Colour.Teal);
            Assert.IsTrue(putFlagBackBitwise == se);
            Assert.IsTrue(addFlagBitwise == (se | Colour.Purple));
            Assert.IsTrue(sameSameBitwise == se);

            var manyToggles = SmartEnum<Colour>.FromObjects(Colour.Teal, Colour.Green, Colour.Purple) ^ se;
            var none = se ^ se;

            Assert.IsTrue(manyToggles == Colour.Purple);
            Assert.IsTrue(none == Colour.None);
        }
    }
}
