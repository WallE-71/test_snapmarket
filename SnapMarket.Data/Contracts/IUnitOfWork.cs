using System.Threading.Tasks;

namespace SnapMarket.Data.Contracts
{
    public interface IUnitOfWork
    {
        Task Commit();
        SnapMarketDBContext Context { get; }
        ICartRepository CartRepository { get; }
        IFileRepository FileRepository { get; }
        IOrderRepository OrderRepository { get; }
        IBrandRepository BrandRepository { get; }
        IColorRepository ColorRepository { get; }
        ISellerRepository SellerRepository { get; }
        ISliderRepository SliderRepository { get; }
        IProductRepository ProductRepository { get; }
        IPaymentRepository PaymentRepository { get; }
        ICommentRepository CommentRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IMaterialRepository MaterialRepository { get; }
        INewsletterRepository NewsletterRepository { get; }
        ICreditCartRepository CreditCartRepository { get; }
        IMessageUserRepository MessageUsersRepository { get; }
        IBaseRepository<TEntity> BaseRepository<TEntity>() where TEntity : class;
    }
}
