using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Gamecontroller : MonoBehaviour {
    public Transform[] playTracks = new Transform[2];
    public Transform[] playTrackPositions = new Transform[2];
    public Transform[] obstacleSpawns = new Transform[2];
    public Transform rider;
    float switchCooldown = 0.07f;
    public Transform block;
    private float counter = 1.0f;
    bool gameLive = false;
    // Use this for initialization
    void Start() {
        InitialiseScene();
    }

    // Update is called once per frame
    void Update() {


        
        if (gameLive)
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (switchCooldown < 0)
                {
                    SwitchTrack();
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (switchCooldown < 0)
                {
                    SwitchTrack();
                }
            }
            counter -= Time.deltaTime;
            if (counter < 0)
            {
                counter = Random.Range(1f, 2.5f);
                SpawnBlock();
            }
        }
        switchCooldown -= Time.deltaTime;

    }

    void InitialiseScene()
    {
        // Run juicy initialisation animations
        InitRing();
        // Activate player so they drop
        // Then deactivate their gravity


    }

    bool CheckSpawnProximity()
    {
        Collider2D[] c = Physics2D.OverlapCircleAll(obstacleSpawns[0].transform.position, 0.75f);
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


    void PlayerActivate()
    {
        // Enable the player to drop
        Debug.Log("Activating player");
        Transform player = GameObject.Find("Rider").transform;
        player.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
        gameLive = true;
    }

    void EndScene()
    {

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
        Transform target = GameObject.Find("Ring").transform;
        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(target.DOScale(0.5f, 0.4f));
        mySequence.Append(target.DOScale(0.3f, 0.4f));
        mySequence.Append(target.DOScale(0.8f, 0.45f));
        mySequence.Append(target.DOScale(0.6f, 0.35f));
        mySequence.Append(target.DOScale(1.2f, 0.5f));
        mySequence.Append(target.DOScale(0.6f, 0.4f));
        mySequence.Append(target.DOScale(1f, 0.8f)).OnComplete(PlayerActivate);

        Transform rider = GameObject.Find("Rider").transform;
        Sequence riderSeq = DOTween.Sequence();
        riderSeq.Append(rider.DOScale(1f, 2.6f));

    }

}
