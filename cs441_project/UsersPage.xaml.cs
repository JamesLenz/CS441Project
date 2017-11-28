using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace cs441_project
{
    public partial class UsersPage : ContentPage
    {
        private SendToServer sts;
        private Uri uri = new Uri("http://54.193.30.236/index.py");
        private TabbedPage _ContainerPage;

        public UsersPage(TabbedPage containerPage)
        {
            InitializeComponent();

            sts = new SendToServer(this);

            _ContainerPage = containerPage;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _ContainerPage.Title = "People";
        }
    }
}
