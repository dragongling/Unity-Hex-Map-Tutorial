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

    public enum Tool { Brush, Elevation }

    private void Awake()
    {
        SelectColor(0);
        toolSelected = Tool.Brush;
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
        if (Input.GetMouseButton(0))
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            EditCell(hexGrid.GetCell(hit.point));
        }
    }

    private void EditCell(HexCell cell)
    {
        if(toolSelected == Tool.Brush)
        {
            cell.color = activeColor;
        }
        if(toolSelected == Tool.Elevation)
        {
            cell.Elevation = activeElevation;
        }        
        hexGrid.Refresh();
    }

}
