using MMiners.Bussiness.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMiners.Bussiness.Interfaces
{
    public interface IRepository<TEntity> : IDisposable where TEntity : Entity
    {
    }
}
