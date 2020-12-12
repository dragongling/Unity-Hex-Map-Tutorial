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
    }

    public void SelectElevationTool()
    {
        toolSelected = Tool.Elevation;
    }

    public void SelectColor(int index)
    {
        activeColor = colors[index];
    }

    public void SetElevation(float newElevation)
    {
        activeElevation = (int)newElevation;
    }

    private void Update()
    {
        /*if (Input.GetMouseButton(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                HandleInput();
            }
        }*/
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(inputRay, out RaycastHit hit))
            {
                Debug.Log(hit.collider.name);
                HexCell cell = hexGrid.GetCell(hit.point);
                if (cell)
                {
                    selectionPlane.SetActive(true);
                    Vector3 pos = cell.Position;
                    pos.y = cell.Elevation + 1f;
                    selectionPlane.transform.position = pos;
                    if (Input.GetMouseButton(0))
                    {
                        EditCells(cell);
                    }
                }
                
            }
        }
        else
        {
            selectionPlane.SetActive(false);
        }
    }

    private void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(inputRay, out RaycastHit hit))
        {
            EditCells(hexGrid.GetCell(hit.point));
        }
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
