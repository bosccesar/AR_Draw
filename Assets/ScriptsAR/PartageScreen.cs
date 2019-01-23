using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;

public class PartageScreen : MonoBehaviour {

    public Text partageUserId;
    public RenderTextureCamera renderCamera;

    // Use this for initialization
    public void TakeScreenshot () {
        StartCoroutine(renderCamera.TakeScreen(imagePath =>
        {
            FacebookLogin();
            FacebookShare(imagePath);
        }));
    }

    private void Awake()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(() =>
            {
                if (FB.IsInitialized)
                    FB.ActivateApp();
                else
                    Debug.LogError("Couldn't initialize");
            },
            isGameShown =>
            {
                if (!isGameShown)
                    Time.timeScale = 0;
                else
                    Time.timeScale = 1;
            });
        }
        else
            FB.ActivateApp();
    }

    #region Login / Logout
    private void FacebookLogin()
    {
        FB.LogInWithReadPermissions(callback:OnLogIn);
    }

    private void FacebookLogout()
    {
        FB.LogOut();
    }

    private void OnLogIn(ILoginResult result)
    {
        if(FB.IsLoggedIn)
        {
            AccessToken token = AccessToken.CurrentAccessToken;
            partageUserId.text = token.UserId;
        }
        else
        {
            Debug.Log("Canceled Login");
        }
    }
    #endregion

    private void FacebookShare(string imagePath)
    {
        FB.ShareLink(
            contentTitle:"#cerealis #coloring #AR",
            contentURL:new System.Uri(imagePath),
            contentDescription:"#Mon animal trop mignon en couleur",
            callback:OnShare);
    }

    private void OnShare(IShareResult result)
    {
        if(result.Cancelled || !string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("ShareLink error: " + result.Error);
        }
        else if(!string.IsNullOrEmpty(result.PostId))
        {
            Debug.Log(result.PostId);
        }
        else
        {
            Debug.Log("Share succeed");
        }
    }
}
