using App.Core.DataAccess.EntityFramework;
using App.DataAccess.Abstract;
using App.Entities.Concrete;

namespace App.DataAccess.Concrete.EFEntityFramework
{
    public class EFProductDal:EFEntityRepositoryBase<Product,NorthwindContext>,IProductDal
    {

    }
}
