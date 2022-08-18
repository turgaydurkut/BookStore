using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            //from _db we can load category and covertype in product
            //_db.Products.Include(p => p.Category).Include(p=>p.CoverType);
            this.dbSet =_db.Set<T>();
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }
        //includeProperties - "Category,CoverType" we can use comma seperated
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filters=null,string? includeProperties=null)
        {
            IQueryable<T> query = dbSet;
            if(filters != null)
            {
                query = query.Where(filters);
            }
           
            if (includeProperties != null)
            {
                foreach (var inludeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                   query=query.Include(inludeProperty);
                }
             }
            return query.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filters,string?includeProperties=null,bool tracked = true)
        {
            IQueryable<T> query;
            if (tracked)
            {
                query=dbSet;
            }
            else
            {
                query= dbSet.AsNoTracking();
            }
            
            query = query.Where(filters);
            if (includeProperties != null)
            {
                foreach (var inludeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(inludeProperty);
                }
            }
            return query.FirstOrDefault();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}
