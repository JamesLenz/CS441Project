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
	public partial class TodoDetailsView : ContentPage
	{
        private SendToServer sts;
        private Uri uri = new Uri("http://54.193.30.236/index.py");
        private TodoItem _model;
        private bool isNew = true;

		public TodoDetailsView (TodoItem model)
        {
            InitializeComponent();

            sts = new SendToServer(this);

            Title = "New Todo Item";

            if (model != null)
            {
                isNew = false;
                _model = new TodoItem(model);
                Description_Editor.Text    = _model.Description;
                Title_Entry.Text           = _model.Title;
                DueDate_DatePicker.Date    = _model.DueDateTime.Date;
                DueDate_TimePicker.Time    = _model.DueDateTime.TimeOfDay;
                HasDueDateSwitch.IsToggled = _model.HasDueDate;

                ToolbarItems.Add(new ToolbarItem("Delete", "Trashcan_Icon", ToolbarItem_OnDelete, ToolbarItemOrder.Primary));
            }
            else
            {
                isNew = true;
                DueDate_DatePicker.Date = DateTime.Now.Date.AddDays(1);
                //DueDate_TimePicker.Time = DateTime.Now.TimeOfDay;
                DueDate_TimePicker.Time = new TimeSpan(0,0,0); //12:00AM next day
            }
                
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();

            TitleEntry_TextChanged(null, null);
            DescriptionEditor_TextChanged(null, null);
        }

        public async void Button_OnSave(object sender, EventArgs e)
        {
            if (App.curClassroom.Id == null || App.userEmail == null || App.userPassword == null)
            {
                //internal error, these should not be null at this point
                await DisplayAlert("Error: Internal Error", "An unexpected error has occurred. Try logging out and back in.", "OK");
                return;
            }

            if (Title_Entry.Text == null || Title_Entry.Text == "")
            {
                await DisplayAlert("Error: Title", "Please enter a title", "OK");
                return;
            }

            object item;
            if (isNew)
            {
                var temp_item             = new CreateTodoItem();
                temp_item.Title           = Title_Entry.Text;
                temp_item.Description     = Description_Editor.Text;
                if (HasDueDateSwitch.IsToggled)
                    temp_item.DueDateTime = DueDate_DatePicker.Date.Add(DueDate_TimePicker.Time);
                else
                    temp_item.DueDateTime = DateTime.MinValue; //1/1/0001 12:00:00AM
                temp_item.DatabaseId      = App.curClassroom.Id;
                temp_item.Email           = App.userEmail;
                temp_item.Password        = App.userPassword;

                if (temp_item.DueDateTime < DateTime.Now && HasDueDateSwitch.IsToggled)
                {
                    await DisplayAlert("Error: Due Date", "The due date cannot be in the past", "OK");
                    return;
                }
                item = temp_item;
            }
            else
            {
                var temp_item             = new EditTodoItem();
                temp_item.Title           = Title_Entry.Text;
                temp_item.Description     = Description_Editor.Text;
                if (HasDueDateSwitch.IsToggled)
                    temp_item.DueDateTime = DueDate_DatePicker.Date.Add(DueDate_TimePicker.Time);
                else
                    temp_item.DueDateTime = DateTime.MinValue; //1/1/0001 12:00:00AM
                temp_item.DatabaseId      = App.curClassroom.Id;
                temp_item.Email           = App.userEmail;
                temp_item.Password        = App.userPassword;
                temp_item.TodoItemId      = _model.Id;

                if (temp_item.DueDateTime < DateTime.Now && HasDueDateSwitch.IsToggled)
                {
                    await DisplayAlert("Error: Due Date", "The due date cannot be in the past", "OK");
                    return;
                }
                item = temp_item;
            }

            sts.send(uri, item, () =>
            {
                Navigation.PopAsync();
            });
        }

        public async void ToolbarItem_OnDelete()
        {
            bool answer = await DisplayAlert("Delete Todo item?", "Are you sure you want to permanently delete this todo item?", "Yes", "No");
            if (!answer)
                return;

            var item        = new DeleteTodoItem();
            item.Email      = App.userEmail;
            item.Password   = App.userPassword;
            item.DatabaseId = App.curClassroom.Id;
            item.TodoItemId = _model.Id;

            sts.send(uri, item, () => 
            {
                
            });
            await Navigation.PopAsync();
        }

        void HasDueDateSwitch_OnToggle(object sender, ToggledEventArgs e)
        {
            if (HasDueDateSwitch.IsToggled)
            {
                //DueDate_DatePicker.IsVisible = true;
                //DueDate_TimePicker.IsVisible = true;
                DueDate_TimePicker.IsEnabled = true;
                DueDate_DatePicker.IsEnabled = true;
                DueDate_TimePicker.FadeTo(1.0, 250);
                DueDate_DatePicker.FadeTo(1.0, 250);
                //DueDate_TimePicker.BackgroundColor = new Color(1, 1, 1);
                //DueDate_DatePicker.BackgroundColor = new Color(1, 1, 1);
            }
            else
            {
                //DueDate_DatePicker.IsVisible = false;
                //DueDate_TimePicker.IsVisible = false;
                DueDate_TimePicker.IsEnabled = false;
                DueDate_DatePicker.IsEnabled = false;
                DueDate_TimePicker.FadeTo(0.6, 250);
                DueDate_DatePicker.FadeTo(0.6, 250);
                //DueDate_TimePicker.BackgroundColor = new Color(0.90, 0.90, 0.90);
                //DueDate_DatePicker.BackgroundColor = new Color(0.90, 0.90, 0.90);
            }
        }

        void TitleEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Title_Entry.Text == null)
            {
                TitleCounter_Label.Text = "0/100";
                return;
            }

            if (Title_Entry.Text.Length > 100)
            {
                Title_Entry.Text = Title_Entry.Text.Substring(0, 100);
            }
            TitleCounter_Label.Text = Title_Entry.Text.Length.ToString() + "/100";
        }

        void DescriptionEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Description_Editor.Text == null)
            {
                DescriptionCounter_Label.Text = "0/1000";
                return;
            }

            if (Description_Editor.Text.Length > 1000)
            {
                Description_Editor.Text = Description_Editor.Text.Substring(0, 1000);
            }
            DescriptionCounter_Label.Text = Description_Editor.Text.Length.ToString() + "/1000";
        }
    }
}