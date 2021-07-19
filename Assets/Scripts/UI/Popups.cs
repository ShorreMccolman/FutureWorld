using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popups : MonoBehaviour
{
    static Popups Instance;
    private void Awake() { Instance = this; }

    [SerializeField] ItemInfoPopup Items;
    [SerializeField] VitalsInfoPopup Vitals;
    [SerializeField] GenericInfoPopup Generic;
    [SerializeField] EnemyInfoPopup Enemies;
    [SerializeField] ScrollPopup Scrolls;
    [SerializeField] SpellInfoPopup Spells;

    bool _supressed;

    public static void Supress()
    {
        if (!Instance._supressed)
        {
            PartyController.Instance.OnReleaseClick += Instance.Release;
            Instance._supressed = true;
            Close();
        }
    }

    void Release()
    {
        PartyController.Instance.OnReleaseClick -= Release;
        _supressed = false;
    }

    public static void ShowScroll(Scroll scroll)
    {
        if (!Instance.Scrolls.gameObject.activeSelf && !Instance._supressed)
        {
            Instance.Scrolls.gameObject.SetActive(true);
            Instance.Scrolls.UpdateUI(scroll);
            PartyController.Instance.OnReleaseClick += Instance.CloseScroll;
        }
    }

    void CloseScroll()
    {
        Scrolls.gameObject.SetActive(false);
        PartyController.Instance.OnReleaseClick -= CloseScroll;
    }

    public static void ShowText(string title, string body, int size = 20, TextAnchor anchor = TextAnchor.UpperCenter)
    {
        if (!Instance._supressed)
        {
            Instance.Generic.gameObject.SetActive(true);
            Instance.Generic.UpdateUI(title, body, size, anchor);
        }
    }

    public static void ShowSpell(SpellData spell)
    {
        if (!Instance._supressed)
        {
            Instance.Spells.gameObject.SetActive(true);
            Instance.Spells.UpdateUI(spell);
        }
    }

    public static void ShowItem(InventoryItem item)
    {
        if (!Instance._supressed)
        {
            Instance.Items.gameObject.SetActive(true);
            Instance.Items.UpdateUI(item);
        }
    }

    public static void ShowVitals(CharacterVitalsDisplay display)
    {
        if (!Instance._supressed)
        {
            Instance.Vitals.gameObject.SetActive(true);
            Instance.Vitals.UpdateUI(display);
        }
    }

    public static void ShowEnemy(Enemy enemy)
    {
        if (!Instance._supressed)
        {
            Instance.Enemies.gameObject.SetActive(true);
            Instance.Enemies.UpdateUI(enemy);
        }
    }

    public static void Close()
    {
        Instance.Items.gameObject.SetActive(false);
        Instance.Vitals.gameObject.SetActive(false);
        Instance.Generic.gameObject.SetActive(false);
        Instance.Enemies.gameObject.SetActive(false);
        Instance.Scrolls.gameObject.SetActive(false);
        Instance.Spells.gameObject.SetActive(false);
    }
}
