﻿using Android.Content;
using Android.Runtime;
using System;

using Travlendar.Core.AppCore.Renderers;
using Travlendar.Droid.Renderers;

using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer (typeof (FacebookButton), typeof (FacebookButtonRenderer))]
namespace Travlendar.Droid.Renderers
{
    /// <summary>
    /// FacebookLogin button renderer implementation for Xamarin.Forms in the Android side.
    /// This implement the native look and feel from Facebook SDK for Android LoginButton object and handle
    /// Facebook Login basic authentication for Android
    /// </summary>
    public class FacebookButtonRenderer : ButtonRenderer
    {
        private LoginButton loginButton;
        private Context _context;
        public FacebookButtonRenderer (Context context) : base (context)
        {
            _context = context;
        }

        protected override void OnElementChanged (ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged (e);
            if ( e.OldElement != null || this.Element == null )
                return;

            //Native Button
            loginButton = new LoginButton (_context);
            loginButton.TextAlignment = Android.Views.TextAlignment.TextEnd;
            loginButton.LoginBehavior = LoginBehavior.NativeWithFallback;

            //Implement FacebookCallback with LoginResult type to handle Callback's result
            var loginCallback = new FacebookCallback<LoginResult>
            {
                HandleSuccess = loginResult =>
                {
                    //If login success, We can now retrieve our needed data and build our FacebookEventArgs parameters

                    FacebookButton facebookButton = (FacebookButton) e.NewElement;
                    FacebookEventArgs fbArgs = new FacebookEventArgs ();
                    if ( loginResult.AccessToken != null )
                    {
                        fbArgs.UserId = loginResult.AccessToken.UserId;
                        fbArgs.AccessToken = loginResult.AccessToken.Token;
                        //Hack from Java.Util.Date to DateTime
                        fbArgs.TokenExpiration = FromUnixTime (loginResult.AccessToken.Expires.Time);
                    }

                    //Pass the parameters into Login method in the FacebookButton 
                    //object and handle it on Xamarin.Forms side
                    facebookButton.Login (facebookButton, fbArgs);
                },
                HandleCancel = () =>
                {
                    //Handle any cancel the user has perform
                    Console.WriteLine ("User deleted the login operation");
                },
                HandleError = loginError =>
                {
                    //Handle any error happends here
                    //TODO: we have to decide what to do here
                }
            };
            LoginManager.Instance.RegisterCallback (MainActivity.CallbackManager, loginCallback);
            //Set the LoginButton as NativeControl
            SetNativeControl (loginButton);
        }

        //Autologin functionality, to be implemented
        public bool IsLogged ()
        {
            var token = AccessToken.CurrentAccessToken;
            return (!string.IsNullOrEmpty (token.UserId) &&
                            !string.IsNullOrEmpty (token.Token) &&
                            !token.IsExpired);
        }

        public DateTime FromUnixTime (long unixTimeMillis)
        {
            var epoch = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds (unixTimeMillis);
        }

        protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged (sender, e);
        }
    }

    /// <summary>
    /// FacebookCallback<TResult> class which will handle any result the FacebookActivity returns.
    /// </summary>
    /// <typeparam name="TResult">The callback result's type you will handle</typeparam>
    public class FacebookCallback<TResult> : Java.Lang.Object, IFacebookCallback where TResult : Java.Lang.Object
    {
        public Action HandleCancel { get; set; }
        public Action<FacebookException> HandleError { get; set; }
        public Action<TResult> HandleSuccess { get; set; }

        public void OnCancel ()
        {
            var c = HandleCancel;
            if ( c != null )
                c ();
        }

        public void OnError (FacebookException error)
        {
            var c = HandleError;
            if ( c != null )
                c (error);
        }

        public void OnSuccess (Java.Lang.Object result)
        {
            var c = HandleSuccess;
            if ( c != null )
                c (result.JavaCast<TResult> ());
        }
    }
}
