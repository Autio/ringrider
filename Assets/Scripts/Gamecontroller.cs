using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Gamecontroller : MonoBehaviour {
    public Transform[] playTracks = new Transform[2];
    public Transform[] playTrackPositions = new Transform[2];
    public Transform rider;
	// Use this for initialization
	void Start () {
        InitialiseScene();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.T))
        {
            SwitchTrack();
        }
	}

    void InitialiseScene()
    {
        // Run juicy initialisation animations
        InitRing();
        // Activate player so they drop
        // Then deactivate their gravity
        
       
    }

    public void SwitchTrack()
    {
        Debug.Log("Switching track!");

        if (rider.transform.parent.name != "PlayTrack1" && rider.transform.parent.name != "PlayTrack2")
        {
            rider.transform.parent = GameObject.Find("PlayTrack1").transform;
            return;
        }

       if(rider.transform.parent.name != "PlayTrack1")
        {
            rider.transform.parent = GameObject.Find("PlayTrack1").transform;
            rider.transform.position = playTrackPositions[0].position;
            // Also move between them!
            Debug.Log("Switching track!");
        }
        else
        {
            Debug.Log("Switching track!");

            rider.transform.parent = GameObject.Find("PlayTrack2").transform;
            // Also move between them!
            rider.transform.position = playTrackPositions[1].position;

        }

    }


    void PlayerActivate()
    {
        // Enable the player to drop
        Debug.Log("Activating player");
        Transform player = GameObject.Find("Rider").transform;
        player.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
        
    }

    void EndScene()
    {

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
        mySequence.Append(target.DOScale(0.75f, 0.4f));
        mySequence.Append(target.DOScale(1f, 0.7f)).OnComplete(PlayerActivate);

        Transform rider = GameObject.Find("Rider").transform;
        Sequence riderSeq = DOTween.Sequence();
        riderSeq.Append(rider.DOScale(1f, 2.6f));

    }
}
