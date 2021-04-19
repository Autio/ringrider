using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cinemachine;

public class Player : Singleton<Player> {

    GameController gc;
    public GameObject cinemachine;
    public int lapCounter = 0;
    // The ring the player is currently in
    public GameObject activeRing;
    public GameObject targetRing;
    public GameObject coinEffectPrefab;
    public GameObject deathEffectPrefab;
    public AudioClip playerDeathSound;
    public AudioClip ringSwitchSound;

    public AudioClip coinSound;
    float prevCoinSoundPitch = 1;
    bool coinSoundsUp = true;
    bool onInnerTrack = true; // vs outer track
    public int ringsReached = 0;
    int points = 0;
    int scoreMultiplier = 1;
    float ticker = 0.4f;
    bool started = false;
    bool ableToHopRings = false;
    float hopCoolDown = .05f;
    float speedModifier = 1.02f; // How much do the rings speed up after finishing a lap
    // Use this for initialization
    void Awake () {
        ResetPlayer();
        
        //GameObject.Find("LapText").GetComponent<Text>().text = "Laps: " + lapCounter.ToString();
    }

    private void Start() {
        gc = GameObject.Find("GameController").GetComponent<GameController>(); 

    }

    public void ResetPlayer()
    {
        ringsReached = 0;
        this.GetComponent<Rigidbody2D>().isKinematic = true;
        this.GetComponent<Rigidbody2D>().useFullKinematicContacts = true;
    }

    public void Ride()
    {
        try{
            // If gamemode is playing
            if(GameController.instance.gameState == GameController.gameStates.playing)
                {
                    // If at a switching point, allow switching
                    if(onInnerTrack && ableToHopRings && hopCoolDown < 0)
                    {
                        hopCoolDown = 0.15f;
                        HopToRing(targetRing);

                    } else if (hopCoolDown < 0)
                    {
                        // Hop to the other track
                        hopCoolDown = 0.15f;
                        HopTracks();
                        
                    }
                }
            
        }
        catch {
            Debug.Log("Error with input");
        }

    }

    public void HopTracks()
    {
        // Amount needed to jump across the circle width
        float offset = .28f;
        if(onInnerTrack)
        {
            float radiusRatio = (activeRing.GetComponent<Ring>().radius + offset) / activeRing.GetComponent<Ring>().radius;
           // Debug.Log(radiusRatio + " should be larger than " + activeRing.GetComponent<Ring>().radius);
            activeRing.transform.Find("Inner Track").GetComponent<Rotate>().speed = activeRing.transform.Find("Inner Track").GetComponent<Rotate>().speed / radiusRatio;
           
            // Hop to the outer track
            transform.position = transform.position + (transform.position - activeRing.transform.position).normalized * offset;
            onInnerTrack = false;
        }
        else 
        {
            float radiusRatio = (activeRing.GetComponent<Ring>().radius + offset) / activeRing.GetComponent<Ring>().radius;
            activeRing.transform.Find("Inner Track").GetComponent<Rotate>().speed = activeRing.transform.Find("Inner Track").GetComponent<Rotate>().speed * radiusRatio;
            Debug.Log(radiusRatio + " should be smaller than " + activeRing.GetComponent<Ring>().radius);
            // Hop to the inner track of the circle
            transform.position = transform.position - (transform.position - activeRing.transform.position).normalized * offset;
            onInnerTrack = true;
        }
    }

    public void HopToRing(GameObject targetRing)
    {
        ringsReached++;      

        // Move the player to the inner track of the target ring
        // Use the trigger point and move then towards the centre of the new ring
        Transform trigger = targetRing.transform.Find("Trigger");
        transform.position = (trigger.position + ((targetRing.transform.position - trigger.position).normalized * .18f));
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

        // Make a sound 
        GameController.Instance.Play2DClipAtPoint(ringSwitchSound, activeRing.GetComponent<Ring>().radius);
        
        innerTrack.GetComponent<Rotate>().active = true;
        innerTrack.GetComponent<Rotate>().enabled = true;
        activeRing = targetRing;
        CameraToNewRing(activeRing);

        // Activate the ring effect 
        activeRing.transform.Find("RingEffect(Clone)").GetComponent<RingEffect>().enabled = true;
        Destroy(activeRing.transform.Find("RingEffect(Clone)"), 15f);

        // Activate the central circle too
        //activeRing.transform.Find("RingCentreCircle(Clone)").GetComponent<CircleCentre>().InitCircle();
        activeRing.transform.Find("RingCentreCircle(Clone)").GetComponent<CircleCentre>().active = true;
        activeRing.transform.Find("CircleText(Clone)").gameObject.SetActive(true);
    }


    // Move the camera to center on the new ring
    void CameraToNewRing(GameObject ring)
    {
        var vcam = cinemachine.GetComponent<CinemachineVirtualCamera>();
        
        vcam.LookAt = ring.transform;
        vcam.Follow = ring.transform;

    }


    public void SetPlayerStart()
    {
            transform.position = new Vector2(0, -GameObject.Find("StartRing").GetComponent<Ring>().radius + 0.14f);
            activeRing = GameObject.Find("StartRing");
            transform.parent = activeRing.transform.Find("Inner Track");
            GameObject.Find("StartRing").transform.Find("Inner Track").GetComponent<Rotate>().enabled = true;
            // Game is now playing
            gc = GameObject.Find("GameController").GetComponent<GameController>();
            gc.gameState = GameController.gameStates.playing;
            //gc.menuReturn.SetActive(false);
            
    }
    // Update is called once per frame
    void Update () {

        
        hopCoolDown -= Time.deltaTime;
        
        
        if(gc.gameState == GameController.gameStates.starting) {
            ticker -= Time.deltaTime;
            Debug.Log("Ticker " + ticker);

        }

        // Starts the game flow
        if(ticker < 0 && GameController.instance.gameState == GameController.gameStates.starting) 
        {   
            SetPlayerStart();

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
        // Play sound effect
        GameController.Instance.Play2DClipAtPoint(playerDeathSound, Random.Range(0.9f, 1.1f));

        // Show points

        // Stop movement
        GameObject.Find("PlayTracks").GetComponent<Rotate>().active = false;
        GameObject.Find("ObstacleTracks").GetComponent<Rotate>().active = false;

        // Set gamestate to transition
        GameController.Instance.SetState(GameController.gameStates.transition);

        // Juice
        // Move player back to centre
        // Sequence seq = DOTween.Sequence();
        // seq.Append(this.transform.DOScale(0.25f, 0.1f));
        // seq.Append(this.transform.DOScale(0.05f, 0.1f));
        // seq.Append(this.transform.DOScale(0.4f, 0.1f));

        // this.transform.DOScale(0.06f, 3.2f);
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

    void CoinSound(){
        float newPitch = 0;
        float step = 0.02f;
        float minPitch = 0.5f;
        float maxPitch = 1.2f;

        int dir = 1;
        if(!coinSoundsUp)
        {
            dir = -1;
        }
    
        if (Random.Range(0, 10) < 5)
        {
            newPitch = prevCoinSoundPitch + step * dir;
        } else
        {
            newPitch = prevCoinSoundPitch + 2 * step * dir;
        }
        
        if(Random.Range(0, 20) < 1)
        {
            newPitch = 1;
        }
        if (newPitch >= maxPitch)
        {
            coinSoundsUp = false;
        }
        if (newPitch <= minPitch)
        {
            coinSoundsUp = true;
        }
        newPitch = Mathf.Clamp(newPitch, 0.3f, 1.3f);

        GameController.Instance.Play2DClipAtPoint(coinSound, newPitch, 0.16f);
        prevCoinSoundPitch = newPitch;

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

    private void Death()
    {
        Destroy(GameObject.Instantiate(deathEffectPrefab, transform.position, Quaternion.identity), 3.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        // Hit a ring when you're riding outside
        if(collision.gameObject.layer == 6)
        {
            if(!onInnerTrack)
            {
                Debug.Log("Game overrr");
                GameOver();
                Death();
                Destroy(gameObject);
            }
        }

        if(collision.gameObject.layer == 9)
        {
            Debug.Log("Game overrr");
            GameOver();
            Death();
            Destroy(gameObject);
        }

        // Allowed to hop between rings
        if(collision.gameObject.layer == 7)
        {
            ableToHopRings = true;
            targetRing = collision.transform.parent.gameObject;
        }


        // Grab a coin
        if(collision.gameObject.layer == 8)
        {
            GameController.Instance.coins++;
            GameController.Instance.UpdateCoinCounter();
            CoinSound();
            
            Destroy(collision.gameObject);
            GameObject coinParticles = Instantiate(coinEffectPrefab, collision.transform.position, Quaternion.identity);
            Destroy(coinParticles, 1.0f);
        }

        if(collision.transform.name == "StartTrigger")
        {
            // The player has landed and should be parented to the inner circle
            // Activate rotation
            GameObject.Find("PlayTracks").GetComponent<Rotate>().active = true;

            GameController.Instance.InitTrack();


        }

    
    }

    

}
