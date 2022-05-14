using System.Threading.Tasks;
using SnapMarket.Data.Contracts;
using SnapMarket.Data.Repositories;

namespace SnapMarket.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public SnapMarketDBContext Context { get; }
        private ICartRepository _cartRepository;
        private IFileRepository _fileRepository;
        private IOrderRepository _orderRepository;
        private IBrandRepository _brandRepository;
        private IColorRepository _colorRepository;
        private ISellerRepository _sellerRepository;
        private ISliderRepository _sliderRepository;
        private IProductRepository _productRepository;
        private ICommentRepository _commentRepository;
        private IPaymentRepository _paymentRepository;
        private ICategoryRepository _categoryRepository;
        private IMaterialRepository _materialRepository;
        private INewsletterRepository _newsletterRepository;
        private ICreditCartRepository _creditCartRepository;
        private IMessageUserRepository _messageUsersRepository;       
        public UnitOfWork(SnapMarketDBContext context)
        {
            Context = context;
        }

        public IBaseRepository<TEntity> BaseRepository<TEntity>() where TEntity : class
        {
            IBaseRepository<TEntity> repository = new BaseRepository<TEntity, SnapMarketDBContext>(Context);
            return repository;
        }

        public ISliderRepository SliderRepository
        {
            get
            {
                if (_sliderRepository == null)
                    _sliderRepository = new SliderRepository(Context);

                return _sliderRepository;
            }
        }

        public IColorRepository ColorRepository
        {
            get
            {
                if (_colorRepository == null)
                    _colorRepository = new ColorRepository(Context);

                return _colorRepository;
            }
        }

        public IMaterialRepository MaterialRepository
        {
            get
            {
                if (_materialRepository == null)
                    _materialRepository = new MaterialRepository(Context);

                return _materialRepository;
            }
        }

        public IFileRepository FileRepository
        {
            get
            {
                if (_fileRepository == null)
                    _fileRepository = new FileRepository(Context);

                return _fileRepository;
            }
        }

        public IBrandRepository BrandRepository
        {
            get
            {
                if (_brandRepository == null)
                    _brandRepository = new BrandRepository(Context);

                return _brandRepository;
            }
        }

        public ICreditCartRepository CreditCartRepository
        {
            get
            {
                if (_creditCartRepository == null)
                    _creditCartRepository = new CreditCartRepository(Context);

                return _creditCartRepository;
            }
        }

        public ISellerRepository SellerRepository
        {
            get
            {
                if (_sellerRepository == null)
                    _sellerRepository = new SellerRepository(Context);

                return _sellerRepository;
            }
        }

        public ICategoryRepository CategoryRepository
        {
            get
            {
                if (_categoryRepository == null)
                    _categoryRepository = new CategoryRepository(Context);

                return _categoryRepository;
            }
        }
   
        public IPaymentRepository PaymentRepository
        {
            get
            {
                if (_paymentRepository == null)
                    _paymentRepository = new PaymentRepository(Context);

                return _paymentRepository;
            }
        }

        public IProductRepository ProductRepository
        {
            get
            {
                if (_productRepository == null)
                    _productRepository = new ProductRepository(Context);

                return _productRepository;
            }
        }

        public ICommentRepository CommentRepository
        {
            get
            {
                if (_commentRepository == null)
                    _commentRepository = new CommentRepository(Context);

                return _commentRepository;
            }
        }

        public IOrderRepository OrderRepository
        {
            get
            {
                if (_orderRepository == null)
                    _orderRepository = new OrderRepository(Context);

                return _orderRepository;
            }
        }

        public ICartRepository CartRepository
        {
            get
            {
                if (_cartRepository == null)
                    _cartRepository = new CartRepository(Context);

                return _cartRepository;
            }
        }

        public INewsletterRepository NewsletterRepository
        {
            get
            {
                if (_newsletterRepository == null)
                    _newsletterRepository = new NewsletterRepository(Context);

                return _newsletterRepository;
            }
        }

        public IMessageUserRepository MessageUsersRepository
        {
            get
            {
                if (_messageUsersRepository == null)
                    _messageUsersRepository = new MessageUserRepository(Context);

                return _messageUsersRepository;
            }
        }

        public async Task Commit()
        {
            await Context.SaveChangesAsync();
        }
    }
}
