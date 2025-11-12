using MunicipalServicesApp.Models;
using MunicipalServicesApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MunicipalServicesApp
{
    public partial class ServiceRequestsPage : Page
    {
        private readonly ServiceRequestRepository repo = ServiceRequestRepository.Instance;
        public ObservableCollection<object> DisplayRequests { get; } = new ObservableCollection<object>();

        public ServiceRequestsPage()
        {
            InitializeComponent();
            DataContext = this;
            LoadDisplayRequests();
        }

        private void LoadDisplayRequests()
        {
            DisplayRequests.Clear();
            foreach (var r in repo.AllRequests.OrderByDescending(x => x.CreatedAt))
            {
                DisplayRequests.Add(new
                {
                    r.Id,
                    r.Title,
                    r.Location,
                    Status = r.Status.ToString(),
                    r.Priority,
                    CreatedAt = r.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm")
                });
            }
        }

        private void OnRefresh(object sender, RoutedEventArgs e)
        {
            LoadDisplayRequests();
            GraphList.Items.Clear();
        }

        private void OnAddRequest(object sender, RoutedEventArgs e)
        {
            var dlg = new AddServiceRequestWindow();
            if (dlg.ShowDialog() == true)
            {
                var r = dlg.Result;
                repo.AddRequest(r);
                ServiceRequestPersistence.Save();
                LoadDisplayRequests();
            }
        }

        private void OnFindById(object sender, RoutedEventArgs e)
        {
            GraphList.Items.Clear();
            var text = SearchBox.Text?.Trim();
            if (string.IsNullOrEmpty(text)) { MessageBox.Show("Enter a GUID or search term."); return; }

            if (Guid.TryParse(text, out Guid id))
            {
                var found = repo.FindById(id);
                if (found == null) { MessageBox.Show("Request not found."); return; }
                MessageBox.Show($"{found.Title}\nStatus: {found.Status}\nPriority: {found.Priority}\nLocation: {found.Location}\nCreated: {found.CreatedAt}", "Request Details", MessageBoxButton.OK, MessageBoxImage.Information);
                var neigh = repo.GetRelated(found).ToList();
                if (neigh.Any())
                {
                    GraphList.Items.Add("Related requests:");
                    foreach (var n in neigh) GraphList.Items.Add($"{n.Title} - {n.Id} - Status: {n.Status} - Priority: {n.Priority}");
                }
                else GraphList.Items.Add("No related requests found.");
                return;
            }

            // treat as general search
            var matches = repo.AllRequests.Where(r => (!string.IsNullOrEmpty(r.Title) && r.Title.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0)
                                                   || (!string.IsNullOrEmpty(r.Location) && r.Location.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0))
                                          .OrderByDescending(r => r.CreatedAt).ToList();
            if (!matches.Any()) { MessageBox.Show("No matches."); return; }
            DisplayRequests.Clear();
            foreach (var r in matches)
            {
                DisplayRequests.Add(new
                {
                    r.Id,
                    r.Title,
                    r.Location,
                    Status = r.Status.ToString(),
                    r.Priority,
                    CreatedAt = r.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm")
                });
            }
        }

        private void OnSearchChanged(object sender, TextChangedEventArgs e)
        {
            // live search small optimization: if empty show all
            if (string.IsNullOrWhiteSpace(SearchBox.Text)) LoadDisplayRequests();
        }

        private void OnView(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Guid id)
            {
                var found = repo.FindById(id);
                if (found == null) return;
                MessageBox.Show($"{found.Title}\n\n{found.Description}\n\nStatus: {found.Status}\nPriority: {found.Priority}\nLocation: {found.Location}\nCreated: {found.CreatedAt}", "Request Details", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Guid id)
            {
                var res = MessageBox.Show("Delete selected request?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (res != MessageBoxResult.Yes) return;
                var ok = repo.RemoveRequest(id);
                if (ok)
                {
                    ServiceRequestPersistence.Save();
                    LoadDisplayRequests();
                }
                else MessageBox.Show("Could not remove request.");
            }
        }

        private void OnTopPriority(object sender, RoutedEventArgs e)
        {
            GraphList.Items.Clear();
            var top = repo.GetByPriorityTopN(3).ToList();
            if (!top.Any()) { GraphList.Items.Add("No requests in heap."); return; }
            GraphList.Items.Add("Top priority requests (1=highest):");
            foreach (var t in top) GraphList.Items.Add($"{t.Title} - Priority: {t.Priority} - ID: {t.Id}");
        }

        private void OnShowMst(object sender, RoutedEventArgs e)
        {
            GraphList.Items.Clear();
            var edges = repo.GetMstEdges().ToList();
            if (!edges.Any()) { GraphList.Items.Add("No MST (graph empty or disconnected)."); return; }
            GraphList.Items.Add("MST Edges (Prim):");
            foreach (var (a, b, w) in edges) GraphList.Items.Add($"{a.Title} <-> {b.Title}  weight={w}");
        }

        private void OnShowNeighbors(object sender, RoutedEventArgs e)
        {
            GraphList.Items.Clear();
            if (RequestsGrid.SelectedItem == null) { GraphList.Items.Add("Select a row first."); return; }
            var dyn = RequestsGrid.SelectedItem;
            var idProp = dyn.GetType().GetProperty("Id");
            if (idProp == null) { GraphList.Items.Add("Could not read ID."); return; }
            var id = (Guid)idProp.GetValue(dyn);
            var found = repo.FindById(id);
            if (found == null) { GraphList.Items.Add("Request not found."); return; }
            var neigh = repo.GetRelated(found).ToList();
            if (!neigh.Any()) { GraphList.Items.Add("No related requests."); return; }
            GraphList.Items.Add($"Neighbors of {found.Title}:");
            foreach (var n in neigh) GraphList.Items.Add($"{n.Title} - {n.Id} - Status: {n.Status}");
        }
    }
}