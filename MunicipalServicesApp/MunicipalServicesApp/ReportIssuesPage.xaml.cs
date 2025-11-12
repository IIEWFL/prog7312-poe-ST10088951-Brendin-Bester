using Microsoft.Win32;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Storage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MunicipalServicesApp
{
    public partial class ReportIssuesPage : Page, INotifyPropertyChanged
    {
        private ReportIssue currentIssue = new ReportIssue();
        private double progressValue;
        private string engagementMessage;

        public ObservableCollection<string> AttachedMediaPaths { get; }
            = new ObservableCollection<string>();

        public double ProgressValue
        {
            get => progressValue;
            set
            {
                progressValue = value;
                OnPropertyChanged(nameof(ProgressValue));
            }
        }

        public string EngagementMessage
        {
            get => engagementMessage;
            set
            {
                engagementMessage = value;
                OnPropertyChanged(nameof(EngagementMessage));
            }
        }

        public ReportIssuesPage()
        {
            InitializeComponent();
            this.DataContext = this;

            ProgressValue = 0;
            EngagementMessage = "Let's get started! Fill in the details below.";
        }

        //Allows user to attach media files to the issue report.
        //adegeo (2025). How to: Open files with the OpenFileDialog component - Windows Forms. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/how-to-open-files-using-the-openfiledialog-component.
        
        private void OnAttachMedia(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Images & Docs|*.jpg;*.png;*.bmp;*.pdf;*.docx"
            };

            if (dlg.ShowDialog() == true)
            {
                foreach (var file in dlg.FileNames)
                {
                    currentIssue.MediaAttachments.Add(file);
                    AttachedMediaPaths.Add(file);
                }

                UpdateProgress();
            }
        }

        //User enters the iformation into the form and error checking is done to ensure all required fields are filled.
        private void OnSubmit(object sender, RoutedEventArgs e)
        {
            var location = LocationBox.Text.Trim();
            var category = (CategoryBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            var description = new TextRange(
                DescriptionBox.Document.ContentStart,
                DescriptionBox.Document.ContentEnd).Text.Trim();

            if (string.IsNullOrWhiteSpace(location))
            {
                MessageBox.Show("Please enter a location.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("Please provide a description.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            currentIssue.Location = location;
            currentIssue.Category = category;
            currentIssue.Description = description;
            currentIssue.ReportedAt = DateTime.Now;

            DataStore.Instance.Issues.Add(currentIssue);

            ProgressValue = 100;
            EngagementMessage = "✅ Thank you! Your issue has been submitted.";
            SubmitButton.IsEnabled = false;
        }

        private void OnBack(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService?.CanGoBack == true)
                this.NavigationService.GoBack();
        }

        private void OnInputChanged(object sender, RoutedEventArgs e)
        {
            UpdateProgress();
        }

        //simple progess calculation based on filled fields for the progress bar and engagement message.
        //Hui (2011). The right way to implement a progressbar in C#. [online] Stack Overflow. Available at: https://stackoverflow.com/questions/6204749/the-right-way-to-implement-a-progressbar-in-c-sharp‌
        private void UpdateProgress()
        {
            int progress = 0;

            if (!string.IsNullOrWhiteSpace(LocationBox.Text))
                progress += 25;

            if (CategoryBox.SelectedItem != null)
                progress += 25;

            var desc = new TextRange(
                DescriptionBox.Document.ContentStart,
                DescriptionBox.Document.ContentEnd).Text.Trim();

            if (!string.IsNullOrWhiteSpace(desc))
                progress += 25;

            if (currentIssue.MediaAttachments.Count > 0)
                progress += 25;

            ProgressValue = progress;

            if (progress < 25)
                EngagementMessage = "Let's get started! Fill in the details below.";
            else if (progress < 50)
                EngagementMessage = "You're off to a good start!";
            else if (progress < 75)
                EngagementMessage = "Almost there — just a few more details.";
            else if (progress < 100)
                EngagementMessage = "Ready to submit!";
            else if (progress == 100)
                EngagementMessage = "✅ All set! You can submit now.";
            else
                EngagementMessage = "Fill in the form to begin.";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}