using RepositoryPatternWithUOW.Core;
using RepositoryPatternWithUOW.Core.Interfaces;
using RepositoryPatternWithUOW.Core.Models;
using RepositoryPatternWithUOW.EF.Repositories;

namespace RepositoryPatternWithUOW.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _Context;
        public IBaseRepository<UserAccount> UserAccounts { get; private set; }

        public IBaseRepository<Category> Category { get; private set; }

        public IBaseRepository<Order> orders { get; private set; }

        public IBaseRepository<Product> products { get; private set; }

        public IBaseRepository<Receipt> Receipts { get; private set; }

        public IBaseRepository<ReceiptItem> ReceiptItems { get; private set; }




        public UnitOfWork(AppDbContext context)
        {
            _Context = context;
            UserAccounts = new BaseRepository<UserAccount>(_Context);
            Category = new BaseRepository<Category>(_Context);
            orders = new BaseRepository<Order>(_Context);
            products = new BaseRepository<Product>(_Context);
            Receipts = new BaseRepository<Receipt>(_Context);
            ReceiptItems = new BaseRepository<ReceiptItem>(_Context);

        }

        public int Complete()
        {
            return _Context.SaveChanges();
        }

        public void Dispose()
        {
            _Context.Dispose();
        }
    }
}
