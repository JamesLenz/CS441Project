using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cs441_project
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TodoDetailsView: ContentPage
	{
        private SendToServer sts;
        private Uri uri = new Uri("http://54.193.30.236/index.py");
        private TodoItem _model;

		public TodoDetailsView (TodoItem model, bool isNew)
        {
            InitializeComponent();

            _model = model;
            Description_Editor.Text = _model.Description;
            Title_Entry.Text = _model.Title;
            DueDate_DatePicker.Date = _model.DueDateTime.Date;
            DueDate_TimePicker.Time = _model.DueDateTime.TimeOfDay;

            sts = new SendToServer(this);

            if (!isNew)
                DeleteButton.IsVisible = true;
		}

        public void OnSave(object sender, EventArgs e)
        {
            var item = new CreateTodoItem();
            item.Title       = Title_Entry.Text;
            item.Description = Description_Editor.Text;
            if (HasDueDateSwitch.IsToggled)
                item.DueDateTime = DueDate_DatePicker.Date.Add(DueDate_TimePicker.Time);
            else
                item.DueDateTime = DateTime.MinValue; //1/1/0001 12:00:00AM
            item.DatabaseId  = App.curDatabaseId;
            item.Email       = App.userEmail;
            item.Password    = App.userPassword;

            //todo: error correction or missing information checks
            if (item.DatabaseId == null || item.Email == null || item.Password == null)
            {
                //internal error, these should not be null at this point
                //todo: display error or force a log out
            }

            if (item.Title == null || item.Title == "")
            {
                //error, title cannot be empty
                //todo: display error
            }

            sts.send(uri, item, () =>
            {
                testLabel.Text = sts.responseItem.ToString();
            });

            /* OBSOLETE CODE, USE SendToServer CLASS. CHECK ABOVE
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
                    testLabel.Text = resItem.ToString();
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
            }*/
        }

        public void OnCancel(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        public void OnDelete(object sender, EventArgs e)
        {
            
            Navigation.PopAsync();
        }

        void HasDueDateSwitch_OnToggle(object sender, ToggledEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}