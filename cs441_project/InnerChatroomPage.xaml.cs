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
    public partial class InnerChatroomPage : ContentPage
    {
        private SendToServer sts;
        private Uri uri = new Uri("http://54.193.30.236/index.py");
        private ObservableCollection<ChatroomMessageItem> _ChatroomMessagesListViewItems = new ObservableCollection<ChatroomMessageItem>();
        private ChatroomItem chatroom;

        public InnerChatroomPage(ChatroomItem chatroom)
        {
            InitializeComponent();

            sts = new SendToServer(this);

            this.chatroom = chatroom;

            Title = "Messages";

            ChatroomMessagesListView.ItemsSource = _ChatroomMessagesListViewItems;

            ChatroomMessagesListView.HeightRequest = App.Current.MainPage.Height - 200;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Handle_Refreshing(null, null);
        }

        public void OnTapped(object sender, ItemTappedEventArgs e)
        {
            var model = (ChatroomMessageItem)e.Item;
            ((ListView)sender).SelectedItem = null;
        }

        public void Handle_Refreshing(object sender, EventArgs e)
        {
            _ChatroomMessagesListViewItems.Clear();
            GetMyChatroomMessagesItems();
            ChatroomMessagesListView.IsRefreshing = false;
        }

        public void SendButton_OnClicked(object sender, EventArgs e)
        {
            if (Message_Editor.Text == null || Message_Editor.Text == "")
                return;

            var item        = new PostChatroomMessageItem();
            item.ChatroomId = chatroom.Id;
            item.Email      = App.userEmail;
            item.Password   = App.userPassword;
            item.DatabaseId = App.curClassroom.Id;
            item.Message    = Message_Editor.Text;

            sts.send(uri, item, () =>
            {
                Message_Editor.Text = "";
                Handle_Refreshing(null, null);
            });
        }

        public void GetMyChatroomMessagesItems()
        {
            var item        = new GetChatroomMessagesItem();
            item.ChatroomId = chatroom.Id;
            item.Email      = App.userEmail;
            item.Password   = App.userPassword;
            item.DatabaseId = App.curClassroom.Id;

            sts.send(uri, item, async () =>
            {
                try
                {
                    if (sts.responseItem.Data != "")
                    {
                        var chatroomMessagesItemList = JsonConvert.DeserializeObject<List<ChatroomMessageItem>>(sts.responseItem.Data);

                        for (int i = 0; i < chatroomMessagesItemList.Count; i++)
                        {
                            _ChatroomMessagesListViewItems.Add(chatroomMessagesItemList[i]);
                        }
                        ChatroomMessagesListView.ScrollTo(chatroomMessagesItemList[_ChatroomMessagesListViewItems.Count - 1], ScrollToPosition.MakeVisible, true);
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
