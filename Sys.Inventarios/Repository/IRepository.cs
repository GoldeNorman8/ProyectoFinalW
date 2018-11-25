using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using PagedList;

namespace Repository
{
    public interface IRepository : IDisposable
    {
        TEntity Create<TEntity>(TEntity newEntity) where TEntity : class;
        bool Update<TEntity>(TEntity updateEntity) where TEntity : class;
        bool Delete<TEntity>(TEntity deleteEntity) where TEntity : class;
        TEntity FindEntity<TEntity>(Expression <Func<TEntity, bool>> criteria) where TEntity : class;
        IEnumerable<TEntity> FindEntitySet<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class;
        IPagedList<TEntity> FindEntitySetPage<TEntity>(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, string>> order, int page, int pageSize) where TEntity : class;
    }
}
