using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundRing : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public float speed = 1.0f;
    public Vector2 dir; 
    public float radius;
    public Vector2 pos;

    public Color ringColor;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        pos += dir * speed * Time.deltaTime;
        DrawPolygon(30, radius, pos, 0.015f, 0.015f, ringColor);
    }

    public void DrawPolygon(int vertexNumber, float radius, Vector3 centerPos, float startWidth, float endWidth, Color color)
    {
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
        lineRenderer.loop = true;
        //color.g += Random.Range(-.02f,.02f);
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
            lineRenderer.SetPosition(vertexNumber -1, lineRenderer.GetPosition(0));

        }
    }

}