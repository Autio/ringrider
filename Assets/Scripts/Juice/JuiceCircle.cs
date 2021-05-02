using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuiceCircle : MonoBehaviour
{
    public Color color;
    public float expansionSpeed = 1.0f;
    public float circleRadius = .1f;
    private LineRenderer lineRenderer;
    public Vector3 position;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        circleRadius += Time.deltaTime * expansionSpeed;
        this.transform.localScale = new Vector3(circleRadius, circleRadius, circleRadius); 
        DrawPolygon(90, circleRadius / 2, this.transform.position, circleRadius + 0.05f, circleRadius + 0.05f, color, lineRenderer);
    }

    public void DrawPolygon(int vertexNumber, float radius, Vector3 centerPos, float startWidth, float endWidth, Color color, LineRenderer lineRenderer)
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
        lineRenderer.SetPosition(vertexNumber -1, lineRenderer.GetPosition(0));
    }
}
