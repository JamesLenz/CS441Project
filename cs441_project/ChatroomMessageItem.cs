using System;
using Xamarin.Forms;
namespace cs441_project
{
    public class ChatroomMessageItem
    {
        public ChatroomMessageItem()
        {
            CreatedDateTime = new DateTime();
        }

        public string Message { get; set; }
        public string CreatorEmail { get; set; }
        public string CreatorName { get; set; }
        public DateTime CreatedDateTime { get; set; }

        public Color getBackgroundColor
        {
            get
            {
                if (CreatorEmail.ToLower() == App.userEmail.ToLower())
                    return Color.White;
                else
                    return Color.Gray;
            }
        }

        public Color getTextColor
        {
            get
            {
                if (CreatorEmail.ToLower() == App.userEmail.ToLower())
                    return Color.Black;
                else
                    return Color.White;
            }
        }

        public Thickness getMargin
        {
            get
            {
                if (CreatorEmail.ToLower() == App.userEmail.ToLower())
                    return new Thickness(50, 0, 0, 5);
                else
                    return new Thickness(0, 0, 50, 5);
            }
        }

        public string getCreatorInfo
        {
            get
            {
                return CreatorName + " at " + CreatedDateTime.ToString("MMM d yy hh:mm tt");
            }
        }
    }
}
