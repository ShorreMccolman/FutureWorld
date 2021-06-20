using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popups : MonoBehaviour
{
    [SerializeField] ItemInfoPopup Items;
    [SerializeField] VitalsInfoPopup Vitals;
    [SerializeField] GenericInfoPopup Generic;
    [SerializeField] EnemyInfoPopup Enemies;

    public void ShowText(string title, string body, int size = 20)
    {
        Generic.gameObject.SetActive(true);
        Generic.UpdateUI(title, body, size);
    }

    public void ShowItem(InventoryItem item)
    {
        Items.gameObject.SetActive(true);
        Items.UpdateUI(item);
    }

    public void ShowVitals(CharacterVitalsDisplay display)
    {
        Vitals.gameObject.SetActive(true);
        Vitals.UpdateUI(display);
    }

    public void ShowEnemy(Enemy enemy)
    {
        Enemies.gameObject.SetActive(true);
        Enemies.UpdateUI(enemy);
    }

    public void Close()
    {
        Items.gameObject.SetActive(false);
        Vitals.gameObject.SetActive(false);
        Generic.gameObject.SetActive(false);
        Enemies.gameObject.SetActive(false);
    }
}
