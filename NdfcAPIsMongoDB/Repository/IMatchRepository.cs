﻿using NdfcAPIsMongoDB.Common;
using NdfcAPIsMongoDB.Models;

namespace NdfcAPIsMongoDB.Repository
{
    public interface IMatchRepository 
    {
        Task<Respaging<Match>> GetAllMatch(int pageNumber = 1, int pageSize = 10, string? searchName = null);

        Task<Match> GetMatchById(string id);

        Task<bool> DeleteMatch(string id); 
    }
}
