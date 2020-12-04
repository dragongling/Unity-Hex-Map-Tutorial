using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridCamera : MonoBehaviour
{
    public HexGrid hexGrid;

    private void Awake()
    {
        TopDownCamera camera = transform.GetComponent<TopDownCamera>();
        camera.minX = hexGrid.LeftmostCellPosition;
        camera.maxX = hexGrid.RightmostCellPosition;
        camera.minZ = hexGrid.BottommostCellPosition;
        camera.maxZ = hexGrid.TopmostCellPosition;
    }
}
