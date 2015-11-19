using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Application.Services.Drills;
using SpeedSlidingTrainer.Application.Services.Game;
using SpeedSlidingTrainer.Core.Model;

namespace SpeedSlidingTrainer.Desktop
{
    public partial class DrillBrowser : INotifyPropertyChanged
    {
        private readonly IGameService gameService = ServiceLocator.GameService;

        private readonly IDrillService drillService = ServiceLocator.DrillService;

        private Drill selectedDrill;

        public DrillBrowser()
        {
            this.DataContext = this;
            this.IsEnabled = false;

            this.InitializeComponent();

            this.Loaded += this.OnLoadedAsync;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [CanBeNull]
        public Drill SelectedDrill
        {
            get
            {
                return this.selectedDrill;
            }

            set
            {
                this.selectedDrill = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.SelectedDrill)));
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.DrillIsSelected)));
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.NoSelectionVisibility)));
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ContentVisibility)));
            }
        }

        public bool DrillIsSelected => this.SelectedDrill != null;

        public Visibility NoSelectionVisibility => this.SelectedDrill == null ? Visibility.Visible : Visibility.Hidden;

        public Visibility ContentVisibility => this.SelectedDrill == null ? Visibility.Hidden : Visibility.Visible;

        private async void OnLoadedAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                ICollection<Drill> drills = await this.drillService.GetDrills();
                this.IsEnabled = true;
                this.UpdateDrillList(drills, drills.FirstOrDefault(x => x.Id == this.gameService.Drill.Id));
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to initialize drill browser.", "Internal error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void OnCreateButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DrillEditor drillEditor = new DrillEditor(null) { Owner = this };
                drillEditor.ShowDialog();

                if (drillEditor.SavedDrill != null)
                {
                    this.IsEnabled = false;
                    ICollection<Drill> drills = await this.drillService.GetDrills();
                    this.IsEnabled = true;
                    this.UpdateDrillList(drills, drillEditor.SavedDrill);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Something went wrong...", "Internal error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void OnEditButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.SelectedDrill == null)
            {
                return;
            }

            try
            {
                DrillEditor drillEditor = new DrillEditor(this.SelectedDrill) { Owner = this };
                drillEditor.ShowDialog();

                if (drillEditor.SavedDrill != null)
                {
                    this.IsEnabled = false;
                    ICollection<Drill> drills = await this.drillService.GetDrills();
                    this.IsEnabled = true;
                    this.UpdateDrillList(drills, drillEditor.SavedDrill);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Something went wrong...", "Internal error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.SelectedDrill == null)
            {
                return;
            }

            MessageBoxResult result = MessageBox.Show("The selected drill will be removed.", "Are you sure?", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                try
                {
                    this.IsEnabled = false;
                    await this.drillService.Remove(this.SelectedDrill);
                    ICollection<Drill> drills = await this.drillService.GetDrills();
                    this.IsEnabled = true;

                    this.UpdateDrillList(drills, drills.FirstOrDefault());
                }
                catch (Exception)
                {
                    MessageBox.Show("Something went wrong...", "Internal error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OnSelectButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.SelectedDrill == null)
            {
                return;
            }

            this.gameService.SetDrill(this.SelectedDrill);
            this.Close();
        }

        private void UpdateDrillList(ICollection<Drill> drills, Drill selectedDrill)
        {
            this.DrillsListBox.ItemsSource = drills.OrderBy(x => x.Name, StringComparer.CurrentCultureIgnoreCase);
            this.DrillsListBox.SelectedItem = drills.FirstOrDefault(x => x.Equals(selectedDrill)) ?? drills.FirstOrDefault();
            this.DrillsListBox.UpdateLayout();
            if (this.DrillsListBox.SelectedItem != null)
            {
                ListBoxItem item = this.DrillsListBox.ItemContainerGenerator.ContainerFromItem(this.DrillsListBox.SelectedItem) as ListBoxItem;
                Debug.Assert(item != null, "Just set the selected item and called UpdateLayout, so the container should already exists.");
                item.Focus();
            }
        }
    }
}
