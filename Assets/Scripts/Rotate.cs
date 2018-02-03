using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {
    public bool dir;
    public float speed = 1.0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (dir)
        {
            transform.Rotate(Vector3.forward, speed * Time.deltaTime);
        } else

        {
            transform.Rotate(Vector3.forward, -speed * Time.deltaTime);
        }
	}
}
