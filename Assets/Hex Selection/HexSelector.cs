using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexSelector : MonoBehaviour
{
    public HexSelectionMesh mesh;

    List<HexCell> selectedCells;

    public void Awake()
    {
        selectedCells = new List<HexCell>();
    }

    public Color BorderColor { 
        set {
            Color.RGBToHSV(value, out float h, out float s, out float v);
            v = 1.0f;
            mesh.GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(h, s, v);
        }
    }

    public void ClearSelection()
    {
        selectedCells.Clear();
        mesh.Triangulate(selectedCells.ToArray());
    }

    public void Select(HexCell cell)
    {
        selectedCells.Clear();
        selectedCells.Add(cell);
        mesh.Triangulate(selectedCells.ToArray());
    }

    public void Select(params HexCell[] cells)
    {
        selectedCells.Clear();
        selectedCells.AddRange(cells);
        mesh.Triangulate(selectedCells.ToArray());
    }

    public void Deselect(HexCell cell)
    {
        selectedCells.Remove(cell);
        mesh.Triangulate(selectedCells.ToArray());
    }

    public void Select(List<HexCell> selectedCells)
    {
        mesh.Triangulate(selectedCells.ToArray());
    }
}
