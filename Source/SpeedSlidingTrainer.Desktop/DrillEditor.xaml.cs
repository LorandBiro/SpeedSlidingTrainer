using System;
using System.Linq;
using System.Windows;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Application.Services.Drills;
using SpeedSlidingTrainer.Core.Model;
using SpeedSlidingTrainer.Core.Model.State;
using SpeedSlidingTrainer.Core.Model.State.Validation;
using SpeedSlidingTrainer.Desktop.BoardFormatting;

namespace SpeedSlidingTrainer.Desktop
{
    public partial class DrillEditor
    {
        [NotNull]
        private readonly IBoardFormatter boardFormatter = ServiceLocator.BoardFormatter;

        [NotNull]
        private readonly IDrillService drillService = ServiceLocator.DrillService;

        [NotNull]
        private readonly Drill drill;

        private readonly bool update;

        public DrillEditor([CanBeNull] Drill drill)
        {
            this.update = drill != null;
            this.drill = drill ?? this.drillService.GetDrillForAdd();

            this.DrillName = this.drill.Name;
            this.TemplateDescription = this.boardFormatter.ToString(this.drill.Template);
            this.GoalDescription = this.boardFormatter.ToString(this.drill.Goal);

            this.DataContext = this;
            this.InitializeComponent();
            this.Loaded += (sender, e) =>
                {
                    this.NameTextBox.Focus();
                    this.NameTextBox.SelectAll();
                };
        }

        public string DrillName { get; set; }

        public string TemplateDescription { get; set; }

        public string GoalDescription { get; set; }

        [CanBeNull]
        public Drill SavedDrill { get; private set; }

        private async void OnOkClicked(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;

            try
            {
                BoardDescription templateDescription = this.boardFormatter.ParseBoardDescription(this.TemplateDescription);
                BoardTemplate template = new BoardTemplate(templateDescription.Width, templateDescription.Height, templateDescription.Values);

                BoardDescription goalDescription = this.boardFormatter.ParseBoardDescription(this.GoalDescription);
                BoardGoal goal = new BoardGoal(goalDescription.Width, goalDescription.Height, goalDescription.Values);

                Drill drillToSave = this.drill.Update(this.DrillName, template, goal);
                if (this.update)
                {
                    await this.drillService.Update(drillToSave);
                }
                else
                {
                    await this.drillService.Add(drillToSave);
                }

                this.SavedDrill = drillToSave;
                this.Close();
            }
            catch (InvalidBoardException exception)
            {
                MessageBox.Show(
                    this,
                    string.Join(Environment.NewLine, exception.Errors.Select(x => x.Message)),
                    "Invalid board",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            catch (InvalidDrillException exception)
            {
                MessageBox.Show(this, exception.Message, "Invalid drill", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch
            {
                MessageBox.Show("An unexpected error occured during saving.");
            }
            finally
            {
                this.IsEnabled = true;
            }
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
