using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellInfoPopup : MonoBehaviour
{
    [SerializeField] RectTransform Background;
    [SerializeField] Image Image;
    [SerializeField] Text Name;
    [SerializeField] Text Body;
    [SerializeField] Text School;
    [SerializeField] Text Cost;

    public void UpdateUI(SpellData data)
    {
        int height = 200;
        int width = 500;

        Name.text = data.DisplayName;

        Body.text = data.Description + "\n\n"
                    + "Novice: " + data.MasteryDescriptions[0] + "\n"
                    + "Expert: " + data.MasteryDescriptions[1] + "\n"
                    + "Master: " + data.MasteryDescriptions[2] + "\n";

        School.text = data.School.ToString() + " Magic";
        Cost.text = data.SPCost.ToString();

        Image.sprite = data.Icon;
        Image.SetNativeSize();

        height = (int)Mathf.Max(200, Name.preferredHeight + Body.preferredHeight + 15);

        Background.sizeDelta = new Vector2(width, height);

        float x = Input.mousePosition.x <= Screen.width / 2 ? Background.sizeDelta.x / 2f : -Background.sizeDelta.x / 2f;
        float y = Input.mousePosition.y <= Screen.height / 4 ? Background.sizeDelta.y / 2f : -Background.sizeDelta.y / 2f;

        Background.position = Input.mousePosition + new Vector3(x, y, 0);
    }
}
