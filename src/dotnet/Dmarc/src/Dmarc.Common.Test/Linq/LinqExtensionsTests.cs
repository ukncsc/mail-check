using System.Collections.Generic;
using System.Linq;
using Dmarc.Common.Linq;
using NUnit.Framework;

namespace Dmarc.Common.Test.Linq
{
    [TestFixture]
    public class LinqExtensionsTests
    {

        [Test]
        public void BatchNoElementsInBatchEmptyEnumerableReturned()
        {
            IEnumerable<IEnumerable<int>> batches = Enumerable.Empty<int>().Batch(2);
            Assert.That(batches.Count(), Is.EqualTo(0));
        }

        [Test]
        public void BatchSizeEqualToNumberOfElementsOneBatchReturned()
        {
            IEnumerable<IEnumerable<int>> batches = Enumerable.Range(0,2).Batch(2);
            Assert.That(batches.Count(), Is.EqualTo(1));
            Assert.That(batches.First().Count(), Is.EqualTo(2));
            Assert.That(batches.First().First(), Is.EqualTo(0));
            Assert.That(batches.First().ElementAt(1), Is.EqualTo(1));
        }

        [Test]
        public void BatchSizeGreaterThanNumberOfElementsTwoBatchesReturned()
        {
            IEnumerable<IEnumerable<int>> batches = Enumerable.Range(0, 3).Batch(2);
            Assert.That(batches.Count(), Is.EqualTo(2));
            Assert.That(batches.First().Count(), Is.EqualTo(2));
            Assert.That(batches.First().First(), Is.EqualTo(0));
            Assert.That(batches.First().ElementAt(1), Is.EqualTo(1));

            Assert.That(batches.ElementAt(1).Count(), Is.EqualTo(1));
            Assert.That(batches.ElementAt(1).First(), Is.EqualTo(2));
        }
    }
}
