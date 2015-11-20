using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Application.Repositories;
using SpeedSlidingTrainer.Core.Model;
using SpeedSlidingTrainer.Core.Model.State;

namespace SpeedSlidingTrainer.Application.Services.Drills
{
    public sealed class DrillService : IDrillService
    {
        [NotNull]
        private readonly IRepository<Drill> repository;

        public DrillService([NotNull] IRepository<Drill> repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            this.repository = repository;
        }

        public async Task<ICollection<Drill>> GetDrills()
        {
            ICollection<Drill> drills = await this.repository.GetAllAsync();
            if (drills.Count == 0)
            {
                drills = new List<Drill>
                    {
                        Drill.CreateNew(
                            "3x3 1",
                            new BoardTemplate(3, 3, new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                            new BoardGoal(3, 3, new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0 })),
                        Drill.CreateNew(
                            "3x3 2,3",
                            new BoardTemplate(3, 3, new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0 }),
                            new BoardGoal(3, 3, new[] { 1, 2, 3, 0, 0, 0, 0, 0, 0 })),
                        Drill.CreateNew(
                            "3x3 4,7",
                            new BoardTemplate(3, 3, new[] { 1, 2, 3, 0, 0, 0, 0, 0, 0 }),
                            new BoardGoal(3, 3, new[] { 1, 2, 3, 4, 0, 0, 7, 0, 0 })),
                        Drill.CreateNew(
                            "3x3 5,6,8",
                            new BoardTemplate(3, 3, new[] { 1, 2, 3, 4, 0, 0, 7, 0, 0 }),
                            new BoardGoal(3, 3, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 0 })),
                        Drill.CreateNew(
                            "4x4 1",
                            new BoardTemplate(4, 4, new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                            new BoardGoal(4, 4, new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })),
                        Drill.CreateNew(
                            "4x4 2",
                            new BoardTemplate(4, 4, new[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                            new BoardGoal(4, 4, new[] { 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })),
                        Drill.CreateNew(
                            "4x4 3,4",
                            new BoardTemplate(4, 4, new[] { 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                            new BoardGoal(4, 4, new[] { 1, 2, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })),
                        Drill.CreateNew(
                            "4x4 5",
                            new BoardTemplate(4, 4, new[] { 1, 2, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                            new BoardGoal(4, 4, new[] { 1, 2, 3, 4, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })),
                        Drill.CreateNew(
                            "4x4 6",
                            new BoardTemplate(4, 4, new[] { 1, 2, 3, 4, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                            new BoardGoal(4, 4, new[] { 1, 2, 3, 4, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 })),
                        Drill.CreateNew(
                            "4x4 7,8",
                            new BoardTemplate(4, 4, new[] { 1, 2, 3, 4, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }),
                            new BoardGoal(4, 4, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0, 0, 0, 0, 0, 0 })),
                        Drill.CreateNew(
                            "4x4 9,13",
                            new BoardTemplate(4, 4, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 0, 0, 0, 0, 0, 0, 0, 0 }),
                            new BoardGoal(4, 4, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 0, 13, 0, 0, 0 })),
                        Drill.CreateNew(
                            "4x4 10,14",
                            new BoardTemplate(4, 4, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0, 0, 13, 0, 0, 0 }),
                            new BoardGoal(4, 4, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0, 0, 13, 14, 0, 0 })),
                        Drill.CreateNew(
                            "4x4 11,12,15",
                            new BoardTemplate(4, 4, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0, 0, 13, 14, 0, 0 }),
                            new BoardGoal(4, 4, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0 }))
                    };

                await this.repository.AddRangeAsync(drills);
            }

            return drills;
        }

        public async Task Add(Drill drill)
        {
            if (drill == null)
            {
                throw new ArgumentNullException(nameof(drill));
            }

            await this.repository.AddAsync(drill);
        }

        public async Task Update(Drill drill)
        {
            if (drill == null)
            {
                throw new ArgumentNullException(nameof(drill));
            }

            await this.repository.UpdateAsync(drill);
        }

        public async Task Remove(Drill drill)
        {
            if (drill == null)
            {
                throw new ArgumentNullException(nameof(drill));
            }

            await this.repository.RemoveAsync(drill);
        }

        public Drill GetDrillForAdd()
        {
            return Drill.CreateNew("Unnamed drill", BoardTemplate.CreateEmpty(4, 4), BoardGoal.CreateCompleted(4, 4));
        }
    }
}
