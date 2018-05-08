using System;

namespace Dmarc.DnsRecord.Evaluator.Explainers
{
    public interface IExplainerStrategy<in T> : IExplainer<T>
    {
        Type Type { get; }
    }
}