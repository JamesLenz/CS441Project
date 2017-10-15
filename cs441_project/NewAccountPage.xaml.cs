using System;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;
using System.Text;

namespace cs441_project
{
    public partial class NewAccountPage : ContentPage
    {
        public NewAccountPage()
        {
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

            //create item we want to send
			var item = new AddUserItem();
            item.Name = Name_Entry.Text;
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
                    await DisplayAlert("Unexpected Error", ex.Message + "...\n..." + response.ToString(), "OK");
                }

                //if no errors, do something
                if (resItem.Success)
                {
                    testLabel.Text = resItem.Success.ToString();
                    //todo:if no errors, display confirmation
                    //remove this page and the login page from the navigation stack,
                    //  the back button should not go back and there should be a
                    //  dedicated log off button
                    //login the user as well
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