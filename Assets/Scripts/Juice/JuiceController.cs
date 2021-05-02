using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuiceController : Singleton<JuiceController>
{
    List<Transform> TransitionCircleList = new List<Transform>();
    public Transform circlePrefab;
    public Color[] circleColors;
    private Color prevColor;

    public float rate = 0.6f;
    float counter = 0;
    int sortingOrder = 0;
    public bool emitting = false;

    public Vector3 position;

    public float expansionSpeed = 3.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(emitting)
        {
            counter += Time.deltaTime;
            if(counter > rate)
            {
                counter = 0;
                TransitionCircles(position);
                sortingOrder++;
            }
        }
    }

    // Expanding concentric circles
    public void TransitionCircles(Vector3 position)
    {   
        // Create a sequence of expanding rings just in the centre of the screen
        Transform circle = Instantiate(circlePrefab, position, Quaternion.identity);
        TransitionCircleList.Add(circle);
        JuiceCircle jc = circle.GetComponent<JuiceCircle>();
        Color newColor = prevColor;
        while (newColor == prevColor)
        {
            newColor = circleColors[Random.Range(0, circleColors.Length)];
        }
        jc.color = newColor;
        prevColor = newColor;
        jc.expansionSpeed = expansionSpeed;
        jc.position = position;
        expansionSpeed -= Time.deltaTime;
        circle.GetComponent<LineRenderer>().sortingOrder = sortingOrder;
    }

    public void DestroyCircles()
    {
        foreach (Transform t in TransitionCircleList)
        {
            Destroy(t.gameObject);
        }
        TransitionCircleList.Clear();
    }

}
