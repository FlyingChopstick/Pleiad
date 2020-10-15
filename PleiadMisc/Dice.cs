using System;

namespace PleiadMisc
{
    namespace Dice
    {
        public static class Roll
        {
            private static Random _random = new Random(Guid.NewGuid().GetHashCode());
            public static int D4(int dice = 1)
            {
                int result = 0;
                for (int i = 0; i < dice; i++)
                {
                    result += _random.Next(1, 5);
                }

                return result;
            }
            public static int D6(int dice = 1)
            {
                int result = 0;
                for (int i = 0; i < dice; i++)
                {
                    result += _random.Next(1, 7);
                }

                return result;
            }
            public static int D8(int dice = 1)
            {
                int result = 0;
                for (int i = 0; i < dice; i++)
                {
                    result += _random.Next(1, 9);
                }

                return result;
            }
            public static int D10(int dice = 1)
            {
                int result = 0;
                for (int i = 0; i < dice; i++)
                {
                    result += _random.Next(1, 11);
                }

                return result;
            }
            public static int D12(int dice = 1)
            {
                int result = 0;
                for (int i = 0; i < dice; i++)
                {
                    result += _random.Next(1, 13);
                }

                return result;
            }
            public static int D20(int dice = 1)
            {
                int result = 0;
                for (int i = 0; i < dice; i++)
                {
                    result += _random.Next(1, 21);
                }

                return result;
            }
            /// <summary>
            /// Custom dice roll. Both upper and lower bounds are inclusive (D4: upper=4, lower=1)
            /// </summary>
            /// <param name="upper">Inclusive upper boundary</param>
            /// <param name="lower">Inclusive lower boundary</param>
            /// <param name="dice">How many rolls</param>
            /// <returns>Roll result</returns>
            public static int DCustom(int upper, int lower = 1, int dice = 1)
            {
                int result = 0;
                for (int i = 0; i < dice; i++)
                {
                    result += _random.Next(lower, upper + 1);
                }

                return result;
            }
        }
    }
}
