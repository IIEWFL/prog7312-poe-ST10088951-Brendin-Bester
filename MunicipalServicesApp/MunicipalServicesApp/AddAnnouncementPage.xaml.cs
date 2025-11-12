using MunicipalServicesApp.Models;
using MunicipalServicesApp.Services;
using MunicipalServicesApp.Storage;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MunicipalServicesApp
{
    public partial class AddAnnouncementPage : Page
    {
        private readonly AnnouncementStore store = AnnouncementStore.Instance;

        public AddAnnouncementPage()
        {
            InitializeComponent();
            PublishDatePicker.SelectedDate = DateTime.Today;
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            var title = TitleBox.Text?.Trim();
            var category = CategoryBox.Text?.Trim();
            var body = BodyBox.Text?.Trim();
            var author = AuthorBox.Text?.Trim();
            var publishDate = PublishDatePicker.SelectedDate ?? DateTime.Today;
            var expiry = ExpiryDatePicker.SelectedDate;

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(body))
            {
                MessageBox.Show("Please enter a title and body for the announcement.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (expiry.HasValue && expiry.Value.Date < publishDate.Date)
            {
                MessageBox.Show("Expiry date cannot be earlier than publish date.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var announcement = new Announcement
            {
                Title = title,
                Body = body,
                Category = category,
                Author = author,
                PublishDate = publishDate.Date,
                ExpiryDate = expiry?.Date
            };

            store.AddAnnouncement(announcement);

            try
            {
                AnnouncementPersistence.Save();
            }
            catch
            {
                // ignore persistence error but keep UI responsive
            }

            NavigationService?.GoBack();
        }
    }
}
