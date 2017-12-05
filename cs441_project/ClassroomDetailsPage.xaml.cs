using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace cs441_project
{
    public partial class ClassroomDetailsPage : ContentPage
    {
        private SendToServer sts;
        private Uri uri = new Uri("http://54.193.30.236/index.py");
        private ClassroomInfoItem classroom;
        private bool isNew = true;

        public ClassroomDetailsPage(ClassroomInfoItem classroom)
        {
            InitializeComponent();

            sts = new SendToServer(this);

            Title = "Classroom Details";

            if (classroom != null)
            {
                isNew                   = false;
                this.classroom          = classroom;
                Description_Editor.Text = classroom.Description;
                Title_Entry.Text        = classroom.Title;

                //ToolbarItems.Add(new ToolbarItem("Delete", "Trashcan_Icon", ToolbarItem_OnDelete, ToolbarItemOrder.Primary));
            }
            else
            {
                isNew = true;
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
            if (App.userEmail == null || App.userPassword == null)
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
                var temp_item         = new CreateClassroomItem();
                temp_item.Title       = Title_Entry.Text;
                temp_item.Description = Description_Editor.Text;
                temp_item.Email       = App.userEmail;
                temp_item.Password    = App.userPassword;

                item = temp_item;
            }
            else
            {
                var temp_item         = new EditClassroomItem();
                temp_item.Title       = Title_Entry.Text;
                temp_item.Description = Description_Editor.Text;
                temp_item.Email       = App.userEmail;
                temp_item.Password    = App.userPassword;
                temp_item.DatabaseId  = classroom.Id;
                
                item = temp_item;
            }

            sts.send(uri, item, () =>
            {
                Navigation.PopAsync();
            });
        }

        /*public async void ToolbarItem_OnDelete()
        {
            bool answer = await DisplayAlert("Delete Todo item?", "Are you sure you want to permanently delete this todo item?", "Yes", "No");
            if (!answer)
                return;

            var item        = new DeleteClassroomItem();
            item.Email      = App.userEmail;
            item.Password   = App.userPassword;
            item.DatabaseId = App.curClassroom.Id;

            sts.send(uri, item, () =>
            {

            });
            await Navigation.PopAsync();
        }*/

        void TitleEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Title_Entry.Text == null)
            {
                TitleCounter_Label.Text = "0/30";
                return;
            }

            if (Title_Entry.Text.Length > 30)
            {
                Title_Entry.Text = Title_Entry.Text.Substring(0, 30);
            }
            TitleCounter_Label.Text = Title_Entry.Text.Length.ToString() + "/30";
        }

        void DescriptionEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Description_Editor.Text == null)
            {
                DescriptionCounter_Label.Text = "0/100";
                return;
            }

            if (Description_Editor.Text.Length > 100)
            {
                Description_Editor.Text = Description_Editor.Text.Substring(0, 100);
            }
            DescriptionCounter_Label.Text = Description_Editor.Text.Length.ToString() + "/100";
        }
    }
}
