using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuController : Singleton<GameController> 
{
    // Start is called before the first frame update
    public GameObject playButton;
    public GameObject shareButton;
    public GameObject rateButton;

    public GameObject characterButton;

    public Color[] ringColors;

    void Start()
    {
        
        // Draw all the relevant circles
        float width = 0.06f;
        float playButtonRadius = 1.2f;
        float gardenButtonRadius = 1f;
        float otherButtonRadius = .6f;
       
        DrawPolygon(60, playButtonRadius, playButton.transform.position, width, width, ringColors[0], playButton.GetComponent<LineRenderer>());
        Vector2 shareButtonPosition = newRingPosition(playButtonRadius, otherButtonRadius, width,3.86f ); // Angle between 0 and 2 * pi
        DrawPolygon(60, otherButtonRadius, shareButtonPosition, width, width, ringColors[1], shareButton.GetComponent<LineRenderer>());  

        Vector2 rateButtonPosition = newRingPosition(playButtonRadius, otherButtonRadius, width, 4.54f); // Angle between 0 and 2 * pi
        DrawPolygon(60, otherButtonRadius, rateButtonPosition, width, width, ringColors[2], rateButton.GetComponent<LineRenderer>());
              
        Vector2 characterButtonPosition = newRingPosition(playButtonRadius, otherButtonRadius, width, 5.22f); // Angle between 0 and 2 * pi
        DrawPolygon(60, otherButtonRadius, characterButtonPosition, width, width, ringColors[3], characterButton.GetComponent<LineRenderer>());

        Debug.Log(shareButtonPosition);
        Debug.Log(rateButtonPosition);
        Debug.Log(characterButtonPosition);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartLevel()
    {
        Debug.Log("Starting to play the level");
         SceneManager.LoadScene("play");
    }

    public void OpenPlayStore()
    {
        Debug.Log("Sending the user to the app page");
        Application.OpenURL ("market://details?id=" + Application.identifier);
    }

    public Vector2 newRingPosition (float playButtonRadius, float radius, float ringWidth, float angle)
    {
        
        float xPos = playButton.transform.position.x + (playButtonRadius + ringWidth + radius) * Mathf.Cos(angle);
        float yPos = playButton.transform.position.y + (playButtonRadius + ringWidth + radius) * Mathf.Sin(angle);
        
        return new Vector2(xPos, yPos);
    }

    public void DrawPolygon(int vertexNumber, float radius, Vector3 centerPos, float startWidth, float endWidth, Color color, LineRenderer lineRenderer)
    {
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
        lineRenderer.loop = true;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        float angle = 2 * Mathf.PI / vertexNumber;
        lineRenderer.positionCount = vertexNumber;

        for (int i = 0; i < vertexNumber; i++)
        {
            Matrix4x4 rotationMatrix = new Matrix4x4(new Vector4(Mathf.Cos(angle * i), Mathf.Sin(angle * i), 0, 0),
                                                    new Vector4(-1 * Mathf.Sin(angle * i), Mathf.Cos(angle * i), 0, 0),
                                    new Vector4(0, 0, 1, 0),
                                    new Vector4(0, 0, 0, 1));
            Vector3 initialRelativePosition = new Vector3(0, radius, 0);
            lineRenderer.SetPosition(i, centerPos + rotationMatrix.MultiplyPoint(initialRelativePosition));

            }
    }
}
