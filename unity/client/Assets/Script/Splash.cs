using Assets.Script;
using Facebook.Unity;
using UnityEngine;

public class Splash : MonoBehaviour
{
    public GameObject PanelContent;
    public bool SkipFacebookLogin;

    private IAppLog appLog_;
    private IFacebookAccess facebook_;
    private IGameManager gameManager_;

    void Awake()
    {
        appLog_ = ServiceManager.GameServices.GetAppLog();
        gameManager_ = ServiceManager.GameServices.GetGameManager();

        appLog_.LogInfo("Splash.Awake");
        PanelContent.SetActive(false);

        appLog_.LogInfo("Splash: Initializing FB, first try");
        facebook_.Initialize(OnFacebookInitialized);
    }

    private void OnFacebookInitialized()
    {
        appLog_.LogInfo("Splash: FB, IsInitialized = {0}, IsLoggedIn = {1}, skip = {2}", facebook_.IsInitialized(), facebook_.IsLoggedIn(), SkipFacebookLogin);

        bool isLoggedIn = facebook_.IsInitialized() && (facebook_.IsLoggedIn() || SkipFacebookLogin);
        if (!isLoggedIn)
        {
            ShowSplash();
            return;
        }

        OnFacebookLoggedIn();
    }

    public void OnLoginButtonClick()
    {
        if (!facebook_.IsInitialized())
        {
            appLog_.LogError("Splash: Error, login button clicked but FB not initialized");
            gameManager_.SetState(GameState.Restart);
            return;
        }

        DoLogin();
    }

    private void DoLogin()
    {
        // On mobile we will get a long-lived token. On web it will be short-lived.
        // It can be converted to long-lived with app secret and app id.
        facebook_.LoginFacebook(FacebookLoginCallback);
    }

    private void OnFacebookLoggedIn()
    {
        facebook_.ActivateApp();
        //        FB.API("/me?fields=name,email", HttpMethod.GET, Api);
        gameManager_.SetState(GameState.MainGame);
    }

    void OnFbError()
    {
        appLog_.LogDebug("Splash: FB error, retrying initialize");
        facebook_.Initialize(OnFacebookInitialized);
    }

    void Api(IGraphResult res)
    {
        if (!string.IsNullOrEmpty(res.Error))
        {
            OnFbError();
            return;
        }

        appLog_.LogInfo("API callback: " + res.ResultDictionary["name"]);
    }

    private void FacebookLoginCallback(ILoginResult result)
    {
        appLog_.LogInfo("Splash: FB login callback, IsLoggedIn = {0}, error = {1}", FB.IsLoggedIn, result.Error);

        if (result.Error == null && FB.IsLoggedIn)
        {
            OnFacebookLoggedIn();
        }
        else
        {
            // Login error. Need to check if FB SDK shows an error or if I have to.
            // Will also get here in case of user cancel.
        }
    }

    private void ShowSplash()
    {
        PanelContent.SetActive(true);
    }
}
