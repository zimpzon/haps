using Facebook.Unity;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script
{
    public interface IFacebookAccess
    {
        void Initialize(Facebook.Unity.InitDelegate initCb);
        bool IsInitialized();
        bool IsLoggedIn();
        void LoginFacebook(FacebookDelegate<ILoginResult> loginCallback);
        void Logout();
        void ActivateApp();
    }

    class FacebookManager : IFacebookAccess
    {
        public void Initialize(InitDelegate initCb)
        {
            if (!FB.IsInitialized)
                FB.Init(initCb, OnHideUnity);
            else
                initCb();
        }

        public bool IsInitialized()
        {
            return FB.IsInitialized;
        }

        public bool IsLoggedIn()
        {
            return FB.IsLoggedIn;
        }

        // An additional benefit of asking for fewer permissions is that you might not need to submit
        // your app for Login Review.You only need to submit for Login Review if you're requesting any
        // permissions other than public_profile, user_friends and email.
        public void LoginFacebook(FacebookDelegate<ILoginResult> loginCallback)
        {
            var perms = new List<string>() { "public_profile", "email" };
            FB.LogInWithReadPermissions(perms, loginCallback); // This is where FB dialog is shown
        }

        public void Logout()
        {
            if (FB.IsLoggedIn)
                FB.LogOut();
        }

        public void ActivateApp()
        {
            FB.ActivateApp();
        }

        /// <summary>
        /// Pause game when login screen appears
        /// </summary>
        /// <param name="isGameShown">If set to <c>true</c> is game shown.</param>
        private static void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
                Time.timeScale = 0; // Pause the game - we will need to hide
            else
                Time.timeScale = 1; // Resume the game - we're getting focus again
        }
    }
}
