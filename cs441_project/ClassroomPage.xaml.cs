using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace cs441_project
{
    public partial class ClassroomPage : TabbedPage
    {
        public ClassroomPage()
        {
            NavigationPage.SetHasNavigationBar(this, true);
            InitializeComponent();

            Children.Add(new testPage1(this));
            Children.Add(new testPage2(this));
            Children.Add(new testPage3(this));
            Children.Add(new UsersPage(this));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
