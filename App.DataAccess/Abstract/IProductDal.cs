using App.Core.DataAccess;
using App.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace App.DataAccess.Abstract
{
    public  interface IProductDal:IEntityRepository<Product>
    {
        //custom operations for product only
        List<Product> Search(Expression<Func<Product, bool>> predicate);
    }
}
