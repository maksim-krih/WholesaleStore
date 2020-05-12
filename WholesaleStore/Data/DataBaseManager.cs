using System.Threading;
using System.Threading.Tasks;
using WholesaleStore.Data.Interfaces;

namespace WholesaleStore.Data
{
    public class DataBaseManager : IDataBaseManager
    {
        #region dbContext

        private readonly WholesaleStoreContext _dbContext;

        #endregion

        private IRepository<Address> _addressRepository;
        private IRepository<Brand> _brandRepository;
        private IRepository<City> _cityRepository;
        private IRepository<Client> _clientRepository;
        private IRepository<Country> _countryRepository;
        private IRepository<Employee> _employeeRepository;
        private IRepository<OrderContent> _orderContentRepository;
        private IRepository<OrderDelivery> _orderDeliveryRepository;
        private IRepository<Order> _orderRepository;
        private IRepository<OrderShipment> _orderShipmentRepository;
        private IRepository<Position> _positionRepository;
        private IRepository<Product> _productRepository;
        private IRepository<ProductsInStorage> _productsInStorageRepository;
        private IRepository<ProductType> _productTypeRepository;
        private IRepository<Region> _regionRepository;
        private IRepository<Storage> _storageRepository;
        private IRepository<Supplier> _supplierRepository;
        private IRepository<Supply> _supplyRepository;
        private IRepository<SupplyContent> _supplyContentRepository;
        private IRepository<SupplyShipment> _supplyShipmentRepository;


        public DataBaseManager(WholesaleStoreContext context)
        {
            _dbContext = context;
        }

        public IRepository<Address> AddressRepository
        {
            get
            {
                _addressRepository = _addressRepository ?? new GenericEFRepository<Address>(_dbContext);
                return _addressRepository;
            }
        }

        public IRepository<Brand> BrandRepository
        {
            get
            {
                _brandRepository = _brandRepository ?? new GenericEFRepository<Brand>(_dbContext);
                return _brandRepository;
            }
        }

        public IRepository<City> CityRepository
        {
            get
            {
                _cityRepository = _cityRepository ?? new GenericEFRepository<City>(_dbContext);
                return _cityRepository;
            }
        }

        public IRepository<Client> ClientRepository
        {
            get
            {
                _clientRepository = _clientRepository ?? new GenericEFRepository<Client>(_dbContext);
                return _clientRepository;
            }
        }

        public IRepository<Country> CountryRepository
        {
            get
            {
                _countryRepository = _countryRepository ?? new GenericEFRepository<Country>(_dbContext);
                return _countryRepository;
            }
        }

        public IRepository<Employee> EmployeeRepository
        {
            get
            {
                _employeeRepository = _employeeRepository ?? new GenericEFRepository<Employee>(_dbContext);
                return _employeeRepository;
            }
        }

        public IRepository<OrderContent> OrderContentRepository
        {
            get
            {
                _orderContentRepository = _orderContentRepository ?? new GenericEFRepository<OrderContent>(_dbContext);
                return _orderContentRepository;
            }
        }

        public IRepository<OrderDelivery> OrderDeliveryRepository
        {
            get
            {
                _orderDeliveryRepository = _orderDeliveryRepository ?? new GenericEFRepository<OrderDelivery>(_dbContext);
                return _orderDeliveryRepository;
            }
        }

        public IRepository<Order> OrderRepository
        {
            get
            {
                _orderRepository = _orderRepository ?? new GenericEFRepository<Order>(_dbContext);
                return _orderRepository;
            }
        }

        public IRepository<OrderShipment> OrderShipmentRepository
        {
            get
            {
                _orderShipmentRepository = _orderShipmentRepository ?? new GenericEFRepository<OrderShipment>(_dbContext);
                return _orderShipmentRepository;
            }
        }

        public IRepository<Position> PositionRepository
        {
            get
            {
                _positionRepository = _positionRepository ?? new GenericEFRepository<Position>(_dbContext);
                return _positionRepository;
            }
        }

        public IRepository<Product> ProductRepository
        {
            get
            {
                _productRepository = _productRepository ?? new GenericEFRepository<Product>(_dbContext);
                return _productRepository;
            }
        }

        public IRepository<ProductsInStorage> ProductsInStorageRepository
        {
            get
            {
                _productsInStorageRepository = _productsInStorageRepository ?? new GenericEFRepository<ProductsInStorage>(_dbContext);
                return _productsInStorageRepository;
            }
        }

        public IRepository<ProductType> ProductTypeRepository
        {
            get
            {
                _productTypeRepository = _productTypeRepository ?? new GenericEFRepository<ProductType>(_dbContext);
                return _productTypeRepository;
            }
        }

        public IRepository<Region> RegionRepository
        {
            get
            {
                _regionRepository = _regionRepository ?? new GenericEFRepository<Region>(_dbContext);
                return _regionRepository;
            }
        }

        public IRepository<Storage> StorageRepository
        {
            get
            {
                _storageRepository = _storageRepository ?? new GenericEFRepository<Storage>(_dbContext);
                return _storageRepository;
            }
        }

        public IRepository<Supplier> SupplierRepository
        {
            get
            {
                _supplierRepository = _supplierRepository ?? new GenericEFRepository<Supplier>(_dbContext);
                return _supplierRepository;
            }
        }

        public IRepository<Supply> SupplyRepository
        {
            get
            {
                _supplyRepository = _supplyRepository ?? new GenericEFRepository<Supply>(_dbContext);
                return _supplyRepository;
            }
        }

        public IRepository<SupplyContent> SupplyContentRepository
        {
            get
            {
                _supplyContentRepository = _supplyContentRepository ?? new GenericEFRepository<SupplyContent>(_dbContext);
                return _supplyContentRepository;
            }
        }

        public IRepository<SupplyShipment> SupplyShipmentRepository
        {
            get
            {
                _supplyShipmentRepository = _supplyShipmentRepository ?? new GenericEFRepository<SupplyShipment>(_dbContext);
                return _supplyShipmentRepository;
            }
        }


        public async Task<int> CommitAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _addressRepository?.Dispose();
            _brandRepository?.Dispose();
            _cityRepository?.Dispose();
            _clientRepository?.Dispose();
            _countryRepository?.Dispose();
            _employeeRepository?.Dispose();
            _orderContentRepository?.Dispose();
            _orderDeliveryRepository?.Dispose();
            _orderRepository?.Dispose();
            _orderShipmentRepository?.Dispose();
            _positionRepository?.Dispose();
            _productRepository?.Dispose();
            _productsInStorageRepository?.Dispose();
            _productTypeRepository?.Dispose();
            _regionRepository?.Dispose();
            _storageRepository?.Dispose();
            _supplierRepository?.Dispose();
            _supplyRepository?.Dispose();
            _supplyContentRepository?.Dispose();
            _supplyShipmentRepository?.Dispose();

            _dbContext?.Dispose();
        }
    }
}