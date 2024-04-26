using System;
using Application.Interfaces;
using Application.Interfaces.ContractDbs;
using Application.Interfaces.Repositories;

namespace Infrastructure.Repositories
{
    public class EntityRepository<T> : EntityRepositoryBase<T>, IEntityRepository<T> where T : class
    {
        public EntityRepository(IDbsFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
