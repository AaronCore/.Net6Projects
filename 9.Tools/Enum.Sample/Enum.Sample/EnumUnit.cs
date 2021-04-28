using System;
using System.ComponentModel;
using System.Linq;
using EnumsNET;

namespace Enum.Sample
{
    public class EnumUnit
    {
        public static void EnumerateTest()
        {
            foreach (var member in Enums.GetMembers<NumericOperator>())
            {
                Console.WriteLine($"value:{member.Value}");
                Console.WriteLine($"description:{member.AsString(EnumFormat.Description)}");
                Console.WriteLine($"name:{member.Name}");
                Console.WriteLine($"description:{member.AsString(EnumFormat.Description)}");
                Console.WriteLine($"attributes:{member.Attributes}");
            }

            foreach (var value in Enums.GetValues<NumericOperator>(EnumMemberSelection.Distinct))
            {
                Console.WriteLine($"getValue:{value.GetName()}");
                Console.WriteLine($"getAttributes:{value.GetAttributes()}");
            }
        }

        public static void FlagEnumOperationsTest()
        {
            // HasAllFlags
            Console.WriteLine((DaysOfWeek.Monday | DaysOfWeek.Wednesday | DaysOfWeek.Friday).HasAllFlags(DaysOfWeek.Monday | DaysOfWeek.Wednesday));
            Console.WriteLine(DaysOfWeek.Monday.HasAllFlags(DaysOfWeek.Monday | DaysOfWeek.Wednesday));

            // HasAnyFlags
            Console.WriteLine(DaysOfWeek.Monday.HasAnyFlags(DaysOfWeek.Monday | DaysOfWeek.Wednesday));
            Console.WriteLine((DaysOfWeek.Monday | DaysOfWeek.Wednesday).HasAnyFlags(DaysOfWeek.Friday));

            // CombineFlags ~ bitwise OR
            Console.WriteLine((DaysOfWeek.Monday | DaysOfWeek.Wednesday, DaysOfWeek.Monday.CombineFlags(DaysOfWeek.Wednesday)));
            Console.WriteLine((DaysOfWeek.Monday | DaysOfWeek.Wednesday | DaysOfWeek.Friday, FlagEnums.CombineFlags(DaysOfWeek.Monday, DaysOfWeek.Wednesday, DaysOfWeek.Friday)));

            // CommonFlags ~ bitwise AND
            Console.WriteLine((DaysOfWeek.Monday, DaysOfWeek.Monday.CommonFlags(DaysOfWeek.Monday | DaysOfWeek.Wednesday)));
            Console.WriteLine((DaysOfWeek.None, DaysOfWeek.Monday.CommonFlags(DaysOfWeek.Wednesday)));

            // RemoveFlags
            Console.WriteLine((DaysOfWeek.Wednesday, (DaysOfWeek.Monday | DaysOfWeek.Wednesday).RemoveFlags(DaysOfWeek.Monday)));
            Console.WriteLine((DaysOfWeek.None, (DaysOfWeek.Monday | DaysOfWeek.Wednesday).RemoveFlags(DaysOfWeek.Monday | DaysOfWeek.Wednesday)));

            // GetFlags, splits out the individual flags in increasing significance bit order
            var flags = DaysOfWeek.Weekend.GetFlags().ToList();
            Console.WriteLine(flags.Count);
            Console.WriteLine(flags[0]);
            Console.WriteLine(flags[1]);
        }

        public static void ToStringTest()
        {
            // AsString, equivalent to ToString
            Console.WriteLine(NumericOperator.Equals.AsString());
            Console.WriteLine(((NumericOperator)(-1)).AsString());

            // GetName
            Console.WriteLine(NumericOperator.Equals.GetName());
            Console.WriteLine(((NumericOperator)(-1)).GetName());

            // Get description
            Console.WriteLine(NumericOperator.Equals.AsString(EnumFormat.Description));
            Console.WriteLine(NumericOperator.LessThan.AsString(EnumFormat.Description));

            // Get description if applied, otherwise the name
            Console.WriteLine(NumericOperator.LessThan.AsString(EnumFormat.Description, EnumFormat.Name));
        }

        public static void ValidateTest()
        {
            // Standard Enums, checks is defined
            Console.WriteLine(NumericOperator.LessThan.IsValid());
            Console.WriteLine(((NumericOperator)20).IsValid());

            // Flag Enums, checks is valid flag combination or is defined
            Console.WriteLine((DaysOfWeek.Sunday | DaysOfWeek.Wednesday).IsValid());
            Console.WriteLine((DaysOfWeek.Sunday | DaysOfWeek.Wednesday | ((DaysOfWeek)(-1))).IsValid());

            // Custom validation through IEnumValidatorAttribute<TEnum>
            Console.WriteLine(DayType.Weekday.IsValid());
            Console.WriteLine((DayType.Weekday | DayType.Holiday).IsValid());
            Console.WriteLine((DayType.Weekday | DayType.Weekend).IsValid());
        }

        public static void CustomEnumFormatTest()
        {
            EnumFormat symbolFormat = Enums.RegisterCustomEnumFormat(member => member.Attributes.Get<SymbolAttribute>()?.Symbol);
            Console.WriteLine(NumericOperator.GreaterThan.AsString(symbolFormat));
            Console.WriteLine((NumericOperator.LessThan, Enums.Parse<NumericOperator>("<", ignoreCase: false, symbolFormat)));
        }

        public static void AttributesTest()
        {
            Console.WriteLine(NumericOperator.NotEquals.GetAttributes().Get<SymbolAttribute>().Symbol);
            Console.WriteLine(Enums.GetMember<NumericOperator>("GreaterThanOrEquals").Attributes.Has<PrimaryEnumMemberAttribute>());
            Console.WriteLine(NumericOperator.LessThan.GetAttributes().Has<DescriptionAttribute>());
        }

        public static void ParsingTest()
        {
            Console.WriteLine((NumericOperator.GreaterThan, Enums.Parse<NumericOperator>("GreaterThan")));
            Console.WriteLine((NumericOperator.NotEquals, Enums.Parse<NumericOperator>("1")));
            Console.WriteLine((NumericOperator.Equals, Enums.Parse<NumericOperator>("Is", ignoreCase: false, EnumFormat.Description)));
            Console.WriteLine((DaysOfWeek.Monday | DaysOfWeek.Wednesday, Enums.Parse<DaysOfWeek>("Monday, Wednesday")));
            Console.WriteLine((DaysOfWeek.Tuesday | DaysOfWeek.Thursday, FlagEnums.ParseFlags<DaysOfWeek>("Tuesday | Thursday", ignoreCase: false, delimiter: "|")));
        }


        [AttributeUsage(AttributeTargets.Field)]
        class SymbolAttribute : Attribute
        {
            public string Symbol { get; }
            public SymbolAttribute(string symbol)
            {
                Symbol = symbol;
            }
        }
        [AttributeUsage(AttributeTargets.Enum)]
        class DayTypeValidatorAttribute : Attribute, IEnumValidatorAttribute<DayType>
        {
            public bool IsValid(DayType value) => value.GetFlagCount(DayType.Weekday | DayType.Weekend) == 1 && FlagEnums.IsValidFlagCombination(value);
        }
        [Flags]
        enum NumericOperator
        {
            [SymbolAttribute("="), Description("Is")]
            Equals,
            [SymbolAttribute("!="), Description("Is not")]
            NotEquals,
            [SymbolAttribute("<")]
            LessThan,
            [SymbolAttribute(">="), PrimaryEnumMember]
            GreaterThanOrEquals,
            NotLessThan = GreaterThanOrEquals,
            [SymbolAttribute(">")]
            GreaterThan,
            [SymbolAttribute("<="), PrimaryEnumMember]
            LessThanOrEquals,
            NotGreaterThan = LessThanOrEquals
        }
        enum DaysOfWeek
        {
            None = 0,
            Sunday = 1,
            Monday = 2,
            Tuesday = 4,
            Wednesday = 8,
            Thursday = 16,
            Friday = 32,
            Weekdays = Monday | Tuesday | Wednesday | Thursday | Friday,
            Saturday = 64,
            Weekend = Sunday | Saturday,
            All = Sunday | Monday | Tuesday | Wednesday | Thursday | Friday | Saturday
        }
        [Flags, DayTypeValidator]
        enum DayType
        {
            Weekday = 1,
            Weekend = 2,
            Holiday = 4
        }
    }
}
