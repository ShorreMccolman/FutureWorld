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

        float x = Input.mousePosition.x <= Screen.width / 2 ? Background.sizeDelta.x / 2f : -Background.sizeDelta.x / 2f;
        float y = Input.mousePosition.y <= Screen.height / 2 ? Background.sizeDelta.y / 3f : -Background.sizeDelta.y / 3f;

        Background.position = Input.mousePosition + new Vector3(x, y, 0);
    }
}
