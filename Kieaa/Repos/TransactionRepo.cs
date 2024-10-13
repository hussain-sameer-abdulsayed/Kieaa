using Kieaa.Data;
using Kieaa.IRepos;

namespace Kieaa.Repos
{
    public class TransactionRepo : ITransactionRepo
    {
        private readonly Context _context;

        public TransactionRepo(Context context)
        {
            _context = context;
        }
        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollBackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }
    }
}
