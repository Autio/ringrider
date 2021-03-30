using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingEffect : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public float radius;
    public float origRadius;
    public float speed = 1;

    public Color color;
    
    float width = .03f;

    public bool enabled = false;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(enabled)
        {   
            
            radius += Time.deltaTime;
            color.a -= Time.deltaTime / origRadius;
            DrawPolygon(60, radius, transform.position, width, width, color);
        }
    }

    public void DrawPolygon(int vertexNumber, float radius, Vector3 centerPos, float startWidth, float endWidth, Color color)
    {
        
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
        lineRenderer.loop = true;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        

        float angle = 2 * Mathf.PI / vertexNumber;
        lineRenderer.positionCount = vertexNumber;

        for (int i = 0; i < vertexNumber; i++)
        {
            Matrix4x4 rotationMatrix = new Matrix4x4(new Vector4(Mathf.Cos(angle * i), Mathf.Sin(angle * i), 0, 0),
                                                    new Vector4(-1 * Mathf.Sin(angle * i), Mathf.Cos(angle * i), 0, 0),
                                    new Vector4(0, 0, 1, 0),
                                    new Vector4(0, 0, 0, 1));
            Vector3 initialRelativePosition = new Vector3(0, radius, 0);
            lineRenderer.SetPosition(i, centerPos + rotationMatrix.MultiplyPoint(initialRelativePosition));

        }

        // Make sure the circle loops perfectly by setting the last and the first points as the same
        lineRenderer.SetPosition(vertexNumber -1, lineRenderer.GetPosition(0));
    }


}
