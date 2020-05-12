using System.Web.Mvc;
using Unity;
using Unity.Mvc5;
using WholesaleStore.Data;
using WholesaleStore.Data.Interfaces;

namespace WholesaleStore
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            container.RegisterType<IGridManager, GridManager>();
            container.RegisterType<IDataExecutor, DataExecutor>();
            container.RegisterType<IDataBaseManager, DataBaseManager>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}