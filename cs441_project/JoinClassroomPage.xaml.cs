using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace cs441_project
{
    public partial class JoinClassroomPage : ContentPage
    {
        private SendToServer sts;
        private Uri uri = new Uri("http://54.193.30.236/index.py");

        public JoinClassroomPage()
        {
            InitializeComponent();

            sts = new SendToServer(this);

            Title = "Join Classroom";
        }

        async void JoinButton_OnClicked(object sender, EventArgs e)
        {
            if (ClassroomId_Entry.Text == null || ClassroomId_Entry.Text == "")
            {
                await DisplayAlert("Error", "Please enter a classroom ID", "OK");
                return;
            }

            if (ClassroomId_Entry.Text.Length != 10)
            {
                await DisplayAlert("Error", "A classroom ID must contain exactly 10 digits", "OK");
                return;
            }

            foreach (char ch in ClassroomId_Entry.Text) {
                if (ch < '0' || ch > '9')
                {
                    await DisplayAlert("Error", "A classroom ID can only contain numbers", "OK");
                    return;
                }
            }

            var item        = new JoinClassroomItem();
            item.DatabaseId = ClassroomId_Entry.Text;
            item.Email      = App.userEmail;
            item.Password   = App.userPassword;

            sts.send(uri, item, async () => {
                await DisplayAlert("Success", "Classroom successfully joined", "OK");
                await Navigation.PopAsync();
            });
        }
    }
}
