using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class HexMapColorBrush : HexMapBrush
{
    private Color primaryColor;
    private Color secondaryColor;
    
    public void SetPrimaryColor(Color newColor)
    {
        primaryColor = newColor;
        hexSelector.BorderColor = primaryColor;
    }

    public void SetSecondaryColor(Color newColor)
    {
        secondaryColor = newColor;
    }

    public override void OnCellDragEdit(HexCell previousCell, HexCell currentCell)
    {
    }

    public override void OnCellEdit(HexCell editedCell)
    {
        editedCell.Color = primaryColor;
    }

    public override void OnCellErase(HexCell erasedCell)
    {
        erasedCell.Color = secondaryColor;
    }
}
