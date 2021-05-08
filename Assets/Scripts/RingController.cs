using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RingController : Singleton<RingController>
{
    // Manage the standard level of rings
    List<GameObject> Rings = new List<GameObject>();
    List<GameObject> CircleCenters = new List<GameObject>();

    List<GameObject> Enemies = new List<GameObject>();

    public AudioClip[] ringDeathSounds;
    public GameObject ringPrefab;
    public GameObject coinPrefab;

    public GameObject enemyPrefab;
    public GameObject ringEffectPrefab;
    public GameObject circleCentrePrefab;
    public GameObject circleTextPrefab;
    
    public GameObject startRing;
    public Color[] ringColours;
 
    // Start is called before the first frame update
    void Start()
    {
        // Grab that start ring
      //  BuildLevel(100,.7f,1.75f, startRing);
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    // Build level
    public void BuildLevel(int rings, float minRingRadius, float maxRingRadius)
    {
        startRing = GameObject.Find("StartRing");
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
            
            if(i % 2 != 0)
            {
                newRing.GetComponent<Ring>().GetEnemyTrack(true).GetComponent<Rotate>().dir = true;
            }

            newRing.GetComponent<Ring>().id = i;

            // Set the initial conditions of the ring appropriately.
            newRing.GetComponent<Ring>().radius = radius;
            newRing.GetComponent<CircleCollider2D>().radius = radius;

            Color ringColour = ringColours[Random.Range(0,ringColours.Length)];
            newRing.GetComponent<Ring>().DrawPolygon(60, radius, spawnPos, ringWidth, ringWidth, ringColour);
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

            // Create the circle at the centre
            GameObject newCircle = CreateCircleCentre(circleCentrePrefab, spawnPos, ringColour, radius, i);
            CircleCenters.Add(newCircle);
            newCircle.transform.parent = newRing.transform;

            // Create the ID text for the circle
            GameObject circleText = Instantiate(circleTextPrefab, spawnPos, Quaternion.identity);
            circleText.GetComponent<TextMeshPro>().text = (i + 1).ToString();
            circleText.transform.SetParent(newRing.transform);
            circleText.SetActive(false);


            
            // Create ring effect
            GameObject ro = CreateRingEffect(spawnPos,ringColour,radius);
            ro.transform.parent = newRing.transform;
            // Add the ring to the list
            Rings.Add(newRing);
            
        }

        // Second run-through
        int ringCount = Rings.Count;

        // Create some coins for the ring
        foreach(GameObject ring in Rings)
        {
            Ring ringScript = ring.GetComponent<Ring>();
            CreateCoins(ring, ring.transform.Find("Trigger").gameObject, ringScript.radius);
            int id = ringScript.id;
            bool inner = false;
            if(Random.Range(0, 10) < 5)
            {
                inner = true;
            }
            if (id > 3 && id < 10)
            {
                if(Random.Range(0, 10) < 2)
                {
                    CreateEnemy(ring.GetComponent<Ring>(), inner);                  
                }
            }
            if (id >= 10 && id < (int)(ringCount / 3))
            {
                if(Random.Range(0, 10) < 4)
                {
                    CreateEnemy(ring.GetComponent<Ring>(), inner);                  
                }
            }
            if (id >= (int)(ringCount / 3) && id < (int)(ringCount / 3 * 2))
            {
                if(Random.Range(0, 10) < 6)
                {
                    CreateEnemy(ring.GetComponent<Ring>(), inner);                  
                }
            }
            if (id > (int)(ringCount / 3 * 2))
            {
                if(Random.Range(0, 10) < 8)
                {
                    CreateEnemy(ring.GetComponent<Ring>(), inner);                  
                }
            }

        }

    }

    public void PrintMe()
    {
        Debug.Log("I exist");
    }

    // Create monsters
    void CreateEnemy(Ring ring, bool inner = true)
    {
        // Can be on the inner or the outer track
        // Ones on the outer track are still and the ones on the inner track move in the opposite direction to the player
        float radius = ring.radius;
        float angle = Random.Range(0, 2 * Mathf.PI);
        GameObject enemy = null;
        if(inner)
        {
            float innerRadius = radius - 0.14f;
            enemy = Instantiate(enemyPrefab, 
                new Vector3(ring.transform.position.x + Mathf.Cos(angle) * innerRadius, 
                ring.transform.position.y + Mathf.Sin(angle) * innerRadius, 0), 
                Quaternion.identity) as GameObject;

            enemy.transform.SetParent(ring.GetEnemyTrack(inner));
            ring.ActivateInnerEnemyTrack();

        } else 
        {
            float outerRadius = radius + 0.14f;

            bool overlaps = true;
            Vector2 spawnPos;
            int s_layerMask = LayerMask.GetMask("Ring"); 
            float xPos = ring.transform.position.x + Mathf.Cos(angle) * outerRadius;
            float yPos = ring.transform.position.y + Mathf.Sin(angle) * outerRadius;

            enemy = Instantiate(enemyPrefab, 
                new Vector3(xPos, yPos, 0), 
                Quaternion.identity) as GameObject;   

            while (overlaps)
            {
                angle = Random.Range(0, 2 * Mathf.PI);
                xPos = ring.transform.position.x + Mathf.Cos(angle) * outerRadius;
                yPos = ring.transform.position.y + Mathf.Sin(angle) * outerRadius;
                spawnPos = new Vector2(xPos, yPos);
                enemy.transform.position = spawnPos;

                // Make sure the new circle doesn't overlap old ones
                var olap = Physics2D.OverlapCircleAll(spawnPos, 0.19f, s_layerMask);
                if (olap.Length <= 1)
                {
                    overlaps = false;
                }
            }

            enemy.transform.SetParent(ring.gameObject.transform);

        }

        Enemies.Add(enemy);

    }

    GameObject CreateCircleCentre(GameObject circleCentrePrefab, Vector2 pos, Color colour, float radius, int id)
    {
        GameObject newCircle = Instantiate(circleCentrePrefab, pos, Quaternion.identity);
        CircleCentre cc = newCircle.GetComponent<CircleCentre>();
        cc.color = colour;
        cc.maxRadius = radius;
        cc.id = id;
        return newCircle;
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

        // TODO: Create occasional alternate rings

        // How many sequences of coins will there be?
        int seq = 0;
        if(radius > 1)
        {
            seq = Random.Range(1,6);
        }
        else 
        {
            seq = Random.Range(0,4);
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
                var overlaps = Physics2D.OverlapCircleAll(coin.transform.position, coin.GetComponent<CircleCollider2D>().radius * 1.35f, r_layerMask).Length;
                var c_overlaps = Physics2D.OverlapCircleAll(coin.transform.position, coin.GetComponent<CircleCollider2D>().radius * 1.3f, c_layerMask).Length;
                
                if((overlaps > 0 && !inner) || c_overlaps > 1) 
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

    GameObject CreateRingEffect(Vector2 spawnPos, Color ringColour, float radius)
    {
        GameObject go = Instantiate(ringEffectPrefab,spawnPos, Quaternion.identity);
        RingEffect re = go.GetComponent<RingEffect>();
        re.radius = radius;
        re.origRadius = radius;
        re.speed = radius * 2;
        re.color = ringColour;

        return go;

    }

    public IEnumerator DestroyLevel(int ringReached = 20)
    {
        Debug.Log("Destroying the level");
        // Start ticking away only from a bit ahead of the player
        int lastRing = Rings.Count - 1;
        int maxRing = Mathf.Min(lastRing, ringReached + 10);
                
        for (int i = lastRing; i > maxRing; i--)
        {
            Destroy(Rings[i]);
        }

        yield return new WaitForSeconds(0.5f);

        float delay = 0.01f;
        for (int i = maxRing; i >= 0; i--)
        {
            delay += 0.01f;
            yield return new WaitForSeconds(delay);

            GameController.Instance.Play2DClipAtPoint(ringDeathSounds[Random.Range(0,ringDeathSounds.Length)], Random.Range(0.9f,1f));
            
            Destroy(Rings[i]);
        }
        Rings.Clear();
        CircleCenters.Clear();
    }

}
