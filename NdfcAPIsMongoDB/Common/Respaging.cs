namespace NdfcAPIsMongoDB.Common
{
    public class Respaging<T>
    {
        public int currentPage { get; set; }
        public int totalPages { get; set; }
        public int pageSize { get; set; }
        public int totalRecords { get; set; }
        public string next { get; set; }
        public string prev { get; set; }
        public List<T> content { get; set; }
    }
}
