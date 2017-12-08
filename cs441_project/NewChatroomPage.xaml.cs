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
    public partial class NewChatroomPage : ContentPage
    {
        private SendToServer sts;
        private Uri uri = new Uri("http://54.193.30.236/index.py");
        private ObservableCollection<UserItem> _UsersListViewItems = new ObservableCollection<UserItem>();
        private List<string> MemberIds;

        public NewChatroomPage()
        {
            InitializeComponent();

            sts = new SendToServer(this);

            Title = "New Chatroom";

            MemberIds = new List<string>();

            UsersListView.ItemsSource = _UsersListViewItems;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Handle_Refreshing(null, null);
        }

        public void OnSelect(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        public void Handle_Refreshing(object sender, System.EventArgs e)
        {
            _UsersListViewItems.Clear();
            getUsers();
            UsersListView.IsRefreshing = false;
        }

        public async void Button_OnCreate(object sender, EventArgs e)
        {
            if (App.userEmail == null || App.userPassword == null)
            {
                //internal error, these should not be null at this point
                await DisplayAlert("Error: Internal Error", "An unexpected error has occurred. Try logging out and back in.", "OK");
                return;
            }

            var item         = new CreateChatroomItem();
            item.MemberIds   = MemberIds;
            item.Email       = App.userEmail;
            item.Password    = App.userPassword;
            item.DatabaseId  = App.curClassroom.Id;

            sts.send(uri, item, async () =>
            {
                await DisplayAlert("Success", "Chatroom successfully created", "OK");
                await Navigation.PopAsync();
            });
        }

        public void UserItemCell_OnToggled(object sender, ToggledEventArgs e)
        {
            var userItem = (UserItem)((ViewCell)((Switch)sender).BindingContext).BindingContext;

            if (e.Value == true)
            {
                if (!MemberIds.Contains(userItem.Id))
                {
                    MemberIds.Add(userItem.Id);
                }
            }
            else
            {
                if (MemberIds.Contains(userItem.Id))
                {
                    MemberIds.Remove(userItem.Id);
                }
            }
        }

        public void getUsers()
        {
            //create the item we want to send
            var item        = new GetUsersItem();
            item.Email      = App.userEmail;
            item.Password   = App.userPassword;
            item.DatabaseId = App.curClassroom.Id;

            sts.send(uri, item, async () =>
            {
                try
                {
                    if (sts.responseItem.Data != "")
                    {
                        var userItemList = JsonConvert.DeserializeObject<List<UserItem>>(sts.responseItem.Data);
                        for (int i = 0; i < userItemList.Count; i++)
                        {
                            if (App.userEmail.ToLower() != userItemList[i].Email.ToLower())
                            {
                                _UsersListViewItems.Add(userItemList[i]);
                            }
                            else
                            {
                                if (!MemberIds.Contains(userItemList[i].Id))
                                {
                                    MemberIds.Add(userItemList[i].Id); //add yourself
                                }
                            }
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
