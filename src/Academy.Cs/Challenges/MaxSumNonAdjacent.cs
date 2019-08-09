using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Academy.Cs.Challenges
{
    /// <summary>
    /// Challenge: Get the maximal sum of non-adjacent values in an array.
    /// 
    /// Tags: dynamic programming, arrays.
    /// 
    /// Description:
    /// 
    /// There are n houses built in a line, each of which contains some value in it. A thief is
    /// going to steal the maximal value in these houses, but he cannot steal in two adjacent houses
    /// because the owner of a stolen house will tell his two neighbors on the left and right side.
    /// What is the maximal stolen value?
    /// 
    /// For example, if there are four houses with values {6, 1, 2, 7}, the maximal stolen value is
    /// 13 when the first and fourth houses are stolen.
    /// 
    /// Analysis:
    /// 
    /// A function f(i) is defined to denote the maximal stolen value from the first house to the
    /// ith house, and the value contained in the ith house is denoted as vi.When the thief reaches
    /// the ith house, he has two choices: to steal or not.Therefore, f(i) can be defined with the
    /// following equation:
    /// 
    ///     f(i) = max[vi + f(i - 2) when the ith house is stolen, f(i - 1) otherwise]
    ///     
    /// It would be much more efficient to calculate in bottom-up order than to calculate
    /// recursively. It looks like a 1D array with size n is needed, but actually it is only
    /// necessary to cache two values for f(i-1) and f(i-2) to calculate f(i).
    /// </summary>
    /// <see cref="http://codercareer.blogspot.com/2013/02/no-44-maximal-stolen-values.html"/>
    public class MaxSumNonAdjacent
    {
        /// <summary>
        /// Gets the maximal sum of non-adjacent values in am array.
        /// </summary>
        /// <param name="values">The array of values. Never null. Does not contain negative values.</param>
        /// <returns>The maximal sum of non-adjacent values. Zero, if <paramref name="values"/> is null or empty.</returns>
        public static int Get(int[] values)
        {
            if (values.Length == 0)
            {
                return 0;
            }

            int value1 = values[0];
            if (values.Length == 1)
            {
                return value1;
            }

            int value2 = Math.Max(values[0], values[1]);
            if (values.Length == 2)
            {
                return value2;
            }

            int value = 0;
            for (int i = 2; i < values.Length; i++)
            {
                value = Math.Max(value2, value1 + values[i]);
                value1 = value2;
                value2 = value;
            }

            return value;
        }

    }

    /// <summary>
    /// Tests the <see cref="MaxSumNonAdjacent"/> class.
    /// </summary>
    [TestClass]
    public class MaxSumNonAdjacentTests
    {
        /// <summary>
        /// Tests the <see cref="MaxSumNonAdjacent.Get(int[])"/> method with a range of inputs.
        /// </summary>
        /// <param name="result">The expected result.</param>
        /// <param name="values">The input values.</param>
        [DataRow(13, new int[] { 6, 1, 2, 7 }, DisplayName = "Example")]
        [DataRow(0, new int[0], DisplayName = "Empty")]
        [DataRow(3, new int[] { 3 }, DisplayName = "One value")]
        [DataRow(3, new int[] { 1, 3 }, DisplayName = "Two values")]
        [DataRow(11, new int[] { 0, 1, 2, 3, 4, 5, 5 }, DisplayName = "Increasing values")]
        [DataTestMethod]
        public void TestGet(int result, int[] values)
        {
            Assert.AreEqual(result, MaxSumNonAdjacent.Get(values));
        }
    }
}