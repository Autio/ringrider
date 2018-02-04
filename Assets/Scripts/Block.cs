using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
    public float time = 10f;
	// Use this for initialization
	void Start () {
        Destroy(this.gameObject, time);	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
