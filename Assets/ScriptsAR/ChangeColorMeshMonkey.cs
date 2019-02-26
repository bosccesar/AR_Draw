using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class ChangeColorMeshMonkey : MonoBehaviour
{
    public GameObject visage;
    public GameObject corps;
    public GameObject museau;
    public GameObject pattes;
    public GameObject tete;
    public RenderTextureCamera renderCamera;
    public RawImage rawImage;
    
    private string face = "face";
    private string body = "body";
    private string muzzle = "muzzle";
    private string paws = "paws";
    private string head = "head";
    private string[] tabPart;
    private int nbPointsByFace = 3; // Front/Joue gauche/Joue droite
    private int nbPointsByBody = 6; // Queue/Jambe gauche/Jambe droite/Bras gauche/Bras droit/Ventre
    private int nbPointsByMuzzle = 1; // Centre
    private int nbPointsByPaws = 4; // Patte gauche/Patte droite/Main gauche/Main droite
    private int nbPointsByHead = 3; // Haut crane/Oreille gauche/Oreille droite
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
    List<string> subPartCorps = new List<string>();
    List<string> subPartMuseau = new List<string>();
    List<string> subPartPattes = new List<string>();
    List<string> subPartTete = new List<string>();
    // Dictionnaire d'une liste de coordonnées pour chaque partie du dessin
    Dictionary<string, List<float>> hashPartCoord = new Dictionary<string, List<float>>();
    List<float> coordFaceHaut = new List<float>();
    List<float> coordFaceGauche = new List<float>();
    List<float> coordFaceDroite = new List<float>();
    List<float> coordVentre = new List<float>();
    List<float> coordJambeGauche = new List<float>();
    List<float> coordJambeDroite = new List<float>();
    List<float> coordBrasGauche = new List<float>();
    List<float> coordBrasDroit = new List<float>();
    List<float> coordQueue = new List<float>();
    List<float> coordMuseauCentre = new List<float>();
    List<float> coordPatteGauche = new List<float>();
    List<float> coordPatteDroite = new List<float>();
    List<float> coordMainGauche = new List<float>();
    List<float> coordMainDroite = new List<float>();
    List<float> coordHautCrane = new List<float>();
    List<float> coordOreilleGauche = new List<float>();
    List<float> coordOreilleDroite = new List<float>();

    // Use this for initialization
    void Start ()
    {
        // Taille du rectangle de récupération des pixels
        sourceRectWidth = 10f;
        sourceRectHeight = 10f;
        width = Mathf.FloorToInt(sourceRectWidth);
        height = Mathf.FloorToInt(sourceRectHeight);

        //// Dimensions de l'ecran de prise des coordonnees
        //computeurWidth = 3840f;
        //computeurHeight = 2160f;
        //// Dimensions du telephone cible
        //phoneTargetWidth = Screen.width; // 1080
        //phoneTargetHeight = Screen.height; // 1920

         // Tableau contenant les 5 parties du dessin du singe
         tabPart = new string[] { face, body, muzzle, paws, head };

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
                if (DefaultTrackableEventHandler.nameTrackable == "Monkey" && noStopCalcul)
                {
                    print(Screen.currentResolution);
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
        #else
            isUnityEditor = true;
        #endif
        return isUnityEditor;
    }

    private void AddDictionnary()
    {
        // 1er dictionnaire
        hashNbPointInEachSubpart.Add(face, nbPointsByFace);
        hashNbPointInEachSubpart.Add(body, nbPointsByBody);
        hashNbPointInEachSubpart.Add(muzzle, nbPointsByMuzzle);
        hashNbPointInEachSubpart.Add(paws, nbPointsByPaws);
        hashNbPointInEachSubpart.Add(head, nbPointsByHead);

        // 2eme dictionnaire
            // Face
        subPartFace.Add("faceHaut");
        subPartFace.Add("faceGauche");
        subPartFace.Add("faceDroite");
        hashSubpartWithHerPart.Add("face", subPartFace);
        // Corps
        subPartCorps.Add("ventre");
        subPartCorps.Add("jambeGauche");
        subPartCorps.Add("jambeDroite");
        subPartCorps.Add("brasGauche");
        subPartCorps.Add("brasDroit");
        subPartCorps.Add("queue");
        hashSubpartWithHerPart.Add("body", subPartCorps);
            // Museau
        subPartMuseau.Add("museauCentre");
        hashSubpartWithHerPart.Add("muzzle", subPartMuseau);
            // Pattes
        subPartPattes.Add("patteGauche");
        subPartPattes.Add("patteDroite");
        subPartPattes.Add("mainGauche");
        subPartPattes.Add("mainDroite");
        hashSubpartWithHerPart.Add("paws", subPartPattes);
            // tete
        subPartTete.Add("hautCrane");
        subPartTete.Add("oreilleGauche");
        subPartTete.Add("oreilleDroite");
        hashSubpartWithHerPart.Add("head", subPartTete);

        if (unityEditor)
        {
            // 3eme dictionnaire
                // Points de la face
            coordFaceHaut.Add(78f); // x
            coordFaceHaut.Add(126f); // y
            hashPartCoord.Add("faceHaut", coordFaceHaut);
            coordFaceGauche.Add(49f);
            coordFaceGauche.Add(99f);
            hashPartCoord.Add("faceGauche", coordFaceGauche);
            coordFaceDroite.Add(118f);
            coordFaceDroite.Add(110f);
            hashPartCoord.Add("faceDroite", coordFaceDroite);
                // Points du corps
            coordVentre.Add(86f);
            coordVentre.Add(60f);
            hashPartCoord.Add("ventre", coordVentre);
            coordJambeGauche.Add(56f);
            coordJambeGauche.Add(54f);
            hashPartCoord.Add("jambeGauche", coordJambeGauche);
            coordJambeDroite.Add(113f);
            coordJambeDroite.Add(53f);
            hashPartCoord.Add("jambeDroite", coordJambeDroite);
            coordBrasGauche.Add(73f);
            coordBrasGauche.Add(69f);
            hashPartCoord.Add("brasGauche", coordBrasGauche);
            coordBrasDroit.Add(100f);
            coordBrasDroit.Add(69f);
            hashPartCoord.Add("brasDroit", coordBrasDroit);
            coordQueue.Add(39f);
            coordQueue.Add(75f);
            hashPartCoord.Add("queue", coordQueue);
                // Points du museau
            coordMuseauCentre.Add(86f);
            coordMuseauCentre.Add(92f);
            hashPartCoord.Add("museauCentre", coordMuseauCentre);
                // Points des pattes
            coordPatteGauche.Add(57f);
            coordPatteGauche.Add(37f);
            hashPartCoord.Add("patteGauche", coordPatteGauche);
            coordPatteDroite.Add(115f);
            coordPatteDroite.Add(33f);
            hashPartCoord.Add("patteDroite", coordPatteDroite);
            coordMainGauche.Add(74f);
            coordMainGauche.Add(33f);
            hashPartCoord.Add("mainGauche", coordMainGauche);
            coordMainDroite.Add(98f);
            coordMainDroite.Add(33f);
            hashPartCoord.Add("mainDroite", coordMainDroite);
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
            coordFaceHaut.Add(78f); // x
            coordFaceHaut.Add(126f); // y
            hashPartCoord.Add("faceHaut", coordFaceHaut);
            coordFaceGauche.Add(49f);
            coordFaceGauche.Add(99f);
            hashPartCoord.Add("faceGauche", coordFaceGauche);
            coordFaceDroite.Add(118f);
            coordFaceDroite.Add(110f);
            hashPartCoord.Add("faceDroite", coordFaceDroite);
            // Points du corps
            coordVentre.Add(86f);
            coordVentre.Add(60f);
            hashPartCoord.Add("ventre", coordVentre);
            coordJambeGauche.Add(56f);
            coordJambeGauche.Add(54f);
            hashPartCoord.Add("jambeGauche", coordJambeGauche);
            coordJambeDroite.Add(113f);
            coordJambeDroite.Add(53f);
            hashPartCoord.Add("jambeDroite", coordJambeDroite);
            coordBrasGauche.Add(73f);
            coordBrasGauche.Add(69f);
            hashPartCoord.Add("brasGauche", coordBrasGauche);
            coordBrasDroit.Add(100f);
            coordBrasDroit.Add(69f);
            hashPartCoord.Add("brasDroit", coordBrasDroit);
            coordQueue.Add(39f);
            coordQueue.Add(75f);
            hashPartCoord.Add("queue", coordQueue);
            // Points du museau
            coordMuseauCentre.Add(86f);
            coordMuseauCentre.Add(92f);
            hashPartCoord.Add("museauCentre", coordMuseauCentre);
            // Points des pattes
            coordPatteGauche.Add(57f);
            coordPatteGauche.Add(37f);
            hashPartCoord.Add("patteGauche", coordPatteGauche);
            coordPatteDroite.Add(115f);
            coordPatteDroite.Add(33f);
            hashPartCoord.Add("patteDroite", coordPatteDroite);
            coordMainGauche.Add(74f);
            coordMainGauche.Add(33f);
            hashPartCoord.Add("mainGauche", coordMainGauche);
            coordMainDroite.Add(98f);
            coordMainDroite.Add(33f);
            hashPartCoord.Add("mainDroite", coordMainDroite);
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

            //// 3eme dictionnaire
            //// Points de la face
            //coordFaceHaut.Add(CoordinateScreenResponsive_X(78f, computeurWidth, phoneTargetWidth)); // x
            //coordFaceHaut.Add(CoordinateScreenResponsive_Y(126f, computeurHeight, phoneTargetHeight)); // y
            //hashPartCoord.Add("faceHaut", coordFaceHaut);
            //coordFaceGauche.Add(CoordinateScreenResponsive_X(49f, computeurWidth, phoneTargetWidth));
            //coordFaceGauche.Add(CoordinateScreenResponsive_Y(99f, computeurHeight, phoneTargetHeight));
            //hashPartCoord.Add("faceGauche", coordFaceGauche);
            //coordFaceDroite.Add(CoordinateScreenResponsive_X(118f, computeurWidth, phoneTargetWidth));
            //coordFaceDroite.Add(CoordinateScreenResponsive_Y(110f, computeurHeight, phoneTargetHeight));
            //hashPartCoord.Add("faceDroite", coordFaceDroite);
            //// Points du corps
            //coordVentre.Add(CoordinateScreenResponsive_X(86f, computeurWidth, phoneTargetWidth));
            //coordVentre.Add(CoordinateScreenResponsive_Y(60f, computeurHeight, phoneTargetHeight));
            //hashPartCoord.Add("ventre", coordVentre);
            //coordJambeGauche.Add(CoordinateScreenResponsive_X(56f, computeurWidth, phoneTargetWidth));
            //coordJambeGauche.Add(CoordinateScreenResponsive_Y(54f, computeurHeight, phoneTargetHeight));
            //hashPartCoord.Add("jambeGauche", coordJambeGauche);
            //coordJambeDroite.Add(CoordinateScreenResponsive_X(113f, computeurWidth, phoneTargetWidth));
            //coordJambeDroite.Add(CoordinateScreenResponsive_Y(53f, computeurHeight, phoneTargetHeight));
            //hashPartCoord.Add("jambeDroite", coordJambeDroite);
            //coordBrasGauche.Add(CoordinateScreenResponsive_X(73f, computeurWidth, phoneTargetWidth));
            //coordBrasGauche.Add(CoordinateScreenResponsive_Y(69f, computeurHeight, phoneTargetHeight));
            //hashPartCoord.Add("brasGauche", coordBrasGauche);
            //coordBrasDroit.Add(CoordinateScreenResponsive_X(100f, computeurWidth, phoneTargetWidth));
            //coordBrasDroit.Add(CoordinateScreenResponsive_Y(69f, computeurHeight, phoneTargetHeight));
            //hashPartCoord.Add("brasDroit", coordBrasDroit);
            //coordQueue.Add(CoordinateScreenResponsive_X(39f, computeurWidth, phoneTargetWidth));
            //coordQueue.Add(CoordinateScreenResponsive_Y(75f, computeurHeight, phoneTargetHeight));
            //hashPartCoord.Add("queue", coordQueue);
            //// Points du museau
            //coordMuseauCentre.Add(CoordinateScreenResponsive_X(86f, computeurWidth, phoneTargetWidth));
            //coordMuseauCentre.Add(CoordinateScreenResponsive_Y(92f, computeurHeight, phoneTargetHeight));
            //hashPartCoord.Add("museauCentre", coordMuseauCentre);
            //// Points des pattes
            //coordPatteGauche.Add(CoordinateScreenResponsive_X(57f, computeurWidth, phoneTargetWidth));
            //coordPatteGauche.Add(CoordinateScreenResponsive_Y(37f, computeurHeight, phoneTargetHeight));
            //hashPartCoord.Add("patteGauche", coordPatteGauche);
            //coordPatteDroite.Add(CoordinateScreenResponsive_X(115f, computeurWidth, phoneTargetWidth));
            //coordPatteDroite.Add(CoordinateScreenResponsive_Y(33f, computeurHeight, phoneTargetHeight));
            //hashPartCoord.Add("patteDroite", coordPatteDroite);
            //coordMainGauche.Add(CoordinateScreenResponsive_X(74f, computeurWidth, phoneTargetWidth));
            //coordMainGauche.Add(CoordinateScreenResponsive_Y(33f, computeurHeight, phoneTargetHeight));
            //hashPartCoord.Add("mainGauche", coordMainGauche);
            //coordMainDroite.Add(CoordinateScreenResponsive_X(98f, computeurWidth, phoneTargetWidth));
            //coordMainDroite.Add(CoordinateScreenResponsive_Y(33f, computeurHeight, phoneTargetHeight));
            //hashPartCoord.Add("mainDroite", coordMainDroite);
            //// Points de la tete
            //coordHautCrane.Add(CoordinateScreenResponsive_X(72f, computeurWidth, phoneTargetWidth));
            //coordHautCrane.Add(CoordinateScreenResponsive_Y(159f, computeurHeight, phoneTargetHeight));
            //hashPartCoord.Add("hautCrane", coordHautCrane);
            //coordOreilleGauche.Add(CoordinateScreenResponsive_X(23f, computeurWidth, phoneTargetWidth));
            //coordOreilleGauche.Add(CoordinateScreenResponsive_Y(132f, computeurHeight, phoneTargetHeight));
            //hashPartCoord.Add("oreilleGauche", coordOreilleGauche);
            //coordOreilleDroite.Add(CoordinateScreenResponsive_X(136f, computeurWidth, phoneTargetWidth));
            //coordOreilleDroite.Add(CoordinateScreenResponsive_Y(133f, computeurHeight, phoneTargetHeight));
            //hashPartCoord.Add("oreilleDroite", coordOreilleDroite);

            //TEST 1 avec resolution  computeurWidth = 2095f; computeurHeight = 1045f;

            //// Points de la face
            //coordFaceHaut.Add(CoordinateScreen(78f, phoneTargetHeight, phoneTargetWidth)); // x
            //coordFaceHaut.Add(CoordinateScreen(126f, phoneTargetHeight, phoneTargetWidth)); // y
            //hashPartCoord.Add("faceHaut", coordFaceHaut);
            //coordFaceGauche.Add(CoordinateScreen(49f, phoneTargetHeight, phoneTargetWidth));
            //coordFaceGauche.Add(CoordinateScreen(99f, phoneTargetHeight, phoneTargetWidth));
            //hashPartCoord.Add("faceGauche", coordFaceGauche);
            //coordFaceDroite.Add(CoordinateScreen(118f, phoneTargetHeight, phoneTargetWidth));
            //coordFaceDroite.Add(CoordinateScreen(110f, phoneTargetHeight, phoneTargetWidth));
            //hashPartCoord.Add("faceDroite", coordFaceDroite);
            //// Points du corps
            //coordVentre.Add(CoordinateScreen(86f, phoneTargetHeight, phoneTargetWidth));
            //coordVentre.Add(CoordinateScreen(60f, phoneTargetHeight, phoneTargetWidth));
            //hashPartCoord.Add("ventre", coordVentre);
            //coordJambeGauche.Add(CoordinateScreen(56f, phoneTargetHeight, phoneTargetWidth));
            //coordJambeGauche.Add(CoordinateScreen(54f, phoneTargetHeight, phoneTargetWidth));
            //hashPartCoord.Add("jambeGauche", coordJambeGauche);
            //coordJambeDroite.Add(CoordinateScreen(113f, phoneTargetHeight, phoneTargetWidth));
            //coordJambeDroite.Add(CoordinateScreen(53f, phoneTargetHeight, phoneTargetWidth));
            //hashPartCoord.Add("jambeDroite", coordJambeDroite);
            //coordBrasGauche.Add(CoordinateScreen(73f, phoneTargetHeight, phoneTargetWidth));
            //coordBrasGauche.Add(CoordinateScreen(69f, phoneTargetHeight, phoneTargetWidth));
            //hashPartCoord.Add("brasGauche", coordBrasGauche);
            //coordBrasDroit.Add(CoordinateScreen(100f, phoneTargetHeight, phoneTargetWidth));
            //coordBrasDroit.Add(CoordinateScreen(69f, phoneTargetHeight, phoneTargetWidth));
            //hashPartCoord.Add("brasDroit", coordBrasDroit);
            //coordQueue.Add(CoordinateScreen(39f, phoneTargetHeight, phoneTargetWidth));
            //coordQueue.Add(CoordinateScreen(75f, phoneTargetHeight, phoneTargetWidth));
            //hashPartCoord.Add("queue", coordQueue);
            //// Points du museau
            //coordMuseauCentre.Add(CoordinateScreen(86f, phoneTargetHeight, phoneTargetWidth));
            //coordMuseauCentre.Add(CoordinateScreen(92f, phoneTargetHeight, phoneTargetWidth));
            //hashPartCoord.Add("museauCentre", coordMuseauCentre);
            //// Points des pattes
            //coordPatteGauche.Add(CoordinateScreen(57f, phoneTargetHeight, phoneTargetWidth));
            //coordPatteGauche.Add(CoordinateScreen(37f, phoneTargetHeight, phoneTargetWidth));
            //hashPartCoord.Add("patteGauche", coordPatteGauche);
            //coordPatteDroite.Add(CoordinateScreen(115f, phoneTargetHeight, phoneTargetWidth));
            //coordPatteDroite.Add(CoordinateScreen(33f, phoneTargetHeight, phoneTargetWidth));
            //hashPartCoord.Add("patteDroite", coordPatteDroite);
            //coordMainGauche.Add(CoordinateScreen(74f, phoneTargetHeight, phoneTargetWidth));
            //coordMainGauche.Add(CoordinateScreen(33f, phoneTargetHeight, phoneTargetWidth));
            //hashPartCoord.Add("mainGauche", coordMainGauche);
            //coordMainDroite.Add(CoordinateScreen(98f, phoneTargetHeight, phoneTargetWidth));
            //coordMainDroite.Add(CoordinateScreen(33f, phoneTargetHeight, phoneTargetWidth));
            //hashPartCoord.Add("mainDroite", coordMainDroite);
            //// Points de la tete
            //coordHautCrane.Add(CoordinateScreen(72f, phoneTargetHeight, phoneTargetWidth));
            //coordHautCrane.Add(CoordinateScreen(159f, phoneTargetHeight, phoneTargetWidth));
            //hashPartCoord.Add("hautCrane", coordHautCrane);
            //coordOreilleGauche.Add(CoordinateScreen(23f, phoneTargetHeight, phoneTargetWidth));
            //coordOreilleGauche.Add(CoordinateScreen(132f, phoneTargetHeight, phoneTargetWidth));
            //hashPartCoord.Add("oreilleGauche", coordOreilleGauche);
            //coordOreilleDroite.Add(CoordinateScreen(136f, phoneTargetHeight, phoneTargetWidth));
            //coordOreilleDroite.Add(CoordinateScreen(133f, phoneTargetHeight, phoneTargetWidth));
            //hashPartCoord.Add("oreilleDroite", coordOreilleDroite);
        }
    }

    //private float CoordinateScreenResponsive_X(float coordinateComputerX, float computerWidth, float targetWidth)
    //{
    //    float coordinateUpdate;
    //    coordinateUpdate = (coordinateComputerX * targetWidth) / computerWidth;
    //    print(coordinateUpdate);
    //    return coordinateUpdate;
    //}

    //private float CoordinateScreenResponsive_Y(float coordinateComputerY, float computerHeight, float targetHeight)
    //{
    //    float coordinateUpdate;
    //    coordinateUpdate = (coordinateComputerY * targetHeight) / computerHeight;
    //    print(coordinateUpdate);
    //    return coordinateUpdate;
    //}

    ////TEST 1
    //private float CoordinateScreen(float coordinateComputer, float targetHeight, float targetWidth)
    //{
    //    float coordinateUpdate;
    //    coordinateUpdate = (targetHeight / targetWidth) * coordinateComputer;
    //    return coordinateUpdate;
    //}

    void DrawingPart(string drawingPart)
    {
        int nbPart = hashNbPointInEachSubpart[drawingPart];
        Texture2D[] tabTextures = new Texture2D[nbPart];
        List<string> listSubPart = hashSubpartWithHerPart[drawingPart];
        foreach (string subPart in listSubPart)
        {
            cpt = 0;
            List<float> coordonnee = hashPartCoord[subPart];
            StartCoroutine(renderCamera.GetTexture2D(text2d =>
            {
                int x = Mathf.FloorToInt(coordonnee[0]);
                int y = Mathf.FloorToInt(coordonnee[1]);

                // rawImage.texture = text2d; // Permet de recuperer la texture complete

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
                }else
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
        else if (gameObjectTarget.Equals(body))
        {
            corps.GetComponent<Renderer>().material.mainTexture = texture;
        }
        else if (gameObjectTarget.Equals(muzzle))
        {
            museau.GetComponent<Renderer>().material.mainTexture = texture;
        }
        else if (gameObjectTarget.Equals(paws))
        {
            pattes.GetComponent<Renderer>().material.mainTexture = texture;
        }
        else if (gameObjectTarget.Equals(head))
        {
            tete.GetComponent<Renderer>().material.mainTexture = texture;
        }
    }

    void WithoutColorGameObject()
    {
        visage.GetComponent<Renderer>().material.mainTexture = Texture2D.whiteTexture;
        corps.GetComponent<Renderer>().material.mainTexture = Texture2D.whiteTexture;
        museau.GetComponent<Renderer>().material.mainTexture = Texture2D.whiteTexture;
        pattes.GetComponent<Renderer>().material.mainTexture = Texture2D.whiteTexture;
        tete.GetComponent<Renderer>().material.mainTexture = Texture2D.whiteTexture;
    }
}
