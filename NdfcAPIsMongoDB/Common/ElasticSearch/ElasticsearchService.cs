using Microsoft.Extensions.Options;
using Nest;

namespace NdfcAPIsMongoDB.Common.ElasticSearch
{
    public class ElasticsearchService : IElasticsearchService
    {
        private readonly ElasticClient _elasticClient;

        public ElasticsearchService(IOptions<ElasticsearchSettings> settings)
        {
            var connectionSettings = new ConnectionSettings(new Uri(settings.Value.Url))
                .DefaultIndex("players");
            _elasticClient = new ElasticClient(connectionSettings);
        }

        public ElasticClient GetClient()
        {
            return _elasticClient;
        }
    }

    public class ElasticsearchSettings
    {
        public string Url { get; set; }
    }
}
