using Xamarin.Forms;
using System;

// items containing data relevant to a certain task
namespace cs441_project
{
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

    // add "to do" item to classroom database
    public class AddTodoItem
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
}
