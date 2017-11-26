using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace cs441_project
{
    public partial class HomePage : ContentPage
    {
        private SendToServer sts;
        private Uri uri = new Uri("http://54.193.30.236/index.py");
        private ObservableCollection<ClassroomInfoItem> _ClassroomListViewItems = new ObservableCollection<ClassroomInfoItem>();

        public HomePage()
        {
            InitializeComponent();
            ClassroomListView.ItemsSource = _ClassroomListViewItems;

            sts = new SendToServer(this);

            Title = "My Classrooms";

            ToolbarItems.Add(new ToolbarItem("Join", "", ToolbarItem_OnJoin, ToolbarItemOrder.Primary));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Handle_Refreshing(null, null);
        }

        void Handle_Refreshing(object sender, System.EventArgs e)
        {
            _ClassroomListViewItems.Clear();
            GetMyClassrooms();
            testLabel.Text = "Refreshed";
            ClassroomListView.IsRefreshing = false;
        }

        async void ClassroomListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            App.curDatabaseId = ((ClassroomInfoItem)e.Item).Id.ToString().PadLeft(10, '0');
            await Navigation.PushAsync(new ClassroomPage()); //goto classroom page
            ((ListView)sender).SelectedItem = null;
        }

        async void ToolbarItem_OnJoin()
        {
            /*var item        = new JoinClassroomItem();
            item.DatabaseId = "na";
            item.Email      = App.userEmail;
            item.Password   = App.userPassword;

            sts.send(uri, item, () => {
                Handle_Refreshing(null, null);
                testLabel.Text = "Joined";
            });*/
            await Navigation.PushAsync(new JoinClassroomPage());
        }

        public void GetMyClassrooms()
        {
            //create the item we want to send
            var item      = new GetUserClassroomsItem();
            item.Email    = App.userEmail;
            item.Password = App.userPassword;

            sts.send(uri, item, async () => 
            {
                try
                {
                    if (sts.responseItem.Data != "")
                    {
                        var classroomList = JsonConvert.DeserializeObject<List<ClassroomInfoItem>>(sts.responseItem.Data);
                        for (int i = 0; i < classroomList.Count; i++)
                        {
                            _ClassroomListViewItems.Add(classroomList[i]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Unexpected Parsing Error", ex.Message, "OK");
                }
            });
        }

        public async void CreateClassroomButton_OnClick(object sender, EventArgs e)
        {
            //create the item we want to send
            var item         = new CreateClassroomItem();
            item.Email       = App.userEmail;
            item.Password    = App.userPassword;
            item.Title       = "Title";
            item.Description = "Description";

            if (item.Title == null)
            {
                await DisplayAlert("Error", "Cannot create a classroom without a title", "OK");
                return;
            }

            sts.send(uri, item, () => 
            {
                testLabel.Text = sts.responseItem.Success.ToString();
                Handle_Refreshing(null, null);
            });
        }

        async void ClassroomItemCell_OnLeave(object sender, System.EventArgs e)
        {
            var classroom = (ClassroomInfoItem)((MenuItem)sender).BindingContext;
            bool leaveResponse = false;
            bool isOwner = false;
            if (classroom.OwnerEmail == App.userEmail)
            {
                isOwner = true;
                leaveResponse = await DisplayAlert("Delete Classroom", "Are you sure you want to delete the classroom '" + classroom.Title + "' and all of its data forever?", "Yes", "No");
            }
            else
            {
                isOwner = false;
                leaveResponse = await DisplayAlert("Leave Classroom", "Are you sure you want to leave the classroom '" + classroom.Title + "'?", "Yes", "No");
            }
            if (leaveResponse == false)
                return;

            //create the item we want to send
            object item;
            if (isOwner)
            {
                item = new DeleteClassroomItem();
                ((DeleteClassroomItem)item).Email      = App.userEmail;
                ((DeleteClassroomItem)item).Password   = App.userPassword;
                ((DeleteClassroomItem)item).DatabaseId = classroom.Id;
            }
            else
            {
                item = new LeaveClassroomItem();
                ((LeaveClassroomItem)item).Email      = App.userEmail;
                ((LeaveClassroomItem)item).Password   = App.userPassword;
                ((LeaveClassroomItem)item).DatabaseId = classroom.Id;
            }

            sts.send(uri, item, () =>
            {
                Handle_Refreshing(null, null);
            });
        }
    }
}
