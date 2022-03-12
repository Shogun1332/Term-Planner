using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Term_Planner.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Term_Planner.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile), QueryProperty(nameof(TermID), nameof(TermID))]
    public partial class TermDetailsPage : ContentPage
    {
        public string TermID
        {
            set
            {
                LoadTerm(value);
            }
        }
        public TermDetailsPage()
        {
            InitializeComponent();
            BindingContext = new Term();

        }
        async void LoadTerm(string termID)
        {
            try
            {
                int id = Convert.ToInt32(termID);
                Term term = await App.Database.GetTermAsync(id);
                StartDate.Text = term.TermStart.ToLocalTime().Date.ToShortDateString();
                EndDate.Text = term.TermEnd.ToLocalTime().Date.ToShortDateString();
                term.TermOwnsCourses = await App.Database.GetTermCoursesAsync(term);
                foreach(Course course in term.TermOwnsCourses)
                {
                    course.CourseStart.ToLocalTime();
                    course.CourseEnd.ToLocalTime();
                    course.FormattedCourseStart = course.CourseStart.ToShortDateString();
                    course.FormattedCourseEnd = course.CourseEnd.ToShortDateString();
                }
                BindingContext = term;
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load Term");
            }
        }
        async void OnEditTermClicked(object sender, EventArgs e)
        {
            var term = (Term)BindingContext;
            await Shell.Current.GoToAsync($"{nameof(TermEntryPage)}?{nameof(TermEntryPage.TermID)}={term.TermID.ToString()}");
        }
        async void OnAddCourseButtonClicked(object sender, EventArgs e)
        {
            var term = (Term)BindingContext;
            if(term.TermOwnsCourses.Count > 5)
            {
                await DisplayAlert("WAAA!", "A term can have a maximum of six courses.", "Go Back");
            }
            else
            {
                await Shell.Current.GoToAsync($"{nameof(CourseEntryPage)}?{nameof(CourseEntryPage.TermID)}={term.TermID.ToString()}");
            }
        }
        async void OnEditCourseButtonClicked(object sender, EventArgs e)
        {
            if (termOwnsCoursesCollectionView.SelectedItem != null)
            {
                var course = (Course)termOwnsCoursesCollectionView.SelectedItem;
                await Shell.Current.GoToAsync($"{nameof(CourseDetailsPage)}?{nameof(CourseDetailsPage.CourseID)}={course.CourseID.ToString()}");
            }
            else
            {
                await DisplayAlert("WAAA!", "You must select a course to view before you can view it!", "Go Back");
            }
        }
        async void OnDeleteCourseButtonClicked(object sender, EventArgs e)
        {
            if(termOwnsCoursesCollectionView.SelectedItem != null)
            {
                Course course = (Course)termOwnsCoursesCollectionView.SelectedItem;
                bool warn = await DisplayAlert("Warning", "Are you sure you want to delete this course and any assessments linked to it? This is permanent!", "Proceed", "Cancel");
                if (warn)
                {
                    await course.DeleteCourseAndChildren(course.CourseID);
                    await DisplayAlert("WAAA!", "Deletion completed successfully.", "Okay");
                }
            }
            else
            {
                await DisplayAlert("WAAA!", "You must select a course to delete before you can delete it!", "Go Back");
            }
        }
    }
}