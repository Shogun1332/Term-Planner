using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Term_Planner.Views;

namespace Term_Planner
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(TermListPage), typeof(TermListPage));
            Routing.RegisterRoute(nameof(TermEntryPage), typeof(TermEntryPage));
            Routing.RegisterRoute(nameof(TermDetailsPage), typeof(TermDetailsPage));
            Routing.RegisterRoute(nameof(CourseEntryPage), typeof(CourseEntryPage));
            Routing.RegisterRoute(nameof(CourseDetailsPage), typeof(CourseDetailsPage));
            Routing.RegisterRoute(nameof(AssessmentEntryPage), typeof(AssessmentEntryPage));
            Routing.RegisterRoute(nameof(AssessmentDetailsPage), typeof(AssessmentDetailsPage));
            Routing.RegisterRoute(nameof(NotesListPage), typeof(NotesListPage));
            Routing.RegisterRoute(nameof(NotesDetailsPage), typeof(NotesDetailsPage));

        }
    }
}
