using Xamarin.Forms;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System;

namespace cs441_project
{
    public partial class cs441_projectPage : ContentPage
    {
		public cs441_projectPage()
        {
			//set the back button's title on the next page
			//only works in constructor, which is unfortunate
			//NavigationPage.SetBackButtonTitle(this, "Sign Out");

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

            //create the item we want to send
            var item = new ValidateUserItem();
			item.Email = Email_Entry.Text;
			item.Password = Password_Entry.Text;

            //set ip address to connect to
            var uri = new Uri("http://54.193.30.236/index.py");

            //serialize object and make it ready for sending over the internet
            var json = JsonConvert.SerializeObject(item);
            var content = new StringContent(json, Encoding.UTF8, "application/json"); //StringContent contains http headers

            //wait for response, then handle it
			var response = await App.client.PostAsync(uri, content); //post
            if (response.IsSuccessStatusCode)
            { //success
                //get our JSON response and convert it to a ResponseItem object
                ResponseItem resItem = new ResponseItem();
                try
                {
                    resItem = JsonConvert.DeserializeObject<ResponseItem>(await response.Content.ReadAsStringAsync());
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Unexpected Error", ex.Message, "OK");
                }

                //if no errors, do something
                if (resItem.Success)
                {
                    testLabel.Text = resItem.Success.ToString();
                    //todo:remove this page from navigation stack,
                    //   back button should not work because
                    //   there should be a dedicated log off button

                    //login was successful, so store the successful login info for future use. (these variables are global to the app)
                    App.userEmail = item.Email;
                    App.userPassword = item.Password;

                    await Navigation.PushAsync(new testPage2());//HomePage()); //goto home page
                }
                else //else, display error
                {
                    await DisplayAlert("Error", resItem.Response, "OK");
                }
            }
            else
            { //error
                await DisplayAlert("Unexpected Error", response.ToString(), "OK");
                return;
            }
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

            //set ip address to connect to
            var uri = new Uri("http://54.193.30.236/index.py");

            //serialize object and make it ready for sending over the internet
			var json = JsonConvert.SerializeObject(item);
			var content = new StringContent(json, Encoding.UTF8, "application/json"); //StringContent contains http headers

            //wait for response, then handle it
			var response = await App.client.PostAsync(uri, content); //post
			if (response.IsSuccessStatusCode)
			{ //success
                //get our JSON response and convert it to a ResponseItem object
                ResponseItem resItem = new ResponseItem();
                try
                {
                    resItem = JsonConvert.DeserializeObject<ResponseItem>(await response.Content.ReadAsStringAsync());
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Unexpected Error", ex.Message, "OK");
                }

                //if no errors, do something
                if (resItem.Success)
                {
                    testLabel.Text = resItem.Data;
                    await DisplayAlert("Email Sent", resItem.Response, "OK");
                }
                else //else, display error
                {
                    await DisplayAlert("Error", resItem.Response, "OK");
                }
			}
			else
			{ //error
                await DisplayAlert("Unexpected Error", response.ToString(), "OK");
                return;
			}
		}
    }
}
