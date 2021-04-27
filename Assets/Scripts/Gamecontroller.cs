﻿
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using DG.Tweening;
using TMPro;

using Random=UnityEngine.Random;


// TODO:
// Sound effect on hopping
// Sound effect on coin collection
// Sound effect on death

// Custom player stats store
// The player's highscore
// The player's coin loot
// The player's unlocked items

// Playerprefs can store: 
// Audio toggle settings

public class GameController : Singleton<GameController> {

    public GameObject playContainer;
    public GameObject menuReturn;
    public Transform[] playTracks = new Transform[2];
    public Transform[] playTrackPositions = new Transform[2];
    public Transform[] obstacleSpawns = new Transform[2];
    public List<Character> unlockedCharacters = new List<Character>();
    public Character activeCharacter;

    public Color[] backgroundColors;
    public Transform rider;
    float switchCooldown = 0.07f;
    public Transform block;
    public Transform bonus;
    private float counter = 1.0f;
    bool gameLive = false;
    int adFrequency = 5;
    public int coins;
    public int highScore;

    int gamePlays;
   
    public enum gameStates { menu, playing, paused, transition, starting, ending };
    public gameStates gameState = gameStates.menu;

    // TEXTS
    Text coinCounterText;

    bool firstMove = true;

    // Use this for initialization
    void Start() {
        ColourTransition();
        Advertisement.Initialize("1696406");
        GameObject.Find("SaveController").GetComponent<SaveController>().LoadGame();
        StartGame();

    }

    public void StartGame()
    {
        Debug.Log("Starting game");
        gameState = gameStates.starting;

        // Hide the menu
       // GameObject.Find("Menu").SetActive(false);
       // playContainer.SetActive(true);

        coinCounterText = GameObject.Find("CoinCounter").GetComponent<Text>();
        coinCounterText.text = coins.ToString();
        // Build the level
        GetComponent<RingController>().BuildLevel(100,.7f,1.75f);
        GameObject.Find("Rider").GetComponent<Player>().ResetPlayer();

        // Show the menu button
        menuReturn.SetActive(true);

    }

    // Input

    

    // Update is called once per frame
    void Update() {


        // // TODO: Refactor to new input system
        // if (gameState == gameStates.starting)
        // {
        //     if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        //     {
        //         InitialiseScene();
        //         GameObject.Find("StartTrigger").GetComponent<Movement>().enabled = true;
        //         gameState = gameStates.transition;
        //     }


        // }
        // if (gameState == gameStates.playing)
        // {

        //     if (Input.GetKeyDown(KeyCode.Space))
        //     {
        //         if (switchCooldown < 0)
        //         {
        //             SwitchTrack();
        //         }
        //     }

        //     if (Input.GetMouseButtonDown(0))
        //     {
        //         if (switchCooldown < 0)
        //         {
        //             SwitchTrack();
        //         }
        //     }

            // Spawning of enemies
        //     counter -= Time.deltaTime;
        //     if (counter < 0)
        //     {
        //         counter = Random.Range(1f, 2.5f);
        //         if (GameObject.Find("Rider").GetComponent<Player>().lapCounter > 1)
        //         {
        //             if (Random.Range(0f, 1f) > 0.5f)
        //             {
        //                 SpawnBonus();
        //             }
        //             else
        //             {
        //                 SpawnBlock();
        //             }
        //         }
        //         else
        //         {
        //             SpawnBlock();
        //         }
        //     }
        // }
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

    void SpawnBlock()
    {
        // Only spawn when playing
        if (gameState == gameStates.playing)
        {
            // Check if close to existing block

            bool tooClose = false;
            tooClose = CheckSpawnProximity();
            if (tooClose)
            {
                // If too close, reset the timer
                return;
            }

            Transform newBlock = Instantiate(block, new Vector2(obstacleSpawns[0].position.x, obstacleSpawns[0].position.y), Quaternion.identity);
            Transform newBlock2 = Instantiate(block, new Vector2(obstacleSpawns[1].position.x, obstacleSpawns[1].position.y), Quaternion.identity);

            if (Random.Range(0, 10) < 5)
            {
                newBlock.transform.GetComponent<Collider2D>().isTrigger = true;
                newBlock.GetComponent<SpriteRenderer>().enabled = false;
                newBlock.transform.tag = "Points";
            }
            else
            {
                newBlock2.transform.GetComponent<Collider2D>().isTrigger = true;
                newBlock2.GetComponent<SpriteRenderer>().enabled = false;
                newBlock2.transform.tag = "Points";
            }

            // Only one of the two is a threat
            newBlock.parent = GameObject.Find("ObstacleTrack1").transform;
            newBlock2.parent = GameObject.Find("ObstacleTrack2").transform;

        }
    }
    void SpawnBonus()
    {
        // Only spawn when playing
        if (gameState == gameStates.playing)
        {
            // Check if close to existing block

            bool tooClose = false;
            tooClose = CheckSpawnProximity();
            if (tooClose)
            {
                // If too close, reset the timer
                return;
            }


            int i = 0;
            if(Random.Range(0f,1f)>0.5f)
            {
                i = 1;
            }
            Transform newBonus = Instantiate(bonus, new Vector2(obstacleSpawns[i].position.x, obstacleSpawns[i].position.y), Quaternion.identity);
            // Only one of the two is a threat
            newBonus.parent = GameObject.Find("ObstacleTrack" + (i+1).ToString()).transform;

        }
    }
    public void SwitchTrack()
    {
        Debug.Log("Switching track!");

        if (rider.transform.parent.name != "PlayTrack1" && rider.transform.parent.name != "PlayTrack2")
        {
            rider.transform.parent = GameObject.Find("PlayTrack1").transform;
            rider.transform.position = playTrackPositions[0].position;
            switchCooldown = 0.1f;
            return;
        }

        if (rider.transform.parent.name == "PlayTrack2")
        {
            rider.transform.parent = GameObject.Find("PlayTrack1").transform;
            rider.transform.position = playTrackPositions[0].position;
            // Also move between them!
            Debug.Log("Switching track!");
            switchCooldown = 0.1f;
        }
        else if (rider.transform.parent.name == "PlayTrack1")
        {
            Debug.Log("Switching track!");
            rider.transform.parent = GameObject.Find("PlayTrack2").transform;
            // Also move between them!
            rider.transform.position = playTrackPositions[1].position;
            switchCooldown = 0.1f;
        }

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

    public void InitRing()
    {
        // Transform target = GameObject.Find("Ring").transform;
        // Sequence mySequence = DOTween.Sequence();

        // mySequence.Append(target.DOScale(0.5f, 0.3f));
        // mySequence.Append(target.DOScale(0.2f, 0.6f));
        // mySequence.Append(target.DOScale(0.8f, 0.45f));
        // mySequence.Append(target.DOScale(0.65f, 0.35f));
        // mySequence.Append(target.DOScale(1.25f, 0.5f));
        // mySequence.Append(target.DOScale(0.8f, 0.5f));
        // mySequence.Append(target.DOScale(1f, 0.9f));

        // StartCoroutine(PlayerActivate());
            
        // Transform rider = GameObject.Find("Rider").transform;
        // Sequence riderSeq = DOTween.Sequence();
        // riderSeq.Append(rider.DOScale(1f, 2.6f));

    }


    public void ColourTransition()
    {
        // Pick a random colour and transition to it gradually
        Color newColor = backgroundColors[Random.Range(0, backgroundColors.Length)];
        Debug.Log("Color switch");
       // Sequence s = DOTween.Sequence();
//        GameObject.Find("Background").GetComponent<SpriteRenderer>().DOColor(newColor, 7);
   //     GameObject.Find("Background").GetComponent<SpriteRenderer>().DOFade(255,7);
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
        Save save = new Save();
        
        save.gamePlays = gamePlays;
        save.coins = coins;
        save.highScore = highScore;
        save.activeCharacter = activeCharacter;

        SaveController.instance.SaveGame(save);

        StartCoroutine(RingController.Instance.DestroyLevel(Player.Instance.ringsReached));
        StartCoroutine(WaitEnd());
    }
    public IEnumerator WaitEnd()
    {
        while (true)
        {
            gamePlays++;
            
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
                yield return new WaitForSeconds(5f);
            }

        
        Debug.Log("Resetting level");

        // Reset level
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        }
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
