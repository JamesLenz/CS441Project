using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace cs441_project
{
    public partial class testPage2 : ContentPage
    {
        public testPage2()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, true);
            BindingContext = this;
        }
        async void TodoButton_OnClicked(object sender, System.EventArgs e)
        {
            var answer = await DisplayAlert("New Item", "Create new task?", "Yes", "No");
            if (answer == false)
                return;
            var model = new TodoItem();
            var view = new TodoDetailsView(model);
            await Navigation.PushAsync(view, true);
        }

        async void OnSelect( object sender, SelectedItemChangedEventArgs e)
        {
            var model = (TodoItem)e.SelectedItem;
            var view = new TodoDetailsView(model);
            await Navigation.PushAsync(view, true);
        }
    }
}
