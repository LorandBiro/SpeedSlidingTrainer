using System;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Core.Model.State;

namespace SpeedSlidingTrainer.Core.Model
{
    public sealed class Drill : IEquatable<Drill>
    {
        public Drill(Guid id, [NotNull] string name, [NotNull] BoardTemplate template, [NotNull] BoardGoal goal)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            if (goal == null)
            {
                throw new ArgumentNullException(nameof(goal));
            }

            if (template.Satisfies(goal))
            {
                throw new InvalidDrillException("The board template will always satisfies the specified goal.");
            }

            this.Id = id;
            this.Name = name;
            this.Template = template;
            this.Goal = goal;
        }

        public Guid Id { get; }

        [NotNull]
        public string Name { get; private set; }

        [NotNull]
        public BoardTemplate Template { get; private set; }

        [NotNull]
        public BoardGoal Goal { get; private set; }

        [NotNull]
        public static Drill CreateNew([NotNull] string name, [NotNull] BoardTemplate template, [NotNull] BoardGoal goal)
        {
            return new Drill(Guid.NewGuid(), name, template, goal);
        }

        [NotNull]
        public Drill Update([NotNull] string name, [NotNull] BoardTemplate template, [NotNull] BoardGoal goal)
        {
            return new Drill(this.Id, name, template, goal);
        }

        public bool Equals([CanBeNull] Drill other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return this.Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Drill);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
