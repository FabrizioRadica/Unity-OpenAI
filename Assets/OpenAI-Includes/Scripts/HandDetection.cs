using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HandDetection : MonoBehaviour
{
    public RawImage rawImage;
    public Text debugText;
    public Color skinColorMin = new Color(0.0f, 0.2f, 0.4f); // Min skin color range
    public Color skinColorMax = new Color(0.2f, 0.6f, 0.8f); // Max skin color range
    public GameObject fingerPrefab; // Prefab per il cubo sulle dita

    private WebCamTexture webcamTexture;
    private Texture2D texture;
    private Color32[] currentFrame;
    private GameObject[] fingerCubes;

    void Start()
    {
        // Inizializza la webcam
        webcamTexture = new WebCamTexture();
        rawImage.texture = webcamTexture;
        webcamTexture.Play();

        // Inizializza la texture per mostrare il feed elaborato
        texture = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGBA32, false);

        // Inizializza i cubi per le dita
        fingerCubes = new GameObject[5];
        for (int i = 0; i < fingerCubes.Length; i++)
        {
            fingerCubes[i] = Instantiate(fingerPrefab);
        }
    }

    void Update()
    {
        if (webcamTexture.didUpdateThisFrame)
        {
            // Ottieni il frame attuale
            currentFrame = new Color32[webcamTexture.width * webcamTexture.height];
            webcamTexture.GetPixels32(currentFrame);

            // Rileva la mano nel frame
            DetectHand(currentFrame, webcamTexture.width, webcamTexture.height);

            // Aggiorna la texture per mostrare il frame elaborato
            texture.SetPixels32(currentFrame);
            texture.Apply();
            rawImage.texture = texture;
        }
    }

    private void DetectHand(Color32[] frame, int width, int height)
    {
        List<Vector2> handPixels = new List<Vector2>();

        for (int i = 0; i < frame.Length; i++)
        {
            Color32 pixel = frame[i];

            // Controlla se il pixel è nel range del colore della pelle
            if (IsSkinColor(pixel))
            {
                // Calcola la posizione del pixel
                int x = i % width;
                int y = i / width;
                handPixels.Add(new Vector2(x, y));

                // Modifica il colore del pixel per visualizzare meglio l'area della mano
                frame[i] = new Color32(255, 0, 0, 255);
            }
        }

        // Trova i punti delle dita
        Vector3[] fingerPositions = FindFingers(handPixels, width, height);

        // Posiziona i cubi delle dita
        for (int i = 0; i < fingerCubes.Length; i++)
        {
            if (i < fingerPositions.Length)
            {
                fingerCubes[i].transform.position = fingerPositions[i];
                fingerCubes[i].SetActive(true);
            }
            else
            {
                fingerCubes[i].SetActive(false);
            }
        }

        // Mostra il numero di pixel della mano rilevati
        debugText.text = "Hand Pixels: " + handPixels.Count;
    }

    private bool IsSkinColor(Color32 pixel)
    {
        Color color = new Color(pixel.r / 255.0f, pixel.g / 255.0f, pixel.b / 255.0f);
        return color.r >= skinColorMin.r && color.r <= skinColorMax.r &&
               color.g >= skinColorMin.g && color.g <= skinColorMax.g &&
               color.b >= skinColorMin.b && color.b <= skinColorMax.b;
    }

    private Vector3[] FindFingers(List<Vector2> handPixels, int width, int height)
    {
        List<Vector3> fingerPositions = new List<Vector3>();

        // Trovare contorni semplificati (convex hull)
        List<Vector2> contour = GetConvexHull(handPixels);

        // Trova punti di interesse lungo il contorno
        for (int i = 0; i < contour.Count; i++)
        {
            Vector2 p1 = contour[i];
            Vector2 p2 = contour[(i + 1) % contour.Count];

            float angle = Vector2.Angle(p1 - p2, Vector2.up);

            if (angle > 30 && angle < 150) // Angolo approssimativo per identificare le dita
            {
                Vector3 fingerPos = new Vector3(p1.x - width / 2, height / 2 - p1.y, 0);
                fingerPositions.Add(fingerPos);

                if (fingerPositions.Count >= 5)
                    break;
            }
        }

        return fingerPositions.ToArray();
    }

    private List<Vector2> GetConvexHull(List<Vector2> points)
    {
        if (points.Count <= 1)
            return new List<Vector2>(points);

        List<Vector2> hull = new List<Vector2>();

        // Trova il punto più a sinistra
        Vector2 start = points[0];
        foreach (Vector2 p in points)
        {
            if (p.x < start.x)
                start = p;
        }

        Vector2 current = start;
        List<Vector2> collinearPoints = new List<Vector2>();
        do
        {
            hull.Add(current);
            Vector2 nextTarget = points[0];

            foreach (Vector2 p in points)
            {
                if (p == current)
                    continue;

                float cross = CrossProduct(current, nextTarget, p);
                if (cross > 0)
                {
                    nextTarget = p;
                    collinearPoints.Clear();
                }
                else if (cross == 0)
                {
                    if (Vector2.Distance(current, p) > Vector2.Distance(current, nextTarget))
                    {
                        collinearPoints.Add(nextTarget);
                        nextTarget = p;
                    }
                    else
                    {
                        collinearPoints.Add(p);
                    }
                }
            }

            foreach (Vector2 p in collinearPoints)
            {
                hull.Add(p);
            }

            current = nextTarget;

        } while (current != start);

        return hull;
    }

    private float CrossProduct(Vector2 o, Vector2 a, Vector2 b)
    {
        return (a.x - o.x) * (b.y - o.y) - (a.y - o.y) * (b.x - o.x);
    }
}
