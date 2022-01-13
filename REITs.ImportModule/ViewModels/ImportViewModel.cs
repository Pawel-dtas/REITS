using Domain.MessageBoxModelsEnums;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using REITs.Application;
using REITs.DataLayer.Services;
using REITs.Domain.Enums;
using REITs.Domain.MenuModels;
using REITs.Domain.Models;
using REITs.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace REITs.ImportModule.ViewModels
{
	public class ImportViewModel : BindableBase, INavigationAware
	{
		#region Properties

		public ObservableCollection<XMLProcessingResult> ImportableFilesToDisplay
		{
			get
			{
				return new ObservableCollection<XMLProcessingResult>(ImportableFiles);
			}
		}

		public XMLProcessingResult CurrentlySelectedImportResultItem
		{
			get
			{
				return _currentlySelectedImportResultItem;
			}
			set
			{
				SetProperty(ref _currentlySelectedImportResultItem, value);
			}
		}

		public IList<XMLProcessingResult> ImportableFiles
		{
			get
			{
				return _importableFiles;
			}
			set
			{
				SetProperty(ref _importableFiles, value);

				RaisePropertyChanged(nameof(ImportableFilesToDisplay));
			}
		}

		public Visibility ProgressBarVisibility
		{
			get { return _progressBarVisibility; }
			set
			{
				SetProperty(ref _progressBarVisibility, value);

				RaisePropertyChanged(nameof(ImportProgressValue));
				RaisePropertyChanged(nameof(ImportProgressTextValue));
			}
		}

		public double ImportProgressValue
		{
			get { return _importProgressValue; }
			set
			{
				SetProperty(ref _importProgressValue, value);
			}
		}

		public string ImportProgressTextValue
		{
			get { return _importProgressTextValue; }
			set
			{
				SetProperty(ref _importProgressTextValue, value);
			}
		}

		public string ImportCurrentFileName
		{
			get { return _importCurrentFileName; }
			set
			{
				SetProperty(ref _importCurrentFileName, value);
			}
		}

		public List<REIT> ImportedXMLREITList
		{
			get { return _importedXMLREITList; }
			set
			{
				SetProperty(ref _importedXMLREITList, value);
			}
		}

		public bool IsImporting
		{
			get { return _isImporting; }
			set
			{
				SetProperty(ref _isImporting, value);
			}
		}

		#endregion Properties

		#region Variables

		private List<REIT> _importedXMLREITList;
		private IList<XMLProcessingResult> _importableFiles = new List<XMLProcessingResult>();
		private XMLProcessingResult _currentlySelectedImportResultItem;
		private bool _isImporting;

		private double _importProgressValue;
		private int totalImportFiles = 0;

		private string _importCurrentFileName;
		private string _importProgressTextValue;

		private Visibility _progressBarVisibility;

		private IEventAggregator _eventAggregator = PrismHelpers.GetEventAggregator();
		private IREITDataService _dataService = PrismHelpers.ResolveService<IREITDataService>();

		#endregion Variables

		#region Delgate Commands

		public DelegateCommand SelectFilesCommand => new DelegateCommand(PerformSelectFiles, CanSelectFiles).ObservesProperty(() => IsImporting);

		public DelegateCommand ValidateFilesCommand => new DelegateCommand(PerformValidateFiles, CanValidateFiles).ObservesProperty(() => ImportableFiles)
																													.ObservesProperty(() => ImportableFilesToDisplay)
																													.ObservesProperty(() => IsImporting);

		public DelegateCommand<XMLProcessingResult> EmailErrorsCommand => new DelegateCommand<XMLProcessingResult>(PerformEmailErrors, CanEmailErrors)
																													.ObservesProperty(() => CurrentlySelectedImportResultItem);

		public DelegateCommand ImportFilesCommand => new DelegateCommand(PerformImportFiles, CanImportFiles).ObservesProperty(() => ImportableFilesToDisplay)
																											.ObservesProperty(() => IsImporting);

		#endregion Delgate Commands

		#region Constructor

		public ImportViewModel()
		{
			Initialise();
		}

		private void Initialise()
		{
			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("Search", true));
		}

		#endregion Constructor

		#region Commands

		private void PerformSelectFiles()
		{
			ImportableFiles = new List<XMLProcessingResult>();

			UpdateProgressValues(string.Empty, 0);

			SelectFiles();

			UpdateListView();
		}

		private void PerformValidateFiles()
		{
			ValidateFiles();
		}

		private void PerformImportFiles()
		{
			ImportFiles();
		}

		private void PerformEmailErrors(XMLProcessingResult xpr)
		{
			var file = Path.GetFileName(xpr.FileName);
			var status = xpr.XMLStatus;
			var valMessage = xpr.ValidationMessage;

			var outputFilename = $"FileName: {file}";
			var outputStatus = $"Status: {status}";
			var outputMessage = $"Validation Errors:\n{valMessage}";

			var output = $"{outputFilename}\n{outputStatus}\n{outputMessage}";

			Clipboard.SetDataObject(output);
		}

		#endregion Commands

		#region Validation

		private bool CanSelectFiles()
		{
			return !IsImporting;
		}

		private bool CanValidateFiles()
		{
			return (ImportableFiles?.Count > 0) && !IsImporting;
		}

		private bool CanImportFiles()
		{
			return (ImportableFiles.Where(x => (x.XMLStatus == ImportXMLStatusTypes.Validated
											|| x.XMLStatus == ImportXMLStatusTypes.ValidatedAndExists)
											&& x.ImportCompanyStatus == ImportCompanyStatusTypes.Exists).Count() > 0
											&& !IsImporting);
		}

		private bool CanEmailErrors(XMLProcessingResult arg)
		{
			return true;
		}

		#endregion Validation

		#region Private Methods

		private void SelectFiles()
		{
			OpenFileDialog fileDialog = new OpenFileDialog();
			fileDialog.Multiselect = true;
			fileDialog.Filter = "XML Files (*.xml)|*.xml";

			if (fileDialog.ShowDialog() == true)
			{
				foreach (string filename in fileDialog.FileNames)
					ImportableFiles.Add(new XMLProcessingResult { FileName = filename });
			}
		}

		private void ValidateFiles()
		{
			if (ImportableFiles.Count > 0)
			{
				try
				{
					using (ImportFunctionality importFunctionality = new ImportFunctionality())
					{
						Task ImportTask = Task.Factory.StartNew(() =>
						{
							WaitCursor();

							importFunctionality.XMLProcessingResults = new List<XMLProcessingResult>(ImportableFiles);

							importFunctionality.ValidateXMLFiles();
						});

						ImportTask.ContinueWith((x) =>
						{
							UpdateListView();

							NormalCursor();
						}, TaskScheduler.FromCurrentSynchronizationContext());
					}
				}
				catch
				{
					NormalCursor();
				}
			}
		}

		private void ImportFiles()
		{
			if (ImportableFiles.Count > 0)
			{
				IsImporting = true;

				WaitCursor();

				BackgroundWorker worker = new BackgroundWorker();
				worker.WorkerReportsProgress = true;
				worker.DoWork += worker_DoWork;
				worker.ProgressChanged += worker_ProgressChanged;
				worker.RunWorkerCompleted += worker_Completed;
				worker.RunWorkerAsync();
			}
		}

		private bool UpdateListView()
		{
			var success = true;

			foreach (var result in ImportableFiles)
			{
				var tempBrush = new SolidColorBrush(Colors.LightYellow);

				switch (result.XMLStatus)
				{
					case ImportXMLStatusTypes.Validated:
						tempBrush = new SolidColorBrush(Colors.LightGreen);
						break;

					case ImportXMLStatusTypes.ValidatedAndExists:
						tempBrush = new SolidColorBrush(Colors.OrangeRed);
						break;

					case ImportXMLStatusTypes.Errors:
					case ImportXMLStatusTypes.ErrorsAndExists:
						tempBrush = new SolidColorBrush(Colors.LightPink);
						success = false;
						break;

					case ImportXMLStatusTypes.ImportedOK:
						tempBrush = new SolidColorBrush(Colors.Green);
						break;

					case ImportXMLStatusTypes.ImportError:
						tempBrush = new SolidColorBrush(Colors.Red);
						success = false;
						break;
				}

				result.BackgroundColor = tempBrush;
			}

			RefreshDisplayList();

			ImportFilesCommand.RaiseCanExecuteChanged();

			return success;
		}

		private void RefreshDisplayList()
		{
			Dispatcher.CurrentDispatcher.Invoke(() => RaisePropertyChanged(nameof(ImportableFilesToDisplay)));
		}

		private void DisplayResult()
		{
			//if (importedFilesSavedSuccessfully == true)
			//{
			//    if (ImportedXMLREITList.Count > 1)
			//    {
			//        CustomMessageBox.Show(MessageContent.GetMessageContent(MessageBoxContentTypes.ImportSuccessful, ImportedXMLREITList.Count));
			//    }
			//    else
			//    {
			//        if (DuplicateReferenceExists == false && ImportedXMLREITList.Count > 0)
			//        {
			//            CustomMessageBox.Show(MessageContent.GetMessageContent(MessageBoxContentTypes.ImportSuccessful));
			//        }
			//    }
			//}
			//else
			//{
			//    CustomMessageBox.Show(MessageContent.GetMessageContent(MessageBoxContentTypes.ImportUnsuccessful));
			//}
		}

		private void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			using (ImportFunctionality importFunctionality = new ImportFunctionality())
			{
				importFunctionality.XMLProcessingResults = new List<XMLProcessingResult>(ImportableFiles);

				totalImportFiles = ImportableFiles.Where(r => (r.XMLStatus == ImportXMLStatusTypes.Validated
														|| r.XMLStatus == ImportXMLStatusTypes.ValidatedAndExists)
														&& r.ImportCompanyStatus == ImportCompanyStatusTypes.Exists).Count();

				importFunctionality.ImportXMLFiles(sender);
			};
		}

		private void worker_Completed(object sender, RunWorkerCompletedEventArgs e)
		{
			bool success = UpdateListView();

			System.Windows.Application.Current.MainWindow.Cursor = Cursors.Arrow;

			if (success)
				CustomMessageBox.Show(MessageBoxContentTypes.ImportSuccessful);
			else
				CustomMessageBox.Show(MessageBoxContentTypes.ImportUnsuccessful);

			IsImporting = false;

			NormalCursor();
		}

		private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			var filesProcessed = e.ProgressPercentage;
			var progressSteps = Math.Round(Convert.ToDouble(100 / Convert.ToDouble((totalImportFiles))), 2);
			var progressValue = filesProcessed * progressSteps;

			ProgressBarVisibility = Visibility.Visible;

			ImportProgressValue = Math.Round(progressValue, 0);
			ImportProgressTextValue = string.Format("{0}%", Math.Round(ImportProgressValue, 0).ToString());
			ImportCurrentFileName = e.UserState.ToString();

			UpdateProgressValues(e.UserState.ToString(), progressValue);

			Thread.Sleep(350);
		}

		private void UpdateProgressValues(string fileName, double progressValue)
		{
			ImportProgressValue = Math.Round(progressValue, 0);
			ImportProgressTextValue = string.Format("{0}%", Math.Round(progressValue, 0).ToString());
			ImportCurrentFileName = fileName;

			RaisePropertyChanged();
		}

		private void WaitCursor()
		{
			System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => { System.Windows.Application.Current.MainWindow.Cursor = Cursors.Wait; }));
		}

		private void NormalCursor()
		{
			System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => { System.Windows.Application.Current.MainWindow.Cursor = Cursors.Arrow; }));
		}

		#endregion Private Methods

		#region Navigation

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return true;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("Import", true));
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("Search", true));

			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("Import", false));

			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("Print", false));
		}

		#endregion Navigation
	}
}