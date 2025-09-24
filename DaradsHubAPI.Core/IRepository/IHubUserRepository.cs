using DaradsHubAPI.Core.Model.Response;
using DaradsHubAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaradsHubAPI.Core.IRepository;
public interface IHubUserRepository : IGenericRepository<userstb>
{
    Task<(bool status, string message, ShortAgentProfileResponse? res)> GetAgentProductProfile(int agentId);
}
