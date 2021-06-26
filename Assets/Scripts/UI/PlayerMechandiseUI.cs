using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerMerchandiseInteraction
{
    Sell,
    Repair,
    Identify
}

public class PlayerMechandiseUI : MonoBehaviour, ISlotable
{
    [SerializeField] int Width;
    [SerializeField] int Height;

    [SerializeField] GameObject ButtonPrefab;
    [SerializeField] RectTransform Anchor;

    List<InventoryGridButton> _gridButtons;
    List<ItemButton> _itemButtons = new List<ItemButton>();

    Inventory _inventory;
    MerchantMenu _menu;

    ItemCheckEvent OnClick;
    bool _destroyOnSuccess;

    public void Init(MerchantMenu menu, ItemCheckEvent onClick, bool destroyOnSuccess = false)
    {
        _menu = menu;
        gameObject.SetActive(true);

        OnClick = onClick;

        _itemButtons = new List<ItemButton>();
        _destroyOnSuccess = destroyOnSuccess;

        _gridButtons = new List<InventoryGridButton>();
        for (int j = 0; j < Height; j++)
        {
            for (int i = 0; i < Width; i++)
            {
                GameObject obj = Instantiate<GameObject>(ButtonPrefab, Anchor);
                obj.transform.position = Anchor.position + Vector3.up * (Height - 1 - j) * Anchor.rect.height + Vector3.right * i * Anchor.rect.width;

                obj.transform.position += new Vector3(-Anchor.rect.width * Width / 2, -Anchor.rect.height * Height / 2, 0);

                InventoryGridButton button = obj.GetComponent<InventoryGridButton>();
                button.Setup(i + j * Width, this);
                _gridButtons.Add(button);
            }
        }
    }

    public void ClearInventory()
    {
        foreach (var obj in _itemButtons)
            Destroy(obj.gameObject);
        _itemButtons.Clear();
    }

    public void DisplayInventory(Inventory inventory)
    {
        _inventory = inventory;

        ClearInventory();

        List<InventoryItem> items = _inventory.Items;
        for (int i = 0; i < items.Count; i++)
        {
            InventoryItem item = items[i];

            float halfDown = (float)(item.Data.Height / 2f) - 0.5f;
            float halfOver = (float)(item.Data.Width / 2f) - 0.5f;

            Vector3 pos = _gridButtons[item.Slot].transform.position + Vector3.down * halfDown * Anchor.rect.height + Vector3.right * halfOver * Anchor.rect.width;
            InventoryItemButton button = InventoryItemButton.Create(item.Data, pos, Anchor);
            button.Setup(item, this, false);
            _itemButtons.Add(button);
        }
    }

    public bool ClickItemButton(InventoryItemButton button)
    {
        bool success = OnClick.Invoke(button.Item);

        if (success)
        {
            if (_destroyOnSuccess)
            {
                _itemButtons.Remove(button);
                Destroy(button.gameObject);
            }
            else
            {
                button.UpdateColor();
            }
        }

        return success;
    }

    public void Select(int slot) { }

    public void Hover(InventoryItem item)
    {
        _menu.HoverItem(item);
    }
}
