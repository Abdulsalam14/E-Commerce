using App.Business.Abstract;
using App.DataAccess.Abstract;
using App.DataAccess.Concrete.EFEntityFramework;
using App.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Business.Concrete
{
    public class ProductService : IProductService
    {
        private IProductDal _productDal;

        public ProductService(IProductDal productDal)
        {
            _productDal = productDal;
        }

        public ProductService()
        {
            _productDal = new EFProductDal();
        }
        public void Add(Product product)
        {
            _productDal.Add(product);
        }

        public void Delete(Product product)
        {
            _productDal.Delete(product);
        }

        public List<Product> GetAll()
        {
            return _productDal.GetList();
        }

        public List<Product> GetByCategory(int categoryid)
        {
            return _productDal.GetList(p => p.CategoryId == categoryid);
        }

        public Product GetById(int productid)
        {
            return _productDal.Get(p => p.ProductId == productid);
        }

        public void Update(Product product)
        {
            _productDal.Update(product);
        }
        public List<Product> Search(string Productname, decimal? Unitprice)
        {
            return Unitprice==null?_productDal.Search(p=>p.ProductName==Productname):
                _productDal.Search(p=>p.ProductName==Productname && p.UnitPrice==Unitprice);
        }
    }
}
