using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;

public class Player : MonoBehaviour {

    public GameObject cinemachine;
    public int lapCounter = 0;
    // The ring the player is currently in
    public GameObject activeRing;
    public GameObject targetRing;
    bool onInnerTrack = true; // vs outer track
    int points = 0;
    int scoreMultiplier = 1;
    float ticker = 0.2f;
    bool started = false;
    bool ableToHopRings = false;
    float hopCoolDown = .05f;
    float speedModifier = 1.02f; // How much do the rings speed up after finishing a lap
    // Use this for initialization
    void Start () {
        this.GetComponent<Rigidbody2D>().isKinematic = true;
        this.GetComponent<Rigidbody2D>().useFullKinematicContacts = true;
        
        //GameObject.Find("LapText").GetComponent<Text>().text = "Laps: " + lapCounter.ToString();
    }

    public void Ride()
    {
        // If gamemode is playing

        // If at a switching point, allow switching
        if(ableToHopRings && hopCoolDown < 0)
        {
            hopCoolDown = 0.15f;
            HopToRing(targetRing);
        } else if (hopCoolDown < 0)
        {
            // Hop to the other track
            HopTracks(onInnerTrack);
            
        }

    }

    public void HopTracks(bool onInnerTrack)
    {
        // Amount needed to jump across the circle width
        float offset = .14f;
        if(onInnerTrack)
        {
            // Hop to the outer track
        }
        else 
        {
            // Hop to the inner track of the circle
        }
    }

    public void HopToRing(GameObject targetRing)
    {
        Debug.Log("hopping to new ring!");
        // Move the player to the inner track of the target ring
        // Use the trigger point and move then towards the centre of the new ring
        Transform trigger = targetRing.transform.Find("Trigger");
        transform.position = (trigger.position + ((targetRing.transform.position - trigger.position).normalized * .26f));
        transform.parent = targetRing.transform.Find("Inner Track").transform;
        // Deactivate the trigger!
        trigger.gameObject.SetActive(false);

        // Stop rotating the old ring, start rotating the new one
        Transform oldInnerTrack = activeRing.transform.Find("Inner Track");
        Transform innerTrack = targetRing.transform.Find("Inner Track");
        oldInnerTrack.GetComponent<Rotate>().enabled = false;

        innerTrack.GetComponent<Rotate>().dir = !oldInnerTrack.GetComponent<Rotate>().dir;

        // How do we set speed appropriately?
        float radiusRatio = activeRing.GetComponent<Ring>().radius / targetRing.GetComponent<Ring>().radius;
        Debug.Log("Radius ratio: " + radiusRatio);
        innerTrack.GetComponent<Rotate>().speed = activeRing.transform.Find("Inner Track").GetComponent<Rotate>().speed * radiusRatio;

        
        innerTrack.GetComponent<Rotate>().active = true;
        innerTrack.GetComponent<Rotate>().enabled = true;
        activeRing = targetRing;
        CameraToNewRing(activeRing);
    }

    // Move the camera to center on the new ring
    void CameraToNewRing(GameObject ring)
    {
        Debug.Log(ring);

        var vcam = cinemachine.GetComponent<CinemachineVirtualCamera>();
        Debug.Log("vpos" + vcam.transform.position);
        vcam.LookAt = ring.transform;
        vcam.Follow = ring.transform;
       // Camera.main.transform.position = new Vector3(ring.transform.position.x, ring.transform.position.y, Camera.main.transform.position.z);
    }

    // Update is called once per frame
    void Update () {
        hopCoolDown -= Time.deltaTime;
        ticker -= Time.deltaTime;
        if(ticker < 0 && !started) 
        {   
            started = true;
            transform.position = new Vector2(0, -1.69f);
            activeRing = GameObject.Find("StartRing");
            transform.parent = activeRing.transform.Find("Inner Track");
            
        }
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 7)
        {
            Debug.Log("Left da trigger");
            ableToHopRings = false;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 7)
        {
            ableToHopRings = true;
            targetRing = collision.transform.parent.gameObject;
        }


        // Grab a coin
        if(collision.gameObject.layer == 8)
        {
            Debug.Log("Coin!");
            GameController.Instance.coins++;
            
            Destroy(collision.gameObject);
            // TODO: Play effect
        }

        if(collision.transform.name == "StartTrigger")
        {
            // The player has landed and should be parented to the inner circle


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

}
