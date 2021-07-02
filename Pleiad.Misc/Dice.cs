using System;

namespace Pleiad.Misc
{
    namespace Dice
    {
        public static class Roll
        {
            private static Random _random = new Random(Guid.NewGuid().GetHashCode());

            /// <summary>
            /// D4 roll
            /// </summary>
            /// <param name="dice">How many rolls</param>
            /// <returns>Roll result</returns>
            public static int D4(int dice = 1) => Custom(4, dice: dice);
            /// <summary>
            /// D6 roll
            /// </summary>
            /// <param name="dice">How many rolls</param>
            /// <returns>Roll result</returns>
            public static int D6(int dice = 1) => Custom(6, dice: dice);
            /// <summary>
            /// D8 roll
            /// </summary>
            /// <param name="dice">How many rolls</param>
            /// <returns>Roll result</returns>
            public static int D8(int dice = 1) => Custom(8, dice: dice);
            /// <summary>
            /// D10 roll
            /// </summary>
            /// <param name="dice">How many rolls</param>
            /// <returns>Roll result</returns>
            public static int D10(int dice = 1) => Custom(10, dice: dice);
            /// <summary>
            /// D12 roll
            /// </summary>
            /// <param name="dice">How many rolls</param>
            /// <returns>Roll result</returns>
            public static int D12(int dice = 1) => Custom(12, dice: dice);
            /// <summary>
            /// D20 roll
            /// </summary>
            /// <param name="dice">How many rolls</param>
            /// <returns>Roll result</returns>
            public static int D20(int dice = 1) => Custom(20, dice: dice);
            /// <summary>
            /// Custom dice roll
            /// </summary>
            /// <param name="upper">Highest score</param>
            /// <param name="lower">Lowest score</param>
            /// <param name="dice">How many rolls</param>
            /// <returns>Roll result</returns>
            public static int Custom(int upper, int lower = 1, int dice = 1)
            {
                int result = 0;
                upper++;
                for (int i = 0; i < dice; i++)
                {
                    result += _random.Next(lower, upper);
                }

                return result;
            }
        }
    }
}
