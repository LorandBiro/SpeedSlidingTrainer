using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpeedSlidingTrainer.Application.Infrastructure;

namespace SpeedSlidingTrainer.Desktop.Infrastructure
{
    public sealed class InMemoryDataStorage : IDataStorage
    {
        private readonly Dictionary<string, byte[]> files = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

        public Task<byte[]> Load(string fileName)
        {
            byte[] content;
            this.files.TryGetValue(fileName, out content);
            return Task.FromResult(content);
        }

        public Task Save(string fileName, byte[] content)
        {
            this.files[fileName] = content;
            return Task.FromResult(0);
        }
    }
}
