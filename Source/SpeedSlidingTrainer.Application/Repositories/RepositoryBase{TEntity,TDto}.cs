using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SpeedSlidingTrainer.Application.Infrastructure;

namespace SpeedSlidingTrainer.Application.Repositories
{
    public abstract class RepositoryBase<TEntity, TDto> : IRepository<TEntity>
        where TEntity : class
        where TDto : class, new()
    {
        [NotNull]
        private readonly IDataStorage dataStorage;

        [NotNull]
        private readonly string fileName;

        [CanBeNull]
        private List<TEntity> entities;

        protected RepositoryBase([NotNull] IDataStorage dataStorage, [NotNull] string fileName)
        {
            if (dataStorage == null)
            {
                throw new ArgumentNullException(nameof(dataStorage));
            }

            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            this.dataStorage = dataStorage;
            this.fileName = fileName;
        }

        public async Task AddAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (this.entities == null)
            {
                this.entities = await this.LoadEntitiesAsync();
            }

            if (this.entities.Any(x => x.Equals(entity)))
            {
                throw new ArgumentException("The specified entity is already added.", nameof(entity));
            }

            this.entities.Add(entity);
            await this.SaveEntitiesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            if (this.entities == null)
            {
                this.entities = await this.LoadEntitiesAsync();
            }

            foreach (TEntity entity in entities)
            {
                if (this.entities.Any(x => x.Equals(entity)))
                {
                    throw new ArgumentException("The specified entity is already added.", nameof(entity));
                }

                this.entities.Add(entity);
            }

            await this.SaveEntitiesAsync();
        }

        public async Task RemoveAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (this.entities == null)
            {
                this.entities = await this.LoadEntitiesAsync();
            }

            TEntity entityToRemove = this.entities.FirstOrDefault(x => x.Equals(entity));
            if (entityToRemove == null)
            {
                throw new ArgumentException("Couldn't find the specified entity.");
            }

            this.entities.Remove(entityToRemove);

            await this.SaveEntitiesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (this.entities == null)
            {
                this.entities = await this.LoadEntitiesAsync();
            }

            TEntity entityToUpdate = this.entities.FirstOrDefault(x => x.Equals(entity));
            if (entityToUpdate == null)
            {
                throw new ArgumentException("Couldn't find the specified entity.");
            }

            this.entities.Remove(entityToUpdate);
            this.entities.Add(entity);

            await this.SaveEntitiesAsync();
        }

        public async Task<ICollection<TEntity>> GetAllAsync()
        {
            if (this.entities == null)
            {
                this.entities = await this.LoadEntitiesAsync();
            }

            return this.entities.ToList();
        }

        [NotNull]
        protected abstract TEntity ToEntity([NotNull] TDto dto);

        [NotNull]
        protected abstract TDto ToDto([NotNull] TEntity entity);

        [NotNull]
        [ItemNotNull]
        private async Task<List<TEntity>> LoadEntitiesAsync()
        {
            byte[] data = await this.dataStorage.Load(this.fileName);
            if (data == null)
            {
                return new List<TEntity>();
            }

            try
            {
                string json = Encoding.UTF8.GetString(data, 0, data.Length);
                TDto[] dtos = JsonConvert.DeserializeObject<TDto[]>(json);
                return dtos.Select(this.ToEntity).ToList();
            }
            catch (Exception e)
            {
                throw new InvalidDataException("Serialized drill data is invalid.", e);
            }
        }

        private async Task SaveEntitiesAsync()
        {
            TDto[] dtos = this.entities.Select(this.ToDto).ToArray();
            string json = JsonConvert.SerializeObject(dtos, Formatting.Indented);
            byte[] data = Encoding.UTF8.GetBytes(json);
            await this.dataStorage.Save(this.fileName, data);
        }
    }
}
