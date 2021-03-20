using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Player : MonoBehaviour {
    public int lapCounter = 0;
    int points = 0;
    int scoreMultiplier = 1;

    float speedModifier = 1.02f; // How much do the rings speed up after finishing a lap
    // Use this for initialization
    void Start () {
        //GameObject.Find("LapText").GetComponent<Text>().text = "Laps: " + lapCounter.ToString();
    }

    // Update is called once per frame
    void Update () {
		if(GameController.Instance.gameState == GameController.gameStates.starting)
        {
            // pulsate
        }
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

        // Set gamestate to transition
        GameController.Instance.SetState(GameController.gameStates.transition);


        // Juice
        // Move player back to centre
        Sequence seq = DOTween.Sequence();
        seq.Append(this.transform.DOScale(0.25f, 0.1f));
        seq.Append(this.transform.DOScale(0.05f, 0.1f));
        seq.Append(this.transform.DOScale(0.4f, 0.1f));

        this.transform.DOScale(0.06f, 3.2f);
        this.transform.DOMove(new Vector3(0, 0, 0), 3.2f);

        // Reset scene after a delay
        GameController.Instance.Ending();
    }

    void Points()
    {
        points++;
        UpdateText(GameObject.Find("PointsText").GetComponent<Text>(), points.ToString());
//        GameObject.Find("PointsText").GetComponent<Text>().text = "Points: " + points.ToString();
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

            GameController.Instance.InitTrack();


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

        if(collision.transform.tag == "Bonus")
        {
            PickUpBonus(collision);
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

    private IEnumerator JuicyFlip()
    {
        GameObject.Find("ObstacleTracks").GetComponent<Rotate>().FlipDir();
        GameObject.Find("PlayTracks").GetComponent<Rotate>().FlipDir();
        yield return new WaitForSeconds(0.04f);
        GameObject.Find("ObstacleTracks").GetComponent<Rotate>().FlipDir();
        GameObject.Find("PlayTracks").GetComponent<Rotate>().FlipDir();
        yield return new WaitForSeconds(0.04f);
        GameObject.Find("ObstacleTracks").GetComponent<Rotate>().FlipDir();
        GameObject.Find("PlayTracks").GetComponent<Rotate>().FlipDir();
        yield return new WaitForSeconds(0.02f);

    }

    private void PickUpBonus(Collider2D collision)
    {
        // Bonus 1 
        // Destroy bonus object
        Destroy(collision.gameObject);
        StartCoroutine(JuicyFlip());

        // Reverse direction
        // Get a multiplier


        // Slow yourself down


        // Bonus 2
        // Bigger multiplier

    }
}
