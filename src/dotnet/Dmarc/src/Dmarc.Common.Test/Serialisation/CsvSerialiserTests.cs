using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmarc.Common.Serialisation;
using NUnit.Framework;

namespace Dmarc.Common.Test.Serialisation
{
    [TestFixture]
    public class CsvSerialiserTests
    {
        [TestCaseSource(nameof(TestData))]
        public async Task TestAsync(List<TestObject> inputData, bool includeHeaders, char separator, string expectedResult)
        {
            Stream serializedStream = await CsvSerialiser.SerialiseAsync(inputData, includeHeaders, separator);

            string result = await StreamToStringsAsync(serializedStream);

            Assert.AreEqual(expectedResult, result);
        }

        [TestCaseSource(nameof(TestData))]
        public void Test(List<TestObject> inputData, bool includeHeaders, char separator, string expectedResult)
        {
            Stream serializedStream = CsvSerialiser.Serialise(inputData, includeHeaders, separator);

            string result = StreamToStrings(serializedStream);

            Assert.AreEqual(expectedResult, result);
        }

        public static IEnumerable<TestCaseData> TestData()
        {
            yield return new TestCaseData(new List<TestObject>(), false, ',', string.Empty).SetName("Empty list with includeHeader=false returns empty result.");
            yield return new TestCaseData(new List<TestObject>(), true, ',', $"item_id,item_name{System.Environment.NewLine}").SetName("Empty list with includeHeader=true returns header.");
            yield return new TestCaseData(new List<TestObject> {new TestObject(1,"two")}, false, ',', $"1,two{System.Environment.NewLine}").SetName("Items with includeHeader=false returns items.");
            yield return new TestCaseData(new List<TestObject> {new TestObject(1,"two")}, true, ',', $"item_id,item_name{System.Environment.NewLine}1,two{System.Environment.NewLine}").SetName("Items with includeHeader=true returns header and items.");
            yield return new TestCaseData(new List<TestObject> {new TestObject(1,"two")}, true, '*', $"item_id*item_name{System.Environment.NewLine}1*two{System.Environment.NewLine}").SetName("Alternative separator respected.");
            yield return new TestCaseData(new List<TestObject> { new TestObject(1, "=(two)") }, false, ',', $"1,'=(two){System.Environment.NewLine}").SetName("CSV injection mitigation for = is '=.");
            yield return new TestCaseData(new List<TestObject> { new TestObject(1, "+(two)") }, false, ',', $"1,'+(two){System.Environment.NewLine}").SetName("CSV injection mitigation for + is '+.");
            yield return new TestCaseData(new List<TestObject> { new TestObject(1, "-(two)") }, false, ',', $"1,'-(two){System.Environment.NewLine}").SetName("CSV injection mitigation for - is '-.");
            yield return new TestCaseData(new List<TestObject> { new TestObject(1, "@(two)") }, false, ',', $"1,'@(two){System.Environment.NewLine}").SetName("CSV injection mitigation for @ is '@.");
            yield return new TestCaseData(new List<TestObject> { new TestObject(1, "two,") }, false, ',', $"1,\"two,\"{System.Environment.NewLine}" ).SetName("Separator included in data is encodeded");
            yield return new TestCaseData(new List<TestObject> { new TestObject(1, "two\r") }, false, ',', $"1,\"two\r\"{System.Environment.NewLine}" ).SetName("Carriage return included in data is encodeded");
            yield return new TestCaseData(new List<TestObject> { new TestObject(1, "two\n") }, false, ',', $"1,\"two\n\"{System.Environment.NewLine}" ).SetName("Line feed included in data is encodeded");
            yield return new TestCaseData(new List<TestObject> { new TestObject(1, "two\"") }, false, ',', $"1,\"two\"\"\"{System.Environment.NewLine}").SetName("Quotes are encoded");
        }

        public class TestObject
        {
            public TestObject(int itemId, string itemName)
            {
                ItemId = itemId;
                ItemName = itemName;
            }

            public int ItemId { get; }
            public string ItemName { get; }
        }

        private static Task<string> StreamToStringsAsync(Stream stream)
        {
            using (StreamReader streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEndAsync();
            }
        }

        private static string StreamToStrings(Stream stream)
        {
            using (StreamReader streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
