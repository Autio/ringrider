using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Gamecontroller : MonoBehaviour {


	// Use this for initialization
	void Start () {
        InitialiseScene();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void InitialiseScene()
    {
        // Run juicy initialisation animations
        InitRing();
        // Activate player so they drop
        // Then deactivate their gravity
        
       
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
        riderSeq.Append(rider.DOScale(1f, 3f));

    }
}
