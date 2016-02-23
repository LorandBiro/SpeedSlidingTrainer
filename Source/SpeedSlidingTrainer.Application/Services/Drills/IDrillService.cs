using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Core.Model;

namespace SpeedSlidingTrainer.Application.Services.Drills
{
    public interface IDrillService
    {
        Task<ICollection<Drill>> GetDrills();

        Task Add([NotNull] Drill drill);

        Task Update([NotNull] Drill drill);

        Task Remove([NotNull] Drill drill);

        [NotNull]
        Drill GetDrillForAdd();
    }
}
