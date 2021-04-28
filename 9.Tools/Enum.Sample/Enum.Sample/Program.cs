using System;

namespace Enum.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                EnumUnit.EnumerateTest();
                EnumUnit.FlagEnumOperationsTest();
                EnumUnit.ToStringTest();
                EnumUnit.ValidateTest();
                EnumUnit.CustomEnumFormatTest();
                EnumUnit.AttributesTest();
                EnumUnit.ParsingTest();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.ReadKey();
        }
    }
}
