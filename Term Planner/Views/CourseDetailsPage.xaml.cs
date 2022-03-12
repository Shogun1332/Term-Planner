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
    [XamlCompilation(XamlCompilationOptions.Compile), QueryProperty(nameof(CourseID), nameof(CourseID))]
    public partial class CourseDetailsPage : ContentPage
    {
        public string CourseID
        {
            set
            {
                LoadCourse(value);
            }
        }
        public CourseDetailsPage()
        {
            InitializeComponent();
            BindingContext = new Course();
        }
        async void LoadCourse(string courseID)
        {
            try
            {
                int id = Convert.ToInt32(courseID);
                Course course = await App.Database.GetCourseAsync(id);
                StartDate.Text = course.CourseStart.ToLocalTime().Date.ToShortDateString();
                EndDate.Text = course.CourseEnd.ToLocalTime().Date.ToShortDateString();
                course.CourseOwnsAssessments = await App.Database.GetCourseAssessmentsAsync(course);
                BindingContext = course;
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load Term");
            }
        }
        async void OnEditCourseClicked(object sender, EventArgs e)
        {
            var course = (Course)BindingContext;
            await Shell.Current.GoToAsync($"{nameof(CourseEntryPage)}?{nameof(CourseEntryPage.CourseID)}={course.CourseID.ToString()}");
        }
        async void OnAddAssessmentButtonClicked(object sender, EventArgs e)
        {
            var course = (Course)BindingContext;
            if (course.CourseOwnsAssessments.Count > 1)
            {
                await DisplayAlert("WAAA!", "A course can have a maximum of two assessments.", "Go Back");
            }
            else
            {
                await Shell.Current.GoToAsync($"{nameof(AssessmentEntryPage)}?{nameof(AssessmentEntryPage.CourseID)}={course.CourseID.ToString()}");

            }
        }
        async void OnViewAssessmentButtonClicked(object sender, EventArgs e)
        {
            if(courseOwnsAssessmentsCollectionView.SelectedItem != null)
            {
                var assessment = (Assessment)courseOwnsAssessmentsCollectionView.SelectedItem;
                await Shell.Current.GoToAsync($"{nameof(AssessmentDetailsPage)}?{nameof(AssessmentDetailsPage.AssessmentID)}={assessment.AssessmentID.ToString()}");
            }
        }
        async void OnDeleteAssessmentButtonClicked(object sender, EventArgs e)
        {
            if(courseOwnsAssessmentsCollectionView.SelectedItem != null)
            {
                var assessment = (Assessment)courseOwnsAssessmentsCollectionView.SelectedItem;
                bool warn = await DisplayAlert("Warning", "Are you sure you want to delete this assessment? This is permanent!", "Proceed", "Cancel");
                if (warn)
                {
                    await App.Database.DeleteAssessmentAsync(assessment);
                    await DisplayAlert("WAAA!", "Deletion completed successfully.", "Okay");
                }
            }
        }
        async void OnCourseNotesButtonClicked(object sender, EventArgs e)
        {
            var course = (Course)BindingContext;
            course.CourseOwnsNotes = await App.Database.GetCourseNotesAsync(course);
            await Shell.Current.GoToAsync($"{nameof(NotesListPage)}?{nameof(NotesListPage.CourseID)}={course.CourseID.ToString()}");
        }
    }
}