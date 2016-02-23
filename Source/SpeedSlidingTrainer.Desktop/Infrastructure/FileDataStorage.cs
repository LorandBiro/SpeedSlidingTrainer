using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Application.Infrastructure;

namespace SpeedSlidingTrainer.Desktop.Infrastructure
{
    public sealed class FileDataStorage : IDataStorage
    {
        public async Task<byte[]> Load([NotNull] string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                if (fs.Length == 0)
                {
                    return null;
                }

                byte[] buffer = new byte[fs.Length];
                await fs.ReadAsync(buffer, 0, (int)fs.Length);
                return buffer;
            }
        }

        public async Task Save([NotNull] string fileName, [NotNull] byte[] content)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                await fs.WriteAsync(content, 0, content.Length);
            }
        }
    }
}
