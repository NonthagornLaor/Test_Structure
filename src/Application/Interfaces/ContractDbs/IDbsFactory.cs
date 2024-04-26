using Microsoft.EntityFrameworkCore;
//using System.Data.Entity;

namespace Application.Interfaces.ContractDbs
{
    public interface IDbsFactory : IDisposable
    {
        DbContext GetContext();

        bool CanConnect();
    }
}
