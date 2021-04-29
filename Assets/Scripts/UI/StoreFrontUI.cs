using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreFrontUI : MonoBehaviour
{
    [SerializeField] List<ProductSlot> _slots;
    public List<ProductSlot> GetSlots()
    {
        return _slots;
    }

    MerchantMenu _menu;
    public MerchantMenu GetMenu()
    {
        return _menu;
    }

    [SerializeField] GameObject ItemButtonPrefab;

    List<GameObject> _buttons = new List<GameObject>();

    public void Setup(MerchantMenu menu, InventoryItem[] items)
    {
        _menu = menu;

        foreach (var button in _buttons)
        {
            Destroy(button);
        }

        _buttons = new List<GameObject>();
        for (int i = 0; i < _slots.Count; i++)
        {
            InventoryItem item = items[i];
            if (item != null)
                CreateButton(_slots[i], item);
        }
    }

    void CreateButton(ProductSlot slot, InventoryItem item)
    {
        GameObject obj = Instantiate(ItemButtonPrefab, slot.transform);
        RectTransform rt = (RectTransform)obj.transform;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x * item.Data.Width, rt.sizeDelta.y * item.Data.Height);

        obj.transform.position = slot.transform.position;

        _buttons.Add(obj);

        ProductItemButton btn = obj.GetComponent<ProductItemButton>();
        btn.Setup(item, this, slot.AlignBottom, slot.AlignTop, slot.RandomY);
    }
}
