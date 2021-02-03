using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolbarToggleButton : MonoBehaviour
{
    public Toggle Toggle;
    public Image Icon;
    public Color SelectedIconColor;
    public Color DisabledIconColor;
    private Color DeselectedIconColor;
    // Start is called before the first frame update
    void Start()
    {
        DeselectedIconColor = Icon.color;
        Toggle.onValueChanged.AddListener(OnValueChanged);        
        OnValueChanged(Toggle.isOn);
    }

    private void Awake()
    {
        if (!Toggle.interactable)
        {
            Debug.Log(Icon.sprite.name);
            Icon.color = DisabledIconColor;
        }
    }

    void Select()
    {
        Icon.color = SelectedIconColor;
    }

    void Deselect()
    {
        Icon.color = DeselectedIconColor;
    }

    public void OnValueChanged(bool on)
    {
        if (on)
            Select();
        else
            Deselect();
    }
}
