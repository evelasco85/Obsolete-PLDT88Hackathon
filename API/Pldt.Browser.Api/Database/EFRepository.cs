using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Pldt.Browser.Api.Database
{
    public interface IEFRepository
    {
        Pldt88HackEntities Entities { get; }
    }

    public delegate void SuccessfullyPersistDelegate<TEntity>(TEntity entity);
    public delegate void DuplicateFoundDelegate<TEntity>(TEntity entity);
    public delegate void FailedToPersistDelegate<TEntity>(TEntity entity, Exception ex);

    public class EFRepository : IEFRepository
    {
        Pldt88HackEntities _entities;

        public EFRepository()
        {
            this._entities = new Pldt88HackEntities();
        }

        public Pldt88HackEntities Entities { get { return this._entities; } }
    }
}