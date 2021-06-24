using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VitalsInfoPopup : MonoBehaviour
{
    [SerializeField] RectTransform Background;
    [SerializeField] Image Image;
    [SerializeField] Text Name;
    [SerializeField] Text Body;
    [SerializeField] Text Spells;

    public void UpdateUI(CharacterVitalsDisplay display)
    {
        int height = 200;
        int width = 500;

        Name.text = display.Member.Profile.FullName;
        Body.text = "Hit Points: " + display.Member.Vitals.CurrentHP + " / " + display.Member.Vitals.Stats.EffectiveTotalHP +
                    "\nSpell Points: " + display.Member.Vitals.CurrentSP + " / " + display.Member.Vitals.Stats.EffectiveTotalSP +
                    "\nCondition: " + display.Member.EffectiveStatusCondition() +
                    "\nQuick Spell: " + display.Member.Profile.QuickSpell;

        Spells.text = "None";

        Image.sprite = display.Sprite;
        Image.SetNativeSize();

        Background.sizeDelta = new Vector2(width, height);

        float x = Input.mousePosition.x <= Screen.width / 2 ? Background.sizeDelta.x / 2f : -Background.sizeDelta.x / 2f;
        float y = Input.mousePosition.y <= Screen.height / 4 ? Background.sizeDelta.y / 2f : -Background.sizeDelta.y / 2f;

        Background.position = Input.mousePosition + new Vector3(x, y, 0);
    }
}
