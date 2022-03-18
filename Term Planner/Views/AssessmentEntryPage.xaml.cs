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
    [XamlCompilation(XamlCompilationOptions.Compile), QueryProperty(nameof(AssessmentID), nameof(AssessmentID)), QueryProperty(nameof(CourseID), nameof(CourseID))]
    public partial class AssessmentEntryPage : ContentPage
    {
        public string AssessmentID
        {
            set
            {
                LoadAssessment(value);
            }
        }
        public string CourseID
        {
            set
            {
                SetAssessmentCourseID(value);
            }
            get
            {
                return CourseID;
            }
        }
        public AssessmentEntryPage()
        {
            InitializeComponent();
            BindingContext = new Assessment();
        }
        async void LoadAssessment(string assessmentID)
        {
            try
            {
                Title = "Edit Assessment";
                int id = Convert.ToInt32(assessmentID);
                Assessment assessment = await App.Database.GetAssessmentAsync(id);
                assessment.AssessmentDue.ToLocalTime().Date.ToShortDateString();
                //assessment.CourseOwnsAssessments = await App.Database.GetCourseAssessmentsAsync(course); not sure there's a similar expression in Assessment
                BindingContext = assessment;
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load Assessment");
            }
        }
        async void SetAssessmentCourseID(string courseID) //Used when adding a new assessment to a course to ensure the proper courseID and termID is set
        {
            try
            {
                Title = "New Assessment";
                int id = Convert.ToInt32(courseID);
                Course course = await App.Database.GetCourseAsync(id);
                Assessment assessment = new Assessment();
                assessment.CourseID = id;
                assessment.TermID = course.TermID;
                assessment.AssessmentDue = DateTime.Today;
                BindingContext = assessment;
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load Assessment");
            }
        }
        private void DueDatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            var assessment = (Assessment)BindingContext;
            assessment.AssessmentDue = DueDatePicker.Date.ToUniversalTime();
            assessment.FormattedAssessmentDue = DueDatePicker.Date.ToUniversalTime().ToShortDateString();
        }
        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var assessment = (Assessment)BindingContext;
            bool assessmentValid = true;
            bool assessmentNameValid = true;
            bool assessmentTypeValid = true;
            if(assessment.CourseID > 0)
            {
                Course course = await App.Database.GetCourseAsync(assessment.CourseID);
                List<Assessment> assessmentsOwnedByParent = await App.Database.GetCourseAssessmentsAsync(course);
                int existingAssessmentCheck = assessmentsOwnedByParent.Where(i => i.AssessmentID == assessment.AssessmentID).Count();
                if (existingAssessmentCheck == 0)
                {
                    int assessmentValidation = assessmentsOwnedByParent.Where(i => i.AssessmentType == assessment.AssessmentType).Count();
                    if (assessmentValidation >= 1)
                    {
                        assessmentValid = false;
                    }
                }
                else if (existingAssessmentCheck > 0)
                {
                    assessmentValid = false;
                    int assessmentValidation = assessmentsOwnedByParent.Where(i => i.AssessmentType == assessment.AssessmentType).Count(i => i.AssessmentID == assessment.AssessmentID);
                    if (assessmentValidation == 1)
                    {
                        assessmentValid = true;
                    }
                    if (assessmentValidation > 1)
                    {
                        assessmentValid = false;
                    }
                }
            }
            if (TypePicker.SelectedItem == null)
            {
                assessmentTypeValid = false;
                await DisplayAlert("Error", "You must select an assessment type to continue", "Okay");
            }
            if (string.IsNullOrWhiteSpace(assessment.AssessmentName))
            {
                assessmentNameValid = false;
                await DisplayAlert("Error", "You must provide an Assessment Name to continue", "Okay");
            }
            if (!assessmentValid)
            {
                await DisplayAlert("Error", "A course can only have one Objective Assessment and one Performance Assessment.", "Okay");
            }
            if (assessmentNameValid && assessmentTypeValid && assessmentValid)
            {
                await App.Database.SaveAssessmentAsync(assessment);
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync($"{nameof(CourseDetailsPage)}?{nameof(CourseDetailsPage.CourseID)}={assessment.CourseID.ToString()}");
            }
        }
        async void OnDeleteAssessmentButtonClicked(object sender, EventArgs e)
        {
            var assessment = (Assessment)BindingContext;
            int cid = assessment.CourseID;
            bool warn = await DisplayAlert("Warning", "Are you sure you want to delete this assessment? This is permanent!", "Proceed", "Cancel");
            if (warn)
            {
                await App.Database.DeleteAssessmentAsync(assessment);
                await DisplayAlert("WAAA!", "Deletion completed successfully.", "Okay");
            }
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync($"{nameof(CourseDetailsPage)}?{nameof(CourseDetailsPage.CourseID)}={cid.ToString()}");
        }
        private void AssessmentNotifications_Toggled(object sender, ToggledEventArgs e)
        {
            var assessment = (Assessment)BindingContext;
            if (AssessmentNotifications.IsToggled)
            {
                assessment.NotifEnabled = true;
            }
            if (!AssessmentNotifications.IsToggled)
            {
                assessment.NotifEnabled = false;
            }
        }
        private void TypePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var assessment = (Assessment)BindingContext;
            assessment.AssessmentType = (string)TypePicker.SelectedItem;
        }
    }
}