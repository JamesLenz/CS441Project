using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Xamarin.Forms;

namespace cs441_project
{
    public partial class testPage2 : ContentPage
    {
        private ObservableCollection<TodoItem> _ListViewItems = new ObservableCollection<TodoItem>();

        public testPage2()
        {
            NavigationPage.SetHasNavigationBar(this, true);

            InitializeComponent();

            BindingContext = this;

            MainListView.ItemsSource = _ListViewItems;
        }
        async void TodoButton_OnClicked(object sender, System.EventArgs e)
        {
            //var answer = await DisplayAlert("New Item", "Create new task?", "Yes", "No");
            //if (answer == false)
            //    return;
            var model = new TodoItem();
            await Navigation.PushAsync(new TodoDetailsView(model, true), true);

            _ListViewItems.Add(model);
        }

        void OnSelect(object sender, SelectedItemChangedEventArgs e)
        {
            var model = (TodoItem)e.SelectedItem;
            ((ListView)sender).SelectedItem = null;
            //await Navigation.PushAsync(new TodoDetailsView(model, false), true);
        }

        void OnDoneToggled(object sender, ToggledEventArgs e)
        {
            var model = (TodoItem)((Switch)sender).BindingContext;
            model.Done = !model.Done;

            //await Navigation.PushAsync(new TodoDetailsView(model, false), true);
        }
    }
}
