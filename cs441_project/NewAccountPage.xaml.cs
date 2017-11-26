using System;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;
using System.Text;

namespace cs441_project
{
    public partial class NewAccountPage : ContentPage
    {
        private SendToServer sts;
        private Uri uri = new Uri("http://54.193.30.236/index.py");

        public NewAccountPage()
        {
            InitializeComponent();

            sts = new SendToServer(this);

            Title = "New Account";
        }

        // this method currently in testing
        public async void CreateAccount(object sender, EventArgs e)
		{
            if(Name_Entry.Text == null || Name_Entry.Text == "")
            { //if entry box was not touched (null) or is touched but empty ("")
                await DisplayAlert("Error: Name", "Please enter a name", "OK");
				return;
            }
            if(Email_Entry.Text == null || Email_Entry.Text == "")
            { //if entry box was not touched (null) or is touched but empty ("")
                await DisplayAlert("Error: Email", "Please enter an email", "OK");
				return;
            }
			if (Password_Entry.Text == null || Password_Entry.Text == "" || Password_Entry.Text != ReEnterPassword_Entry.Text)
			{ //if the entry box was not touched (null), is touched but empty (""), or both password entries do not match
                await DisplayAlert("Error: Password", "Please make sure both password fields match and are not empty", "OK");
				return;
			}

            //create item we want to send
			var item = new AddUserItem();
            item.Name = Name_Entry.Text;
            item.Email = Email_Entry.Text;
            item.Password = Password_Entry.Text;

            sts.send(uri, item, () => 
            {
                testLabel.Text = sts.responseItem.Success.ToString();
                //todo:if no errors, display confirmation
                //remove this page and the login page from the navigation stack,
                //  the back button should not go back and there should be a
                //  dedicated log off button
                //login the user as well
            });
        }
    }
}