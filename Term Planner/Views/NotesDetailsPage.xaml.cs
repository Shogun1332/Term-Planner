using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Term_Planner.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace Term_Planner.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile), QueryProperty(nameof(NoteID), nameof(NoteID)), QueryProperty(nameof(CourseID), nameof(CourseID))]
    public partial class NotesDetailsPage : ContentPage
    {
        public string NoteID
        {
            set
            {
                LoadNote(value);
            }
        }
        public string CourseID
        {
            set
            {
                NewNoteOwnedByCourse(value);
            }
        }
        public NotesDetailsPage()
        {
            InitializeComponent();
            BindingContext = new Note();
        }
        async void LoadNote(string noteID)
        {
            try
            {
                Title = "Edit Note";
                int id = Convert.ToInt32(noteID);
                Note note = await App.Database.GetNoteAsync(id);
                BindingContext = note;
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load Notes for this Course");
            }
        }
        async void NewNoteOwnedByCourse(string courseID)
        {
            try
            {
                Title = "New Note";
                int id = Convert.ToInt32(courseID);
                Course course = await App.Database.GetCourseAsync(id);
                DateTime tempDT = DateTime.Today.ToUniversalTime();
                Note note = new Note();
                note.CourseID = course.CourseID;
                note.NoteCreated = tempDT;
                note.FormattedNoteCreated = note.NoteCreated.ToShortDateString();
                BindingContext = note;
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to associate new note with Course");
            }
        }
        async void OnShareNoteClicked(object sender, EventArgs e)
        {
            var note = (Note)BindingContext;
            bool titleValid = true;
            bool contentValid = true;
            if (string.IsNullOrWhiteSpace(note.NoteTitle))
            {
                titleValid = false;
                await DisplayAlert("Error", "You must give this note a title to continue", "Okay");
            }
            if (string.IsNullOrWhiteSpace(note.NoteContent))
            {
                contentValid = false;
                await DisplayAlert("Error", "Your note's content cannot be empty.", "Okay");
            }
            if (titleValid && contentValid)
            {
                note.NoteCreated = DateTime.UtcNow;
                note.FormattedNoteCreated = DateTime.UtcNow.ToShortDateString();
                await App.Database.SaveNoteAsync(note);
            }
            await Share.RequestAsync(new ShareTextRequest
            {
                Title = note.NoteTitle,
                Subject = note.NoteTitle,
                Text = note.NoteContent
            });
        }
        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var note = (Note)BindingContext;
            bool titleValid = true;
            bool contentValid = true;
            if (string.IsNullOrWhiteSpace(note.NoteTitle))
            {
                titleValid = false;
                await DisplayAlert("Error", "You must give this note a title to continue", "Okay");
            }
            if (string.IsNullOrWhiteSpace(note.NoteContent))
            {
                contentValid = false;
                await DisplayAlert("Error", "Your note's content cannot be empty.", "Okay");
            }
            if (titleValid && contentValid)
            {
                await App.Database.SaveNoteAsync(note);
                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync($"../{nameof(NotesListPage)}?{nameof(NotesListPage.CourseID)}={note.CourseID.ToString()}");
            }
        }
        async void OnDeleteNoteButtonClicked(object sender, EventArgs e)
        {
            var note = (Note)BindingContext;
            int cid = note.CourseID;
            bool warn = await DisplayAlert("Warning", "Are you sure you want to delete this note? This is permanent!", "Proceed", "Cancel");
            if (warn)
            {
                await App.Database.DeleteNoteAsync(note);
                await DisplayAlert("WAAA!", "Deletion completed successfully.", "Okay");

            }
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync($"../{nameof(NotesListPage)}?{nameof(NotesListPage.CourseID)}={cid.ToString()}");
        }
    }
}