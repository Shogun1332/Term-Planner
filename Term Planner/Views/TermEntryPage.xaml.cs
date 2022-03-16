using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Term_Planner.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Term_Planner.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile), QueryProperty(nameof(TermID), nameof(TermID))]
    public partial class TermEntryPage : ContentPage
    {
        public string TermID
        {
            set
            {
                LoadTerm(value);
            }
        }
        public TermEntryPage()
        {
            InitializeComponent();
            BindingContext = new Term();
            Title = "New Term";
            StartDatePicker.Date = DateTime.Today;
            EndDatePicker.Date = DateTime.Today.AddDays(180);
        }
        async void LoadTerm(string termID)
        {
            try
            {
                Title = "Edit Term";
                int id = Convert.ToInt32(termID);
                Term term = await App.Database.GetTermAsync(id);
                StartDatePicker.Date = term.TermStart.ToLocalTime();
                EndDatePicker.Date = term.TermEnd.ToLocalTime();
                term.TermOwnsCourses = await App.Database.GetTermCoursesAsync(term);
                BindingContext = term;
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load Term");
            }
        }
        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var term = (Term)BindingContext;
            bool nameValid = true;
            bool startValid = true;
            bool endValid = true;
            if (string.IsNullOrWhiteSpace(term.TermName))
            {
                await DisplayAlert("Error", "You must provide a Term Name to continue", "Okay");
                nameValid = false;
            }
            if (term.FormattedTermStart == "")
            {
                await DisplayAlert("Error", "You must provide a Start Date for the Term to continue.", "Okay");
                startValid = false;
            }
            if (term.FormattedTermEnd == "")
            {
                await DisplayAlert("Error", "You must provide an End Date for the Term to continue.", "Okay");
                endValid = false;
            }
            if (EndDatePicker.Date < StartDatePicker.Date)
            {
                await DisplayAlert("Error", "The Term's anticipated end date cannot be before the term's start date.", "Okay");
                endValid = false;
            }
            if (nameValid && startValid && endValid)
            {
                term.TermStart = StartDatePicker.Date.ToUniversalTime();
                term.TermEnd = EndDatePicker.Date.ToUniversalTime();
                await App.Database.SaveTermAsync(term);
                await Shell.Current.Navigation.PopToRootAsync();
            }
        }
        async void OnDeleteTermButtonClicked(object sender, EventArgs e)
        {
            var term = (Term)BindingContext;
            bool warn = await DisplayAlert("Warning", "Are you sure you want to delete this term and any courses linked to this term? This is permanent!","Proceed", "Cancel");
            if (warn)
            {
                await term.DeleteTermChildren(term.TermID);
                await App.Database.DeleteTermAsync(term);
                await DisplayAlert("WAAA!", "Deletion completed successfully.", "Okay");
            }
            await Shell.Current.Navigation.PopToRootAsync();
        }
        private void StartDatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            var term = (Term)BindingContext;
            term.TermStart = StartDatePicker.Date.ToUniversalTime();
            term.FormattedTermStart = StartDatePicker.Date.ToUniversalTime().ToShortDateString();
        }

        private async void EndDatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            var term = (Term)BindingContext;
            if (term.FormattedTermStart == "")
            {
                await DisplayAlert("Error", "You must set a start date for the term before setting an end date.", "Okay");
            }
            if (EndDatePicker.Date < StartDatePicker.Date)
            {
                await DisplayAlert("Error", "The Term's anticipated end date cannot be before the term's start date.", "Okay");
            }
            term.TermEnd = EndDatePicker.Date.ToUniversalTime();
            term.FormattedTermEnd = EndDatePicker.Date.ToUniversalTime().ToShortDateString();
        }
    }
}