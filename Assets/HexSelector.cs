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
}
