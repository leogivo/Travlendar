﻿
using Travlendar.Core.AppCore.Tools;
using Travlendar.Core.AppCore.ViewModels;
using Travlendar.Pages;
using Travlendar.Renderers;
using Xamarin.Forms;

namespace Travlendar.Core.AppCore.Pages
{
    public class LandingPage : ContentPage
    {
        LoginViewModel _viewModel;

        private RelativeLayout relativeLayout;
        private StackLayout buttons;
        private StackLayout layout;
        private Image backgroundImage;
        private Label title;
        private Button loginButton;
        private Button registerButton;
        public FacebookButton fbButton;

        //No VM needed for the moment

        public LandingPage (LoginViewModel vm)
        {
            DependencyService.Get<IStatusBar> ().HideStatusBar ();
            _viewModel = vm;
            relativeLayout = new RelativeLayout
            {
                VerticalOptions = LayoutOptions.Fill
            };

            buttons = new StackLayout
            {
                Padding = new Thickness (0, 20, 0, 20),
            };

            layout = new StackLayout
            {
                VerticalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.White
            };

            backgroundImage = new Image
            {
                Source = "login_background.jpg",
            };

            title = new Label ()
            {
                Text = "Travlendar+",
                FontSize = 32,
                TextColor = Color.White
            };

            fbButton = new FacebookButton ();
            fbButton.HorizontalOptions = LayoutOptions.Center;
            fbButton.HeightRequest = 50;
            fbButton.VerticalOptions = LayoutOptions.Center;
            //Add your event handler for the OnLogin to operate with the Facebook credentials comming from SDK

            registerButton = new Button
            {
                Text = "Create Account",
                TextColor = Constants.TravlendarBlue,
                BackgroundColor = Color.White,
                BorderColor = Constants.TravlendarBlue,
                BorderRadius = 20,
                BorderWidth = 2,
                HorizontalOptions = LayoutOptions.Center,
                WidthRequest = 200
            };

            loginButton = new Button
            {
                Text = "Login",
                TextColor = Constants.TravlendarBlue,
                BackgroundColor = Color.White,
                BorderColor = Constants.TravlendarBlue,
                BorderRadius = 20,
                BorderWidth = 2,
                HorizontalOptions = LayoutOptions.Center,
                WidthRequest = 200
            };

            RegisterEvents ();

            relativeLayout.Children.Add (backgroundImage, Constraint.Constant (0), Constraint.Constant (0));
            relativeLayout.Children.Add (title, Constraint.RelativeToParent (parent => (parent.Width / 2) - 75), Constraint.RelativeToParent (parent => parent.Height / 2));

            layout.Children.Add (relativeLayout);
            buttons.Children.Add (registerButton);
            buttons.Children.Add (loginButton);

            buttons.Children.Add (fbButton);
            layout.Children.Add (buttons);

            this.Content = layout;
        }

        private async void LoginButton_ClickedAsync (object sender, System.EventArgs e)
        {
            var vm = LoginViewModel.GetInstance ();
            var page = new LoginPage (vm);
            NavigationPage.SetHasNavigationBar (page, false);
            await Navigation.PushAsync (page);
        }

        private void RegisterButton_Clicked (object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException ();
            //Handle manual register
        }

        private void RegisterEvents ()
        {
            fbButton.OnLogin -= _viewModel.LoginWithFacebook;
            fbButton.OnLogin += _viewModel.LoginWithFacebook;

            registerButton.Clicked -= RegisterButton_Clicked;
            registerButton.Clicked += RegisterButton_Clicked;

            loginButton.Clicked -= LoginButton_ClickedAsync;
            loginButton.Clicked += LoginButton_ClickedAsync;
        }
    }
}
