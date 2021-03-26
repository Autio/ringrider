using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingController : Singleton<GameController>
{
    // Manage the standard level of rings
    List<GameObject> Rings = new List<GameObject>();
    public GameObject ringPrefab;
    public GameObject coinPrefab;
    public GameObject startRing;
    public Color[] ringColours;

    // Start is called before the first frame update
    void Start()
    {
        // Grab that start ring
        BuildLevel(100,.6f,1.6f, startRing);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    // Build level
    void BuildLevel(int rings, float minRingRadius, float maxRingRadius, GameObject startRing)
    {
        // Create a sequence of rings in a chain where the next ring is tangential to the previous one
        // And the rings don't overlap with previous rings
        GameObject ringHolder = this.gameObject;
        try{
            ringHolder = GameObject.Find("Rings");
        } catch{
            Debug.Log("Couldn't find Rings gameobject to hold Rings in scene");
        }

        float ringWidth = 0.07f; // To account for the width itself
        Vector2 previousSpawnPos = new Vector2(0,0);
        float radius = startRing.GetComponent<Ring>().radius;
        float previousRadius = 0f;
        float angle = 0;
        int s_layerMask = LayerMask.GetMask("Ring"); 

        // Draw the start ring
        startRing.GetComponent<CircleCollider2D>().radius = radius;
        startRing.GetComponent<Ring>().DrawPolygon(60, radius, Vector2.zero, ringWidth, ringWidth, ringColours[Random.Range(0,ringColours.Length)]);

        Vector3 spawnPos = new Vector2(0,0);
        for (int i = 0; i < rings; i++)
        {
            previousSpawnPos = spawnPos;
            previousRadius = radius;
            bool overlaps = true;
            float xPos = 0;
            float yPos = 0;
            float intersectionX = 0;
            float intersectionY = 0;

            GameObject newRing = Instantiate(ringPrefab, spawnPos, Quaternion.identity);

            while (overlaps)
            {
                angle = Random.Range(0, 3.14f);
                radius = Random.Range(minRingRadius, maxRingRadius);
                intersectionX = previousSpawnPos.x + (previousRadius + ringWidth / 2) * Mathf.Cos(angle);
                intersectionY = previousSpawnPos.y + (previousRadius + ringWidth / 2) * Mathf.Sin(angle);

                xPos = previousSpawnPos.x + (previousRadius + ringWidth + radius) * Mathf.Cos(angle);
                yPos = previousSpawnPos.y + (previousRadius + ringWidth + radius) * Mathf.Sin(angle);
                spawnPos = new Vector2(xPos, yPos);
                newRing.transform.position = spawnPos;

                // Make sure the new circle doesn't overlap old ones
                var olap = Physics2D.OverlapCircleAll(spawnPos, radius + 0.05f, s_layerMask);
                if (olap.Length <= 1)
                {
                    overlaps = false;
                }
            }
            
            // Set the initial conditions of the ring appropriately.
            newRing.GetComponent<Ring>().radius = radius;
            newRing.GetComponent<CircleCollider2D>().radius = radius;

            newRing.GetComponent<Ring>().DrawPolygon(60, radius, spawnPos, ringWidth, ringWidth,  ringColours[Random.Range(0,ringColours.Length)]);
            
            newRing.transform.parent = ringHolder.transform;
            // Create the trigger which the player can hit to switch here
            GameObject triggerObject = new GameObject();
            triggerObject.AddComponent<CircleCollider2D>();
            // Size of the area where a player can hop between rings
            triggerObject.GetComponent<CircleCollider2D>().radius = 0.33f; 
            triggerObject.GetComponent<CircleCollider2D>().isTrigger = true;
            triggerObject.layer = 7;
            triggerObject.transform.name = "Trigger";
            triggerObject.transform.position = new Vector2(intersectionX, intersectionY);
            triggerObject.transform.parent = newRing.transform;

            // Create some coins for the ring

            // Add the ring to the list
            Rings.Add(newRing);
            
        }

        // Now add rings
        foreach(GameObject ring in Rings)
        {
            CreateCoins(ring, ring.transform.Find("Trigger").gameObject, ring.GetComponent<Ring>().radius);

        }

    }

    void CreateCoins(GameObject newRing, GameObject triggerObject, float radius)
    {
        // Pass the triggerobject to have a sense of where the player
        // enters the ring
        int r_layerMask = LayerMask.GetMask("Ring"); 
        int c_layerMask = LayerMask.GetMask("Coin"); 
        int mask = r_layerMask | c_layerMask;


        float start = SignedAngleBetween(Vector2.right,  triggerObject.transform.position - newRing.transform.position, Vector3.forward) * Mathf.Deg2Rad;
        // Assuming a static ring width here
        float innerRadius = radius - 0.14f;
        float outerRadius = radius + .14f; 
        // Random amounts, but in sequence
        // Has to be aware of the size of the ring

        float angle = .314f / radius;

        // .6f = approximate arc length of the co
        // .28f
        // for (float i = start; i < start + Mathf.PI * 2 - 1f; i += angle * 2)
        // {
        //     GameObject coin = Instantiate(coinPrefab, 
        //     new Vector3(newRing.transform.position.x + Mathf.Cos(i) * innerRadius, 
        //     newRing.transform.position.y + Mathf.Sin(i) * innerRadius, 0), 
        //     Quaternion.identity) as GameObject;   
        //     coin.transform.parent = newRing.transform;
        // }

        // for (float i = 0; i < Mathf.PI * 2 -.28f; i += angle)
        // {
        //     GameObject coin = Instantiate(coinPrefab, 
        //     new Vector3(newRing.transform.position.x + Mathf.Cos(i) * outerRadius, 
        //     newRing.transform.position.y + Mathf.Sin(i) * outerRadius, 0), 
        //     Quaternion.identity) as GameObject;   
        //     coin.transform.parent = newRing.transform;
        // }

        // TODO: Sequence of inner and outer coins
        // Starting point should be a bit after the trigger        
        int dir;
        if(Random.Range(0,10) < 5)
        {
            dir = 1;
        } else
        {
            dir = -1;
        }

        // TODO: Create occasional enemies

        // TODO: Increase speeds

        // How many sequences of coins will there be?
        int seq = 0;
        if(radius > 1)
        {
            seq = Random.Range(1,5);
        }
        else 
        {
            seq = Random.Range(0,3);
        }
        // How long can the sequences be? 
        int maxLength = 0;
        if(radius > 1)
        {
            maxLength = 18;
        } else 
        {
            maxLength = 12;
        }
        for (int i = 0; i < seq; i++)
        {
            int lengths = 0;
            int sequenceLength = Random.Range(maxLength / 2, maxLength + 1);
            lengths += sequenceLength;
            bool inner = true;
            if(Random.Range(0,10) < 4)
            {
                inner = false;
            }

            for (int j = 0; j < sequenceLength; j++)
            {
                GameObject coin;
                float coinAngle = start + lengths * angle * dir + j * angle * dir;

                if(inner) {
                    coin = Instantiate(coinPrefab, 
                    new Vector3(newRing.transform.position.x + Mathf.Cos(coinAngle) * innerRadius, 
                    newRing.transform.position.y + Mathf.Sin(coinAngle) * innerRadius, 0), 
                    Quaternion.identity) as GameObject;   
                }
                else {
                    coin = Instantiate(coinPrefab, 
                    new Vector3(newRing.transform.position.x + Mathf.Cos(coinAngle) * outerRadius, 
                    newRing.transform.position.y + Mathf.Sin(coinAngle) * outerRadius, 0), 
                    Quaternion.identity) as GameObject;   
                }
                coin.transform.parent = newRing.transform;
                var overlaps = Physics2D.OverlapCircleAll(coin.transform.position, coin.GetComponent<CircleCollider2D>().radius * 1.1f, mask).Length;
                Debug.Log(overlaps);

                if(overlaps > 1 && !inner) 
                {
                    Destroy(coin);
                    i++;

                } 
                // If overlaps with a ring, end sequence

            }
        }

        // float angle = i * Mathf.PI*2f / nodeCount;
        // Vector3 newPos = new Vector3(Mathf.Cos(angle)*radius, Mathf.Sin(angle)*radius, 0);
        // GameObject newNode = Instantiate(button, transform.position + newPos, Quaternion.identity);
        // newNode.transform.parent = GameObject.Find("NodeCanvas").transform;
        // newNode.GetComponent<RectTransform>().localPosition = new Vector2(newPos.x, newPos.y + yOffset);



    }

    float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n){
    // angle in [0,180]
    float angle = Vector3.Angle(a,b);
    float sign = Mathf.Sign(Vector3.Dot(n,Vector3.Cross(a,b)));

    // angle in [-179,180]
    float signed_angle = angle * sign;

    // angle in [0,360] (not used but included here for completeness)
    //float angle360 =  (signed_angle + 180) % 360;

    return signed_angle;
}

}
