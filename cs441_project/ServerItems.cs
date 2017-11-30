using Xamarin.Forms;
using System;

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

            //wait for response, then handle it
            try
            {
                responseMessage = await App.client.PostAsync(uri, content); //post
            }
            catch (Exception ex)
            {
                await bindingPage.DisplayAlert("Unexpected Error", ex.Message, "OK");
                return;
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

    // ask the server to check if email and password are in the database
    public class ValidateUserItem
    {
        public readonly string Command = "VALIDATE_USER"; //every item must have a "Command" data member
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // ask the server to check if email is in database and if it is, email the password
    public class ForgotPasswordItem
    {
        public readonly string Command = "FORGOT_PASSWORD";
        public string Email { get; set; }
    }

    // ask the server to add a new user
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

    //
    public class GetUserClassroomsItem
    {
        public readonly string Command = "GET_USER_CLASSROOMS";
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    //
    public class GetUsersItem
    {
        public readonly string Command = "GET_USERS";
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }

    //
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

    //
    public class DropUserItem
    {
        public readonly string Command = "DROP_USER";
        public string DropEmail { get; set; }
        //which classroom/database to interact with
        public string DatabaseId { get; set; }
        //validate user's permission
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
