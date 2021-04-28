using System;
using System.ComponentModel;
using System.Linq;
using EnumsNET;

namespace Enum.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                foreach (var member in Enums.GetMembers<NumericOperator>())
                {
                    Console.WriteLine($"value:{member.Value}");
                    Console.WriteLine($"name:{member.Name}");
                    Console.WriteLine($"attributes:{member.Attributes}");
                }

                Console.WriteLine(NumericOperator.Equals.AsString(EnumFormat.Description));
                Console.WriteLine(NumericOperator.LessThan.AsString(EnumFormat.Description, EnumFormat.Name));

                var flags = DaysOfWeek.Weekend.GetFlags().ToList();
                Console.WriteLine(flags.Count);
                Console.WriteLine(flags[0]);
                Console.WriteLine(flags[1]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadKey();
        }
        [AttributeUsage(AttributeTargets.Field)]
        public class SymbolAttribute : Attribute
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
            [Symbol("="), Description("Is")]
            Equals,
            [Symbol("!="), Description("Is not")]
            NotEquals,
            [Symbol("<")]
            LessThan,
            [Symbol(">="), PrimaryEnumMember]
            GreaterThanOrEquals,
            NotLessThan = GreaterThanOrEquals,
            [Symbol(">")]
            GreaterThan,
            [Symbol("<="), PrimaryEnumMember]
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
