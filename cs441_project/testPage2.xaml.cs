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
        private TabbedPage _ContainerPage;
        private bool isOwner = false;

        public testPage2(TabbedPage containerPage)
        {
            InitializeComponent();

            BindingContext = this;

            sts = new SendToServer(this);

            _ContainerPage = containerPage;

            if (App.curClassroom.OwnerEmail.ToLower() == App.userEmail.ToLower())
                isOwner = true;

            TodoListView.ItemsSource = _TodoListViewItems;

            if (isOwner)
                ToolbarItems.Add(new ToolbarItem("New", "Add_Icon.png", ToolbarItem_OnAdd, ToolbarItemOrder.Primary));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _ContainerPage.Title = "Todo";

            Handle_Refreshing(null, null);
        }

        public async void ToolbarItem_OnAdd()
        {
            await Navigation.PushAsync(new TodoDetailsView(null), true);

            Handle_Refreshing(null, null);
        }

        public async void OnSelect(object sender, ItemTappedEventArgs e)
        {
            if (!isOwner)
            {
                ((ListView)sender).SelectedItem = null;
                return;
            }
            var model = (TodoItem)e.Item;
            await Navigation.PushAsync(new TodoDetailsView(model), true);
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
            item.DatabaseId = App.curClassroom.Id;

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
    }
}
