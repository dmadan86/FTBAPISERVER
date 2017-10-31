using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LinqKit;
using System.Data.SqlTypes;
using FTBAPISERVER.EntityFrameWork;

namespace FTBAPISERVER.EntityFrameWork.Repository
{
    public interface IRepository:IDisposable

    {
        IQueryable<T> All<T>() where T : class;
        IQueryable<T> AllIncluding<T>(params Expression<Func<T, object>>[] include) where T : class;
        IQueryable<T> AllSearchBy<T>(params Expression<Func<T, bool>>[] search) where T : class;
        IQueryable<T> SearchByPredicate<T>(Expression<Func<T, bool>> predicate) where T : class;
        IQueryable<T> SearchByPredicateIncludeChildren<T>(Expression<Func<T, bool>> predicate, List<string> children) where T : class;
        void Add<T>(T model) where T : class;
        void Update<T>(T model) where T : class;
        void Remove<T>(T model) where T : class;
        List<string> GetParseData(string s);
        //string docReport(int id);
    }
    public class Repository : IRepository
    {
        private readonly FTBFileMgtEntities _context;

        public Repository()
        {
            _context = new FTBFileMgtEntities();
        }

        public IQueryable<T> All<T>() where T : class
        {
            return _context.Set<T>();
        }

        public IQueryable<T> AllIncluding<T>(params Expression<Func<T, object>>[] include) where T : class
        {
            IQueryable<T> result = _context.Set<T>();

            foreach (var item in include)
            {
                result = result.Include(item);
            }

            return result;
        }

        public IQueryable<T> AllSearchBy<T>(params Expression<Func<T, bool>>[] search) where T : class
        {
            IQueryable<T> result = _context.Set<T>();

            foreach (var item in search)
            {
                result = result.Where(item);
            }

            return result;
        }

        public IQueryable<T> SearchByPredicate<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            IQueryable<T> result = _context.Set<T>();
            result = result.AsExpandable().Where(predicate);
            return result;
        }

        public IQueryable<T> SearchByPredicateIncludeChildren<T>(Expression<Func<T, bool>> predicate,
            List<string> children) where T : class
        {
            IQueryable<T> result = _context.Set<T>();
            // Now add child entities requested
            foreach (var child in children)
            {
                result = result.Include(child);
            }
            result = result.AsExpandable().Where(predicate);
            return result;
        }


        public void Add<T>(T model) where T : class
        {
            _context.Set<T>().Add(model);
            _context.SaveChanges();
        }

        public void Update<T>(T model) where T : class
        {
            _context.Set<T>().Add(model);
            _context.Entry<T>(model).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Remove<T>(T model) where T : class
        {
            _context.Set<T>().Remove(model);
            _context.SaveChanges();
        }

        public List<string> GetParseData(string str)
        {
            return _context.Database.SqlQuery<string>(str).ToList();
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
