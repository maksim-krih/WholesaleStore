using System;
using System.Threading;
using System.Threading.Tasks;

namespace WholesaleStore.Data.Interfaces
{
    public interface IDataBaseManager : IDisposable
    {
        IRepository<Address> AddressRepository { get; }

        IRepository<Brand> BrandRepository { get; }

        IRepository<City> CityRepository { get; }

        IRepository<Client> ClientRepository { get; }

        IRepository<Country> CountryRepository { get; }

        IRepository<Employee> EmployeeRepository { get; }

        IRepository<OrderContent> OrderContentRepository { get; }

        IRepository<OrderDelivery> OrderDeliveryRepository { get; }

        IRepository<Order> OrderRepository { get; }

        IRepository<OrderShipment> OrderShipmentRepository { get; }

        IRepository<Position> PositionRepository { get; }

        IRepository<Product> ProductRepository { get; }

        IRepository<ProductsInStorage> ProductsInStorageRepository { get; }

        IRepository<ProductType> ProductTypeRepository { get; }

        IRepository<Region> RegionRepository { get; }

        IRepository<Storage> StorageRepository { get; }

        IRepository<Supplier> SupplierRepository { get; }

        IRepository<Supply> SupplyRepository { get; }

        IRepository<SupplyContent> SupplyContentRepository { get; }

        IRepository<SupplyShipment> SupplyShipmentRepository { get; }


        Task<int> CommitAsync(CancellationToken cancellationToken);

        Task<int> CommitAsync();
    }
}