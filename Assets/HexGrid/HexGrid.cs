using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    public int CellCountX { get { return cells.sizeX; } }
    public int CellCountZ { get { return cells.sizeZ; } }

    public int chunkCountX = 4;
    public int chunkCountZ = 3;

    public HexCell cellPrefab;
    public Text cellLabelPrefab;
    public Texture2D noiseSource;
    public HexGridChunk chunkPrefab;

    public float LeftmostCellPosition {
        get {
            return transform.position.x;
        }
    }

    public float BottommostCellPosition {
        get {
            return transform.position.z;
        }
    }

    public float RightmostCellPosition {
        get {
            return LeftmostCellPosition + (chunkCountX * HexMetrics.chunkSizeX - 0.5f) * (2f * HexMetrics.innerRadius);
        }
    }

    public float TopmostCellPosition {
        get {
            return BottommostCellPosition + (chunkCountZ * HexMetrics.chunkSizeZ - 1f) * (1.5f * HexMetrics.outerRadius);
        }
    }

    HexContainer<HexCell> cells;
    HexGridChunk[] chunks;

    public Color defaultColor;

    private void Awake()
    {
        HexMetrics.noiseSource = noiseSource;
        CreateChunks();
        CreateCells();
    }

    public void ShowUI(bool visible)
    {
        for (int i = 0; i < chunks.Length; i++)
        {
            chunks[i].ShowUI(visible);
        }
    }

    private void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for(int z = 0, i = 0; z < chunkCountZ; ++z)
        {
            for(int x = 0; x < chunkCountX; ++x)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    public List<HexCell> GetCellsAround(HexCell center, int radius)
    {
        return cells.GetItemsAround(center.coordinates.X, center.coordinates.Z, radius);
    }

    private void CreateCells()
    {
        cells = new HexContainer<HexCell>(chunkCountX * HexMetrics.chunkSizeX, chunkCountZ * HexMetrics.chunkSizeZ);
        for (int z = 0, i = 0; z < cells.sizeZ; z++)
        {
            for (int x = 0; x < cells.sizeX; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    private void OnEnable()
    {
        HexMetrics.noiseSource = noiseSource;
    }

    private void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate(cellPrefab);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.Color = defaultColor;

        if(x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }
        if(z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - cells.sizeX]);
                if (x > 0)
                    cell.SetNeighbor(HexDirection.SW, cells[i - cells.sizeX - 1]);
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - cells.sizeX]);
                if (x != cells.sizeX - 1)
                    cell.SetNeighbor(HexDirection.SE, cells[i - cells.sizeX + 1]);
            }
        }

        Text label = Instantiate(cellLabelPrefab);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();

        cell.uiRect = label.rectTransform;
        cell.Elevation = 0;

        AddCellToChunk(x, z, cell);
    }

    private void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cells.sizeX + coordinates.Z / 2;
        if (index < 0 || index >= cells.Length)
            return null;
        return cells[index];
    }
}
