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

    public enum Tool { Brush, Elevation, River }
    bool brushSizeEnabled = true;

    bool isDrag;
    HexDirection dragDirection, prevDragDirection;
    HexCell previousCell;

    bool LMBPressed, RMBPressed, prevRMBPressed;

    private void Start()
    {
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
                if (toolSelected == Tool.Brush && (prevRMBPressed != RMBPressed))
                {
                    hexSelector.BorderColor = RMBPressed ? secondaryColor : primaryColor;
                }
                prevRMBPressed = RMBPressed;

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
                if (LMBPressed)
                    cell.Elevation = activeElevation;
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
        }
    }
}
