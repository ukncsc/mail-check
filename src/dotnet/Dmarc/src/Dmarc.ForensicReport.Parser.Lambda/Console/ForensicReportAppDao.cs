using Dmarc.Common.Report.Conversion;
using Dmarc.Common.Report.Persistance;
using Dmarc.Common.Report.Persistance.Dao;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Domain;

namespace Dmarc.ForensicReport.Parser.Lambda.Console
{
    public class ForensicReportAppPersistor : IReportPersistor<ForensicReportInfo>
    {
        private readonly IReportDaoAsync<ForensicReportEntity> _forensicReportDao;
        private readonly IToEntityConverter<ForensicReportInfo, ForensicReportEntity> _converter;

        public ForensicReportAppPersistor(IReportDaoAsync<ForensicReportEntity> forensicReportDao,
            IToEntityConverter<ForensicReportInfo, ForensicReportEntity> converter)
        {
            _forensicReportDao = forensicReportDao;
            _converter = converter;
        }
        
        public void Persist(ForensicReportInfo t)
        {
            ForensicReportEntity forensicReportEntity = _converter.Convert(t);
            _forensicReportDao.Add(forensicReportEntity).Wait();
        }
    }
}
