using App.Business.Abstract;
using App.DataAccess.Abstract;
using App.DataAccess.Concrete.EFEntityFramework;
using App.Entities.Concrete;
using System.Collections.Generic;

namespace App.Business.Concrete
{
    public class CategoryService : ICategoryService
    {
        private ICategoryDal _categoryDal;

        public CategoryService(ICategoryDal categorydal)
        {
            _categoryDal = categorydal;
        }

        public CategoryService()
        {
            _categoryDal = new EFCategoryDal();
        }

        public List<Category> GetList()
        {
            return _categoryDal.GetList();
;        }
    }
}
