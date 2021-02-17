using UnityEngine;

public abstract class HexMapBrush : MonoBehaviour
{
    public GameObject brushUI;
    public HexSelector hexSelector;

    public void Select()
    {
        if(brushUI)
            brushUI.SetActive(true);
    }

    public void Deselect()
    {
        if (brushUI)
            brushUI.SetActive(false);
    }

    public abstract void OnCellEdit(HexCell editedCell);
    public abstract void OnCellErase(HexCell erasedCell);
    public abstract void OnCellDragEdit(HexCell previousCell, HexCell currentCell);
}
