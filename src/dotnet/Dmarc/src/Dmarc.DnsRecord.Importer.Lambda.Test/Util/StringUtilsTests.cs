using Dmarc.DnsRecord.Importer.Lambda.Util;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Importer.Lambda.Test.Util
{
    [TestFixture]
    public class StringUtilsTests
    {
        [TestCase("", "", true, TestName = "Empty strings are equal.")]
        [TestCase(null, null, true, TestName = "Null strings are equal.")]
        [TestCase("abc", "abc", true, TestName = "Equivalent strings are equal.")]
        [TestCase("AbC", "aBc", true, TestName = "strings with different cases are equal.")]
        [TestCase("Ab C", "aBc", true, TestName = "strings with different spaces 1 are equal.")]
        [TestCase("A  b  C", "a B c", true, TestName = "strings with different numbers of spaces are equal.")]
        [TestCase(" A  b  C", "a B c", true, TestName = "strings with different leading spaces are equal.")]
        [TestCase("A  b  C ", "a B c", true, TestName = "strings with different trailing spaces are equal.")]
        [TestCase("abc", "abd", false, TestName = "Non equivalent strings are not equal.")]
        [TestCase(null, "", false, TestName = "Null and empty strings are different.")]
        public void Test(string a, string b, bool expectedEqual)
        {
            bool equal = StringUtils.SpaceInsensitiveEquals(a, b);
            Assert.That(equal, Is.EqualTo(expectedEqual));
        }
    }
}
