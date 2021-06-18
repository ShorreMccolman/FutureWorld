using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogEntryUI : MonoBehaviour
{
    [SerializeField] Text Label;
    [SerializeField] Image Seperator;

    public float Height { get; protected set; }

    public void Setup(string description, bool hideBar = false)
    {
        Label.text = description;
        Height = Label.preferredHeight;
        ((RectTransform)transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Height);
        Seperator.enabled = false;
    }

    public void ShowSeperator()
    {
        Seperator.enabled = true;
    }
}
