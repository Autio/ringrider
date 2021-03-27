using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleCentre : MonoBehaviour
{
    // Start is called before the first frame update
    public int id;
    public Color color;
    public float maxRadius;
    public float expansionSpeed = 1.0f;
    float circleRadius = 0.1f;
    private LineRenderer lineRenderer;
    public bool active = false;
    void Start()
    {
        //radius = this.transform.localScale.x;
        lineRenderer = GetComponent<LineRenderer>();

       // DrawCircle(texture, new Color(.2f,.2f,.2f), transform.position.x, transform.position.y, 1); 
    }

    // Update is called once per frame
    void Update()
    {
        if(active && circleRadius < maxRadius)
        {
            circleRadius += Time.deltaTime * expansionSpeed;
            this.transform.localScale = new Vector3(circleRadius, circleRadius, circleRadius); 
            DrawPolygon(60, circleRadius / 2, this.transform.position, circleRadius, circleRadius, color);

        }
    }

    
    public void DrawPolygon(int vertexNumber, float radius, Vector3 centerPos, float startWidth, float endWidth, Color color)
    {
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
        lineRenderer.loop = true;
      //  color.g += Random.Range(-.02f,.02f);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        float angle = 2 * Mathf.PI / vertexNumber;
        lineRenderer.positionCount = vertexNumber;

        for (int i = 0; i < vertexNumber; i++)
        {
            Matrix4x4 rotationMatrix = 
                                    new Matrix4x4(new Vector4(Mathf.Cos(angle * i), Mathf.Sin(angle * i), 0, 0),
                                    new Vector4(-1 * Mathf.Sin(angle * i), Mathf.Cos(angle * i), 0, 0),
                                    new Vector4(0, 0, 1, 0),
                                    new Vector4(0, 0, 0, 1));
            Vector3 initialRelativePosition = new Vector3(0, radius, 0);
            lineRenderer.SetPosition(i, centerPos + rotationMatrix.MultiplyPoint(initialRelativePosition));
        }
        // Make sure the circle loops perfectly by setting the last and the first points as the same
        lineRenderer.SetPosition(vertexNumber - 1, lineRenderer.GetPosition(0));

  
    }
    


}