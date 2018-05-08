using System;
using System.Xml.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc;
using Dmarc.AggregateReport.Parser.Lambda.Utils;

namespace Dmarc.AggregateReport.Parser.Lambda.Serialisation.AggregateReportDeserialisation
{
    public interface IRowDeserialiser
    {
        Row Deserialise(XElement row);
    }

    public class RowDeserialiser : IRowDeserialiser
    {
        private readonly IPolicyEvaluatedDeserialiser _policyEvaluatedDeserialiser;

        public RowDeserialiser(IPolicyEvaluatedDeserialiser policyEvaluatedDeserialiser)
        {
            _policyEvaluatedDeserialiser = policyEvaluatedDeserialiser;
        }

        public Row Deserialise(XElement row)
        {
            if (row.Name != "row")
            {
                throw new ArgumentException("Root element must be row");
            }

            string sourceIp = row.Single("source_ip").Value;

            int count = int.Parse(row.Single("count").Value);

            PolicyEvaluated policyEvaluated = _policyEvaluatedDeserialiser.Deserialise(row.Single("policy_evaluated"));

            return new Row(sourceIp, count, policyEvaluated);
        }
    }
}