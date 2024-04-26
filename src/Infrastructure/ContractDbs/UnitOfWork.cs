using Application.Interfaces.ContractDbs;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.ContractDbs
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbsFactory _dbFactory;
        private DbContext _context;

        public UnitOfWork(IDbsFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }
        protected DbContext DbContext
        {
            get { return _context ??= _dbFactory.GetContext(); }
        }
        public void Persist()
        {
            try
            {
                DbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                DbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                foreach (var x in ex.Entries)
                    DbContext.Entry(x).Reload();

                DbContext.SaveChanges();
            }
        }
        public async Task PersistAsync()
        {
            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                DbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                foreach (var x in ex.Entries)
                    await DbContext.Entry(x).ReloadAsync();

                await DbContext.SaveChangesAsync();
            }
        }
    }
}
