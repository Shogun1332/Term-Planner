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
    [XamlCompilation(XamlCompilationOptions.Compile), QueryProperty(nameof(AssessmentID), nameof(AssessmentID))]
    public partial class AssessmentDetailsPage : ContentPage
    {
        public string AssessmentID
        {
            set
            {
                LoadAssessment(value);
            }
        }
        public AssessmentDetailsPage()
        {
            InitializeComponent();
            BindingContext = new Assessment();
        }
        async void LoadAssessment(string assessmentID)
        {
            try
            {
                int id = Convert.ToInt32(assessmentID);
                Assessment assessment = await App.Database.GetAssessmentAsync(id);
                DueDate.Text = assessment.AssessmentDue.ToLocalTime().Date.ToShortDateString();
                BindingContext = assessment;
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load Term");
            }
        }
        async void OnEditAssessmentClicked(object sender, EventArgs e)
        {
            var assessment = (Assessment)BindingContext;
            await Shell.Current.GoToAsync($"{nameof(AssessmentEntryPage)}?{nameof(AssessmentEntryPage.AssessmentID)}={assessment.AssessmentID.ToString()}");
        }
    }
}