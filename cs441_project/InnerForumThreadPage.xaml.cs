using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace cs441_project
{
    public partial class InnerForumThreadPage : ContentPage
    {
        private SendToServer sts;
        private Uri uri = new Uri("http://54.193.30.236/index.py");
        private ObservableCollection<ForumPostItem> _ForumPostsListViewItems = new ObservableCollection<ForumPostItem>();
        private ForumThreadItem _forumThread;

        public InnerForumThreadPage(ForumThreadItem forumThread)
        {
            InitializeComponent();

            sts = new SendToServer(this);

            if (forumThread == null) //should not be null
                Navigation.PopAsync();

            _forumThread = forumThread;

            Title = "Thread Posts";

            ForumPostsListView.ItemsSource = _ForumPostsListViewItems;

            ToolbarItems.Add(new ToolbarItem("New", "Add_Icon.png", ToolbarItem_OnAdd, ToolbarItemOrder.Primary));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Handle_Refreshing(null, null);
        }

        public async void ToolbarItem_OnAdd()
        {
            await Navigation.PushAsync(new ForumPostDetailsPage(_forumThread, null), true);

            Handle_Refreshing(null, null);
        }

        public async void OnTapped(object sender, ItemTappedEventArgs e)
        {
            var model = (ForumPostItem)e.Item;
            if (App.userEmail.ToLower() == model.CreatorEmail.ToLower() || App.curClassroom.curUserIsOwner)
            {
                await Navigation.PushAsync(new ForumPostDetailsPage(_forumThread, model));
            }
            ((ListView)sender).SelectedItem = null;
        }

        public void Handle_Refreshing(object sender, EventArgs e)
        {
            _ForumPostsListViewItems.Clear();
            GetForumPostItems();
            ForumPostsListView.IsRefreshing = false;
        }

        public void GetForumPostItems()
        {
            var item        = new GetForumPostsItem();
            item.ThreadId   = _forumThread.Id;
            item.Email      = App.userEmail;
            item.Password   = App.userPassword;
            item.DatabaseId = App.curClassroom.Id;

            sts.send(uri, item, async () =>
            {
                try
                {
                    if (sts.responseItem.Data != "")
                    {
                        var ForumPostsItemList = JsonConvert.DeserializeObject<List<ForumPostItem>>(sts.responseItem.Data);

                        for (int i = 0; i < ForumPostsItemList.Count; i++)
                        {
                            if (i == 0)
                                ForumPostsItemList[i].isFirstPost = true;
                            _ForumPostsListViewItems.Add(ForumPostsItemList[i]);
                        }
                        ForumPostsListView.ScrollTo(ForumPostsItemList[_ForumPostsListViewItems.Count - 1], ScrollToPosition.MakeVisible, true);
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
