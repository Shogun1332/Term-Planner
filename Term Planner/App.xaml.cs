using System;
using System.IO;
using System.Threading.Tasks;
using Term_Planner.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Term_Planner
{
    public partial class App : Application
    {
        static Database database;
        public static Database Database
        {
            get
            {
                if(database == null)
                {
                    database = new Database(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "database.db3"));
                    
                }
                return database;
            }
        }
        public App()
        {
            if (Application.Current.Properties.ContainsKey("FirstUse"))
            {
                Task.Run(async () => { await Notifications.NotifyOnStart(); }).Wait();
            }
            else
            {
                Application.Current.Properties["FirstUse"] = false;
                SavePropertiesAsync();
                Task.Run(async () => { await Models.Term.GenerateStartingData(); }).Wait();
                Task.Run(async () => { await Notifications.NotifyOnStart(); }).Wait();
            }
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
