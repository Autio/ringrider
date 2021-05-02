
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using DG.Tweening;
using TMPro;

using Random=UnityEngine.Random;

public class GameController : Singleton<GameController> {

    public GameObject playContainer;
    public GameObject menuReturn;
    public Transform[] playTracks = new Transform[2];
    public Transform[] playTrackPositions = new Transform[2];
    public Transform[] obstacleSpawns = new Transform[2];
    public List<int> unlockedCharacters = new List<int>();
    
    [SerializeField]
    public int activeCharacter;

    public Color[] backgroundColors;
    public Transform rider;
    float switchCooldown = 0.07f;
    public Transform block;
    public Transform bonus;
    int adFrequency = 5;
    public int coins;
    public int highScore;

    int gamePlays;

    Save activeSave;
   
    public enum gameStates { menu, playing, paused, transition, starting, ending };
    public gameStates gameState = gameStates.menu;

    // TEXTS
    Text coinCounterText;

    // Use this for initialization
    void Start() {
        ColourTransition();
        Advertisement.Initialize("1696406");
        activeSave = SaveController.Instance.LoadGame();
        activeCharacter = activeSave.activeCharacter;
        gamePlays = activeSave.gamePlays;
        coins = activeSave.coins;

        PlayerCharacterController.Instance.UpdateCharacter(activeCharacter);
        StartGame();
    }

    public void StartGame()
    {
        Debug.Log("Starting game");
        gameState = gameStates.starting;

        coinCounterText = GameObject.Find("CoinCounter").GetComponent<Text>();
        coinCounterText.text = coins.ToString();

        // Show a transition effect

        // Don't build the level right away
        StartCoroutine(BuildLevel(.1f));
        GameObject.Find("Rider").GetComponent<Player>().ResetPlayer();

        // Show the menu button
        menuReturn.SetActive(true);
    }

    private IEnumerator BuildLevel(float delay)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<RingController>().BuildLevel(100,.7f,1.75f);
    }
    

    // Update is called once per frame
    void Update() {


        switchCooldown -= Time.deltaTime;

    }

    void InitialiseScene()
    {
        // Start moving the player
        

        // Run juicy initialisation animations
        // InitRing();
        // Activate player so they drop
        // Then deactivate their gravity


    }

    bool CheckSpawnProximity()
    {
        Collider2D[] c = Physics2D.OverlapCircleAll(obstacleSpawns[0].transform.position, 0.95f);
        try
        {
            foreach(Collider2D cd in c)
            {
                if(cd.transform.tag == "Obstacle")
                {
                    return true;
                }
            }
        }
        catch
        {

        }
        return false;
    }

    public void InitTrack()
    {
        Debug.Log("Switching track!");

        if (rider.transform.parent.name != "PlayTrack1" && rider.transform.parent.name != "PlayTrack2")
        {
            rider.transform.parent = GameObject.Find("PlayTrack1").transform;
            rider.transform.position = playTrackPositions[0].position;
            switchCooldown = 0.2f;
            return;
        }
    }

    public void ColourTransition()
    {
        // Pick a random colour and transition to it gradually
        Color newColor = backgroundColors[Random.Range(0, backgroundColors.Length)];
        // Sequence s = DOTween.Sequence();
        // GameObject.Find("Background").GetComponent<SpriteRenderer>().DOColor(newColor, 7);
        // GameObject.Find("Background").GetComponent<SpriteRenderer>().DOFade(255,7);
    }

    public void SetState(gameStates gs)
    {
        gameState = gs;
    }

    public void UpdateCoinCounter()
    {
        coinCounterText.text = coins.ToString();
    }

    public void Ending()
    {
        StartCoroutine(TransitionEffect());
        StartCoroutine(RingController.Instance.DestroyLevel(Player.Instance.ringsReached));
        StartCoroutine(WaitEnd());
    }

    public IEnumerator TransitionEffect()
    {
        JuiceController.Instance.position = GameObject.Find("Rider").transform.position;

        yield return new WaitForSeconds(.8f);
        JuiceController.Instance.emitting = true;

    }
    public IEnumerator WaitEnd()
    {
        // DEBUG

        gamePlays++;
        Debug.Log("Gameplays: " + gamePlays);
        
        if (gamePlays % adFrequency == 0)
        {
            gameState = gameStates.transition;
            // Show ad
            Debug.Log("Showing advertisment");
            
            if(Advertisement.IsReady("reward"))
            {
                var options = new ShowOptions { resultCallback = HandleShowResult };
                Advertisement.Show("reward", options);
                yield return new WaitForSeconds(5f);
            }

        } else
        {
            yield return new WaitForSeconds(2f);
        }
        
        Debug.Log("Resetting level");
        yield return new WaitForSeconds(3f);
        Save save = new Save();
        
        save.gamePlays = gamePlays;
        save.coins = coins;
        save.highScore = highScore;
        save.activeCharacter = activeCharacter;
        save.unlockedCharacters = unlockedCharacters;

        SaveController.Instance.SaveGame(save);

        // Reset level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        
    }
    
    private void HandleShowResult(ShowResult result)
    {
        // The result was received, so 
        gameState = gameStates.starting;

        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                //
                // Reward gamer
                // Give coins etc.
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }

    public void ReturnToMenu()
    {
        Debug.Log("Returning to Menu");
        SceneManager.LoadScene("menu");
    }

    public void Play2DClipAtPoint(AudioClip clip, float pitch, float volume = 1)
    {
        //  Create a temporary audio source object
        GameObject tempAudioSource = new GameObject("TempAudio");

        //  Add an audio source
        AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();

        //  Add the clip to the audio source
        audioSource.clip = clip;

        audioSource.pitch = pitch;

        //  Set the volume
        audioSource.volume = volume;

        //  Set properties so it's 2D sound
        audioSource.spatialBlend = 0.0f;

        //  Play the audio
        audioSource.Play();

        //  Set it to self destroy
        Destroy(tempAudioSource, clip.length);
    }

}
