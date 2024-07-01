using RepositoryPatternWithUOW.Core.Interfaces;
using RepositoryPatternWithUOW.Core.Models;

namespace RepositoryPatternWithUOW.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<UserAccount> UserAccounts { get; }
        IBaseRepository<Category> Category { get; }
        IBaseRepository<Order> orders { get; }
        IBaseRepository<Product> products { get; }
        IBaseRepository<Receipt> Receipts { get; }
        IBaseRepository<ReceiptItem> ReceiptItems { get; }

        int Complete();

    }
}

