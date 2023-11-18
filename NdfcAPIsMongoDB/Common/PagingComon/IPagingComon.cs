namespace NdfcAPIsMongoDB.Common.PagingComon
{
    public interface IPagingComon
    {
        Task<Respaging<T>> GetAllData<T>(int pageNumber = 1, int pageSize = 10, string? searchName = null);
    }
}
