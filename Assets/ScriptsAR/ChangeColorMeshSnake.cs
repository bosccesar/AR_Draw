using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class ChangeColorMeshSnake : MonoBehaviour
{
    public GameObject visage;
    public GameObject ecailles;
    public GameObject corpsHaut;
    public GameObject corpsBas;
    public GameObject tete;
    public RenderTextureCamera renderCamera;
    public RawImage rawImage;

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
    private int width;
    private int height;
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

        // Tableau contenant les 5 parties du dessin du serpent
        tabPart = new string[] { face, scales, upperBody, lowerBody, head };

        AddDictionnary();
        GoCalcul();
    }

    public void GoCalcul()
    {
        for (int i = 0; i < tabPart.Length; i++)
        {
            DrawingPart(tabPart[i]);
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

        // 3eme dictionnaire
            // Points de la face
        coordFaceCentre.Add(224f); // x
        coordFaceCentre.Add(125f); // y
        hashPartCoord.Add("faceCentre", coordFaceCentre);
        coordFaceGauche.Add(193f);
        coordFaceGauche.Add(123f);
        hashPartCoord.Add("faceGauche", coordFaceGauche);
        coordFaceDroite.Add(258f);
        coordFaceDroite.Add(108f);
        hashPartCoord.Add("faceDroite", coordFaceDroite);
            // Points des ecailles
        coordEcaillesHaut.Add(240f);
        coordEcaillesHaut.Add(164f);
        hashPartCoord.Add("ecaillesHaut", coordEcaillesHaut);
        coordEcaillesArriere.Add(266f);
        coordEcaillesArriere.Add(61f);
        hashPartCoord.Add("ecaillesArriere", coordEcaillesArriere);
            // Points du haut du corps
        coordCorpsHautDessus.Add(230f);
        coordCorpsHautDessus.Add(88f);
        hashPartCoord.Add("hautDessus", coordCorpsHautDessus);
        coordCorpsHautGauche.Add(213f);
        coordCorpsHautGauche.Add(55f);
        hashPartCoord.Add("HautGauche", coordCorpsHautGauche);
        coordCorpsHautBas.Add(240f);
        coordCorpsHautBas.Add(40f);
        hashPartCoord.Add("HautBas", coordCorpsHautBas);
        coordCorpsHautDroite.Add(272f);
        coordCorpsHautDroite.Add(45f);
        hashPartCoord.Add("HautDroite", coordCorpsHautDroite);
            // Points du bas du corps
        coordCorpsBasGauche.Add(218f);
        coordCorpsBasGauche.Add(87f);
        hashPartCoord.Add("BasGauche", coordCorpsBasGauche);
        coordCorpsBasDroite.Add(289f);
        coordCorpsBasDroite.Add(77f);
        hashPartCoord.Add("BasDroite", coordCorpsBasDroite);
            // Points de la tete
        coordHautCrane.Add(238f);
        coordHautCrane.Add(149f);
        hashPartCoord.Add("hautCrane", coordHautCrane);
        coordOreilleGauche.Add(196f);
        coordOreilleGauche.Add(146f);
        hashPartCoord.Add("oreilleGauche", coordOreilleGauche);
        coordOreilleDroite.Add(264f);
        coordOreilleDroite.Add(127f);
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

                //rawImage.texture = text2d; // Permet de recuperer la texture complete

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
