﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace cs441_project
{
    public partial class HomePage : TabbedPage
    {
        public HomePage()
        {
            NavigationPage.SetHasNavigationBar(this, true);
            InitializeComponent();
        }
    }
}
