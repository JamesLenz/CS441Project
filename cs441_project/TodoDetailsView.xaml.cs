using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cs441_project
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TodoDetailsView: ContentPage
	{
		public TodoDetailsView (TodoItem model)
		{
            NavigationPage.SetHasNavigationBar(this, true);
            InitializeComponent();
		}

        public void OnCancel(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
	}
}