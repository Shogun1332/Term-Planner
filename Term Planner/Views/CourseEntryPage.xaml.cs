using Plugin.LocalNotifications;
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
    [XamlCompilation(XamlCompilationOptions.Compile), QueryProperty(nameof(CourseID), nameof(CourseID)), QueryProperty(nameof(TermID), nameof(TermID))]
    public partial class CourseEntryPage : ContentPage
    {
        public string CourseID
        {
            set
            {
                LoadCourse(value);
            }
        }
        public string TermID
        {
            set
            {
                SetCourseTermID(value);
            }
            get
            {
                return TermID;
            }
        }
        public CourseEntryPage()
        {
            InitializeComponent();
            BindingContext = new Course();
        }
        async void LoadCourse(string courseID)
        {
            try
            {
                Title = "Edit Course";
                int id = Convert.ToInt32(courseID);
                Course course = await App.Database.GetCourseAsync(id);
                CourseStatusPicker.SelectedItem = course.CourseStatus;
                BindingContext = course;
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load Course");
            }
        }
        void SetCourseTermID(string termID) //Used when adding a new course to a term to ensure the proper termID is set
        {
            try
            {
                Title = "New Course";
                int id = Convert.ToInt32(termID);
                Course course = new Course();
                course.TermID = id;
                course.CourseStart = DateTime.Today;
                course.CourseEnd = DateTime.Today.AddDays(30);
                BindingContext = course;
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load Course");
            }
        }
        async void OnDeleteCourseButtonClicked(object sender, EventArgs e)
        {
            var course = (Course)BindingContext;
            int tid = course.TermID;
            List<Assessment> courseAssessments = await App.Database.GetCourseAssessmentsAsync(course);
            List<Note> courseNotes = await App.Database.GetCourseNotesAsync(course);
            bool warn = await DisplayAlert("Warning", "Are you sure you want to delete this course, any assessments linked to it, and any notes linked to it? This is permanent!", "Proceed", "Cancel");
            if (warn)
            {
                foreach(Assessment assessment in courseAssessments)
                {
                    await App.Database.DeleteAssessmentAsync(assessment);
                }
                foreach (Note note in courseNotes)
                {
                    await App.Database.DeleteNoteAsync(note);
                }
                await App.Database.DeleteCourseAsync(course);
                await DisplayAlert("WAAA!", "Deletion completed successfully.", "Okay");
            }
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync($"{nameof(TermDetailsPage)}?{nameof(TermDetailsPage.TermID)}={tid.ToString()}");
        }
        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var course = (Course)BindingContext;
            course.InstructorName = InstructorName.Text;
            course.InstructorEmail = InstructorEmail.Text;
            course.InstructorPhone = InstructorPhone.Text;
            bool courseNameValid = true;
            bool instructorEmailValid = true;
            bool instructorNameValid = true;
            bool instructorPhoneValid = true;
            bool startValid = true;
            bool endValid = true;
            if (string.IsNullOrWhiteSpace(course.CourseName))
            {
                courseNameValid = false;
                await DisplayAlert("Error", "You must provide a Course Name to continue", "Okay");
            }
            if (string.IsNullOrWhiteSpace(course.InstructorName))
            {
                instructorNameValid = false;
                await DisplayAlert("Error", "You must provide an instructor name to continue", "Okay");
            }
            if (!Course.IsInstructorEmailValid(course.InstructorEmail))
            {
                instructorEmailValid = false;
                await DisplayAlert("Error", "You must provide a valid instructor email address to continue", "Okay");
            }
            if (string.IsNullOrWhiteSpace(course.InstructorPhone))
            {
                instructorPhoneValid = false;
                await DisplayAlert("Error", "You must provide an instructor phone number to continue", "Okay");
            }
            if (course.FormattedCourseStart == "")
            {
                await DisplayAlert("Error", "You must provide a Start Date for the Course to continue.", "Okay");
                startValid = false;
            }
            if (course.FormattedCourseEnd == "")
            {
                await DisplayAlert("Error", "You must provide an End Date for the Course to continue.", "Okay");
                endValid = false;
            }
            if (EndDatePicker.Date < StartDatePicker.Date)
            {
                await DisplayAlert("Error", "The courses's anticipated end date cannot be before the courses's start date.", "Okay");
                endValid = false;
            }
            if (courseNameValid && instructorEmailValid && instructorNameValid && instructorPhoneValid && startValid && endValid)
            {
                await App.Database.SaveCourseAsync(course);
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync($"{nameof(TermDetailsPage)}?{nameof(TermDetailsPage.TermID)}={course.TermID.ToString()}");
            }
        }
        private void StartDatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            var course = (Course)BindingContext;
            course.CourseStart = StartDatePicker.Date.ToUniversalTime();
            course.FormattedCourseStart = StartDatePicker.Date.ToUniversalTime().ToShortDateString();

        }

        private async void EndDatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            var course = (Course)BindingContext;
            if (EndDatePicker.Date < StartDatePicker.Date)
            {
                await DisplayAlert("Error", "The courses's anticipated end date cannot be before the courses's start date.", "Okay");
            }
            course.CourseEnd = EndDatePicker.Date.ToUniversalTime();
            course.FormattedCourseEnd = EndDatePicker.Date.ToUniversalTime().ToShortDateString();

        }

        private void CourseNotifications_Toggled(object sender, ToggledEventArgs e)
        {
            var course = (Course)BindingContext;
            if (CourseNotifications.IsToggled)
            {
                course.NotifEnabled = true;
            }
            if (!CourseNotifications.IsToggled)
            {
                course.NotifEnabled = false;
            }
        }
    }
}