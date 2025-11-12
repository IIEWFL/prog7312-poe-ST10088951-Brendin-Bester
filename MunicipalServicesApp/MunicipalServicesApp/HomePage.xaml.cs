using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

//the sequence of pages is MainWindow -> HomePage -> ReportIssuesPage/ReportedIssuesPage that can be navigated to from HomePage
namespace MunicipalServicesApp
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void OnReportIssues(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ReportIssuesPage());
        }

        private void OnAnnouncements(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AnnouncementsPage());
        }

        private void OnServiceRequests(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ServiceRequestsPage());
        }
        private void OnViewReportedIssues(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ReportedIssuesPage());
        }

        private void OnAddAnnouncement(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddAnnouncementPage());
        }
        private void OnLocalEvents(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new LocalEventsPage());
        }


    }
}