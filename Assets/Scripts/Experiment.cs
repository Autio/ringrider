using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experiment : MonoBehaviour {
    public Transform block;
    public Color[] colors;
	// Use this for initialization
	void Start () {
        MakeBlocks(35, 15, 15);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void MakeBlocks(int h, int w, int d)
    {
        for (int a = 0; a < h; a++)
        {
            for (int b = 0; b < w; b++)
            {
                for (int c = 0; c < d; c++)
                {
                    Transform t = Instantiate(block, new Vector3(a, b, c), Quaternion.identity);
                    t.GetComponent<Renderer>().material.color = colors[Random.Range(0, colors.Length)];
                }
            }
        }
    }
    
}
