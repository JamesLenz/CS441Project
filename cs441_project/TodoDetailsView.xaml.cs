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
        private TodoItem _model;
		public TodoDetailsView (TodoItem model, bool isNew)
		{
            ToolbarItem test = new ToolbarItem();

            InitializeComponent();

            _model = model;
            _model.Title = "This is a sample title";
            _model.CreatedDateTime = DateTime.Now;
            _model.DueDateTime = DateTime.Now.AddDays(7);
            _model.Done = false;

            Random rand = new Random();
            switch(rand.Next(4))
            {
                case 0:
                    _model.Title = "This todo item seems to also have a very long title in it";
                    _model.Description = "This is a long sample description to see how well the text looks and if it wraps correctly. This is another sentence that I am writing to further test the limitations of this todo item. As you can see, it is very long.";
                    break;
                case 1:
                    _model.Description = "This is a short sample description.";
                    break;
                case 2:
                    _model.Description = "This is a medium sample description to see how well the text looks and if it wraps correctly. ";
                    break;
                case 3:
                    _model.Description = "";
                    break;
            }


            if (!isNew)
                DeleteButton.IsVisible = true;
		}

        public async void OnSave(object sender, EventArgs e)
        {
            var item = new AddTodoItem();
            item.Title       = "Title";
            item.Description = "Description";
            item.DueDateTime = DateTime.Now.AddDays(7);
            item.DatabaseId  = App.curDatabaseId;
            item.Email       = App.userEmail;
            item.Password    = App.userPassword;

            //todo: error correction or missing information checks

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
            }
        }

        public void OnCancel(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        public void OnDelete(object sender, EventArgs e)
        {
            
            Navigation.PopAsync();
        }
	}
}