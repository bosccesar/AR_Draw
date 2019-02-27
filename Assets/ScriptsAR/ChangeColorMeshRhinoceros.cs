using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class ChangeColorMeshRhinoceros : MonoBehaviour
{
    public GameObject visage;
    public GameObject corne;
    public GameObject corpsHaut;
    public GameObject corpsBas;
    public GameObject tete;
    public RenderTextureCamera renderCamera;
    public RawImage rawImage;

    private string face = "face";
    private string horn = "horn";
    private string upperBody = "upperBody";
    private string lowerBody = "lowerBody";
    private string head = "head";
    private string[] tabPart;
    private int nbPointsByFace = 3; // Sous la corne/Joue gauche/Joue droite
    private int nbPointsByHorn = 1; // Milieu de la corne
    private int nbPointsByUpperBody = 5; // Patte avant gauche/Patte avant droite/Patte arriere droite/fesses/Queue
    private int nbPointsByLowerBody = 42; // Ventre avant/ Ventre arriere
    private int nbPointsByHead = 3; // Haut crane/Oreille gauche/Oreille droite
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

        // Tableau contenant les 5 parties du dessin du rhinoceros
        tabPart = new string[] { face, horn, upperBody, lowerBody, head };

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

        // 3eme dictionnaire
            // Points de la face
        coordFaceCentre.Add(440f); // x
        coordFaceCentre.Add(89f); // y
        hashPartCoord.Add("faceCentre", coordFaceCentre);
        coordJoueGauche.Add(398f);
        coordJoueGauche.Add(95f);
        hashPartCoord.Add("joueGauche", coordJoueGauche);
        coordJoueDroite.Add(472f);
        coordJoueDroite.Add(115f);
        hashPartCoord.Add("joueDroite", coordJoueDroite);
            // Points de la corne
        coordCorne.Add(435f);
        coordCorne.Add(110f);
        hashPartCoord.Add("centre", coordCorne);
            // Points du haut du corps
        coordQueue.Add(350f);
        coordQueue.Add(86f);
        hashPartCoord.Add("queue", coordQueue);
        coordCorpsGauche.Add(375f);
        coordCorpsGauche.Add(93f);
        hashPartCoord.Add("corpsGauche", coordCorpsGauche);
        coordPatteGauche.Add(380f);
        coordPatteGauche.Add(65f);
        hashPartCoord.Add("patteGauche", coordPatteGauche);
        coordPatteCentre.Add(415f);
        coordPatteCentre.Add(62f);
        hashPartCoord.Add("patteCentre", coordPatteCentre);
        coordPatteDroite.Add(435f);
        coordPatteDroite.Add(48f);
        hashPartCoord.Add("patteDroite", coordPatteDroite);
            // Points du bas du corps
        coordVentreDevant.Add(436f);
        coordVentreDevant.Add(57f);
        hashPartCoord.Add("ventreDevant", coordVentreDevant);
        coordVentreGauche.Add(397f);
        coordVentreGauche.Add(53f);
        hashPartCoord.Add("ventreGauche", coordVentreGauche);
            // Points de la tete
        coordHautCrane.Add(422f);
        coordHautCrane.Add(143f);
        hashPartCoord.Add("hautCrane", coordHautCrane);
        coordOreilleGauche.Add(376f);
        coordOreilleGauche.Add(138f);
        hashPartCoord.Add("oreilleGauche", coordOreilleGauche);
        coordOreilleDroite.Add(458f);
        coordOreilleDroite.Add(164f);
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

                Color[] pix = text2d.GetPixels(x, y, width, height);
                Texture2D destTex = new Texture2D(width, height);
                destTex.SetPixels(pix);
                destTex.Apply();

                //rawImage.texture = text2d; // Permet de recuperer la texture complete

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
