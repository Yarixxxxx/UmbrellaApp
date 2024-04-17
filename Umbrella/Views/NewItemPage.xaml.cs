using System;
using System.Collections.Generic;
using System.ComponentModel;
using Umbrella.Models;
using Umbrella.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Umbrella.Views
{
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
        }
    }
}