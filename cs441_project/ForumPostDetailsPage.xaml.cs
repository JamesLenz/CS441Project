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
    public partial class ForumPostDetailsPage : ContentPage
    {
        private SendToServer sts;
        private Uri uri = new Uri("http://54.193.30.236/index.py");
        private ForumThreadItem thread;
        private ForumPostItem post;
        private bool isNewThread = false;
        private bool isNewPost = false;

        public ForumPostDetailsPage(ForumThreadItem thread, ForumPostItem post)
        {
            InitializeComponent();

            sts = new SendToServer(this);

            if (thread == null)
            {
                isNewThread = true;
                Title = "Create Thread";
            }
            else
            {
                isNewThread = false;
                this.thread = thread;
                Title_Entry.Text = thread.Title;
                if (post == null)
                {
                    isNewPost = true;
                    Title = "Create Post";
                    Title_StackLayout.IsVisible = false;
                }
                else
                {
                    isNewPost = false;
                    this.post = post;
                    Description_Editor.Text = post.Content;
                    Title = "Edit Post";
                    if (!post.isFirstPost || App.curClassroom.curUserIsOwner)
                    {
                        ToolbarItems.Add(new ToolbarItem("Delete", "Trashcan_Icon", ToolbarItem_OnDelete, ToolbarItemOrder.Primary));
                    }
                    if (!post.isFirstPost)
                    {
                        Title_StackLayout.IsVisible = false;
                    }
                }
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

            if (Description_Editor.Text == null || Description_Editor.Text == "")
            {
                await DisplayAlert("Error: Description", "Please enter a description", "OK");
                return;
            }

            object item;
            if (isNewThread)
            {
                var temp_item        = new CreateForumThreadItem();
                temp_item.Title      = Title_Entry.Text;
                temp_item.Content    = Description_Editor.Text;
                temp_item.DatabaseId = App.curClassroom.Id;
                temp_item.Email      = App.userEmail;
                temp_item.Password   = App.userPassword;

                item = temp_item;
            }
            else if (!isNewPost)
            {
                var temp_item         = new EditForumPostItem();
                temp_item.Title       = Title_Entry.Text;
                temp_item.Content     = Description_Editor.Text;
                temp_item.DatabaseId  = App.curClassroom.Id;
                temp_item.PostId      = post.Id;
                temp_item.Email       = App.userEmail;
                temp_item.Password    = App.userPassword;

                thread.Title = temp_item.Title;

                item = temp_item;
            }
            else
            {
                var temp_item        = new CreateForumPostItem();
                temp_item.Content    = Description_Editor.Text;
                temp_item.DatabaseId = App.curClassroom.Id;
                temp_item.Email      = App.userEmail;
                temp_item.Password   = App.userPassword;
                temp_item.ThreadId   = thread.Id;

                item = temp_item;
            }

            sts.send(uri, item, async () =>
            {
                await Navigation.PopAsync();
            });
        }

        public async void ToolbarItem_OnDelete()
        {
            object item;
            if (post.isFirstPost)
            {
                bool answer = await DisplayAlert("Delete Forum Thread?", "Are you sure you want to permanently delete this forum thread and all of the responses?", "Yes", "No");
                if (!answer)
                    return;
                
                var temp_item        = new DeleteForumThreadItem();
                temp_item.Email      = App.userEmail;
                temp_item.Password   = App.userPassword;
                temp_item.DatabaseId = App.curClassroom.Id;
                temp_item.ThreadId    = thread.Id;

                item = temp_item;
            }
            else
            {
                bool answer = await DisplayAlert("Delete Forum Post?", "Are you sure you want to permanently delete this forum post?", "Yes", "No");
                if (!answer)
                    return;
                
                var temp_item        = new DeleteForumPostItem();
                temp_item.Email      = App.userEmail;
                temp_item.Password   = App.userPassword;
                temp_item.DatabaseId = App.curClassroom.Id;
                temp_item.PostId     = post.Id;

                item = temp_item;
            }

            sts.send(uri, item, async () =>
            {
                await Navigation.PopAsync();
            });
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
