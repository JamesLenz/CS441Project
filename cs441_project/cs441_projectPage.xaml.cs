using Xamarin.Forms;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System;

namespace cs441_project
{
    //todo: should add to a file for http items
	public class ValiddateUserItem // an item containing data relevant to a certain task
	{
		public readonly string Command = "VALIDATE_USER"; //every item must have a "Command" data member
		public string Email { get; set; }
		public string Password { get; set; }
	}

	public class ForgotPasswordItem
	{
		public readonly string Command = "FORGOT_PASSWORD";
		public string Email { get; set; }
	}

    public partial class cs441_projectPage : ContentPage
    {
        private HttpClient client; //todo: should move to global object accross all pages?

		public cs441_projectPage()
        {
			//set the back button's title on the next page
			//only works in constructor, which is unfortunate
			//NavigationPage.SetBackButtonTitle(this, "Sign Out");

			client = new HttpClient();
			client.MaxResponseContentBufferSize = 256000; //256KB

			NavigationPage.SetHasNavigationBar(this, false); //remove navigation bar for sign in page
            InitializeComponent();
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

			// RestUrl = http://developer.xamarin.com:8081/api/todoitems/
			var uri = new Uri("http://54.193.30.236/index.py");

            var item = new ValiddateUserItem();
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

                //todo:if no errors, then go to next page
                    //remove this page from navigation stack,
                    //   back button should not work because
                    //   there should be a dedicated log off button
                    //Navigation.PushAsync(new HomePage()); //goto home page
                //todo:else, display error
                    
			}
			else
			{ //error
				testLabel.Text = response.ToString();
			}
        }

		void NewAccount_Clicked(object sender, System.EventArgs e)
		{
            Navigation.PushAsync(new NewAccountPage()); //goto the new account creation page
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

            if (answer == false)
                return;
            
			//send request for "forgot password" to server
			//server will send back a response if that email exists
			//if the email does exist, the server will send the email

			// RestUrl = http://developer.xamarin.com:8081/api/todoitems/
			var uri = new Uri("http://54.193.30.236/index.py");

			var item = new ForgotPasswordItem();
			item.Email = Email_Entry.Text;
            
			var json = JsonConvert.SerializeObject(item);
			var content = new StringContent(json, Encoding.UTF8, "application/json"); //StringContent contains http headers

			var response = await client.PostAsync(uri, content); //post
			if (response.IsSuccessStatusCode)
			{ //success
			  //todo: success just means there was no connection problems, but
			  //we need to handle/display errors the server gives back to us

				var temp = await response.Content.ReadAsStringAsync();
                testLabel.Text = temp;

                //todo:if no errors, then display confirmation
                    
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
