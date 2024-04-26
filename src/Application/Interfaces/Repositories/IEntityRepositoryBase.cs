using System.Data;
using System.Linq.Expressions;

namespace Application.Interfaces.Repositories
{
    public interface IEntityRepositoryBase<T> where T : class
    {
        void Create(T entity);
        Task CreateAsync(T entity);
        void Create(IEnumerable<T> entity);
        Task CreateAsync(IEnumerable<T> entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(IEnumerable<T> entity);
        void Delete(Func<T, bool> where);
        T GetByKey(decimal key);
        Task<T> GetByKeyAsync(decimal key);
        T GetByKey(int key);
        Task<T> GetByKeyAsync(int key);
        IQueryable<T> Get();
        IQueryable<T> Get(Expression<Func<T, bool>> where);
        IEnumerable<T> SqlQuery(string query, Dictionary<string, object> parameters = null!);
        Task<IEnumerable<T>> SqlQueryAsync(string query, Dictionary<string, object> parameters = null!);
        List<DataTable> ExecuteStroePorcedure(string functionName, object[] parameters = null);
    }
}
