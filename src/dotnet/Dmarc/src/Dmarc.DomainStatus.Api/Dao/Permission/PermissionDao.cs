using System.Data.Common;
using System.Threading.Tasks;
using Dmarc.Common.Data;
using Dmarc.DomainStatus.Api.Domain;
using Microsoft.Extensions.Logging;

namespace Dmarc.DomainStatus.Api.Dao.Permission
{
    public interface IPermissionDao
    {
        Task<DomainPermissions> GetPermissions(int userId, int domainId);
    }

    public class PermissionDao : IPermissionDao
    {
        private readonly IConnectionInfoAsync _connectionInfo;
        private readonly ILogger<IPermissionDao> _log;

        public PermissionDao(IConnectionInfoAsync connectionInfo, ILogger<IPermissionDao> log)
        {
            _connectionInfo = connectionInfo;
            _log = log;
        }

        public Task<DomainPermissions> GetPermissions(int userId, int domainId)
        {
            return Db.ExecuteReaderSingleResultTimed(
                _connectionInfo,
                PermissionDaoResource.SelectPermissionByDomain,
                _ => { _.AddWithValue("userId", userId); _.AddWithValue("domainId", domainId); },
                _ => CreateDomainPermissions(domainId, _),
                _ => _log.LogDebug(_),
                nameof(GetPermissions));
        }

        private DomainPermissions CreateDomainPermissions(int domainId, DbDataReader reader)
        {
            bool aggregatePermission = reader.GetBoolean("aggregate_permission");
            bool domainPermission = reader.GetBoolean("domain_permission");

            return new DomainPermissions(domainId, aggregatePermission, domainPermission);
        }
    }
}
