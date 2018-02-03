using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.name == "StartTrigger")
        {
            // The player has landed and should be parented to the inner circle
            this.GetComponent<Rigidbody2D>().simulated = false;
            this.transform.parent = GameObject.Find("PlayTrack1").transform;
            GameObject.Find("- GameController").GetComponent<Gamecontroller>().SwitchTrack();
        }
    }
}
