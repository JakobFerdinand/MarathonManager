using Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using UI.RunnerManagement.Common;
using UI.RunnerManagement.Models;
using UI.RunnerManagement.Services;

namespace UI.RunnerManagement.ViewModels
{
    internal class RunnersViewModel : ViewModelBase
    {
        private readonly IRunnersService runnersService;
        private readonly ICategoriesService categoriesService;
        private readonly IDialogService dialogService;
        private readonly INotificationService notificationService;

        private IEnumerable<Category> _categories;
        private Runner _selectedRunner;
        private ICommand _initializeCommand;
        private ICommand _newRunnerCommand;
        private ICommand _reloadCommand;
        private ICommand _removeRunnerCommand;
        private ICommand _saveCommand;
        private bool _areStartnumbersUnic = true;
        private bool _areChipIdsUnic = true;

        public ObservableCollection<ImmutableRunner> Runners { get; } = new ObservableCollection<ImmutableRunner>();
        public ObservableCollection<ImmutableCategory> Categories { get; } = new ObservableCollection<ImmutableCategory>();

        public RunnersViewModel(
            IRunnersService runnersService
            , ICategoriesService categoriesService
            , IDialogService dialogService
            , INotificationService notificationService)
        {
            this.runnersService = runnersService ?? throw new ArgumentNullException(nameof(runnersService));
            this.categoriesService = categoriesService ?? throw new ArgumentNullException(nameof(categoriesService));
            this.dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService), $"{nameof(dialogService)} must not be null.");
            this.notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }


        public Runner SelectedRunner
        {
            get => _selectedRunner;
            set
            {
                if (_selectedRunner?.IsValid() == false)
                {
                    dialogService.ShowOkMessageBox("Bitte geben Sie für den aktuellen Läufer einen Vor- und einen Nachnamen ein.", "Läuferdaten ungültig");
                    return;
                }

                _selectedRunner = value;
                if (_selectedRunner != null && _selectedRunner.Category != null)
                {
                    unitOfWork.Attach(_selectedRunner);
                    _selectedRunner.Category = Categories.Single(c => c.Id == _selectedRunner.Category.Id);
                }
                RaisePropertyChanged();
            }
        }
        public ImmutableList<string> SportClubs =>
            Runners?.Where(r => r.SportsClub != null)
                    .Select(r => r.SportsClub)
                    .Distinct()
                    .OrderBy(s => s)
                    .ToImmutableList()
            ?? ImmutableList<string>.Empty;

        public ImmutableList<string> Cities =>
            Runners?.Where(r => r.City != null)
                    .Select(r => r.City)
                    .Distinct()
                    .OrderBy(r => r)
                    .ToImmutableList()
            ?? ImmutableList<string>.Empty;
        public bool AreStartnumbersUnic
        {
            get => _areStartnumbersUnic;
            set => Set(ref _areStartnumbersUnic, value);
        }
        public bool AreChipIdsUnic
        {
            get => _areChipIdsUnic;
            set => Set(ref _areChipIdsUnic, value);
        }
        public ImmutableList<ImmutableRunner> InvalidRunners =>
            Runners.Where(r => string.IsNullOrWhiteSpace(r.Firstname)
                            || string.IsNullOrWhiteSpace(r.Lastname)
                            || (r.Category.Id == 0 && r.Category == null)).ToImmutableList();

        public ICommand InitializeCommand => _initializeCommand ?? (_initializeCommand = new Command(() =>
        {
            LoadCategories();
            LoadRunners();
        }));
        public ICommand NewRunnerCommand => _newRunnerCommand ?? (_newRunnerCommand = new Command(NewRunner, () => SelectedRunner.IsValid() != false));
        public ICommand ReloadCommand => _reloadCommand ?? (_reloadCommand = new Command(Reload));
        public ICommand RemoveRunnerCommand => _removeRunnerCommand ?? (_removeRunnerCommand = new Command(RemoveRunner));
        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new Command(
            () => SaveRunners(),
            () => SelectedRunner?.IsValid() != false
                  && AreStartnumbersUnic
                  && AreChipIdsUnic));

        internal void Reload()
        {
            var messageBoxResult = MessageBoxResult.No;
            if (unitOfWork.HasChanges())
                messageBoxResult = dialogService.ShowYesNoMessageBox("Sie haben ungespeicherte Änderungen. wollen sie diese speichern?", "Ungespeicherte Änderungen");
            if (messageBoxResult is MessageBoxResult.Yes)
                unitOfWork.Complete();

            unitOfWork.Dispose();
            unitOfWork = _getNewUnitOfWork();

            Runners = null;
            Categories = null;
            SelectedRunner = null;
            LoadData();
        }
        internal void LoadData()
        {
            LoadRunners();
            LoadCategories();
        }

        internal void LoadRunners()
            => Runners.AddRange(runnersService.GetAll().Result);

        internal void LoadCategories()
            => Categories.AddRange(categoriesService.GetAll().Result);

        internal void NewRunner()
        {
            var runner = new Runner
            {
                Gender = Gender.Mann
            };
            unitOfWork.Runners.Add(runner);
            Runners.Add(runner);
            SelectedRunner = runner;
        }

        internal void SaveRunners()
        {
            unitOfWork.Complete();
            NotifySportsClubAndCitiesAndInvalidRunners();

            notificationService.ShowNotification("Es wurde erfolgreich gespeichert.", "Gespeichert", NotificationType.Success);
        }

        internal void RemoveRunner()
        {
            SaveRunners();

            var result = dialogService.ShowYesNoMessageBox($"Wollen sie den Läufer {SelectedRunner.Startnumber} {SelectedRunner.Firstname} {SelectedRunner.Lastname} wirklich löschen?", "Läufer löschen");

            switch (result)
            {
                case MessageBoxResult.Yes:
                    unitOfWork.Runners.Remove(SelectedRunner);
                    SelectedRunner = null;
                    SaveRunners();
                    LoadRunners();
                    break;

                case MessageBoxResult.No:
                default: return;
            }
        }

        internal void NotifySportsClubAndCitiesAndInvalidRunners()
        {
            RaisePropertyChanged(nameof(SportClubs));
            RaisePropertyChanged(nameof(Cities));
        }
    }
}
