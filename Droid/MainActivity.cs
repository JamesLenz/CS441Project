using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

//customer renderer to stop android from forcing shadows on certain elements
using Xamarin.Forms.Platform.Android;
using cs441_project.Droid;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(Xamarin.Forms.Button), typeof(FlatButtonRenderer))]
namespace cs441_project.Droid
{
	public class FlatButtonRenderer : ButtonRenderer
	{
		protected override void OnDraw(Android.Graphics.Canvas canvas)
		{
			base.OnDraw(canvas);
		}
	}
}
//end customer renderer

namespace cs441_project.Droid
{
    [Activity(Label = "cs441_project.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            LoadApplication(new App());
		}
    }
}
