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

            ToolbarItems.Add(new ToolbarItem("Join", "Enter_Icon", ToolbarItem_OnJoin, ToolbarItemOrder.Primary));
            ToolbarItems.Add(new ToolbarItem("Create", "Add_Icon", ToolbarItem_OnCreate, ToolbarItemOrder.Primary));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Title = "My Classrooms";

            Handle_Refreshing(null, null);
        }

        public void ClassroomItem_BindingContextChanged(object sender, EventArgs e)
        {
            if (((ViewCell)sender).BindingContext == null)
            {
                return;
            }

            bool isOwner = ((ClassroomInfoItem)((ViewCell)sender).BindingContext).OwnerEmail.ToLower() == App.userEmail.ToLower();
            var leaveAction = new MenuItem { Text = isOwner ? "Delete" : "Leave", IsDestructive = true };
            leaveAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
            leaveAction.Clicked += (sender_, eventArgs_) => {
                var mi = ((MenuItem)sender_);

                ClassroomItemCell_OnLeave(mi, eventArgs_);
            };

            ((ViewCell)sender).ContextActions.Add(leaveAction);

            if (isOwner)
            {
                var editAction = new MenuItem { Text = "Edit" };
                editAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
                editAction.Clicked += async (sender_, eventArgs_) =>
                {
                    var mi = ((MenuItem)sender_);

                    await Navigation.PushAsync(new ClassroomDetailsPage((ClassroomInfoItem)mi.BindingContext));
                };

                ((ViewCell)sender).ContextActions.Add(editAction);
            }
        }

        public void Handle_Refreshing(object sender, EventArgs e)
        {
            _ClassroomListViewItems.Clear();
            GetMyClassrooms();
            ClassroomListView.IsRefreshing = false;
        }

        public async void ClassroomListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var classroom = (ClassroomInfoItem)e.Item;
            App.curClassroom = new ClassroomInfoItem(classroom);
            await Navigation.PushAsync(new ClassroomPage()); //goto classroom page
            ((ListView)sender).Unfocus();
        }

        public async void ToolbarItem_OnJoin()
        {
            await Navigation.PushAsync(new JoinClassroomPage());
        }

        public async void ToolbarItem_OnCreate()
        {
            await Navigation.PushAsync(new ClassroomDetailsPage(null));
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

        public async void ClassroomItemCell_OnLeave(object sender, EventArgs e)
        {
            var classroom = (ClassroomInfoItem)((MenuItem)sender).BindingContext;
            bool leaveResponse = false;
            bool isOwner = false;
            
            if (classroom.OwnerEmail.ToLower() == App.userEmail.ToLower())
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
                item                                   = new DeleteClassroomItem();
                ((DeleteClassroomItem)item).Email      = App.userEmail;
                ((DeleteClassroomItem)item).Password   = App.userPassword;
                ((DeleteClassroomItem)item).DatabaseId = classroom.Id;
            }
            else
            {
                item                                  = new LeaveClassroomItem();
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
