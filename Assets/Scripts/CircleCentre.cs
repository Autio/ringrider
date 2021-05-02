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
    float circleRadius = 0.25f;
    private LineRenderer lineRenderer;
    public bool active = false;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void InitCircle() {
        DrawPolygon(42, .1f, this.transform.position, .25f, .25f, color);
    }

    // Update is called once per frame
    void Update()
    {
        if(active && circleRadius < maxRadius - .02f)
        {
            circleRadius += Time.deltaTime * expansionSpeed;
            this.transform.localScale = new Vector3(circleRadius, circleRadius, circleRadius); 
            DrawPolygon(42, circleRadius / 2, this.transform.position, circleRadius, circleRadius, color);
        }
    }

    public void DrawPolygon(int vertexNumber, float radius, Vector3 centerPos, float startWidth, float endWidth, Color color)
    {
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
        lineRenderer.loop = true;
      //  color.g += Random.Range(-.02f,.02f);
        color.a = 1.0f;
        color.r += 0.15f;
        color.g += 0.15f;
        color.b += 0.15f;
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
        lineRenderer.SetPosition(vertexNumber -1, lineRenderer.GetPosition(0));

    }
    
}