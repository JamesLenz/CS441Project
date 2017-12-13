using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace cs441_project
{
    public partial class testPage1 : ContentPage
    {
        private SendToServer sts;
        private Uri uri = new Uri("http://54.193.30.236/index.py");
        private ObservableCollection<ForumThreadItem> _ForumListViewItems = new ObservableCollection<ForumThreadItem>();
        private TabbedPage _ContainerPage;
        private bool isOwner = false;

        public testPage1(TabbedPage containerPage)
        {
            InitializeComponent();

            sts = new SendToServer(this);

            _ContainerPage = containerPage;

            if (App.curClassroom.OwnerEmail == App.userEmail)
                isOwner = true;

            ForumListView.ItemsSource = _ForumListViewItems;

            ToolbarItems.Add(new ToolbarItem("New", "Add_Icon.png", ToolbarItem_OnAdd, ToolbarItemOrder.Primary));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _ContainerPage.Title = "Forums";

            Handle_Refreshing(null, null);
            //Handle_Refreshing(null, null);
        }

        public async void ToolbarItem_OnAdd()
        {
            await Navigation.PushAsync(new ForumPostDetailsPage(null, null), true);

            Handle_Refreshing(null, null);
        }

        public async void OnSelect(object sender, ItemTappedEventArgs e)
        {
            var model = (ForumThreadItem)e.Item;
            await Navigation.PushAsync(new InnerForumThreadPage(model));
            ((ListView)sender).SelectedItem = null;
        }

        public void Handle_Refreshing(object sender, System.EventArgs e)
        {
            _ForumListViewItems.Clear();
            getForumItems();
            ForumListView.IsRefreshing = false;
        }

        public void getForumItems()
        {
            //create the item we want to send
            var item        = new GetForumThreadsItem();
            item.Email      = App.userEmail;
            item.Password   = App.userPassword;
            item.DatabaseId = App.curClassroom.Id;

            sts.send(uri, item, async () =>
            {
                try
                {
                    if (sts.responseItem.Data != "")
                    {
                        var forumItemList = JsonConvert.DeserializeObject<List<ForumThreadItem>>(sts.responseItem.Data);
                        for (int i = 0; i < forumItemList.Count; i++)
                        {
                            if (!_ForumListViewItems.Contains(forumItemList[i]))
                                _ForumListViewItems.Add(forumItemList[i]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Unexpected Parsing Error", ex.Message, "OK");
                }
            });
        }

        public void ForumThreadItem_BindingContextChanged(object sender, EventArgs e)
        {
            if (((ViewCell)sender).BindingContext == null || App.userEmail.ToLower() != App.curClassroom.OwnerEmail.ToLower())
            {
                return;
            }

            var item = (ForumThreadItem)((ViewCell)sender).BindingContext;
            var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true };
            deleteAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
            deleteAction.Clicked += (sender_, eventArgs_) => {
                var mi = ((MenuItem)sender_);

                ForumThreadItemCell_OnDelete(sender_, eventArgs_);
            };

            ((ViewCell)sender).ContextActions.Add(deleteAction);
        }

        public async void ForumThreadItemCell_OnDelete(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Delete Thread", "Are you sure you want to delete this thread and all of its replies?", "Yes", "No");
            if (answer == false)
                return;

            var selectedThread = (ForumThreadItem)((MenuItem)sender).BindingContext;

            var item        = new DeleteForumThreadItem();
            item.DatabaseId = App.curClassroom.Id;
            item.Email      = App.userEmail;
            item.Password   = App.userPassword;
            item.ThreadId   = selectedThread.Id;

            sts.send(uri, item, async () =>
            {
                await DisplayAlert("Success", "The thread has been successfully deleted", "OK");
                Handle_Refreshing(null, null);
            });
        }
    }
}

