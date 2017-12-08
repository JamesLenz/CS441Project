using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

using System.Diagnostics;

namespace cs441_project
{
    public partial class testPage3 : ContentPage
    {
        private SendToServer sts;
        private Uri uri = new Uri("http://54.193.30.236/index.py");
        private ObservableCollection<ChatroomItem> _ChatroomListViewItems = new ObservableCollection<ChatroomItem>();
        private TabbedPage _ContainerPage;

        public testPage3(TabbedPage containerPage)
        {
            InitializeComponent();

            sts = new SendToServer(this);
            _ContainerPage = containerPage;

            ChatroomListView.ItemsSource = _ChatroomListViewItems;

            ToolbarItems.Add(new ToolbarItem("New Chatroom", "Add_Icon.png", ToolbarItem_OnNew, ToolbarItemOrder.Primary));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _ContainerPage.Title = "Chatrooms";

            Handle_Refreshing(null, null);
        }

        public async void ToolbarItem_OnNew()
        {
            await Navigation.PushAsync(new NewChatroomPage(), true);
        }

        public async void OnTapped(object sender, ItemTappedEventArgs e)
        {
            var model = (ChatroomItem)e.Item;
            await Navigation.PushAsync(new InnerChatroomPage(model), true);
            ((ListView)sender).SelectedItem = null;
        }

        public void Handle_Refreshing(object sender, EventArgs e)
        {
            _ChatroomListViewItems.Clear();
            GetMyChatroomItems();
            ChatroomListView.IsRefreshing = false;
        }

        void ChatroomItemCell_OnLeave(object sender, EventArgs e)
        {
            Debug.WriteLine(((MenuItem)sender).BindingContext.ToString());
            var item        = new LeaveChatroomItem();
            item.ChatroomId = ((ChatroomItem)((MenuItem)sender).BindingContext).Id;
            item.Email      = App.userEmail;
            item.Password   = App.userPassword;
            item.DatabaseId = App.curClassroom.Id;

            sts.send(uri, item, () =>
            {
                Handle_Refreshing(null, null);
            });
        }

        public void GetMyChatroomItems()
        {
            var item        = new GetUserChatroomsItem();
            item.Email      = App.userEmail;
            item.Password   = App.userPassword;
            item.DatabaseId = App.curClassroom.Id;

            sts.send(uri, item, async () =>
            {
                try
                {
                    if (sts.responseItem.Data != "")
                    {
                        var chatroomItemList = JsonConvert.DeserializeObject<List<ChatroomItem>>(sts.responseItem.Data);

                        for (int i = 0; i < chatroomItemList.Count; i++)
                        {
                            _ChatroomListViewItems.Add(chatroomItemList[i]);
                            //for (int j = 0; j < chatroomItemList[i].Members.Count; j++)
                                //Debug.WriteLine(chatroomItemList[i].Members[j].Name);
                            //Debug.WriteLine("");
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
