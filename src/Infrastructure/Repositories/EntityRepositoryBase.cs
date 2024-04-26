using Application.Interfaces.ContractDbs;
using Infrastructure.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Data;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class EntityRepositoryBase<T> where T : class
    {
        private DbContext _context;
        private readonly DbSet<T> _entity;
        protected IDbsFactory DbFactory { get; private set; }
        public EntityRepositoryBase(IDbsFactory dbFactory)
        {
            DbFactory = dbFactory;
            _entity = DbContext.Set<T>();
        }
        protected DbContext DbContext
        {
            get { return _context ?? (_context = DbFactory.GetContext()); }
        }

        public virtual void Create(T entity)
        {
            _entity.Add(entity);
        }

        public virtual async Task CreateAsync(T entity)
        {
            await _entity.AddAsync(entity);
        }

        public virtual void Create(IEnumerable<T> entity)
        {
            _entity.AddRange(entity);
        }

        public virtual async Task CreateAsync(IEnumerable<T> entity)
        {
            await _entity.AddRangeAsync(entity);
        }

        public virtual void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Update(T entity, T oldEntity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Cannot add a null entity.");
            }

            var entry = _context.Entry<T>(entity);

            if (entry.State == EntityState.Detached)
            {
                if (oldEntity != null)
                {
                    var attachedEntry = _context.Entry(oldEntity);
                    attachedEntry.CurrentValues.SetValues(entity);
                }
                else
                {
                    entry.State = EntityState.Modified; // This should attach entity
                }
            }
        }

        public virtual void Delete(T entity)
        {
            _entity.Remove(entity);
        }

        public virtual void Delete(IEnumerable<T> entity)
        {
            _entity.RemoveRange(entity);
        }

        public virtual void Delete(Func<T, bool> where)
        {
            IQueryable<T> entities = _entity.Where<T>(where).AsQueryable<T>();

            foreach (T entity in entities)
                _entity.Remove(entity);
        }

        public virtual T GetByKey(decimal key)
        {
            return _entity.Find(key);
        }

        public virtual async Task<T> GetByKeyAsync(decimal key)
        {
            return await _entity.FindAsync(key);
        }

        public virtual T GetByKey(int key)
        {
            return _entity.Find(key);
        }

        public virtual async Task<T> GetByKeyAsync(int key)
        {
            return await _entity.FindAsync(key);
        }

        public virtual IQueryable<T> Get()
        {
            return _entity.AsQueryable().AsNoTracking();
        }

        public virtual IQueryable<T> Get(Expression<Func<T, bool>> where)
        {
            return _entity.Where<T>(where).AsQueryable<T>().AsNoTracking();
        }

        public virtual IEnumerable<T> SqlQuery(string query, Dictionary<string, object> parameters = null!)
        {
            IEnumerable<T> _results;
            try
            {
                if (parameters == null)
                {
                    var queryable = _entity.FromSqlRaw(query);
                    _results = queryable.ToList();
                }
                else
                {
                    var pamars = new List<SqlParameter>();

                    foreach (var parameter in parameters)
                        pamars.Add(new SqlParameter(parameter.Key, parameter.Value));

                    var queryable = _entity.FromSqlRaw(query, pamars.ToArray());
                    _results = queryable.ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error occured SqlQuery.");
                throw;
            }
            return _results;
        }

        public virtual async Task<IEnumerable<T>> SqlQueryAsync(string query, Dictionary<string, object> parameters = null!)
        {
            IEnumerable<T> _results;
            try
            {
                if (parameters == null)
                {
                    var queryable = _entity.FromSqlRaw(query);
                    _results = await queryable.ToListAsync();
                }
                else
                {
                    var pamars = new List<SqlParameter>();

                    foreach (var parameter in parameters)
                        pamars.Add(new SqlParameter(parameter.Key, parameter.Value));

                    var queryable = _entity.FromSqlRaw(query, pamars.ToArray());
                    _results = await queryable.ToListAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error occured SqlQuery.");
                throw;
            }
            return _results;
        }

        public List<DataTable> ExecuteStroePorcedure(string functionName, object[] parameters = null)
        {
            var _array = new List<DataTable>();
            var _refcur = new List<string>();

            var connectionString = _context.Database.GetConnectionString();
            SqlConnection _connection = new SqlConnection(connectionString);
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            var _args = PrepareOraArguments(functionName, parameters);

            if (_args.Item2 != null)
                _refcur = _args.Item2.GetCursor();

            using (var _transaction = _connection.BeginTransaction())
            {
                //for (int i = 0; i < _args.Item3.Count; i++)
                //{
                //    _transaction.Connection?.MapCompositeFactory(_args.Item3[i]);
                //}
                var _command = new SqlCommand(_args.Item1, _connection, _transaction);
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandTimeout = 120;//seconds
                _command.AddParameter(_args.Item2);
                _command.ExecuteNonQuery();

                if (_refcur.Count > 0)
                {
                    SqlDataReader reader = _command.ExecuteReader();
                    DataTable dataTable = ConvertToDataTable(reader);
                    //List<object> results = reader.Cast<object>().ToList();
                    _array.Add(dataTable);
                    reader.Close();

                }
                for (int i = 0; i < _command.Parameters.Count; i++)
                {
                    var _npgsql = _command.Parameters[i];
                    if (_npgsql.Direction == ParameterDirection.Input) continue;
                    if (_npgsql.Direction == ParameterDirection.InputOutput || _npgsql.Direction == ParameterDirection.Output)
                    {
                        _args.Item2[i].Value = _command.Parameters[i];
                    }
                }
                _transaction.Commit();
            }
            return _array;
        }

        private Tuple<string, List<SqlParameter>, List<string>> PrepareOraArguments(string storedProcedure, object[] parameters)
        {
            var parameterNames = new List<string>();
            var arguments = new List<SqlParameter>();
            var arguments_udt = new List<string>();
            if (parameters != null)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    SqlParameter outParam = (SqlParameter)parameters[i];
                    parameterNames.Add("@" + outParam.ParameterName);
                    arguments.Add(outParam);
                    var outType = outParam.Value?.GetType();

                }

            }

            return Tuple.Create(storedProcedure, arguments, arguments_udt);
        }
        public static DataTable ConvertToDataTable(SqlDataReader reader)
        {
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            return dataTable;
        }
    }
}
