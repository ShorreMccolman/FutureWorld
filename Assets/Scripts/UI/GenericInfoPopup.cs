using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericInfoPopup : MonoBehaviour
{
    [SerializeField] RectTransform Background;
    [SerializeField] Text Title;
    [SerializeField] Text Body;

    public void UpdateUI(string title, string body)
    {
        float height = 60f;
        float width = 450f;

        Title.text = title;
        Body.text = body;
        Body.fontSize = 20;
        Body.alignment = TextAnchor.UpperCenter;
        if(!string.IsNullOrEmpty(body))
            height += Body.preferredHeight + 30;

        Background.sizeDelta = new Vector2(width, height);

        float x = Input.mousePosition.x <= Screen.width / 2 ? Background.sizeDelta.x / 2f : -Background.sizeDelta.x / 2f;
        float y = Input.mousePosition.y <= Screen.height / 4 ? Background.sizeDelta.y / 2f : -Background.sizeDelta.y / 2f;

        Background.position = Input.mousePosition + new Vector3(x, y, 0);
    }

    public void UpdateUI(string title, string body, int size, TextAnchor anchor)
    {
        float height = 60f;
        float width = 450f;

        Title.text = title;
        Body.text = body;
        Body.fontSize = size;
        Body.alignment = anchor;
        if (!string.IsNullOrEmpty(body))
            height += Body.preferredHeight + 30;

        Background.sizeDelta = new Vector2(width, height);

        float x = Input.mousePosition.x <= Screen.width / 2 ? Background.sizeDelta.x / 2f : -Background.sizeDelta.x / 2f;
        float y = Input.mousePosition.y <= Screen.height / 4 ? Background.sizeDelta.y / 2f : -Background.sizeDelta.y / 2f;

        Background.position = Input.mousePosition + new Vector3(x, y, 0);
    }
}
