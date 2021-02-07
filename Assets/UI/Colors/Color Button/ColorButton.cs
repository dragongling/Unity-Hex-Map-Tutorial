using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorButton : MonoBehaviour, IPointerClickHandler
{
    public HSVPicker.ColorPicker picker;
    Image image;

    public Color Color {
        get {
            return image.color;
        }
        set {
            image.color = value;
        }
    }

    [Serializable]
    public class ValueChangedEvent : UnityEvent<Color> { }

    [SerializeField]
    private ValueChangedEvent ValueChanged = new ValueChangedEvent();
    public ValueChangedEvent OnValueChanged { get { return ValueChanged; } set { ValueChanged = value; } }

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        picker.onValueChanged.RemoveAllListeners();
        picker.AssignColor(image.color);
        picker.gameObject.SetActive(true);        
        picker.onValueChanged.AddListener(color =>
        {
            image.color = color;
            OnValueChanged.Invoke(color);
        });
    }
}
