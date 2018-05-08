using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Domain;
using Dmarc.AggregateReport.Parser.Lambda.Persistence.Single;
using Dmarc.Common.Linq;

namespace Dmarc.AggregateReport.Parser.Lambda.Persistence.Multi
{
    public interface IMultiDenormalisedRecordPersistor : IDenormalisedRecordPersistor
    {
        int Count { get; }

        bool Active { get; }
    }

    public class MulitDenormalisedRecordPersistor : IMultiDenormalisedRecordPersistor, IDisposable
    {
        private readonly IEnumerable<IDenormalisedRecordPersistor> _persistors;

        public MulitDenormalisedRecordPersistor(IEnumerable<IDenormalisedRecordPersistor> persistors)
        {
            _persistors = persistors;
        }

        public void Persist(IEnumerable<DenormalisedRecord> denormalisedRecords)
        {
            _persistors.ForEach(_ => _.Persist(denormalisedRecords));
        }

        public int Count => _persistors.Count();

        public bool Active => _persistors.Any();
        public void Dispose()
        {
            _persistors.ForEach(_ => { (_ as IDisposable)?.Dispose();});
        }
    }
}