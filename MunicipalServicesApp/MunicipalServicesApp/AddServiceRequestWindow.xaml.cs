using MunicipalServicesApp.Models;
using System;
using System.Windows;

namespace MunicipalServicesApp
{
    public partial class AddServiceRequestWindow : Window
    {
        public ServiceRequest Result { get; private set; }

        public AddServiceRequestWindow()
        {
            InitializeComponent();
        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtTitle.Text))
            {
                MessageBox.Show("Enter a title.");
                return;
            }

            int priority = 5;
            if (ComboPriority.SelectedItem is System.Windows.Controls.ComboBoxItem ci)
            {
                int.TryParse(ci.Content.ToString(), out priority);
            }

            Result = new ServiceRequest
            {
                Title = TxtTitle.Text.Trim(),
                Location = TxtLocation.Text.Trim(),
                Description = TxtDescription.Text.Trim(),
                Priority = priority,
                CreatedAt = DateTime.UtcNow,
                Status = RequestStatus.Submitted
            };

            DialogResult = true;
            Close();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}