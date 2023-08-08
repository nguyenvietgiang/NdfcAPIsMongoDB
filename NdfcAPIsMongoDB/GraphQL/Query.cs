using HotChocolate;
using NdfcAPIsMongoDB.Models;
using NdfcAPIsMongoDB.Repository.PlayerService;

namespace NdfcAPIsMongoDB.GraphQL
{
    public class Query
    {
        public async Task<Player> GetPlayerById(
            [Service] IPlayerRepository playerRepository,
            string id)
        {
            return await playerRepository.GetPlayerById(id);
        }
    }
}
