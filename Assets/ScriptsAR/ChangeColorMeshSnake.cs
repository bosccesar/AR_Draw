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

    // Dictionnaire affiliant chaque sous-partie avec le nombre de point de coordonnees
    Dictionary<string, int> hashNbPointInEachSubpart = new Dictionary<string, int>();
    // Dictionnaire affiliant chaque sous-partie avec la partie mere
    Dictionary<string, List<string>> hashSubpartWithHerPart = new Dictionary<string, List<string>>();
    List<string> subPartFace = new List<string>();
    List<string> subPartMuseau = new List<string>();
    List<string> subPartBody = new List<string>();
    List<string> subPartPattes = new List<string>();
    List<string> subPartTete = new List<string>();
    // Dictionnaire d'une liste de coordonnées pour chaque partie du dessin
    Dictionary<string, List<float>> hashPartCoord = new Dictionary<string, List<float>>();
    List<float> coordFaceHaut = new List<float>();
    List<float> coordFaceGauche = new List<float>();
    List<float> coordFaceDroite = new List<float>();
    List<float> coordQueue = new List<float>();
    List<float> coordJambeGauche = new List<float>();
    List<float> coordJambeDroite = new List<float>();
    List<float> coordBrasGauche = new List<float>();
    List<float> coordBrasDroit = new List<float>();
    List<float> coordVentre = new List<float>();
    List<float> coordMuseauCentre = new List<float>();
    List<float> coordPatteGauche = new List<float>();
    List<float> coordPatteDroite = new List<float>();
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

        AddDictionnary();
    }

    // Update is called once per frame
    void Update()
    {
        bool detected = DefaultTrackableEventHandler.detected;
        if (detected)
        {
            if (DefaultTrackableEventHandler.nameTrackable == "Snake")
            {
                // Tableau contenant les 5 parties du dessin du singe
                string[] tabPart = { scales, face, upperBody, lowerBody, head };
                for (int i = 0; i < tabPart.Length; i++)
                {
                    DrawingPart(tabPart[i]);
                }
            }
        }
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
        subPartFace.Add("faceHaut");
        subPartFace.Add("faceGauche");
        subPartFace.Add("faceDroite");
        hashSubpartWithHerPart.Add("face", subPartFace);
        // Corps
        subPartBody.Add("ventre");
        subPartBody.Add("jambeGauche");
        subPartBody.Add("jambeDroite");
        subPartBody.Add("brasGauche");
        subPartBody.Add("brasDroit");
        subPartBody.Add("queue");
        hashSubpartWithHerPart.Add("body", subPartBody);
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

        // 3eme dictionnaire
        // Points de la face
        coordFaceHaut.Add(250f); // x
        coordFaceHaut.Add(208f); // y
        hashPartCoord.Add("faceHaut", coordFaceHaut);
        coordFaceGauche.Add(200f);
        coordFaceGauche.Add(163f);
        hashPartCoord.Add("faceGauche", coordFaceGauche);
        coordFaceDroite.Add(314f);
        coordFaceDroite.Add(182f);
        hashPartCoord.Add("faceDroite", coordFaceDroite);
        // Points du corps
        coordVentre.Add(261f);
        coordVentre.Add(98f);
        hashPartCoord.Add("ventre", coordVentre);
        coordJambeGauche.Add(328f);
        coordJambeGauche.Add(82f);
        hashPartCoord.Add("jambeGauche", coordJambeGauche);
        coordJambeDroite.Add(332f);
        coordJambeDroite.Add(87f);
        hashPartCoord.Add("jambeDroite", coordJambeDroite);
        coordBrasGauche.Add(244f);
        coordBrasGauche.Add(110f);
        hashPartCoord.Add("brasGauche", coordBrasGauche);
        coordBrasDroit.Add(285f);
        coordBrasDroit.Add(115f);
        hashPartCoord.Add("brasDroit", coordBrasDroit);
        coordQueue.Add(189f);
        coordQueue.Add(123f);
        hashPartCoord.Add("queue", coordQueue);
        // Points du museau
        coordMuseauCentre.Add(260f);
        coordMuseauCentre.Add(151f);
        hashPartCoord.Add("museauCentre", coordMuseauCentre);
        // Points des pattes
        coordPatteGauche.Add(217f);
        coordPatteGauche.Add(60f);
        hashPartCoord.Add("patteGauche", coordPatteGauche);
        coordPatteDroite.Add(284f);
        coordPatteDroite.Add(60f);
        hashPartCoord.Add("patteDroite", coordPatteDroite);
        coordMainGauche.Add(244f);
        coordMainGauche.Add(60f);
        hashPartCoord.Add("mainGauche", coordMainGauche);
        coordMainDroite.Add(313f);
        coordMainDroite.Add(60f);
        hashPartCoord.Add("mainDroite", coordMainDroite);
        // Points de la tete
        coordHautCrane.Add(244f);
        coordHautCrane.Add(257f);
        hashPartCoord.Add("hautCrane", coordHautCrane);
        coordOreilleGauche.Add(165f);
        coordOreilleGauche.Add(222f);
        hashPartCoord.Add("oreilleGauche", coordOreilleGauche);
        coordOreilleDroite.Add(346f);
        coordOreilleDroite.Add(222f);
        hashPartCoord.Add("oreilleDroite", coordOreilleDroite);
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
