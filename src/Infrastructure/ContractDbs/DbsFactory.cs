using System;
using Application.Interfaces.ContractDbs;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.ContractDbs
{
    public class DbsFactory : IDisposable, IDbsFactory
    {
        private DbContext _context;

        public DbsFactory()
        {
        }

        public DbsFactory(DbContext context)
        {
            this._context = context;
        }

        public DbContext GetContext() => _context;

        public bool CanConnect()
        {
            return this._context.Database.CanConnect();
        }


        public void Dispose()
        {

            if (_context != null)
                _context.Dispose();
        }
    }
}
