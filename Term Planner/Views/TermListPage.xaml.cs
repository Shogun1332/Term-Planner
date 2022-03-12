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
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermListPage : ContentPage
    {
        public TermListPage()
        {
            InitializeComponent();

        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            List<Term> termCollectionViewSource = await App.Database.GetTermsAsync();
            foreach(Term term in termCollectionViewSource)
            {
                term.TermStart.ToLocalTime();
                term.TermEnd.ToLocalTime();
                term.FormattedTermStart = term.TermStart.ToShortDateString();
                term.FormattedTermEnd = term.TermEnd.ToShortDateString();
            }
            termCollectionView.ItemsSource = termCollectionViewSource;
        }
        async void OnAddTermClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(TermEntryPage));
        }
        async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null)
            {
                Term term = (Term)e.CurrentSelection.FirstOrDefault();
                await Shell.Current.GoToAsync($"{nameof(TermDetailsPage)}?{nameof(TermDetailsPage.TermID)}={term.TermID.ToString()}");
            }
        }
    }
}