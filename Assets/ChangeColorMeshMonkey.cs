using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ChangeColorMeshMonkey : MonoBehaviour {
    public GameObject face;
    public GameObject corps;
    public GameObject museau;
    public GameObject pattes;
    public GameObject tete;

    Vuforia.Image image;
    Image.PIXEL_FORMAT mPixelFormat = Image.PIXEL_FORMAT.UNKNOWN_FORMAT;
    bool mAccessCameraImage = true;
    bool mFormatRegistered = false;

    public RenderTextureCamera renderCamera;
    public Rect sourceRect;
    private RenderTexture CameraOutputTexture;

    // Use this for initialization
    void Start () {

#if UNITY_EDITOR
        mPixelFormat = Image.PIXEL_FORMAT.GRAYSCALE; // Need Grayscale for Editor
#else
        mPixelFormat = Image.PIXEL_FORMAT.RGB888; // Use RGB888 for mobile
#endif
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);
    }

    // Update is called once per frame
    void Update () {

    }

    void OnVuforiaStarted()
    {
        // Try register camera image format
        if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
        {
            Debug.Log("Successfully registered pixel format " + mPixelFormat.ToString());

            mFormatRegistered = true;
        }
        else
        {
            Debug.LogError(
                "\nFailed to register pixel format: " + mPixelFormat.ToString() +
                "\nThe format may be unsupported by your device." +
                "\nConsider using a different pixel format.\n");

            mFormatRegistered = false;
        }

    }

    void OnTrackablesUpdated()
    {
        // Repere la couleur (gris) et applique sur le mesh la couleur équivalente
        if (mFormatRegistered)
        {
            if (mAccessCameraImage)
            {
                image = CameraDevice.Instance.GetCameraImage(mPixelFormat);
                if (image != null)
                { 
                    byte[] pixels = image.Pixels;

                    if (pixels != null && pixels.Length > 0)
                    {
                        //// Couleur appliquée sur le dessin
                        //Color32 drawingColor = new Color32(pixels[0], pixels[1], pixels[2], 1);
                        //// Applique la couleur sur le model
                        //face.GetComponent<Renderer>().material.SetColor("_Color", drawingColor);
                        //Debug.Log("Color components are " + pixels[0] + ", " + pixels[1] + ", " + pixels[2]);


                        StartCoroutine(renderCamera.GetTexture2D(text2d =>
                        {
                            int x = Mathf.FloorToInt(sourceRect.x);
                            int y = Mathf.FloorToInt(sourceRect.y);
                            int width = Mathf.FloorToInt(sourceRect.width);
                            int height = Mathf.FloorToInt(sourceRect.height);

                            Color[] pix = text2d.GetPixels(x, y, width, height);
                            Texture2D destTex = new Texture2D(width, height);
                            destTex.SetPixels(pix);
                            destTex.Apply();

                            // Set the current object's texture to show the
                            // extracted rectangle.
                            face.GetComponent<Renderer>().material.mainTexture = destTex;
                        }));
                    }
                }
            }
        }
    }
}
