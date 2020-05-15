using System.Web.Mvc;
using WholesaleStore.Data.Interfaces;

namespace WholesaleStore.Controllers.Base
{
    [Route("api/[controller]")]
    public class BaseController : Controller
    {
        protected readonly IGridManager _gridManager;
        protected readonly IDataExecutor _dataExecutor;
        protected readonly IDataBaseManager _dataBaseManager;

        public BaseController(IGridManager gridManager, IDataExecutor dataExecutor, IDataBaseManager dataBaseManager)
        {
            _gridManager = gridManager;
            _dataExecutor = dataExecutor;
            _dataBaseManager = dataBaseManager;
        }
    }
}