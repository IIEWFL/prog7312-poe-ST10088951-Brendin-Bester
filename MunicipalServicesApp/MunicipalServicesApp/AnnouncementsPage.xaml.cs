using MunicipalServicesApp.Models;
using MunicipalServicesApp.Storage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MunicipalServicesApp
{
    public partial class AnnouncementsPage : Page, INotifyPropertyChanged
    {
        private readonly AnnouncementStore store = AnnouncementStore.Instance;

        public ObservableCollection<Announcement> Announcements { get; } = new ObservableCollection<Announcement>();

        public AnnouncementsPage()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += AnnouncementsPage_Loaded;
        }

        private void AnnouncementsPage_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshList();
        }

        private void RefreshList()
        {
            Announcements.Clear();
            foreach (var a in store.GetAll().OrderByDescending(x => x.PublishDate)) Announcements.Add(a);
            FooterText.Text = $"Showing {Announcements.Count} announcement(s)";
        }

        private void OnReturnHome(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomePage());
        }

        private void OnOpenAnnouncement(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn) || !(btn.Tag is Announcement ann)) return;
            NavigationService?.Navigate(new AnnouncementDetailPage(ann.Id));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}