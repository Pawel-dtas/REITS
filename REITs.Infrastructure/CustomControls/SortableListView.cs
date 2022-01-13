using Domain.MessageBoxModelsEnums;
using Domain.Models;
using REITs.Domain.Enums;
using REITs.Domain.Models;
using REITs.Infrastructure.Adorners;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace REITs.Infrastructure.CustomControls
{
	public class SortableListView : ListView
	{
		private GridViewColumnHeader _lastHeaderClicked = null;
		private ListSortDirection _lastDirection = ListSortDirection.Ascending;
		private ListViewSortAdorner currentAdorner = null;

		public SortableListView()
		{
			this.AddHandler(
				GridViewColumnHeader.ClickEvent,
				new RoutedEventHandler(GridViewColumnHeaderClickedHandler));

			TypeDescriptor.GetProperties(this)["ItemsSource"].AddValueChanged(this, new EventHandler(ItemSourceChanged));
		}

		public override void EndInit()
		{
			base.EndInit();

			if (UserSecurityDetails.AccessLevel == AccessLevels.Admin)
			{
				this.AddHandler(
				KeyUpEvent,
				new RoutedEventHandler(ListViewKeyUpHandler));
			}
		}

		private void ListViewKeyUpHandler(object sender, RoutedEventArgs e)
		{
			KeyEventArgs kev = (KeyEventArgs)e;

			if ((kev.Key == Key.C) && (Keyboard.IsKeyDown(Key.LeftCtrl)
										|| Keyboard.IsKeyDown(Key.RightCtrl)))
			{
				ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.ItemsSource);

				try
				{
					if (CustomMessageBox.Show(MessageBoxContentTypes.CopyToClipboard) == System.Windows.MessageBoxResult.Yes)
					{
						ToHelper.CopyToClipboard(collectionView.CreateListFrom<Adjustment>().ToDataTable<Adjustment>());
					}
				}
				catch { }
			}
		}

		private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
		{
			GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;

			ICollectionView dataView = CollectionViewSource.GetDefaultView(this.ItemsSource);
			ListSortDirection direction;

			if (headerClicked != null && headerClicked.Role != GridViewColumnHeaderRole.Padding)
			{
				if (headerClicked != _lastHeaderClicked)
				{
					direction = ListSortDirection.Ascending;
				}
				else
				{
					direction = (_lastDirection == ListSortDirection.Ascending) ? ListSortDirection.Descending : ListSortDirection.Ascending;
				}

				var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
				var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

				if (dataView != null)
				{
					ClearCurrentAdorner();

					dataView.SortDescriptions.Clear();

					if (sortBy != null)
					{
						SortDescription sd = new SortDescription(sortBy, direction);
						dataView.SortDescriptions.Add(sd);

						currentAdorner = new ListViewSortAdorner(headerClicked, direction);
						AdornerLayer.GetAdornerLayer(headerClicked).Add(currentAdorner);

						dataView.Refresh();
					}
				}

				_lastHeaderClicked = headerClicked;
				_lastDirection = direction;
			}
		}

		private void ItemSourceChanged(object sender, EventArgs e)
		{
			ClearCurrentAdorner();
		}

		private void ClearCurrentAdorner()
		{
			try
			{
				if (currentAdorner != null)
					AdornerLayer.GetAdornerLayer(_lastHeaderClicked).Remove(currentAdorner);
			}
			catch { }
		}
	}
}