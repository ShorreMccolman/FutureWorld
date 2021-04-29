using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollPopup : MonoBehaviour
{
    [SerializeField] RectTransform Background;

    [SerializeField] Text Title;
    [SerializeField] Text Date;
    [SerializeField] Text Text;

    public void UpdateUI(Scroll item)
    {
        Title.text = item.DisplayName;
        Date.text = item.Date;
        Text.text = item.Description;

        Background.sizeDelta = new Vector2(Background.sizeDelta.x, Text.preferredHeight + 55);
    }
}
