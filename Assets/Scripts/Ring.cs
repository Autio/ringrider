using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    // A ring needs to have
    // 1. Its circumference
    // 2. The inside and outside tracks
    // 3. The tangents to adjacent circles where the player can jump = trigger

    // A ring can generate another adjacent ring but such that it doesn't overlap with a previous ring 

    // The linerenderer forms the circumference
    // Inner track and outer track invisible circles get 
    private LineRenderer lineRenderer;
    public Transform innerTrack;
    public Transform outerTrack;
    public float radius = 2.0f;
    float width = 0.07f;
    // Start is called before the first frame update
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // 2.58f and 2.5f are modifiers to get the object to nicely fit 
        innerTrack.localScale = new Vector3(radius *.98f / 2.58f, radius *.98f / 2.58f, 1);
        outerTrack.localScale = new Vector3(radius *1.02f / 2.5f, radius * 1.02f / 2.5f, 1);

    }

    public void DrawPolygon(int vertexNumber, float radius, Vector3 centerPos, float startWidth, float endWidth, Color color)
    {
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
        lineRenderer.loop = true;
        color.g += Random.Range(-.02f,.02f);
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
}

}
