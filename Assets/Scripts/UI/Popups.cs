using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popups : MonoBehaviour
{
    [SerializeField] ItemInfoPopup Items;
    [SerializeField] VitalsInfoPopup Vitals;
    [SerializeField] GenericInfoPopup Generic;
    [SerializeField] EnemyInfoPopup Enemies;
    [SerializeField] ScrollPopup Scrolls;

    bool _supressed;

    public void Supress()
    {
        if (!_supressed)
        {
            Debug.LogError("Supressed");
            PartyController.Instance.OnReleaseClick += Release;
            _supressed = true;
            Close();
        }
    }

    void Release()
    {
        Debug.LogError("Released");
        PartyController.Instance.OnReleaseClick -= Release;
        _supressed = false;
    }

    public void ShowScroll(Scroll scroll)
    {
        if (!Scrolls.gameObject.activeSelf && !_supressed)
        {
            Scrolls.gameObject.SetActive(true);
            Scrolls.UpdateUI(scroll);
            PartyController.Instance.OnReleaseClick += CloseScroll;
        }
    }

    void CloseScroll()
    {
        Scrolls.gameObject.SetActive(false);
        PartyController.Instance.OnReleaseClick -= CloseScroll;
    }

    public void ShowText(string title, string body, int size = 20, TextAnchor anchor = TextAnchor.UpperCenter)
    {
        if (!_supressed)
        {
            Generic.gameObject.SetActive(true);
            Generic.UpdateUI(title, body, size, anchor);
        }
    }

    public void ShowItem(InventoryItem item)
    {
        if (!_supressed)
        {
            Items.gameObject.SetActive(true);
            Items.UpdateUI(item);
        }
    }

    public void ShowVitals(CharacterVitalsDisplay display)
    {
        if (!_supressed)
        {
            Vitals.gameObject.SetActive(true);
            Vitals.UpdateUI(display);
        }
    }

    public void ShowEnemy(Enemy enemy)
    {
        if (!_supressed)
        {
            Enemies.gameObject.SetActive(true);
            Enemies.UpdateUI(enemy);
        }
    }

    public void Close()
    {
        Items.gameObject.SetActive(false);
        Vitals.gameObject.SetActive(false);
        Generic.gameObject.SetActive(false);
        Enemies.gameObject.SetActive(false);
    }
}
