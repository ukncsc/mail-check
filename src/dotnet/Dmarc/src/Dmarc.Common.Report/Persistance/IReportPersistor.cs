namespace Dmarc.Common.Report.Persistance
{
    public interface IReportPersistor<in TDomain>
    {
        void Persist(TDomain report);
    }
}