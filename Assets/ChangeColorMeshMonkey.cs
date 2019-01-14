using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ChangeColorMeshMonkey : MonoBehaviour {
    public GameObject visage;
    public GameObject corps;
    public GameObject museau;
    public GameObject pattes;
    public GameObject tete;

    public RenderTextureCamera renderCamera;
    //public Rect sourceRect;
    private string face = "face";
    private string body = "body";
    private string muzzle = "muzzle";
    private string paws = "paws";
    private string head = "head";
    private int nbPointsByFace = 3; // Front/Joue gauche/Joue droite
    private int nbPointsByBody = 6; // Queue/Jambe gauche/Jambe droite/Bras gauche/Bras droit/Ventre
    private int nbPointsByMuzzle = 1; // Centre
    private int nbPointsByPaws = 5; // Patte gauche/Patte droite/Main gauche/Main droite/Bas ventre
    private int nbPointsByHead = 3; // Haut crane/Oreille gauche/Oreille droite
    private float sourceRectWidth;
    private float sourceRectHeight;

    // Dictionnaire affiliant chaque sous-partie avec le nombre de point de coordonnees
    Dictionary<string, int> hashNbPointInEachSubpart = new Dictionary<string, int>();
    // Dictionnaire d'une liste de coordonnées pour chaque partie du dessin
    Dictionary<string, List<float>> hashPartCoord = new Dictionary<string, List<float>>();
    List<float> coord = new List<float>();
    // Dictionnaire affiliant chaque sous-partie avec la partie mere
    Dictionary<string, List<string>> hashSubpartWithHerPart = new Dictionary<string, List<string>>();
    List<string> subPart = new List<string>();

    // Use this for initialization
    void Start ()
    {
        // Taille du rectangle de récupération des pixels
        sourceRectWidth = 25f;
        sourceRectHeight = 25f;

        AddDictionnary();
    }

    // Update is called once per frame
    void Update ()
    {
        // Tableau contenant les 5 parties du dessin
        string[] tabPart = { face, body, muzzle, paws, head };
        for (int i = 0; i < tabPart.Length; i++)
        {
            DrawingPart(tabPart[i]);
        }
    }

    void AddDictionnary()
    {
        // 1er dictionnaire
        hashNbPointInEachSubpart.Add(face, nbPointsByFace);
        hashNbPointInEachSubpart.Add(body, nbPointsByBody);
        hashNbPointInEachSubpart.Add(muzzle, nbPointsByMuzzle);
        hashNbPointInEachSubpart.Add(paws, nbPointsByPaws);
        hashNbPointInEachSubpart.Add(head, nbPointsByHead);
        
        // 2eme dictionnaire
            // Points de la face
        coord.Add(256f); // x
        coord.Add(200f); // y
        hashPartCoord.Add("faceHaut", coord);
        coord.Clear();
        coord.Add(200f);
        coord.Add(180f);
        hashPartCoord.Add("faceGauche", coord);
        coord.Clear();
        coord.Add(300f);
        coord.Add(180f);
        hashPartCoord.Add("faceDroite", coord);
        coord.Clear();
            // Points du corps
        coord.Add(256f);
        coord.Add(200f);
        hashPartCoord.Add("queue", coord);
        coord.Clear();
        coord.Add(256f);
        coord.Add(200f);
        hashPartCoord.Add("jambeGauche", coord);
        coord.Clear();
        coord.Add(256f);
        coord.Add(200f);
        hashPartCoord.Add("jambeDroite", coord);
        coord.Clear();
        coord.Add(256f);
        coord.Add(200f);
        hashPartCoord.Add("brasGauche", coord);
        coord.Clear();
        coord.Add(256f);
        coord.Add(200f);
        hashPartCoord.Add("brasDroit", coord);
        coord.Clear();
        coord.Add(256f);
        coord.Add(200f);
        hashPartCoord.Add("ventre", coord);
        coord.Clear();
            // Points du museau
        coord.Add(256f);
        coord.Add(139f);
        hashPartCoord.Add("centre", coord);
        coord.Clear();
        // Points des pattes
        coord.Add(256f);
        coord.Add(200f);
        hashPartCoord.Add("patteGauche", coord);
        coord.Clear();
        coord.Add(256f);
        coord.Add(200f);
        hashPartCoord.Add("patteDroite", coord);
        coord.Clear();
        coord.Add(256f);
        coord.Add(200f);
        hashPartCoord.Add("mainGauche", coord);
        coord.Clear();
        coord.Add(256f);
        coord.Add(200f);
        hashPartCoord.Add("mainDroite", coord);
        coord.Clear();
        coord.Add(256f);
        coord.Add(200f);
        hashPartCoord.Add("basVentre", coord);
        coord.Clear();
            // Points de la tete
        coord.Add(256f);
        coord.Add(200f);
        hashPartCoord.Add("hautCrane", coord);
        coord.Clear();
        coord.Add(256f);
        coord.Add(200f);
        hashPartCoord.Add("oreilleGauche", coord);
        coord.Clear();
        coord.Add(256f);
        coord.Add(200f);
        hashPartCoord.Add("oreilleDroite", coord);
        coord.Clear();

        // 3eme dictionnaire
            // Face
        subPart.Add("faceHaut");
        subPart.Add("faceGauche");
        subPart.Add("faceDroite");
        hashSubpartWithHerPart.Add("face", subPart);
        subPart.Clear();
            // Corps
        subPart.Add("queue");
        subPart.Add("jambeGauche");
        subPart.Add("jambeDroite");
        subPart.Add("brasGauche");
        subPart.Add("brasDroit");
        subPart.Add("ventre");
        hashSubpartWithHerPart.Add("body", subPart);
        subPart.Clear();
            // Museau
        subPart.Add("centre");
        hashSubpartWithHerPart.Add("muzzle", subPart);
        subPart.Clear();
            // Pattes
        subPart.Add("patteGauche");
        subPart.Add("patteDroite");
        subPart.Add("mainGauche");
        subPart.Add("mainDroite");
        subPart.Add("basVentre");
        hashSubpartWithHerPart.Add("paws", subPart);
        subPart.Clear();
            // tete
        subPart.Add("hautCrane");
        subPart.Add("oreilleGauche");
        subPart.Add("oreilleDroite");
        hashSubpartWithHerPart.Add("head", subPart);
        subPart.Clear();
    }

    void DrawingPart(string drawingPart)
    {
        int nbPart = hashNbPointInEachSubpart[drawingPart];
        //for (int i = 0; i < nbPart; i++)
        //{
            Texture2D[] tabTextures = new Texture2D[nbPart];
            List<string> listSubPart = hashSubpartWithHerPart[drawingPart];
            foreach (string subPart in listSubPart)
            {
                tabTextures[listSubPart.IndexOf(subPart)] = RecupPixels(hashPartCoord[subPart], drawingPart);
            }
            Texture2D targetTexture = tabTextures[0];
            for (int i = 1; i < nbPart; i++)
            {
                if(tabTextures[i] != tabTextures[i-1])
                {
                    targetTexture = tabTextures[i];
                }
            }
            DrawingGameObjectTarget(drawingPart, targetTexture);
        //}
    }

    Texture2D RecupPixels(List<float> coordonnee, string gameObjectTarget)
    {
        Texture2D desTexReturn = null;
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
            desTexReturn = destTex;

            // Set the current object's texture to show the
            // extracted rectangle.
            // DrawingGameObjectTarget(gameObjectTarget, destTex);
        }));
        return desTexReturn;
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
}
