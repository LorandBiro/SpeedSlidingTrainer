using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SpeedSlidingTrainer.Application.Repositories
{
    public interface IRepository<T>
    {
        Task AddAsync([NotNull] T entity);

        Task AddRangeAsync([NotNull] [ItemNotNull] IEnumerable<T> entities);

        Task RemoveAsync([NotNull] T entity);

        Task UpdateAsync([NotNull] T entity);

        [NotNull]
        [ItemNotNull]
        Task<ICollection<T>> GetAllAsync();
    }
}
