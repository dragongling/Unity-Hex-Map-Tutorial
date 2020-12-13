using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{
    public HexGrid hexGrid;
    public Color[] colors;
    private Color activeColor;
    private int activeElevation;
    private Tool toolSelected;
    int brushSize;

    public GameObject selectionPlane;
    public HexSelector hexSelector;

    public enum Tool { Brush, Elevation }

    private void Awake()
    {
        SelectColor(0);
        toolSelected = Tool.Brush;        
    }

    public void SetBrushSize(float newSize)
    {
        brushSize = (int)newSize;
    }

    public void SelectBrushTool()
    {
        toolSelected = Tool.Brush;
        hexSelector.BorderColor = activeColor;
    }

    public void SelectElevationTool()
    {
        toolSelected = Tool.Elevation;
        hexSelector.BorderColor = Color.white;
    }

    public void SelectColor(int index)
    {
        activeColor = colors[index];
        if(toolSelected == Tool.Brush)
            hexSelector.BorderColor = activeColor;
    }

    public void SetElevation(float newElevation)
    {
        activeElevation = (int)newElevation;
    }

    private void Update()
    {
        if (!Input.GetKey(KeyCode.LeftControl))
        {
            float wheelDelta = Input.GetAxis("Mouse ScrollWheel");
            if (wheelDelta > 0f)
            {
                brushSize++;
            }
            else if(wheelDelta < 0f && brushSize > 0)
            {
                brushSize--;
            }
        }

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(inputRay, out RaycastHit hit))
            {
                HexCell centerCell = hexGrid.GetCell(hit.point);
                List<HexCell> affectedCells = GetCellsAround(centerCell, brushSize);
                if (centerCell)
                {
                    hexSelector.Select(affectedCells);
                    if (Input.GetMouseButton(0))
                    {
                        EditCells(affectedCells);
                    }
                }
                else
                {
                    hexSelector.ClearSelection();
                }
                
            }
        }
        else
        {
            hexSelector.ClearSelection();
        }
    }

    List<HexCell> GetCellsAround(HexCell center, int radius)
    {
        List<HexCell> result = new List<HexCell> { center };
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for (int r = 0, z = centerZ - radius; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + radius; x++)
            {
                HexCell cell = hexGrid.GetCell(new HexCoordinates(x, z));
                if(cell)
                    result.Add(cell);
            }
        }
        for (int r = 0, z = centerZ + radius; z > centerZ; z--, r++)
        {
            for (int x = centerX - radius; x <= centerX + r; x++)
            {
                HexCell cell = hexGrid.GetCell(new HexCoordinates(x, z));
                if (cell)
                    result.Add(cell);
            }
        }
        return result;
    }

    void EditCells(HexCell center)
    {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for(int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
        {
            for(int x = centerX - r; x <= centerX + brushSize; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));    
            }
        }
        for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
        {
            for (int x = centerX - brushSize; x <= centerX + r; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
    }

    public void ShowUI(bool visible)
    {
        hexGrid.ShowUI(visible);
    }

    void EditCells(ICollection<HexCell> cells)
    {
        foreach (HexCell cell in cells)
            EditCell(cell);
    }

    private void EditCell(HexCell cell)
    {
        if (cell)
        {
            if (toolSelected == Tool.Brush)
            {
                cell.Color = activeColor;
            }
            if (toolSelected == Tool.Elevation)
            {
                cell.Elevation = activeElevation;
            }
        }
    }
}
