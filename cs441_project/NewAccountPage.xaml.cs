using System;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;
using System.Text;

namespace cs441_project
{
    //todo: should add to a file for http items
	public class AddUserItem // an item containing data relevant to a certain task
	{
        public readonly string Command = "ADD_USER"; //every item must have a "Command" data member
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
	}

    public partial class NewAccountPage : ContentPage
    {
        private HttpClient client; //todo: should move to global object accross all pages?

        public NewAccountPage()
        {
			client = new HttpClient();
			client.MaxResponseContentBufferSize = 256000; //256KB

            Title = "New Account";
            InitializeComponent();
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

            // RestUrl = http://developer.xamarin.com:8081/api/todoitems/
            var uri = new Uri("http://54.193.30.236/index.py");

			var item = new AddUserItem();
            item.Name = Name_Entry.Text;
            item.Email = Email_Entry.Text;
            item.Password = Password_Entry.Text;

            var json = JsonConvert.SerializeObject(item);
            var content = new StringContent(json, Encoding.UTF8, "application/json"); //StringContent contains http headers

            var response = await client.PostAsync(uri, content); //post
			if (response.IsSuccessStatusCode)
			{ //success
                //todo: success just means there was no connection problems, but
			    //we need to handle/display errors the server gives back to us
				
                var temp = await response.Content.ReadAsStringAsync();
                testLabel.Text = temp;

                //todo:if no errors, display confirmation
                    //remove this page and the login page from the navigation stack,
                    //  the back button should not go back and there should be a
                    //  dedicated log off button
                    //login the user as well
                //todo:else, display error
                    
            }
            else
            { //error
				await DisplayAlert("Unexpected Error", response.ToString(), "OK");
				return;
            }
        }
    }
}