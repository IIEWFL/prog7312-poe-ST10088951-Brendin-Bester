using MunicipalServicesApp.Models;
using MunicipalServicesApp.Storage;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MunicipalServicesApp
{
    public partial class AddEventPage : Page
    {
        public AddEventPage()
        {
            InitializeComponent();
        }

        private void OnSubmit(object sender, RoutedEventArgs e)
        {
            var title = TitleBox.Text.Trim();
            var category = CategoryBox.Text.Trim();
            var location = LocationBox.Text.Trim();
            var description = DescriptionBox.Text.Trim();
            var imagePath = ImagePathBox.Text.Trim();
            var date = DateBox.SelectedDate;

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(category) || date == null)
            {
                MessageBox.Show("Please fill in Title, Category, and Date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newEvent = new EventItem
            {
                Title = title,
                Category = category,
                Date = date.Value,
                Location = location,
                Description = description,
                ImagePath = imagePath
            };

            EventStore.Instance.AddEvent(newEvent);
            MessageBox.Show("✅ Event added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            NavigationService?.Navigate(new LocalEventsPage());
        }

        private void OnBack(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}