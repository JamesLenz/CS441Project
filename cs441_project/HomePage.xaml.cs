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
        private ObservableCollection<ClassroomInfoItem> _ClassroomListViewItems = new ObservableCollection<ClassroomInfoItem>();

        public HomePage()
        {
            InitializeComponent();
            ClassroomListView.ItemsSource = _ClassroomListViewItems;
        }

        void Handle_Refreshing(object sender, System.EventArgs e)
        {
            ClassroomListView.IsRefreshing = false;
            testLabel.Text = "Refreshed";
            _ClassroomListViewItems.Clear();
            GetMyClassrooms();
        }

        async void ClassroomListView_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            App.curDatabaseId = ((ClassroomInfoItem)e.Item).Id;
            await Navigation.PushAsync(new ClassroomPage()); //goto classroom page
            ((ListView)sender).SelectedItem = null;
        }

        public async void GetMyClassrooms()
        {
            //create the item we want to send
            var item = new GetUserClassroomsItem();
            item.Email = App.userEmail;
            item.Password = App.userPassword;

            //set ip address to connect to
            var uri = new Uri("http://54.193.30.236/index.py");

            //serialize object and make it ready for sending over the internet
            var json = JsonConvert.SerializeObject(item);
            var content = new StringContent(json, Encoding.UTF8, "application/json"); //StringContent contains http headers

            //wait for response, then handle it
            var response = await App.client.PostAsync(uri, content); //post
            if (response.IsSuccessStatusCode)
            { //success
                //get our JSON response and convert it to a ResponseItem object
                ResponseItem resItem = new ResponseItem();
                try
                {
                    resItem = JsonConvert.DeserializeObject<ResponseItem>(await response.Content.ReadAsStringAsync());
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Unexpected Error", ex.Message, "OK");
                }

                //if no errors, do something
                if (resItem.Success)
                {
                    try
                    {
                        if (resItem.Data != "")
                        {
                            var classroomList = JsonConvert.DeserializeObject<List<ClassroomInfoItem>>(resItem.Data);
                            //foreach (ClassroomInfoItem classroom in classroomList)
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
                }
                else //else, display error
                {
                    await DisplayAlert("Error", resItem.Response, "OK");
                }
            }
            else
            { //error
                await DisplayAlert("Unexpected Error", response.ToString(), "OK");
                return;
            }
        }

        public async void CreateClassroomButton_OnClick(object sender, EventArgs e)
        {
            //create the item we want to send
            var item = new CreateClassroomItem();
            item.Email = App.userEmail;
            item.Password = App.userPassword;
            item.Title = "Title";
            item.Description = "Description";

            if (item.Title == null)
            {
                await DisplayAlert("Error", "Cannot create a classroom without a title", "OK");
                return;
            }

            //set ip address to connect to
            var uri = new Uri("http://54.193.30.236/index.py");

            //serialize object and make it ready for sending over the internet
            var json = JsonConvert.SerializeObject(item);
            var content = new StringContent(json, Encoding.UTF8, "application/json"); //StringContent contains http headers

            //wait for response, then handle it
            var response = await App.client.PostAsync(uri, content); //post
            if (response.IsSuccessStatusCode)
            { //success
                //get our JSON response and convert it to a ResponseItem object
                ResponseItem resItem = new ResponseItem();
                try
                {
                    resItem = JsonConvert.DeserializeObject<ResponseItem>(await response.Content.ReadAsStringAsync());
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Unexpected Error", ex.Message, "OK");
                }

                //if no errors, do something
                if (resItem.Success)
                {
                    testLabel.Text = resItem.Success.ToString();
                    Handle_Refreshing(null, null);
                }
                else //else, display error
                {
                    await DisplayAlert("Error", resItem.Response, "OK");
                }
            }
            else
            { //error
                await DisplayAlert("Unexpected Error", response.ToString(), "OK");
                return;
            }
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
                ((LeaveClassroomItem)item).Email    = App.userEmail;
                ((LeaveClassroomItem)item).Password = App.userPassword;
                ((LeaveClassroomItem)item).DatabaseId = classroom.Id;
            }

            //set ip address to connect to
            var uri = new Uri("http://54.193.30.236/index.py");

            //serialize object and make it ready for sending over the internet
            var json = JsonConvert.SerializeObject(item);
            var content = new StringContent(json, Encoding.UTF8, "application/json"); //StringContent contains http headers

            //wait for response, then handle it
            var response = await App.client.PostAsync(uri, content); //post
            if (response.IsSuccessStatusCode)
            { //success
                //get our JSON response and convert it to a ResponseItem object
                ResponseItem resItem = new ResponseItem();
                try
                {
                    resItem = JsonConvert.DeserializeObject<ResponseItem>(await response.Content.ReadAsStringAsync());
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Unexpected Error", ex.Message, "OK");
                }

                //if no errors, do something
                if (resItem.Success)
                {
                    Handle_Refreshing(null, null);
                }
                else //else, display error
                {
                    await DisplayAlert("Error", resItem.Response, "OK");
                }
            }
            else
            { //error
                await DisplayAlert("Unexpected Error", response.ToString(), "OK");
                return;
            }
        }
    }
}
