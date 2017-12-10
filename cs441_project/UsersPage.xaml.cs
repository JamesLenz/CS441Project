using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace cs441_project
{
    public partial class UsersPage : ContentPage
    {
        private SendToServer sts;
        private Uri uri = new Uri("http://54.193.30.236/index.py");
        private TabbedPage _ContainerPage;
        private ObservableCollection<UserItem> _UsersListViewItems = new ObservableCollection<UserItem>();

        public UsersPage(TabbedPage containerPage)
        {
            InitializeComponent();

            sts = new SendToServer(this);

            _ContainerPage = containerPage;

            UsersListView.ItemsSource = _UsersListViewItems;

            if (App.userEmail.ToLower() == App.curClassroom.OwnerEmail.ToLower())
                ToolbarItems.Add(new ToolbarItem("Invite", "Invite_Icon.png", ToolbarItem_OnInvite, ToolbarItemOrder.Primary));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Handle_Refreshing(null, null);
            _ContainerPage.Title = "People";
        }

        public void UserItem_BindingContextChanged(object sender, EventArgs e)
        {
            if (((ViewCell)sender).BindingContext == null || ((UserItem)((ViewCell)sender).BindingContext).isOwner || App.userEmail.ToLower() != App.curClassroom.OwnerEmail.ToLower())
            {
                return;
            }

            var dropAction = new MenuItem { Text = "Drop", IsDestructive = true };
            dropAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
            dropAction.Clicked += (sender_, eventArgs_) => {
                var mi = ((MenuItem)sender_);

                UserItemCell_OnDrop(sender_, eventArgs_);
            };

            ((ViewCell)sender).ContextActions.Add(dropAction);
        }

        public async void ToolbarItem_OnInvite()
        {
            await Navigation.PushAsync(new InvitePage());
        }

        public void Handle_Refreshing(object sender, System.EventArgs e)
        {
            _UsersListViewItems.Clear();
            getUsers();
            UsersListView.IsRefreshing = false;
        }

        public void OnSelect(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        public async void UserItemCell_OnDrop(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Drop User", "Are you sure you want to drop this user from the classroom?", "Yes", "No");
            if (answer == false)
                return;

            var selectedUser = (UserItem)((MenuItem)sender).BindingContext;

            var item        = new DropUserItem();
            item.DatabaseId = App.curClassroom.Id;
            item.Email      = App.userEmail;
            item.Password   = App.userPassword;
            item.DropId     = selectedUser.Id;

            sts.send(uri, item, async () => 
            {
                await DisplayAlert("Success", "User successfully dropped from the classroom", "OK");
                Handle_Refreshing(null, null);
            });
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
                            if (App.curClassroom.OwnerEmail.ToLower() == userItemList[i].Email.ToLower())
                            {
                                userItemList[i].isOwner = true;
                                _UsersListViewItems.Insert(0, userItemList[i]);
                            }
                            else
                            {
                                _UsersListViewItems.Add(userItemList[i]);
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
