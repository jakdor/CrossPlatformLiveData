using System;
using System.Linq;

namespace XFTests.Utils
{
    /// <summary>
    /// Test helper methods
    /// </summary>
    internal class TestUtils
    {
        public static readonly Random Random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}
