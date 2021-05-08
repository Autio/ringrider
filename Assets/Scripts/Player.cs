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
    public AudioClip[] ringSwitchSounds;

    public AudioClip coinSound;
    float prevCoinSoundPitch = 1;
    bool coinSoundsUp = true;
    bool onInnerTrack = true; // vs outer track
    public int ringsReached = 0;

    float ticker = 0.4f;
    bool ableToHopRings = false;
    float hopCoolDown = .05f;    // Use this for initialization
    void Awake () {
        ResetPlayer();
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
        // Can make dynamic?
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
        innerTrack.GetComponent<Rotate>().speed = activeRing.transform.Find("Inner Track").GetComponent<Rotate>().speed * radiusRatio;

        innerTrack.GetComponent<Rotate>().active = true;
        innerTrack.GetComponent<Rotate>().enabled = true;

        activeRing = targetRing;
        CameraToNewRing(activeRing);

        // Make a sound 
        RingSwitchSound(activeRing.GetComponent<Ring>().radius);

        // Activate the ring effect 
        activeRing.transform.Find("RingEffect(Clone)").GetComponent<RingEffect>().alive = true;
        Destroy(activeRing.transform.Find("RingEffect(Clone)").gameObject, 15f);

        // Activate the central circle too
        activeRing.transform.Find("RingCentreCircle(Clone)").GetComponent<CircleCentre>().active = true;
        activeRing.transform.Find("CircleText(Clone)").gameObject.SetActive(true);
    }

    public void RingSwitchSound(float radius)
    {
        // Play from a scale on the basis of the radius (currently .7 - 1.75) 
       
        AudioClip ringSwitchSFX = ringSwitchSounds[5];

        if(radius > .7f)
        {
            ringSwitchSFX = ringSwitchSounds[0];
        }
        if(radius > .75f)
        {
            ringSwitchSFX = ringSwitchSounds[1];
        }
        if(radius > .85f)
        {
            ringSwitchSFX = ringSwitchSounds[2];
        }
        if(radius > .95f)
        {
            ringSwitchSFX = ringSwitchSounds[3];
        }
        if(radius > 1.05f)
        {
            ringSwitchSFX = ringSwitchSounds[4];
        }
        if(radius > 1.15f)
        {
            ringSwitchSFX = ringSwitchSounds[5];
        }
        if(radius > 1.25f)
        {
            ringSwitchSFX = ringSwitchSounds[6];
        }
        if(radius > 1.35f)
        {
            ringSwitchSFX = ringSwitchSounds[7];
        }
        if(radius > 1.45f)
        {
            ringSwitchSFX = ringSwitchSounds[8];
        }
        if(radius > 1.55f)
        {
            ringSwitchSFX = ringSwitchSounds[9];
        }
        if(radius > 1.65f)
        {
            ringSwitchSFX = ringSwitchSounds[10];
        }


        GameController.Instance.Play2DClipAtPoint(ringSwitchSFX, 1f);

    }


    // Move the camera to center on the new ring
    void CameraToNewRing(GameObject ring)
    {
        var vcam = cinemachine.GetComponent<CinemachineVirtualCamera>();
        
        vcam.LookAt = ring.transform;
        vcam.Follow = ring.transform;

        // Should the camera rotate?
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
    }

    // Update is called once per frame
    void Update () {
        
        hopCoolDown -= Time.deltaTime;
        if(gc.gameState == GameController.gameStates.starting) {
            ticker -= Time.deltaTime;
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

    void GameOver()
    {
        // Play sound effect
        GameController.Instance.Play2DClipAtPoint(playerDeathSound, Random.Range(0.9f, 1.1f));
       
        // Set gamestate to transition
        GameController.Instance.SetState(GameController.gameStates.transition);

        // this.transform.DOScale(0.06f, 3.2f);
        this.transform.DOMove(new Vector3(0, 0, 0), 3.2f);

        // Reset scene after a delay
        GameController.Instance.Ending();

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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 7)
        {
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

        // Collision with enemy
        if(collision.gameObject.layer == 10)
        {

            Debug.Log("Game over");
            GameOver();
            Death();
            Destroy(gameObject);

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
