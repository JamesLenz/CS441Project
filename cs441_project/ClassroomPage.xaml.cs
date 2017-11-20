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
        }
    }
}
