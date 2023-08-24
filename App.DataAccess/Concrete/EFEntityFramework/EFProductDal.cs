using App.Core.DataAccess.EntityFramework;
using App.DataAccess.Abstract;
using App.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace App.DataAccess.Concrete.EFEntityFramework
{
    public class EFProductDal : EFEntityRepositoryBase<Product, NorthwindContext>, IProductDal
    {
        public List<Product> Search(Expression<Func<Product, bool>> pr)
        {
            using (var context = new NorthwindContext())
            {
                return context.Set<Product>().Where(pr).ToList();
            }
        }
    }
}
