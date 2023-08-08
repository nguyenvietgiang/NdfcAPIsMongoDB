using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository.HistoryService
{
    public interface IHistoryRepositorycs
    {
        Task<ClubHistory> GetClubHistoryAsync();
        Task UpdateClubHistoryAsync(ClubHistory clubHistory);
        Task DeleteClubHistoryAsync();
    }
}
