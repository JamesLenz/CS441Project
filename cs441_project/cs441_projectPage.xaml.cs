using Xamarin.Forms;

namespace cs441_project
{
    public partial class cs441_projectPage : ContentPage
    {
        public cs441_projectPage()
        {
            InitializeComponent();
        }

		void Login_Clicked(object sender, System.EventArgs e)
		{
            Navigation.PushAsync(new LoginPage());
		}

		void NewAccount_Clicked(object sender, System.EventArgs e)
		{
            Navigation.PushAsync(new NewAccountPage());
		}
    }
}
