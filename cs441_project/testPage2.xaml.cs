using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace cs441_project
{
    public partial class testPage2 : ContentPage
    {
        private SendToServer sts;
        private Uri uri = new Uri("http://54.193.30.236/index.py");
        private ObservableCollection<TodoItem> _TodoListViewItems = new ObservableCollection<TodoItem>();

        public testPage2()
        {
            NavigationPage.SetHasNavigationBar(this, true);

            InitializeComponent();

            BindingContext = this;

            sts = new SendToServer(this);

            Title = "Todo";

            TodoListView.ItemsSource = _TodoListViewItems;

            ToolbarItems.Add(new ToolbarItem("Create", "Todo_Icon.png", () => {; }, ToolbarItemOrder.Primary));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Handle_Refreshing(null, null);
        }

        public async void NewTaskButton_OnClicked(object sender, EventArgs e)
        {
            var model = new TodoItem();
            await Navigation.PushAsync(new TodoDetailsView(model, true), true);

            Handle_Refreshing(null, null);
        }

        public async void OnSelect(object sender, SelectedItemChangedEventArgs e)
        {
            var model = (TodoItem)e.SelectedItem;
            await Navigation.PushAsync(new TodoDetailsView(model, false), true);
        }

        public void OnDoneToggled(object sender, ToggledEventArgs e)
        {
            try //this is needed to prevent to weird error with the Done switch
            {
                var model = (TodoItem)((Switch)sender).BindingContext;
                model.Done = !model.Done;
            }
            catch (Exception)
            {
                ;
            }
        }

        public void Handle_Refreshing(object sender, System.EventArgs e)
        {
            _TodoListViewItems.Clear();
            getTodoItems();
            TodoListView.IsRefreshing = false;
        }

        public void getTodoItems()
        {
            //create the item we want to send
            var item        = new GetTodoItems();
            item.Email      = App.userEmail;
            item.Password   = App.userPassword;
            item.DatabaseId = App.curDatabaseId;

            sts.send(uri, item, async () =>
            {
                try
                {
                    if (sts.responseItem.Data != "")
                    {
                        var todoItemList = JsonConvert.DeserializeObject<List<TodoItem>>(sts.responseItem.Data);
                        for (int i = 0; i < todoItemList.Count; i++)
                        {
                            if (todoItemList[i].DueDateTime == DateTime.MinValue)
                                todoItemList[i].HasDueDate = false;
                            else
                                todoItemList[i].HasDueDate = true;
                            _TodoListViewItems.Add(todoItemList[i]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Unexpected Parsing Error", ex.Message, "OK");
                }
            });
        }

        void OnCreate(object sender, EventArgs e)
        {
            testLabel.Text = "Created";
        }
    }
}
