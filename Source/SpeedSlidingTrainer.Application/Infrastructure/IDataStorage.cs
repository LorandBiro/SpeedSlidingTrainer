using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SpeedSlidingTrainer.Application.Infrastructure
{
    public interface IDataStorage
    {
        Task<byte[]> Load([NotNull] string fileName);

        Task Save([NotNull] string fileName, [NotNull] byte[] content);
    }
}
