using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{
    public HexGrid hexGrid;
    public TwoColorPanel colorPanel;
    public Color primaryColor, secondaryColor;
    public GameObject elevationPanel;
    private int activeElevation;
    private Tool toolSelected;
    int brushSize;

    public HexSelector hexSelector;

    public enum Tool { Brush, Elevation, River,
        Road
    }
    bool brushSizeEnabled = true;

    bool isDrag;
    HexDirection dragDirection, prevDragDirection;
    HexCell previousCell;

    bool LMBPressed, RMBPressed, prevLMBPressed, prevRMBPressed;

    private HexContainer<bool> cellElevationEdited;
    private bool elevationChangeStarted = false;
    private int elevationToChange;

    private void Start()
    {
        cellElevationEdited = new HexContainer<bool>(hexGrid.CellCountX, hexGrid.CellCountZ);
        colorPanel.PrimaryColorButton.Color = primaryColor;
        colorPanel.SecondaryColorButton.Color = secondaryColor;
        SelectBrushTool();
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
        hexSelector.BorderColor = primaryColor;
        brushSizeEnabled = true;
        colorPanel.gameObject.SetActive(true);
        elevationPanel.SetActive(false);
    }

    public void SelectElevationTool()
    {
        toolSelected = Tool.Elevation;
        hexSelector.BorderColor = Color.white;
        brushSizeEnabled = true;
        colorPanel.gameObject.SetActive(false);
        elevationPanel.SetActive(true);
    }

    public void SelectRiverTool()
    {
        toolSelected = Tool.River;
        hexSelector.BorderColor = Color.white;
        brushSizeEnabled = true;
        brushSize = 0;
        colorPanel.gameObject.SetActive(false);
        elevationPanel.SetActive(false);
    }

    public void SelectRoadTool()
    {
        toolSelected = Tool.Road;
        hexSelector.BorderColor = Color.white;
        brushSizeEnabled = true;
        brushSize = 0;
        colorPanel.gameObject.SetActive(false);
        elevationPanel.SetActive(false);
    }

    public void SetPrimaryColor(Color newColor)
    {
        primaryColor = newColor;
        if (toolSelected == Tool.Brush)
            hexSelector.BorderColor = primaryColor;
    }

    public void SetSecondaryColor(Color newColor)
    {
        secondaryColor = newColor;
    }

    public void SetElevation(float newElevation)
    {
        activeElevation = (int)newElevation;
    }

    private void Update()
    {
        AdjustBrushSize();

        LMBPressed = Input.GetMouseButton(0);
        RMBPressed = Input.GetMouseButton(1);

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }
        else
        {
            previousCell = null;
            hexSelector.ClearSelection();
        }

        prevRMBPressed = RMBPressed;
        prevLMBPressed = LMBPressed;
    }

    private void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(inputRay, out RaycastHit hit))
        {
            HexCell centerCell = hexGrid.GetCell(hit.point);
            List<HexCell> affectedCells = hexGrid.GetCellsAround(centerCell, brushSize);
            if (centerCell)
            {
                if (toolSelected == Tool.Brush && (prevRMBPressed != RMBPressed))
                {
                    hexSelector.BorderColor = RMBPressed ? secondaryColor : primaryColor;
                }
                if (toolSelected == Tool.Elevation && (!LMBPressed && prevLMBPressed) || (!RMBPressed && prevRMBPressed))
                {
                    cellElevationEdited.Clear(sizeof(bool));
                    elevationChangeStarted = false;
                }

                hexSelector.Select(affectedCells);
                if (LMBPressed || RMBPressed)
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
        prevDragDirection = dragDirection;
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
                if (LMBPressed)
                    cell.Color = primaryColor;
                if (RMBPressed)
                    cell.Color = secondaryColor;
            }
            if (toolSelected == Tool.Elevation)
            {
                if (cellElevationEdited[cell.coordinates])
                    return;
                if (elevationChangeStarted && cell.Elevation != elevationToChange)
                    return;
                if (LMBPressed)
                {
                    elevationToChange = cell.Elevation;
                    cell.Elevation += 1;
                    cellElevationEdited[cell.coordinates] = true;                    
                    elevationChangeStarted = true;
                }
                if (RMBPressed && cell.Elevation > 0)
                {
                    elevationToChange = cell.Elevation;
                    cell.Elevation -= 1;
                    cellElevationEdited[cell.coordinates] = true;
                    elevationChangeStarted = true;
                }
            }
            if (toolSelected == Tool.River)
            {
                if (isDrag && LMBPressed && dragDirection.Opposite() != prevDragDirection)
                {
                    previousCell.SetOutgoingRiver(dragDirection);
                }
                else if (RMBPressed)
                {
                    cell.RemoveRiver();
                }
            }
            if (toolSelected == Tool.Road)
            {
                if (isDrag && LMBPressed && dragDirection.Opposite() != prevDragDirection)
                {
                    previousCell.AddRoad(dragDirection);
                }
                else if (RMBPressed)
                {
                    cell.RemoveRoads();
                }
            }
        }
    }
}
