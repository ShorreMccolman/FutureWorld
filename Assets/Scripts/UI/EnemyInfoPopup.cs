using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoPopup : MonoBehaviour
{
    [SerializeField] RectTransform Background;
    [SerializeField] Text Title;
    [SerializeField] Slider Slider;
    [SerializeField] Image Fill;
    [SerializeField] Image Portrait;

    public void UpdateUI(Enemy enemy)
    {
        float height = 150f;
        float width = 450f;

        Title.text = enemy.NPC != null ? enemy.NPC.DisplayName : enemy.Data.DisplayName;

        float health = (float)enemy.CurrentHP / (float)enemy.Data.HitPoints;
        ((RectTransform)Slider.transform).sizeDelta = new Vector2(50 + enemy.Data.HitPoints * 3, 20);
        Slider.value = health;
        if (health > 0.5f)
            Fill.color = Color.green;
        else if (health > 0.25f)
            Fill.color = Color.yellow;
        else
            Fill.color = Color.red;

        Portrait.sprite = enemy.GetPortrait();

        Background.sizeDelta = new Vector2(width, height);

        float x = Input.mousePosition.x <= Screen.width / 2 ? Background.sizeDelta.x / 2f : -Background.sizeDelta.x / 2f;
        float y = Input.mousePosition.y <= Screen.height / 4 ? Background.sizeDelta.y / 2f : -Background.sizeDelta.y / 2f;

        Background.position = Input.mousePosition + new Vector3(x, y, 0);
    }
}
