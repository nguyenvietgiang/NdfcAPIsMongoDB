using Nest;

namespace NdfcAPIsMongoDB.Common.ElasticSearch
{
    public interface IElasticsearchService
    {
        ElasticClient GetClient();
    }
}
