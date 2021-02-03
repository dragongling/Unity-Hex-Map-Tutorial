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

    public enum Tool { Brush, Elevation, River }
    bool brushSizeEnabled = true;

    bool isDrag;
    HexDirection dragDirection;
    HexCell previousCell;

    private void Awake()
    {
        SelectColor(0);
        toolSelected = Tool.Brush;        
    }

    public void SetBrushSize(float newSize)
    {
        if (brushSizeEnabled)
        {
            brushSize = (int)newSize;
        }
    }

    public void SelectBrushTool()
    {
        toolSelected = Tool.Brush;
        hexSelector.BorderColor = activeColor;
        brushSizeEnabled = true;
    }

    public void SelectElevationTool()
    {
        toolSelected = Tool.Elevation;
        hexSelector.BorderColor = Color.white;
        brushSizeEnabled = true;
    }

    public void SelectRiverTool()
    {
        toolSelected = Tool.River;
        hexSelector.BorderColor = Color.white;
        brushSizeEnabled = false;
        brushSize = 0;
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
        AdjustBrushSize();

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }
        else
        {
            previousCell = null;
            hexSelector.ClearSelection();
        }
    }

    private void HandleInput()
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
                    if (previousCell && previousCell != centerCell)
                    {
                        ValidateDrag(centerCell);
                    }
                    else
                    {
                        isDrag = false;
                    }
                    EditCells(affectedCells);
                    previousCell = centerCell;
                }
            }
            else
            {
                previousCell = null;
                hexSelector.ClearSelection();
            }
        }
    }

    private void ValidateDrag(HexCell currentCell)
    {
        for(dragDirection = HexDirection.NE; dragDirection <= HexDirection.NW; ++dragDirection)
        {
            if(previousCell.GetNeighbor(dragDirection) == currentCell)
            {
                isDrag = true;
                return;
            }
        }
        isDrag = false;
    }

    private void AdjustBrushSize()
    {
        if (brushSizeEnabled && Input.GetKey(KeyCode.LeftControl))
        {
            float wheelDelta = Input.GetAxis("Mouse ScrollWheel");
            if (wheelDelta > 0f)
            {
                brushSize++;
            }
            else if (wheelDelta < 0f && brushSize > 0)
            {
                brushSize--;
            }
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
            if (toolSelected == Tool.River && isDrag)
            {
                previousCell.SetOutgoingRiver(dragDirection);
            }
        }
    }
}
