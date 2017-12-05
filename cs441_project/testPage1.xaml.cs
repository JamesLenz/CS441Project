using System;
using System.Collections.Generic;



using Xamarin.Forms;

namespace cs441_project
{
    public partial class testPage1 : ContentPage
    {
        private TabbedPage _ContainerPage;

        public testPage1(TabbedPage containerPage)
        {
            InitializeComponent();

            _ContainerPage = containerPage;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _ContainerPage.Title = "Forum";
        }
    }
}
