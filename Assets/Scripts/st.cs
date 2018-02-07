using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistics : MonoBehaviour {
    public int gamePlays = 1;
	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this.transform.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
