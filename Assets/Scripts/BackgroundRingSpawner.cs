using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundRingSpawner : MonoBehaviour
{
    
    // Spawn subtle rings that float in the background 
    public GameObject backgroundRing;
    public Color ringColor;
    List<GameObject> backgroundRings = new List<GameObject>();
    // Choose the overall direction of flow for all of the rings
    Vector2 dir;
    public float radius = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        dir = new Vector2(Random.Range(-0.2f, 0.2f), Random.Range(0.1f, 0.5f));
        for(int i = 0; i < 50; i++)
        {
            SpawnRing();
        }
    }

    void SpawnInitialRings()
    {
        
    }

    void SpawnRing()
    {
        // radius, position, direction of movement
        Vector2 pos = new Vector2(Random.Range(-10f,10f), Random.Range(-10f,10f));
        GameObject ring = Instantiate(backgroundRing, pos,Quaternion.identity);
        backgroundRings.Add(ring);
        BackgroundRing br = ring.GetComponent<BackgroundRing>();
        br.dir = dir;
        br.radius = Random.Range(radius * 0.5f, radius * 2f);
        br.pos = pos;
        br.speed = 0.2f;
        br.ringColor = ringColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

