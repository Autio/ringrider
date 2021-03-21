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
        float angle = 0;
        int s_layerMask = LayerMask.GetMask("Ring"); 

        Vector3 spawnPos = new Vector2(0,0);
        for (int i = 0; i < rings; i++)
        {
            previousSpawnPos = spawnPos;
            previousRadius = radius;
            bool overlaps = true;
            float xPos = 0;
            float yPos = 0;
            GameObject newRing = Instantiate(ringPrefab, spawnPos, Quaternion.identity);

            while (overlaps)
            {
                angle = Random.Range(0, 3.14f);
                radius = Random.Range(minRingRadius, maxRingRadius);
                xPos = previousSpawnPos.x + (previousRadius + ringWidth + radius) * Mathf.Cos(angle);
                yPos = previousSpawnPos.y + (previousRadius + ringWidth + radius) * Mathf.Sin(angle);
                spawnPos = new Vector2(xPos, yPos);
                newRing.transform.position = spawnPos;

                // Make sure the new circle doesn't overlap old ones
                var olap = Physics2D.OverlapCircleAll(spawnPos, radius, s_layerMask);
                if (olap.Length <= 1)
                {
                    overlaps = false;
                }
            }
            
            // Set the initial conditions of the ring appropriately.
            newRing.GetComponent<Ring>().radius = radius;
            newRing.GetComponent<CircleCollider2D>().radius = radius;
            newRing.GetComponent<Ring>().DrawPolygon(60, radius, spawnPos, ringWidth, ringWidth);
            // Add the ring to the list
            Rings.Add(newRing);

            // TODO: Add triggers for the tangent point
        }
    }

}
