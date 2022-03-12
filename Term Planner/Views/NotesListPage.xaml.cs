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
    public partial class NotesListPage : ContentPage
    {
        public string CourseID
        {
            set
            {
                LoadCourseNotes(value);
            }
        }
        public NotesListPage()
        {
            InitializeComponent();
            BindingContext = new Course();
        }
        async void LoadCourseNotes(string courseID)
        {
            try
            {
                int id = Convert.ToInt32(courseID);
                Course course = await App.Database.GetCourseAsync(id);
                Title = $"{course.CourseName} Notes";
                List<Note> notelist = await App.Database.GetCourseNotesAsync(course);
                foreach(Note note in notelist)
                {
                    note.NoteCreated.ToLocalTime();
                    note.FormattedNoteCreated = note.NoteCreated.ToShortDateString();
                }
                NotesCollectionView.ItemsSource = notelist;
                BindingContext = course;
                
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load Notes for this Course");
            }
        }
        async void OnNewNoteClicked(object sender, EventArgs e)
        {
            var course = (Course)BindingContext;
            await Shell.Current.GoToAsync($"{nameof(NotesDetailsPage)}?{nameof(NotesDetailsPage.CourseID)}={course.CourseID.ToString()}");
        }
        async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null)
            {
                Note note = (Note)e.CurrentSelection.FirstOrDefault();
                await Shell.Current.GoToAsync($"{nameof(NotesDetailsPage)}?{nameof(NotesDetailsPage.NoteID)}={note.NoteID.ToString()}");
            }
        }
    }
}