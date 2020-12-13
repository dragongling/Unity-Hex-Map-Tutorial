using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexSelectionMesh : MonoBehaviour
{
    Mesh hexMesh;
    public Color selectionBorderAlbedoColor;

    static List<Vector3> vertices = new List<Vector3>();
    static List<int> triangles = new List<int>();
    static List<Color> colors = new List<Color>();

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        hexMesh.name = "Hex Selection Mesh";
    }

    public void Triangulate(HexCell[] cells)
    {
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();
        for (int i = 0; i < cells.Length; ++i)
        {
            Triangulate(cells[i]);
        }
        hexMesh.vertices = vertices.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.colors = colors.ToArray();
        hexMesh.RecalculateNormals();
    }

    private void Triangulate(HexCell cell)
    {
        Vector3 center = cell.Position;
        center.y += HexMetrics.selectionHeight;
        
        for (HexDirection direction = HexDirection.NE; direction <= HexDirection.NW; ++direction)
        {
            Vector3 vFarLeft = center + HexMetrics.GetFirstCorner(direction);
            Vector3 vFarRight = center + HexMetrics.GetSecondCorner(direction);
            Vector3 vCloseLeft = center + HexMetrics.GetFirstCorner(direction) * 0.9f;
            Vector3 vCloseRight = center + HexMetrics.GetSecondCorner(direction) * 0.9f;
            AddQuad(vCloseLeft, vCloseRight, vFarLeft, vFarRight);
            AddQuadColor(selectionBorderAlbedoColor, selectionBorderAlbedoColor, selectionBorderAlbedoColor, selectionBorderAlbedoColor);
        }
    }

    void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }

    void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
    {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);
        colors.Add(c4);
    }
}
