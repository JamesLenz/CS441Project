using Xamarin.Forms;
using System.Net.Http;

namespace cs441_project
{
    public partial class App : Application
    {
        public static HttpClient client;
        public static string userEmail = null;
        public static string userPassword = null;
        public static ClassroomInfoItem curClassroom;

        public App()
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000; //256KB

            InitializeComponent();

            MainPage = new NavigationPage(new cs441_projectPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
