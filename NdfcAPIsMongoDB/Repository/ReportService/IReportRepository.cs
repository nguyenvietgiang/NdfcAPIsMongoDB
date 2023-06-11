using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository.ReportService
{
    public interface IReportRepository
    {
        Task<Respaging<Report>> GetAllReports(int pageNumber = 1, int pageSize = 10, string? searchTitle = null);
    }
}
