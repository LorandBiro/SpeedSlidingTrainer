using System;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Application.Infrastructure;
using SpeedSlidingTrainer.Core.Model;
using SpeedSlidingTrainer.Core.Model.State;

namespace SpeedSlidingTrainer.Application.Repositories
{
    public sealed class DrillRepository : RepositoryBase<Drill, DrillRepository.DrillDto>
    {
        public DrillRepository([NotNull] IDataStorage dataStorage)
            : base(dataStorage, "Drills.txt")
        {
        }

        protected override Drill ToEntity(DrillDto dto)
        {
            return new Drill(dto.Id, dto.Name, new BoardTemplate(dto.Width, dto.Height, dto.TemplateValues), new BoardGoal(dto.Width, dto.Height, dto.GoalValues));
        }

        protected override DrillDto ToDto(Drill entity)
        {
            return new DrillDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Width = entity.Template.Width,
                Height = entity.Template.Height,
                TemplateValues = entity.Template.GetValues(),
                GoalValues = entity.Goal.GetValues()
            };
        }

        public class DrillDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public int Width { get; set; }

            public int Height { get; set; }

            public int[] TemplateValues { get; set; }

            public int[] GoalValues { get; set; }
        }
    }
}
