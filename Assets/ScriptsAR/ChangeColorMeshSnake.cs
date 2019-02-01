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
    private string[] tabPart;
    private int nbPointsByFace = 3; // Milieu/Joue gauche/Joue droite
    private int nbPointsByScales = 2; // En haut de la tete/Corps haut
    private int nbPointsByUpperBody = 4; // Dessous la tete/Gauche/Droite/Bas
    private int nbPointsByLowerBody = 2; // Gauche/Droite
    private int nbPointsByHead = 3; // Haut crane/Oeil gauche/Oeil droit
    private int cpt;
    private int cptDetected;
    private int width;
    private int height;
    private bool unityEditor;
    private bool noStopCalcul;
    private float sourceRectWidth;
    private float sourceRectHeight;
    private float computeurWidth;
    private float computeurHeight;
    private float phoneTargetWidth;
    private float phoneTargetHeight;

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
        width = Mathf.FloorToInt(sourceRectWidth);
        height = Mathf.FloorToInt(sourceRectHeight);

        // Dimensions de l'ecran de prise des coordonnes
        computeurWidth = 2095f;
        computeurHeight = 1045f;
        // Dimensions du telephone cible
        phoneTargetWidth = Screen.width;
        phoneTargetHeight = Screen.height;

        // Tableau contenant les 5 parties du dessin du serpent
        tabPart = new string[] { face, scales, upperBody, lowerBody, head };

        unityEditor = targetDevice();
        AddDictionnary();
    }

    // Update is called once per frame
    void Update()
    {
        bool detected = DefaultTrackableEventHandler.detected;
        if (detected)
        {
            if (cptDetected < 20)
            {
                noStopCalcul = true;
                if (DefaultTrackableEventHandler.nameTrackable == "Snake" && noStopCalcul)
                {
                    cptDetected++;
                    for (int i = 0; i < tabPart.Length; i++)
                    {
                        DrawingPart(tabPart[i]);
                    }
                    noStopCalcul = false;
                }
            }
        }
        else
        {
            cptDetected = 0;
            WithoutColorGameObject();
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
            coordFaceCentre.Add(230f); // x
            coordFaceCentre.Add(116f); // y
            hashPartCoord.Add("faceCentre", coordFaceCentre);
            coordFaceGauche.Add(195f);
            coordFaceGauche.Add(119f);
            hashPartCoord.Add("faceGauche", coordFaceGauche);
            coordFaceDroite.Add(262f);
            coordFaceDroite.Add(103f);
            hashPartCoord.Add("faceDroite", coordFaceDroite);
                // Points des ecailles
            coordEcaillesHaut.Add(244f);
            coordEcaillesHaut.Add(164f);
            hashPartCoord.Add("ecaillesHaut", coordEcaillesHaut);
            coordEcaillesArriere.Add(270f);
            coordEcaillesArriere.Add(59f);
            hashPartCoord.Add("ecaillesArriere", coordEcaillesArriere);
                // Points du haut du corps
            coordCorpsHautDessus.Add(230f);
            coordCorpsHautDessus.Add(77f);
            hashPartCoord.Add("hautDessus", coordCorpsHautDessus);
            coordCorpsHautGauche.Add(213f);
            coordCorpsHautGauche.Add(47f);
            hashPartCoord.Add("HautGauche", coordCorpsHautGauche);
            coordCorpsHautBas.Add(242f);
            coordCorpsHautBas.Add(35f);
            hashPartCoord.Add("HautBas", coordCorpsHautBas);
            coordCorpsHautDroite.Add(275f);
            coordCorpsHautDroite.Add(39f);
            hashPartCoord.Add("HautDroite", coordCorpsHautDroite);
                // Points du bas du corps
            coordCorpsBasGauche.Add(218f);
            coordCorpsBasGauche.Add(80f);
            hashPartCoord.Add("BasGauche", coordCorpsBasGauche);
            coordCorpsBasDroite.Add(288f);
            coordCorpsBasDroite.Add(69f);
            hashPartCoord.Add("BasDroite", coordCorpsBasDroite);
                // Points de la tete
            coordHautCrane.Add(240f);
            coordHautCrane.Add(147f);
            hashPartCoord.Add("hautCrane", coordHautCrane);
            coordOreilleGauche.Add(198f);
            coordOreilleGauche.Add(144f);
            hashPartCoord.Add("oreilleGauche", coordOreilleGauche);
            coordOreilleDroite.Add(267f);
            coordOreilleDroite.Add(126f);
            hashPartCoord.Add("oreilleDroite", coordOreilleDroite);
        }
        else
        {
            // 3eme dictionnaire
                // Points de la face
            coordFaceCentre.Add(CoordinateScreenResponsive_X(230f, computeurWidth, phoneTargetWidth)); // x
            coordFaceCentre.Add(CoordinateScreenResponsive_Y(116f, computeurHeight, phoneTargetHeight)); // y
            hashPartCoord.Add("faceHaut", coordFaceCentre);
            coordFaceGauche.Add(CoordinateScreenResponsive_X(195f, computeurWidth, phoneTargetWidth));
            coordFaceGauche.Add(CoordinateScreenResponsive_Y(119f, computeurHeight, phoneTargetHeight));
            hashPartCoord.Add("faceGauche", coordFaceGauche);
            coordFaceDroite.Add(CoordinateScreenResponsive_X(262f, computeurWidth, phoneTargetWidth));
            coordFaceDroite.Add(CoordinateScreenResponsive_Y(103f, computeurHeight, phoneTargetHeight));
            hashPartCoord.Add("faceDroite", coordFaceDroite);
                // Points des ecailles
            coordEcaillesHaut.Add(CoordinateScreenResponsive_X(244f, computeurWidth, phoneTargetWidth));
            coordEcaillesHaut.Add(CoordinateScreenResponsive_Y(164f, computeurHeight, phoneTargetHeight));
            hashPartCoord.Add("ecaillesHaut", coordEcaillesHaut);
            coordEcaillesArriere.Add(CoordinateScreenResponsive_X(270f, computeurWidth, phoneTargetWidth));
            coordEcaillesArriere.Add(CoordinateScreenResponsive_Y(59f, computeurHeight, phoneTargetHeight));
            hashPartCoord.Add("ecaillesArriere", coordEcaillesArriere);
                // Points du haut du corps
            coordCorpsHautDessus.Add(CoordinateScreenResponsive_X(230f, computeurWidth, phoneTargetWidth));
            coordCorpsHautDessus.Add(CoordinateScreenResponsive_Y(77f, computeurHeight, phoneTargetHeight));
            hashPartCoord.Add("hautDessus", coordCorpsHautDessus);
            coordCorpsHautGauche.Add(CoordinateScreenResponsive_X(213f, computeurWidth, phoneTargetWidth));
            coordCorpsHautGauche.Add(CoordinateScreenResponsive_Y(47f, computeurHeight, phoneTargetHeight));
            hashPartCoord.Add("HautGauche", coordCorpsHautGauche);
            coordCorpsHautBas.Add(CoordinateScreenResponsive_X(242f, computeurWidth, phoneTargetWidth));
            coordCorpsHautBas.Add(CoordinateScreenResponsive_Y(35f, computeurHeight, phoneTargetHeight));
            hashPartCoord.Add("HautBas", coordCorpsHautBas);
            coordCorpsHautDroite.Add(CoordinateScreenResponsive_X(275f, computeurWidth, phoneTargetWidth));
            coordCorpsHautDroite.Add(CoordinateScreenResponsive_Y(39f, computeurHeight, phoneTargetHeight));
            hashPartCoord.Add("HautDroite", coordCorpsHautDroite);
                // Points du bas du corps
            coordCorpsBasGauche.Add(CoordinateScreenResponsive_X(218f, computeurWidth, phoneTargetWidth));
            coordCorpsBasGauche.Add(CoordinateScreenResponsive_Y(80f, computeurHeight, phoneTargetHeight));
            hashPartCoord.Add("BasGauche", coordCorpsBasGauche);
            coordCorpsBasDroite.Add(CoordinateScreenResponsive_X(288f, computeurWidth, phoneTargetWidth));
            coordCorpsBasDroite.Add(CoordinateScreenResponsive_Y(69f, computeurHeight, phoneTargetHeight));
            hashPartCoord.Add("BasDroite", coordCorpsBasDroite);
                // Points de la tete
            coordHautCrane.Add(CoordinateScreenResponsive_X(240f, computeurWidth, phoneTargetWidth));
            coordHautCrane.Add(CoordinateScreenResponsive_Y(147f, computeurHeight, phoneTargetHeight));
            hashPartCoord.Add("hautCrane", coordHautCrane);
            coordOreilleGauche.Add(CoordinateScreenResponsive_X(198f, computeurWidth, phoneTargetWidth));
            coordOreilleGauche.Add(CoordinateScreenResponsive_Y(144f, computeurHeight, phoneTargetHeight));
            hashPartCoord.Add("oreilleGauche", coordOreilleGauche);
            coordOreilleDroite.Add(CoordinateScreenResponsive_X(267f, computeurWidth, phoneTargetWidth));
            coordOreilleDroite.Add(CoordinateScreenResponsive_Y(126f, computeurHeight, phoneTargetHeight));
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

    void WithoutColorGameObject()
    {
        visage.GetComponent<Renderer>().material.mainTexture = Texture2D.whiteTexture;
        ecailles.GetComponent<Renderer>().material.mainTexture = Texture2D.whiteTexture;
        corpsHaut.GetComponent<Renderer>().material.mainTexture = Texture2D.whiteTexture;
        corpsBas.GetComponent<Renderer>().material.mainTexture = Texture2D.whiteTexture;
        tete.GetComponent<Renderer>().material.mainTexture = Texture2D.whiteTexture;
    }
}
