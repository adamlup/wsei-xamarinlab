﻿using System;
using System.Collections.Generic;
using AirMonitor.Models;
using AirMonitor.ViewModels;
using Xamarin.Forms;

namespace AirMonitor.Views
{
    public partial class HomePage : ContentPage
    {

        private HomeViewModel _viewModel => BindingContext as HomeViewModel;

        public HomeViewModel homeViewModel;

        public HomePage()
        {
            InitializeComponent();

            homeViewModel = new HomeViewModel(Navigation);
            itemsListView.ItemsSource = homeViewModel.itemsList;

            BindingContext = new HomeViewModel(Navigation);
        }

        void ListView_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            _viewModel.GoToDetailsCommand.Execute(e.Item as Measurement);
        }
    }
}
