using MunicipalServicesApp.Models;
using MunicipalServicesApp.Services;
using MunicipalServicesApp.Storage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Specialized;

namespace MunicipalServicesApp
{
    public partial class LocalEventsPage : Page, INotifyPropertyChanged
    {
        private readonly EventStore store = EventStore.Instance;
        private readonly RecommendationService recommender = RecommendationService.Instance;

        public ObservableCollection<EventItem> EventsView { get; } = new ObservableCollection<EventItem>();
        public ObservableCollection<EventItem> Recommendations { get; } = new ObservableCollection<EventItem>();

        public LocalEventsPage()
        {
            InitializeComponent();
            DataContext = this;

            // Subscribe to store collection changes so UI and recommendations update automatically
            if (store?.Events != null)
                store.Events.CollectionChanged += Events_CollectionChanged;

            Loaded += (s, e) =>
            {
                SeedCategoryBox();
                RefreshAll();
            };
        }

        private void Events_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Ensure UI updates happen on UI thread
            Dispatcher.Invoke(() =>
            {
                ApplyFilterSortSearch();
                RefreshRecommendations();
                if (StatusText != null)
                    StatusText.Text = $"Showing {EventsView.Count} event(s)";
            });
        }

        private void SeedCategoryBox()
        {
            try
            {
                var list = store?.Categories?.OrderBy(c => c).ToList();
                if (list == null || list.Count == 0)
                {
                    list = new System.Collections.Generic.List<string> { "Community", "Utilities", "Roads", "Education", "Health" };
                }

                CategoryFilter.ItemsSource = list;
                CategoryFilter.SelectedIndex = -1;
            }
            catch
            {
                // defensive fallback
                if (CategoryFilter != null)
                {
                    CategoryFilter.ItemsSource = new[] { "Community", "Utilities", "Roads" };
                    CategoryFilter.SelectedIndex = -1;
                }
            }
        }

        private void RefreshAll()
        {
            ApplyFilterSortSearch();
            RefreshRecommendations();

            if (StatusText != null)
                StatusText.Text = $"Showing {EventsView.Count} event(s)";
        }

        private void ApplyFilterSortSearch()
        {
            var items = store?.GetAll() ?? Enumerable.Empty<EventItem>();

            var selectedCategory = CategoryFilter?.SelectedItem as string;
            if (!string.IsNullOrWhiteSpace(selectedCategory))
            {
                items = items.Where(e => string.Equals(e.Category, selectedCategory, StringComparison.OrdinalIgnoreCase));
                recommender.RecordCategorySearch(selectedCategory);
            }

            if (DateFilter?.SelectedDate is DateTime selectedDate)
            {
                items = items.Where(e => e.Date.Date == selectedDate.Date);
                recommender.RecordDateSearch(selectedDate);
            }

            var search = SearchBox?.Text?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                items = items.Where(e =>
                    (!string.IsNullOrEmpty(e.Title) && e.Title.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(e.Location) && e.Location.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0));
            }

            string sortTag = null;
            if (SortBox?.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag != null)
                sortTag = selectedItem.Tag.ToString();

            switch (sortTag)
            {
                case "date_asc":
                    items = items.OrderBy(e => e.Date);
                    break;
                case "category":
                    items = items.OrderBy(e => e.Category).ThenBy(e => e.Date);
                    break;
                case "title":
                    items = items.OrderBy(e => e.Title);
                    break;
                default:
                    items = items.OrderByDescending(e => e.Date);
                    break;
            }

            EventsView.Clear();
            foreach (var ev in items) EventsView.Add(ev);
        }

        private void RefreshRecommendations()
        {
            try
            {
                var list = recommender.RecommendTop(5)?.ToList() ?? new System.Collections.Generic.List<EventItem>();
                Recommendations.Clear();
                foreach (var r in list) Recommendations.Add(r);
            }
            catch
            {
                // swallow recommender errors so UI remains responsive
            }
        }

        private void OnFilterChanged(object sender, RoutedEventArgs e)
        {
            ApplyFilterSortSearch();
            RefreshRecommendations();
        }

        private void OnSearchChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilterSortSearch();
            RefreshRecommendations();
        }

        private void OnSortChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilterSortSearch();
            RefreshRecommendations();
        }

        private void OnViewEvent(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is EventItem evt)
            {
                // provide feedback to recommender
                try
                {
                    recommender.RecordView(evt.Id);
                }
                catch { }

                // push to store last viewed stack if you use it
                try { store?.LastViewedStack?.Push(evt.Id); } catch { }

                MessageBox.Show($"{evt.Title}\n\n{evt.Description}", "Event Details", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshRecommendations();
            }
        }

        private void OnAddSamples(object sender, RoutedEventArgs e)
        {
            // navigate to AddEventPage; when it adds an event the store.Events.CollectionChanged will trigger refresh
            NavigationService?.Navigate(new AddEventPage());
        }

        private void OnDeleteEvent(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn) || !(btn.Tag is EventItem evt)) return;

            var result = MessageBox.Show($"Delete event \"{evt.Title}\"?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;

            var removed = store.RemoveEventById(evt.Id);
            if (removed)
            {
                try { Services.EventPersistenceService.Save(); } catch { }
                // Events_CollectionChanged handles UI and recommendations refresh
                MessageBox.Show("Event deleted.", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Could not delete the event.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnReturnHome(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomePage());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}