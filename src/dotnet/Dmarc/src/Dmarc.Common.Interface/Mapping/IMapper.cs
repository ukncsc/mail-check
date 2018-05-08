namespace Dmarc.Common.Interface.Mapping
{
    public interface IMapper<in TIn, out TOut>
    {
        TOut Map(TIn t);
    }
}