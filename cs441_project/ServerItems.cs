using Xamarin.Forms;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

// items containing data relevant to a certain task
namespace cs441_project
{   
    public class SendToServer : ContentPage
    {
        public HttpResponseMessage responseMessage = new HttpResponseMessage();
        public ResponseItem responseItem = new ResponseItem();
        public Page bindingPage;

        public SendToServer(Page bindingPage)
        {
            this.bindingPage = bindingPage;
        }

        public async void send(Uri uri, object item, Action HandleSuccess)
        {
            
            //serialize object and make it ready for sending over the internet
            var json = JsonConvert.SerializeObject(item);
            var content = new StringContent(json, Encoding.UTF8, "application/json"); //StringContent contains http headers

            int attempts = 0;
            int maxAttempts = 3;
            while (attempts < maxAttempts)
            {
                //wait for response, then handle it
                try
                {
                    responseMessage = await App.client.PostAsync(uri, content); //post
                }
                catch (Exception ex)
                {
                    //await bindingPage.DisplayAlert("Unexpected Error", ex.Message, "OK");
                    //return;
                    attempts++;
                    continue;
                }
                break;
            }
            if (responseMessage.IsSuccessStatusCode)
            { //success
                //get our JSON response and convert it to a ResponseItem object
                try
                {
                    responseItem = JsonConvert.DeserializeObject<ResponseItem>(await responseMessage.Content.ReadAsStringAsync());
                }
                catch (Exception ex)
                {
                    await bindingPage.DisplayAlert("Unexpected Error", ex.Message, "OK");
                }

                //if no errors, do something
                if (responseItem.Success)
                {
                    try
                    {
                        HandleSuccess();
                    }
                    catch (Exception ex)
                    {
                        await bindingPage.DisplayAlert("Unexpected Error", ex.Message, "OK");
                    }
                }
                else //else, display error
                {
                    await bindingPage.DisplayAlert("Error", responseItem.Response, "OK");
                }
            }
            else
            { //error
                await bindingPage.DisplayAlert("Unexpected Error", responseMessage.ToString(), "OK");
            }
        }
    }

    //=======================//
    //     Response Item     //
    //=======================//

    // the object returned from the server
    public class ResponseItem
    {
        public bool Success { get; set; }
        public string Response { get; set; }
        public string Data { get; set; }
    }

    //=======================//
    //     Command Items     //
    //=======================//
    
    // check if email and password are in the database
    public class ValidateUserItem
    {
        public readonly string Command = "VALIDATE_USER"; //every item must have a "Command" data member
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // check if email is in database and if it is, email the password
    public class ForgotPasswordItem
    {
        public readonly string Command = "FORGOT_PASSWORD";
        public string Email { get; set; }
    }

    // add a new user
    public class AddUserItem
    {
        public readonly string Command = "ADD_USER";
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // add to do item to classroom database
    public class CreateTodoItem
    {
        public readonly string Command = "CREATE_TODO_ITEM";
        public DateTime DueDateTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // edit to do item
    public class EditTodoItem
    {
        public readonly string Command = "EDIT_TODO_ITEM";
        public DateTime DueDateTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //which to do item to interact with
        public string TodoItemId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // delete a to do item
    public class DeleteTodoItem
    {
        public readonly string Command = "DELETE_TODO_ITEM";
        public string TodoItemId { get; set; }
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // get to do items
    public class GetTodoItems
    {
        public readonly string Command = "GET_TODO_ITEMS";
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // join classroom
    public class JoinClassroomItem
    {
        public readonly string Command = "JOIN_CLASSROOM";
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // leave classroom
    public class LeaveClassroomItem
    {
        public readonly string Command = "LEAVE_CLASSROOM";
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // create classroom
    public class CreateClassroomItem
    {
        public readonly string Command = "CREATE_CLASSROOM";
        public string Title { get; set; }
        public string Description { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // edit classroom
    public class EditClassroomItem
    {
        public readonly string Command = "EDIT_CLASSROOM";
        public string Title { get; set; }
        public string Description { get; set; }
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // delete classroom
    public class DeleteClassroomItem
    {
        public readonly string Command = "DELETE_CLASSROOM";
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // get classrooms that the user is joined to
    public class GetUserClassroomsItem
    {
        public readonly string Command = "GET_USER_CLASSROOMS";
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // get all members in the current classroom
    public class GetUsersItem
    {
        public readonly string Command = "GET_USERS";
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // send an email inviting them to join the current classroom
    public class InviteUserItem
    {
        public readonly string Command = "INVITE_USER";
        public string InviteEmail { get; set; }
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // kick a member out of the current classroom
    public class DropUserItem
    {
        public readonly string Command = "DROP_USER";
        public string DropId { get; set; }
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // retrieve user chatrooms
    public class GetUserChatroomsItem
    {
        public readonly string Command = "GET_USER_CHATROOMS";
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    //
    public class CreateChatroomItem
    {
        public CreateChatroomItem()
        {
            MemberIds = new List<string>();
        }
        public readonly string Command = "CREATE_CHATROOM";
        public List<string> MemberIds { get; set; }
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    //
    public class LeaveChatroomItem
    {
        public readonly string Command = "LEAVE_CHATROOM";
        public string ChatroomId { get; set; }
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    //
    public class GetChatroomMessagesItem
    {
        public readonly string Command = "GET_CHATROOM_MESSAGES";
        public string ChatroomId { get; set; }
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    //
    public class PostChatroomMessageItem
    {
        public readonly string Command = "POST_CHATROOM_MESSAGE";
        public string ChatroomId { get; set; }
        public string Message { get; set; }
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // add  forum to classroom database
    public class CreateForumThreadItem
    {
        public readonly string Command = "CREATE_FORUM_THREAD";
        //public DateTime DueDateTime { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // delete a forum item
    public class DeleteForumThreadItem
    {
        public readonly string Command = "DELETE_FORUM_THREAD";
        public string ThreadId { get; set; }
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // get forum threads
    public class GetForumThreadsItem
    {
        public readonly string Command = "GET_FORUM_THREADS";
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // edit forum post item
    public class EditForumPostItem
    {
        public readonly string Command = "EDIT_THREAD_POST";
        public string Title { get; set; } //if it applies (only first post will use this)
        public string Content { get; set; }
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //which to post to interact with
        public string PostId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    //
    public class DeleteForumPostItem
    {
        public readonly string Command = "DELETE_THREAD_POST";
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //which to post to interact with
        public string PostId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    //
    public class CreateForumPostItem
    {
        public readonly string Command = "CREATE_THREAD_POST";
        public string Content { get; set; }
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //which to do item to interact with
        public string ThreadId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    //
    public class GetForumPostsItem
    {
        public readonly string Command = "GET_THREAD_POSTS";
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //which to do item to interact with
        public string ThreadId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
