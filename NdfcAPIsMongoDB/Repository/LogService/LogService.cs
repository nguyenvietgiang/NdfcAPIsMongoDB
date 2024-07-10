using NdfcAPIsMongoDB.Models.ElasticSearchModel;
using Nest;

namespace NdfcAPIsMongoDB.Repository.LogService
{
    public class LogService
    {
        private readonly IElasticClient _elasticClient;

        public LogService()
        {
            var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                .DefaultIndex("logstash-*"); // Thay đổi index nếu cần
            _elasticClient = new ElasticClient(settings);
        }

        public async Task<ISearchResponse<LogEntry>> GetLogsAsync(string query, int page = 1, int pageSize = 10)
        {
            var searchResponse = await _elasticClient.SearchAsync<LogEntry>(s => s
                .Query(q => q
                    .QueryString(d => d
                        .Query(query)
                    )
                )
                .From((page - 1) * pageSize)
                .Size(pageSize)
            );

            return searchResponse;
        }
    }

}
