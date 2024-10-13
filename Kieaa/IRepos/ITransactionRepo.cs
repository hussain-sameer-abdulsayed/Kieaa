namespace Kieaa.IRepos
{
    public interface ITransactionRepo
    {
        Task CommitTransactionAsync();
        Task BeginTransactionAsync();
        Task RollBackTransactionAsync();
    }
}
