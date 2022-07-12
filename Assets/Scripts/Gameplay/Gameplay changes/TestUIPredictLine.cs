using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUIPredictLine : Graphic
{
    private List<Vector2> points;

    public float Thickness = 10f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (points == null || points.Count < 2) {
            return;
        }

        for (int i = 0; i < points.Count; i++) {
            Vector2 point = points[i];
            DrawVerticesForPoint(point, vh);
        }

        for (int i = 0; i < points.Count - 1; i++)
        {
            int index = i * 2;
            vh.AddTriangle(index + 0, index + 1, index + 3);
            vh.AddTriangle(index + 3, index + 2, index + 0);
        }
    }

    public void DrawLines(List<Vector2> points) {
        this.points = points;
    }

    private void DrawVerticesForPoint(Vector2 point, VertexHelper vh) {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = new Vector3(Thickness / -2, 0);
        vertex.position += new Vector3(point.x, point.y);
        vh.AddVert(vertex);

        vertex.position = new Vector3(Thickness / 2, 0);
        vertex.position += new Vector3(point.x, point.y);
        vh.AddVert(vertex);
    }

    private void Update()
    {
        SetVerticesDirty();
    }
}
