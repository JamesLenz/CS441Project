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

            _model = new TodoItem(model);
            Description_Editor.Text = _model.Description;
            Title_Entry.Text        = _model.Title;
            DueDate_DatePicker.Date = _model.DueDateTime.Date;
            DueDate_TimePicker.Time = _model.DueDateTime.TimeOfDay;

            sts = new SendToServer(this);

            Title = "New Todo Item";

            if (!isNew)
                DeleteButton.IsVisible = true;
		}

        public async void OnSave(object sender, EventArgs e)
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

            if (item.DatabaseId == null || item.Email == null || item.Password == null)
            {
                //internal error, these should not be null at this point
                await DisplayAlert("Error: Internal Error", "An unexpected error has occurred. Try logging out and back in.", "OK");
                return;
            }

            if (item.Title == null || item.Title == "")
            {
                await DisplayAlert("Error: Title", "Please enter a title", "OK");
                return;
            }

            if (item.DueDateTime < DateTime.Now && HasDueDateSwitch.IsToggled)
            {
                await DisplayAlert("Error: Due Date","The due date cannot be in the past","OK");
                return;
            }

            sts.send(uri, item, () =>
            {
                testLabel.Text = sts.responseItem.Response;
                Navigation.PopAsync();
            });
        }

        public void OnCancel(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        public void OnDelete(object sender, EventArgs e)
        {
            //todo: confirm that user wants to delete it

            var item        = new DeleteTodoItem();
            item.Email      = App.userEmail;
            item.Password   = App.userPassword;
            item.DatabaseId = App.curDatabaseId;
            item.TodoItemId = _model.Id;

            sts.send(uri, item, () => 
            {
                
            });
            Navigation.PopAsync();
        }

        void HasDueDateSwitch_OnToggle(object sender, ToggledEventArgs e)
        {
            if (HasDueDateSwitch.IsEnabled)
            {
                DueDate_DatePicker.IsEnabled = true;
                DueDate_TimePicker.IsEnabled = true;
            }
            else
            {
                DueDate_DatePicker.IsEnabled = false;
                DueDate_TimePicker.IsEnabled = false;
            }
        }
    }
}