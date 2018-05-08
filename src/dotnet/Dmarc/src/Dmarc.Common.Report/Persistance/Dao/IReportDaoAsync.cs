using System.Threading.Tasks;

namespace Dmarc.Common.Report.Persistance.Dao
{
    public interface IReportDaoAsync<in T>
    {
        Task<bool> Add(T t);
    }
}
