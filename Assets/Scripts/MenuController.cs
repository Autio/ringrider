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

    public GameObject menuJuice1;
    public GameObject menuJuice2;
    public GameObject menuJuice3;
    public GameObject menuJuice4;
    public GameObject menuJuice5;
    public GameObject menuJuice6;

    Save activeSave;
    // TODO: Character limitations
    // We allow all four characters to be selected at the start
    List<int> unlockedCharacters = new List<int>() { 0,1,2,3 } ;
    int activeCharacter = 0;

    public Color[] ringColors;

    void Start()
    {
        StartCoroutine(LoadGame());

        // Draw all the relevant circles
        float width = 0.06f;
        float playButtonRadius = 1.2f;
        //float gardenButtonRadius = 1f;
        float otherButtonRadius = .6f;

        // Visual juice
        DrawPolygon(60, 3.5f, new Vector2(0, 6), width, width, ringColors[4], menuJuice1.GetComponent<LineRenderer>());
        DrawPolygon(64, .7f, new Vector2(0, -6.5f), 3, 3, ringColors[6], menuJuice4.GetComponent<LineRenderer>());

        DrawPolygon(60, 1.5f, new Vector2(-3.4f, -6), 3, 3, ringColors[5], menuJuice2.GetComponent<LineRenderer>());
        DrawPolygon(62, 1.5f, new Vector2(3.4f, -6f), 3, 3, ringColors[5], menuJuice3.GetComponent<LineRenderer>());
        DrawPolygon(62, 1.6f, new Vector2(0f, 6f), 3.25f, 3.25f, ringColors[7], menuJuice5.GetComponent<LineRenderer>());



        // Menu buttons
       
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

    private IEnumerator LoadGame()
    {
        yield return new WaitForSeconds(0.3f);
        try {
            Debug.Log("Loading save");
            activeSave = GameObject.Find("SaveController").GetComponent<SaveController>().LoadGame();
            // GetComponent<CharacterController>().LoadCharacters();
            activeCharacter = activeSave.activeCharacter;
            // Create a new save if there's none
            if(activeSave == null)
            {
                activeSave = new Save();
                activeCharacter = 0;
            }
        } catch{
             Debug.Log("Error loading game");
        }

        // unlockedCharacters = activeSave.unlockedCharacters;
        
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

    public void ShareLink()
    {
        #if UNITY_ANDROID
        // Get the Intent and UnityPlayer classes
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

        // Using intents
        AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
        intent.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        intent.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "Ring Rider");
        intent.Call<AndroidJavaObject>("setType", "text/plain");

        // Display the chooser 
        AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intent, "Share");
        currentActivity.Call("startActivity", chooser);
        #endif
    }

    public void ChangeCharacter()
    {
        // Cycle through colours of the character and save the choice 
        // Current index in the list of unlocked characters
        int index = unlockedCharacters.IndexOf(activeCharacter);

        index++;
        if (index > unlockedCharacters.Count - 1)
        {
            index = 0;
        }

        activeCharacter = unlockedCharacters[index];
        Debug.Log("Updating character colour");
        PlayerCharacterController.Instance.UpdateCharacter(activeCharacter);

        activeSave.activeCharacter = activeCharacter;
        SaveController.Instance.SaveGame(activeSave);

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

        lineRenderer.SetPosition(vertexNumber -1, lineRenderer.GetPosition(0));

    }
}
