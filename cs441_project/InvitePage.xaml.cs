using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace cs441_project
{
    public partial class InvitePage : ContentPage
    {
        private SendToServer sts;
        private Uri uri = new Uri("http://54.193.30.236/index.py");

        public InvitePage()
        {
            InitializeComponent();

            sts = new SendToServer(this);

            Title = "Invite";
        }

        async void InviteButton_OnClicked(object sender, EventArgs e)
        {
            if (Email_Entry.Text == null || Email_Entry.Text == "")
            {
                await DisplayAlert("Error", "Please enter an email", "OK");
                return;
            }

            var item         = new InviteUserItem();
            item.DatabaseId  = App.curClassroom.Id;
            item.Email       = App.userEmail;
            item.Password    = App.userPassword;
            item.InviteEmail = Email_Entry.Text;

            sts.send(uri, item, async () => {
                await DisplayAlert("Success", "Invite successfully sent", "OK");
                //await Navigation.PopAsync();
            });
        }
    }
}
