using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ChangeColorMeshSnake : MonoBehaviour
{
    public GameObject visage;
    public GameObject ecailles;
    public GameObject corpsHaut;
    public GameObject corpsBas;
    public GameObject tete;
    public RenderTextureCamera renderCamera;

    private string face = "face";
    private string scales = "scales";
    private string upperBody = "upperBody";
    private string lowerBody = "lowerBody";
    private string head = "head";
    private int nbPointsByFace = 3; // Milieu/Joue gauche/Joue droite
    private int nbPointsByScales = 2; // En haut de la tete/Corps haut
    private int nbPointsByUpperBody = 4; // Dessous la tete/Gauche/Droite/Bas
    private int nbPointsByLowerBody = 2; // Gauche/Droite
    private int nbPointsByHead = 3; // Haut crane/Oeil gauche/Oeil droit
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
    List<string> subPartEcailles = new List<string>();
    List<string> subPartHautCorps = new List<string>();
    List<string> subPartBasCorps = new List<string>();
    List<string> subPartTete = new List<string>();
    // Dictionnaire d'une liste de coordonnées pour chaque partie du dessin
    Dictionary<string, List<float>> hashPartCoord = new Dictionary<string, List<float>>();
    List<float> coordFaceCentre = new List<float>();
    List<float> coordFaceGauche = new List<float>();
    List<float> coordFaceDroite = new List<float>();
    List<float> coordEcaillesHaut = new List<float>();
    List<float> coordEcaillesArriere = new List<float>();
    List<float> coordCorpsHautDessus = new List<float>();
    List<float> coordCorpsHautGauche = new List<float>();
    List<float> coordCorpsHautBas = new List<float>();
    List<float> coordCorpsHautDroite = new List<float>();
    List<float> coordCorpsBasGauche = new List<float>();
    List<float> coordCorpsBasDroite = new List<float>();
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

        // Tableau contenant les 5 parties du dessin du serpent
        tabPart = new string[] { face, scales, upperBody, lowerBody, head };
    }

    // Update is called once per frame
    void Update()
    {
        bool detected = DefaultTrackableEventHandler.detected;
        if (detected)
        {
            if (DefaultTrackableEventHandler.nameTrackable == "Snake")
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
        hashNbPointInEachSubpart.Add(scales, nbPointsByScales);
        hashNbPointInEachSubpart.Add(upperBody, nbPointsByUpperBody);
        hashNbPointInEachSubpart.Add(lowerBody, nbPointsByLowerBody);
        hashNbPointInEachSubpart.Add(head, nbPointsByHead);

        // 2eme dictionnaire
            // Face
        subPartFace.Add("faceCentre");
        subPartFace.Add("faceGauche");
        subPartFace.Add("faceDroite");
        hashSubpartWithHerPart.Add("face", subPartFace);
            // Ecailles
        subPartEcailles.Add("ecaillesHaut");
        subPartEcailles.Add("ecaillesArriere");
        hashSubpartWithHerPart.Add("scales", subPartEcailles);
            // Haut du corps
        subPartHautCorps.Add("hautDessus");
        subPartHautCorps.Add("HautGauche");
        subPartHautCorps.Add("HautBas");
        subPartHautCorps.Add("HautDroite");
        hashSubpartWithHerPart.Add("upperBody", subPartHautCorps);
            // Bas du corps
        subPartBasCorps.Add("BasGauche");
        subPartBasCorps.Add("BasDroite");
        hashSubpartWithHerPart.Add("lowerBody", subPartBasCorps);
            // tete
        subPartTete.Add("hautCrane");
        subPartTete.Add("oreilleGauche");
        subPartTete.Add("oreilleDroite");
        hashSubpartWithHerPart.Add("head", subPartTete);

        if (unityEditor)
        {
            // 3eme dictionnaire
                // Points de la face
            coordFaceCentre.Add(78f); // x
            coordFaceCentre.Add(126f); // y
            hashPartCoord.Add("faceCentre", coordFaceCentre);
            coordFaceGauche.Add(49f);
            coordFaceGauche.Add(99f);
            hashPartCoord.Add("faceGauche", coordFaceGauche);
            coordFaceDroite.Add(118f);
            coordFaceDroite.Add(110f);
            hashPartCoord.Add("faceDroite", coordFaceDroite);
                // Points des ecailles
            coordEcaillesHaut.Add(86f);
            coordEcaillesHaut.Add(92f);
            hashPartCoord.Add("ecaillesHaut", coordEcaillesHaut);
            coordEcaillesArriere.Add(86f);
            coordEcaillesArriere.Add(92f);
            hashPartCoord.Add("ecaillesArriere", coordEcaillesArriere);
                // Points du haut du corps
            coordCorpsHautDessus.Add(86f);
            coordCorpsHautDessus.Add(60f);
            hashPartCoord.Add("hautDessus", coordCorpsHautDessus);
            coordCorpsHautGauche.Add(56f);
            coordCorpsHautGauche.Add(54f);
            hashPartCoord.Add("HautGauche", coordCorpsHautGauche);
            coordCorpsHautBas.Add(113f);
            coordCorpsHautBas.Add(53f);
            hashPartCoord.Add("HautBas", coordCorpsHautBas);
            coordCorpsHautDroite.Add(73f);
            coordCorpsHautDroite.Add(69f);
            hashPartCoord.Add("HautDroite", coordCorpsHautDroite);
                // Points du bas du corps
            coordCorpsBasGauche.Add(57f);
            coordCorpsBasGauche.Add(37f);
            hashPartCoord.Add("BasGauche", coordCorpsBasGauche);
            coordCorpsBasDroite.Add(115f);
            coordCorpsBasDroite.Add(36f);
            hashPartCoord.Add("BasDroite", coordCorpsBasDroite);
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
            hashPartCoord.Add("faceHaut", coordFaceCentre);
            coordFaceGauche.Add(CoordinateScreenResponsive_X(49f, 3840f, 2160f));
            coordFaceGauche.Add(CoordinateScreenResponsive_Y(99f, 2160f, 1080));
            hashPartCoord.Add("faceGauche", coordFaceGauche);
            coordFaceDroite.Add(CoordinateScreenResponsive_X(118f, 3840f, 2160f));
            coordFaceDroite.Add(CoordinateScreenResponsive_Y(110f, 2160f, 1080));
            hashPartCoord.Add("faceDroite", coordFaceDroite);
                // Points des ecailles
            coordEcaillesHaut.Add(CoordinateScreenResponsive_X(86f, 3840f, 2160f));
            coordEcaillesHaut.Add(CoordinateScreenResponsive_Y(92f, 2160f, 1080));
            hashPartCoord.Add("ecaillesHaut", coordEcaillesHaut);
            coordEcaillesArriere.Add(CoordinateScreenResponsive_X(86f, 3840f, 2160f));
            coordEcaillesArriere.Add(CoordinateScreenResponsive_Y(92f, 2160f, 1080));
            hashPartCoord.Add("ecaillesArriere", coordEcaillesArriere);
                // Points du haut du corps
            coordCorpsHautDessus.Add(CoordinateScreenResponsive_X(86f, 3840f, 2160f));
            coordCorpsHautDessus.Add(CoordinateScreenResponsive_Y(60f, 2160f, 1080));
            hashPartCoord.Add("hautDessus", coordCorpsHautDessus);
            coordCorpsHautGauche.Add(CoordinateScreenResponsive_X(56f, 3840f, 2160f));
            coordCorpsHautGauche.Add(CoordinateScreenResponsive_Y(54f, 2160f, 1080));
            hashPartCoord.Add("HautGauche", coordCorpsHautGauche);
            coordCorpsHautBas.Add(CoordinateScreenResponsive_X(113f, 3840f, 2160f));
            coordCorpsHautBas.Add(CoordinateScreenResponsive_Y(53f, 2160f, 1080));
            hashPartCoord.Add("HautBas", coordCorpsHautBas);
            coordCorpsHautDroite.Add(CoordinateScreenResponsive_X(73f, 3840f, 2160f));
            coordCorpsHautDroite.Add(CoordinateScreenResponsive_Y(69f, 2160f, 1080));
            hashPartCoord.Add("HautDroite", coordCorpsHautDroite);
                // Points du bas du corps
            coordCorpsBasGauche.Add(CoordinateScreenResponsive_X(57f, 3840f, 2160f));
            coordCorpsBasGauche.Add(CoordinateScreenResponsive_Y(37f, 2160f, 1080));
            hashPartCoord.Add("BasGauche", coordCorpsBasGauche);
            coordCorpsBasDroite.Add(CoordinateScreenResponsive_X(115f, 3840f, 2160f));
            coordCorpsBasDroite.Add(CoordinateScreenResponsive_Y(36f, 2160f, 1080));
            hashPartCoord.Add("BasDroite", coordCorpsBasDroite);
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
        else if (gameObjectTarget.Equals(scales))
        {
            ecailles.GetComponent<Renderer>().material.mainTexture = texture;
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
