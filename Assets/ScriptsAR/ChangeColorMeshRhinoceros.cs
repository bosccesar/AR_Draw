using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ChangeColorMeshRhinoceros : MonoBehaviour
{
    public GameObject visage;
    public GameObject corne;
    public GameObject corpsHaut;
    public GameObject corpsBas;
    public GameObject tete;
    public RenderTextureCamera renderCamera;

    private string face = "face";
    private string horn = "horn";
    private string upperBody = "upperBody";
    private string lowerBody = "lowerBody";
    private string head = "head";
    private int nbPointsByFace = 3; // Sous la corne/Joue gauche/Joue droite
    private int nbPointsByHorn = 1; // Milieu de la corne
    private int nbPointsByUpperBody = 5; // Patte avant gauche/Patte avant droite/Patte arriere droite/fesses/Queue
    private int nbPointsByLowerBody = 42; // Ventre avant/ Ventre arriere
    private int nbPointsByHead = 3; // Haut crane/Oreille gauche/Oreille droite
    private float sourceRectWidth;
    private float sourceRectHeight;
    private int cpt;
    private bool unityEditor;
    private string[] tabPart;

    // Dictionnaire affiliant chaque sous-partie avec le nombre de point de coordonnees
    Dictionary<string, int> hashNbPointInEachSubpart = new Dictionary<string, int>();
    // Dictionnaire affiliant chaque sous-partie avec la partie mere
    Dictionary<string, List<string>> hashSubpartWithHerPart = new Dictionary<string, List<string>>();
    List<string> subPartFace = new List<string>();
    List<string> subPartCorne = new List<string>();
    List<string> subPartHautCorps = new List<string>();
    List<string> subPartBasCorps = new List<string>();
    List<string> subPartHead = new List<string>();
    // Dictionnaire d'une liste de coordonnées pour chaque partie du dessin
    Dictionary<string, List<float>> hashPartCoord = new Dictionary<string, List<float>>();
    List<float> coordFaceCentre = new List<float>();
    List<float> coordJoueGauche = new List<float>();
    List<float> coordJoueDroite = new List<float>();
    List<float> coordCorne = new List<float>();
    List<float> coordCorpsGauche = new List<float>();
    List<float> coordPatteGauche = new List<float>();
    List<float> coordPatteCentre = new List<float>();
    List<float> coordPatteDroite = new List<float>();
    List<float> coordQueue = new List<float>();
    List<float> coordVentreDevant = new List<float>();
    List<float> coordVentreGauche = new List<float>();
    List<float> coordMainGauche = new List<float>();
    List<float> coordMainDroite = new List<float>();
    List<float> coordHautCrane = new List<float>();
    List<float> coordOreilleGauche = new List<float>();
    List<float> coordOreilleDroite = new List<float>();

    // Use this for initialization
    void Start()
    {
        // Taille du rectangle de récupération des pixels
        sourceRectWidth = 10f;
        sourceRectHeight = 10f;

        unityEditor = targetDevice();
        AddDictionnary();

        // Tableau contenant les 5 parties du dessin du rhinoceros
        tabPart = new string[] { face, horn, upperBody, lowerBody, head };
    }

    // Update is called once per frame
    void Update()
    {
        bool detected = DefaultTrackableEventHandler.detected;
        if (detected)
        {
            if (DefaultTrackableEventHandler.nameTrackable == "Rhinoceros")
            {
                for (int i = 0; i < tabPart.Length; i++)
                {
                    DrawingPart(tabPart[i]);
                }
            }
        }
    }

    private bool targetDevice()
    {
        bool isUnityEditor;
        #if UNITY_EDITOR
            isUnityEditor = true;
        #elif UNITY_ANDROID || UNITY_IPHONE
            isUnityEditor = false;
        #endif
        return isUnityEditor;
    }

    void AddDictionnary()
    {
        // 1er dictionnaire
        hashNbPointInEachSubpart.Add(face, nbPointsByFace);
        hashNbPointInEachSubpart.Add(horn, nbPointsByHorn);
        hashNbPointInEachSubpart.Add(upperBody, nbPointsByUpperBody);
        hashNbPointInEachSubpart.Add(lowerBody, nbPointsByLowerBody);
        hashNbPointInEachSubpart.Add(head, nbPointsByHead);

        // 2eme dictionnaire
            // Face
        subPartFace.Add("faceCentre");
        subPartFace.Add("joueGauche");
        subPartFace.Add("joueDroite");
        hashSubpartWithHerPart.Add("face", subPartFace);
            // Corne
        subPartCorne.Add("centre");
        hashSubpartWithHerPart.Add("horn", subPartCorne);
            // Haut du corps
        subPartHautCorps.Add("corpsGauche");
        subPartHautCorps.Add("patteGauche");
        subPartHautCorps.Add("patteCentre");
        subPartHautCorps.Add("patteDroite");
        subPartHautCorps.Add("queue");
        hashSubpartWithHerPart.Add("upperBody", subPartHautCorps);
            // Bas du corps
        subPartBasCorps.Add("ventreDevant");
        subPartBasCorps.Add("ventreGauche");
        hashSubpartWithHerPart.Add("lowerBody", subPartBasCorps);
            // tete
        subPartHead.Add("hautCrane");
        subPartHead.Add("oreilleGauche");
        subPartHead.Add("oreilleDroite");
        hashSubpartWithHerPart.Add("head", subPartHead);

        if (unityEditor)
        {
            // 3eme dictionnaire
                // Points de la face
            coordFaceCentre.Add(78f); // x
            coordFaceCentre.Add(126f); // y
            hashPartCoord.Add("faceCentre", coordFaceCentre);
            coordJoueGauche.Add(49f);
            coordJoueGauche.Add(99f);
            hashPartCoord.Add("joueGauche", coordJoueGauche);
            coordJoueDroite.Add(118f);
            coordJoueDroite.Add(110f);
            hashPartCoord.Add("joueDroite", coordJoueDroite);
                // Points de la corne
            coordCorne.Add(86f);
            coordCorne.Add(60f);
            hashPartCoord.Add("centre", coordCorne);
                // Points du haut du corps
            coordCorpsGauche.Add(57f);
            coordCorpsGauche.Add(37f);
            hashPartCoord.Add("corpsGauche", coordCorpsGauche);
            coordPatteGauche.Add(57f);
            coordPatteGauche.Add(37f);
            hashPartCoord.Add("patteGauche", coordPatteGauche);
            coordPatteCentre.Add(115f);
            coordPatteCentre.Add(36f);
            hashPartCoord.Add("patteCentre", coordPatteCentre);
            coordPatteDroite.Add(74f);
            coordPatteDroite.Add(37f);
            hashPartCoord.Add("patteDroite", coordPatteDroite);
            coordQueue.Add(98f);
            coordQueue.Add(36f);
            hashPartCoord.Add("queue", coordQueue);
                // Points du bas du corps
            coordVentreDevant.Add(86f);
            coordVentreDevant.Add(92f);
            hashPartCoord.Add("ventreDevant", coordVentreDevant);
            coordVentreGauche.Add(92f);
            coordVentreGauche.Add(92f);
            hashPartCoord.Add("ventreGauche", coordVentreGauche);
                // Points de la tete
            coordHautCrane.Add(72f);
            coordHautCrane.Add(159f);
            hashPartCoord.Add("hautCrane", coordHautCrane);
            coordOreilleGauche.Add(23f);
            coordOreilleGauche.Add(132f);
            hashPartCoord.Add("oreilleGauche", coordOreilleGauche);
            coordOreilleDroite.Add(136f);
            coordOreilleDroite.Add(133f);
            hashPartCoord.Add("oreilleDroite", coordOreilleDroite);
        }
        else
        {
            // 3eme dictionnaire
                // Points de la face
            coordFaceCentre.Add(CoordinateScreenResponsive_X(78f, 3840f, 2160f)); // x
            coordFaceCentre.Add(CoordinateScreenResponsive_Y(126f, 2160f, 1080)); // y
            hashPartCoord.Add("faceCentre", coordFaceCentre);
            coordJoueGauche.Add(CoordinateScreenResponsive_X(49f, 3840f, 2160f));
            coordJoueGauche.Add(CoordinateScreenResponsive_Y(99f, 2160f, 1080));
            hashPartCoord.Add("joueGauche", coordJoueGauche);
            coordJoueDroite.Add(CoordinateScreenResponsive_X(118f, 3840f, 2160f));
            coordJoueDroite.Add(CoordinateScreenResponsive_Y(110f, 2160f, 1080));
            hashPartCoord.Add("joueDroite", coordJoueDroite);
                // Points de la corne
            coordCorne.Add(CoordinateScreenResponsive_X(86f, 3840f, 2160f));
            coordCorne.Add(CoordinateScreenResponsive_Y(60f, 2160f, 1080));
            hashPartCoord.Add("centre", coordCorne);
                // Points du haut du corps
            coordCorpsGauche.Add(CoordinateScreenResponsive_X(57f, 3840f, 2160f));
            coordCorpsGauche.Add(CoordinateScreenResponsive_Y(37f, 2160f, 1080));
            hashPartCoord.Add("corpsGauche", coordCorpsGauche);
            coordPatteGauche.Add(CoordinateScreenResponsive_X(115f, 3840f, 2160f));
            coordPatteGauche.Add(CoordinateScreenResponsive_Y(36f, 2160f, 1080));
            hashPartCoord.Add("patteGauche", coordPatteGauche);
            coordPatteCentre.Add(CoordinateScreenResponsive_X(74f, 3840f, 2160f));
            coordPatteCentre.Add(CoordinateScreenResponsive_Y(37f, 2160f, 1080));
            hashPartCoord.Add("patteCentre", coordPatteCentre);
            coordPatteDroite.Add(CoordinateScreenResponsive_X(98f, 3840f, 2160f));
            coordPatteDroite.Add(CoordinateScreenResponsive_Y(36f, 2160f, 1080));
            hashPartCoord.Add("patteDroite", coordPatteDroite);
            coordQueue.Add(CoordinateScreenResponsive_X(98f, 3840f, 2160f));
            coordQueue.Add(CoordinateScreenResponsive_Y(36f, 2160f, 1080));
            hashPartCoord.Add("queue", coordQueue);
                // Points du bas du corps
            coordVentreDevant.Add(CoordinateScreenResponsive_X(86f, 3840f, 2160f));
            coordVentreDevant.Add(CoordinateScreenResponsive_Y(92f, 2160f, 1080));
            hashPartCoord.Add("ventreDevant", coordVentreDevant);
            coordVentreGauche.Add(CoordinateScreenResponsive_X(86f, 3840f, 2160f));
            coordVentreGauche.Add(CoordinateScreenResponsive_Y(92f, 2160f, 1080));
            hashPartCoord.Add("ventreGauche", coordVentreGauche);
                // Points de la tete
            coordHautCrane.Add(CoordinateScreenResponsive_X(72f, 3840f, 2160f));
            coordHautCrane.Add(CoordinateScreenResponsive_Y(159f, 2160f, 1080));
            hashPartCoord.Add("hautCrane", coordHautCrane);
            coordOreilleGauche.Add(CoordinateScreenResponsive_X(23f, 3840f, 2160f));
            coordOreilleGauche.Add(CoordinateScreenResponsive_Y(132f, 2160f, 1080));
            hashPartCoord.Add("oreilleGauche", coordOreilleGauche);
            coordOreilleDroite.Add(CoordinateScreenResponsive_X(136f, 3840f, 2160f));
            coordOreilleDroite.Add(CoordinateScreenResponsive_Y(133f, 2160f, 1080));
            hashPartCoord.Add("oreilleDroite", coordOreilleDroite);
        }
    }

    private float CoordinateScreenResponsive_X(float coordinateComputerX, float computerWidth, float targetWidth)
    {
        float coordinateUpdate;
        coordinateUpdate = (coordinateComputerX * targetWidth) / computerWidth;
        return coordinateUpdate;
    }

    private float CoordinateScreenResponsive_Y(float coordinateComputerY, float computerHeight, float targetHeight)
    {
        float coordinateUpdate;
        coordinateUpdate = (coordinateComputerY * targetHeight) / computerHeight;
        return coordinateUpdate;
    }

    void DrawingPart(string drawingPart)
    {
        List<string> listSubPart = hashSubpartWithHerPart[drawingPart];
        foreach (string subPart in listSubPart)
        {
            cpt = 0;
            int nbPart = hashNbPointInEachSubpart[drawingPart];
            List<float> coordonnee = hashPartCoord[subPart];
            Texture2D[] tabTextures = new Texture2D[nbPart];
            StartCoroutine(renderCamera.GetTexture2D(text2d =>
            {
                int x = Mathf.FloorToInt(coordonnee[0]);
                int y = Mathf.FloorToInt(coordonnee[1]);
                int width = Mathf.FloorToInt(sourceRectWidth);
                int height = Mathf.FloorToInt(sourceRectHeight);

                Color[] pix = text2d.GetPixels(x, y, width, height);
                Texture2D destTex = new Texture2D(width, height);
                destTex.SetPixels(pix);
                destTex.Apply();

                tabTextures[cpt] = destTex;
                if (cpt == nbPart - 1)
                {
                    Texture2D targetTexture = tabTextures[0];
                    for (int i = 1; i < nbPart; i++)
                    {
                        if (tabTextures[i] != tabTextures[i - 1] && (tabTextures[i] != Texture2D.whiteTexture || tabTextures[i] != Texture2D.blackTexture))
                        {
                            targetTexture = tabTextures[i];
                        }
                    }
                    DrawingGameObjectTarget(drawingPart, targetTexture);
                    cpt = 0;
                }
                else
                {
                    cpt++;
                }
            }));
        }
    }

    void DrawingGameObjectTarget(string gameObjectTarget, Texture2D texture)
    {
        if (gameObjectTarget.Equals(face))
        {
            visage.GetComponent<Renderer>().material.mainTexture = texture;
        }
        else if (gameObjectTarget.Equals(horn))
        {
            corne.GetComponent<Renderer>().material.mainTexture = texture;
        }
        else if (gameObjectTarget.Equals(upperBody))
        {
            corpsHaut.GetComponent<Renderer>().material.mainTexture = texture;
        }
        else if (gameObjectTarget.Equals(lowerBody))
        {
            corpsBas.GetComponent<Renderer>().material.mainTexture = texture;
        }
        else if (gameObjectTarget.Equals(head))
        {
            tete.GetComponent<Renderer>().material.mainTexture = texture;
        }
    }
}
