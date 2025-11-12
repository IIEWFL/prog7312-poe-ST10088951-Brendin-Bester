using MunicipalServicesApp.Models;
using MunicipalServicesApp.Storage;
using MunicipalServicesApp.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MunicipalServicesApp
{
    public partial class AnnouncementDetailPage : Page
    {
        private readonly AnnouncementStore store = AnnouncementStore.Instance;
        private readonly Guid announcementId;
        private Announcement current;

        public AnnouncementDetailPage(Guid id)
        {
            InitializeComponent();
            announcementId = id;
            Loaded += AnnouncementDetailPage_Loaded;
        }

        private void AnnouncementDetailPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAnnouncement();
        }

        private void LoadAnnouncement()
        {
            current = store.FindById(announcementId);
            if (current == null)
            {
                MessageBox.Show("Announcement not found.", "Not found", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService?.GoBack();
                return;
            }

            TitleText.Text = current.Title;
            CategoryText.Text = current.Category ?? string.Empty;
            PublishDateText.Text = $"Published: {current.PublishDate:yyyy-MM-dd}";
            ExpiryDateText.Text = current.ExpiryDate.HasValue ? $"Expires: {current.ExpiryDate:yyyy-MM-dd}" : string.Empty;
            BodyText.Text = current.Body ?? string.Empty;
            AuthorText.Text = string.IsNullOrWhiteSpace(current.Author) ? string.Empty : $"Author: {current.Author}";
            Footer.Text = $"ID: {current.Id}";
        }

        private void OnBack(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            if (current == null) return;

            var res = MessageBox.Show($"Delete announcement \"{current.Title}\"?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (res != MessageBoxResult.Yes) return;

            var removed = store.RemoveAnnouncementById(current.Id);
            if (removed)
            {
                try { AnnouncementPersistence.Save(); } catch { }
                MessageBox.Show("Announcement deleted.", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService?.GoBack();
            }
            else
            {
                MessageBox.Show("Could not delete announcement.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}