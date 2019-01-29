using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#pragma warning disable 0219
#endif

public class RenderTextureCamera : MonoBehaviour
{
	[Space(20)]
	public int TextureResolution = 512;
	[Space(10)]
	public bool GenerateMipmap;

	private int TextureResolutionX;
	private int TextureResolutionY;
	private Camera Render_Texture_Camera;
	private RenderTexture CameraOutputTexture;
    private float targetCameraWidth;
    private float targetCameraHeight;

    public RenderTexture GetRenderTexture()
	{
		return CameraOutputTexture;
	}

	void Start() 
	{
		Render_Texture_Camera = GetComponent<Camera>();
        StartCoroutine(StartRenderingToTexture());
	}

	IEnumerator StartRenderingToTexture() 
	{
		yield return new WaitForSeconds(0.5f);

		if (transform.parent && transform.parent.localScale.x >= transform.parent.localScale.z)
		{
			TextureResolutionX = TextureResolution;
			TextureResolutionY = (int)(TextureResolution * transform.parent.localScale.z / transform.parent.localScale.x);
		}

		if (transform.parent && transform.parent.localScale.x < transform.parent.localScale.z)
		{
			TextureResolutionX =  (int)(TextureResolution * transform.parent.localScale.x / transform.parent.localScale.z);
			TextureResolutionY = TextureResolution;
		}

		CameraOutputTexture = new RenderTexture(TextureResolutionX, TextureResolutionY, 0);

		if (GenerateMipmap)
		{
			CameraOutputTexture.useMipMap = true;
			CameraOutputTexture.autoGenerateMips = true;
		}
		else
		{
			CameraOutputTexture.useMipMap = false;
			CameraOutputTexture.autoGenerateMips = false;
		}

		Render_Texture_Camera.targetTexture = CameraOutputTexture;
		
		gameObject.layer = transform.parent.gameObject.layer;
		Render_Texture_Camera.cullingMask = 1 << gameObject.layer;
	}


	public void RecalculateTextureSize() 
	{
		StartCoroutine(RecalculateRenderTexture());
	}

	
	private IEnumerator RecalculateRenderTexture() 
	{
		yield return new WaitForEndOfFrame();

		Render_Texture_Camera.targetTexture = null;
		CameraOutputTexture.Release();
		CameraOutputTexture = null;

		StartCoroutine(StartRenderingToTexture());
	}

    public IEnumerator GetTexture2D(System.Action<Texture2D> result)
    {
        yield return new WaitForEndOfFrame();

        Texture2D FrameTexture = new Texture2D(CameraOutputTexture.width, CameraOutputTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = CameraOutputTexture;
        FrameTexture.ReadPixels(new Rect(0, 0, CameraOutputTexture.width, CameraOutputTexture.height), 0, 0);
        RenderTexture.active = null;

        FrameTexture.Apply();
        result.Invoke(FrameTexture);
    }

    public IEnumerator TakeScreen(System.Action<string> result)
    {
        yield return new WaitForEndOfFrame();
        string screensPath = null;
        if (screensPath == null)
        {
        #if UNITY_ANDROID && !UNITY_EDITOR
			screensPath = "/Pictures/Screenshot";
        #elif UNITY_IPHONE && !UNITY_EDITOR
			screensPath = Application.persistentDataPath;
        #else
            screensPath = Application.dataPath + "/Screens";
        #endif
            if (!System.IO.Directory.Exists(screensPath))
            {
                System.IO.Directory.CreateDirectory(screensPath);
            }

            string fileName = screensPath + "/screen_" + System.DateTime.Now.ToString("dd_MM_HH_mm_ss") + ".png";
            string pathToSave = fileName;
            ScreenCapture.CaptureScreenshot(pathToSave);

            yield return new WaitForEndOfFrame();
        #if UNITY_EDITOR
            AssetDatabase.Refresh();
        #endif
            result.Invoke(pathToSave);
        }
    }
}