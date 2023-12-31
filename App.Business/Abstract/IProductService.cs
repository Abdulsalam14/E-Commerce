﻿using App.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Business.Abstract
{
    public interface IProductService
    {
        List<Product> GetAll();
        List<Product> GetByCategory(int categoryid);
        void Add(Product product);
        void Update(Product product);
        void Delete(Product product);
        Product GetById(int productid);
        List<Product> Search(string ProductName, decimal? Unitprice);

    }
}
