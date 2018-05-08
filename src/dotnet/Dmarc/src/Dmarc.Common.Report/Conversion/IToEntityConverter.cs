namespace Dmarc.Common.Report.Conversion
{
    public interface IToEntityConverter<in TIn, out TOut>
    {
        TOut Convert(TIn forensicReportInfo);
    }
}
