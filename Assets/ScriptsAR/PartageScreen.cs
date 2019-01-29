using System.Collections;
using UnityEngine;
using NatShareU;

public class PartageScreen : MonoBehaviour {

    // Use this for initialization
    public void TakeScreenshot ()
    {
        StartCoroutine(TakeScreen());
    }

    IEnumerator TakeScreen()
    {
        yield return new WaitForEndOfFrame();

        Texture2D screen = ScreenCapture.CaptureScreenshotAsTexture();
        NatShare.Share(screen);
    }
}
