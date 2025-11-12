using MunicipalServicesApp.Models;
using MunicipalServicesApp.Storage;
using MunicipalServicesApp.DataStructures;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MunicipalServicesApp
{
    public partial class ReportedIssuesPage : Page
    {
        public List<ReportIssue> IssuesList { get; private set; }

        public ReportedIssuesPage()
        {
            InitializeComponent();
            LoadIssues();
        }

        private void LoadIssues()
        {
            IssuesList = DataStore.Instance.Issues.Enumerate().ToList();
            this.DataContext = null;
            this.DataContext = this;
        }

        private void OnBack(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService?.CanGoBack == true)
                this.NavigationService.GoBack();
        }

        private void OnDeleteIssue(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ReportIssue issue)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete the issue reported at \"{issue.Location}\"?",
                    "Confirm Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    var store = DataStore.Instance;
                    var updatedList = new CustomLinkedList<ReportIssue>();

                    foreach (var item in store.Issues.Enumerate())
                    {
                        if (item != issue)
                            updatedList.Add(item);
                    }

                    store.Issues.Clear();
                    foreach (var item in updatedList.Enumerate())
                        store.Issues.Add(item);

                    LoadIssues(); // Refresh UI
                }
            }
        }
    }
}
