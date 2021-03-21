using DepsWebApp.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DepsWebApp.Services
{
    public interface IDbService
    {
        Task<bool> Add(Account newAccount);

        Task<bool> Find(Account newAccount);
    }
}
