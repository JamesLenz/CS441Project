using Xamarin.Forms;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System;

namespace cs441_project
{
    public partial class cs441_projectPage : ContentPage
    {
        private SendToServer sts;
        private Uri uri = new Uri("http://54.193.30.236/index.py");

		public cs441_projectPage()
        {
			NavigationPage.SetHasNavigationBar(this, false); //remove navigation bar for sign in page
            InitializeComponent();

            sts = new SendToServer(this);
        }

		async void Login_Clicked(object sender, System.EventArgs e)
		{
            if (Email_Entry.Text == null || Email_Entry.Text == "")
			{ //if entry box was not touched (null) or is touched but empty ("")
				await DisplayAlert("Error: Email", "Please enter an email", "OK");
				return;
			}
            if (Password_Entry.Text == null || Password_Entry.Text == "")
            { //if entry box was not touched (null) or is touched but empty ("")
                await DisplayAlert("Error: Password", "Please enter a password", "OK");
                return;
            }

            //create the item we want to send
            var item = new ValidateUserItem();
			item.Email = Email_Entry.Text;
			item.Password = Password_Entry.Text;

            sts.send(uri, item, async () => 
            {
                App.userEmail = item.Email;
                App.userPassword = item.Password;

                await Navigation.PushAsync(new HomePage()); //goto home page
            });
        }

		async void NewAccount_Clicked(object sender, System.EventArgs e)
		{
            await Navigation.PushAsync(new NewAccountPage()); //goto the new account creation page
        }

        async void ForgotPassword_Clicked(object sender, System.EventArgs e)
        {
			if (Email_Entry.Text == null || Email_Entry.Text == "")
			{ //if entry box was not touched (null) or is touched but empty ("")
				await DisplayAlert("Error: Email", "Please enter an email", "OK");
				return;
			}

            //ask user before sending email
            var answer = await DisplayAlert("Forgot Password?", "Would you like us to send your password to the email provided?", "Yes", "No");
            if (answer == false) //user changed their mind
                return;

            //create the item we want to send
            var item = new ForgotPasswordItem();
            item.Email = Email_Entry.Text;

            sts.send(uri, item, async () => 
            {
                await DisplayAlert("Email Sent", sts.responseItem.Response, "OK");
            });
		}
    }
}
