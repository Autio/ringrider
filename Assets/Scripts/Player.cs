﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Player : MonoBehaviour {
    int lapCounter = 0;
    int points = 0;
    float speedModifier = 1.05f;
    // Use this for initialization
    void Start () {
        //GameObject.Find("LapText").GetComponent<Text>().text = "Laps: " + lapCounter.ToString();
    }

    // Update is called once per frame
    void Update () {
		
	}

    void DoLap()
    {
        // increase rotation speed
        lapCounter++;
        //Debug.Log("Lap! " + lapCounter.ToString());

        GameObject.Find("PlayTracks").GetComponent<Rotate>().speed *= speedModifier;

        // Display laps
        UpdateText(GameObject.Find("LapText").GetComponent<Text>(), lapCounter.ToString());

    }

    void GameOver()
    {
        // Show points

        // Stop movement
        GameObject.Find("PlayTracks").GetComponent<Rotate>().active = false;
        GameObject.Find("ObstacleTracks").GetComponent<Rotate>().active = false;
        StartCoroutine(WaitEnd());
        // Reset scene after a delay
    }

    void Points()
    {
        points++;
        UpdateText(GameObject.Find("PointsText").GetComponent<Text>(), points.ToString());
//        GameObject.Find("PointsText").GetComponent<Text>().text = "Points: " + points.ToString();
    }

    private IEnumerator WaitEnd()
    {
        while(true)
        {
            yield return new WaitForSeconds(2.0f);
            Application.LoadLevel(0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.name == "StartTrigger")
        {
            // The player has landed and should be parented to the inner circle
            this.GetComponent<Rigidbody2D>().isKinematic = true;
            this.GetComponent<Rigidbody2D>().useFullKinematicContacts = true;

            // Activate rotation
            GameObject.Find("PlayTracks").GetComponent<Rotate>().active = true;

            GameObject.Find("- GameController").GetComponent<Gamecontroller>().InitTrack();


        }

        if(collision.transform.tag == "Lap")
        {
            // Lap has been circumnavigated
            DoLap();
        }

        if(collision.transform.tag == "Obstacle")
        {
            GameOver();
        }


        if (collision.transform.tag == "Points")
        {
            Points();
        }
    }


    private void UpdateText(Text t, string s)
    {
        t.text = s;

        Sequence seq = DOTween.Sequence();

        seq.Append(t.transform.DOScale(1.1f, 0.2f));
        seq.Append(t.transform.DOScale(0.9f, 0.2f));
        seq.Append(t.transform.DOScale(1.0f, 0.2f));

    }
}
