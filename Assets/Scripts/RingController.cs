using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingController : MonoBehaviour
{
    // Manage the standard level of rings
    List<GameObject> Rings = new List<GameObject>();
    public GameObject ringPrefab;
    // Start is called before the first frame update
    void Start()
    {
        // Grab that start ring
        BuildLevel(100,1,4);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    // Build level
    void BuildLevel(int rings, float minRingRadius, float maxRingRadius)
    {
        // Create a sequence of rings in a chain where the next ring is tangential to the previous one
        // And the rings don't overlap with previous rings
        float ringWidth = 0.07f; // To account for the width itself
        Vector2 previousSpawnPos = new Vector2(0,0);
        float radius = 2f;
        float previousRadius = 0f;
    
        Vector3 spawnPos = new Vector2(0,0);
        for (int i = 0; i < rings; i++)
        {
            previousSpawnPos = spawnPos;
            previousRadius = radius;
            radius = Random.Range(minRingRadius, maxRingRadius);
            // xPosition of the centre of thew new circle is 
            // a point on the existing circumference plus 
            // a vector in that direction, magnitude new radius
            float angle = Random.Range(0, 3.14f);
            float xPos = previousSpawnPos.x + (previousRadius + ringWidth + radius) * Mathf.Cos(angle);
            float yPos = previousSpawnPos.y + (previousRadius + ringWidth + radius) * Mathf.Sin(angle);
            
            spawnPos = new Vector2(xPos, yPos);
            
            GameObject newRing = Instantiate(ringPrefab, spawnPos, Quaternion.identity);
            // Set the initial conditions of the ring appropriately.
            newRing.GetComponent<Ring>().radius = radius;
            newRing.GetComponent<Ring>().DrawPolygon(60, radius, spawnPos, ringWidth, ringWidth);
            // Add the ring to the list
            Rings.Add(newRing);
        }
    }

}
