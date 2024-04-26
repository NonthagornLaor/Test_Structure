namespace Application.Interfaces.ContractDbs
{
    public interface IUnitOfWork
    {
        void Persist();
        Task PersistAsync();
    }
}
