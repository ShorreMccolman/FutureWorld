using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InventoryMenu : CharacterMenu, ISlotable {

    [SerializeField] int Width;
    [SerializeField] int Height;

    [SerializeField] GameObject ButtonPrefab;
    [SerializeField] RectTransform Anchor;

    List<InventoryGridButton> _gridButtons;
    List<GameObject> _itemButtons = new List<GameObject>();

    Inventory _inventory;

    protected override void Init()
    {
        Init(Width, Height);
    }

    public void Init(int width, int height)
    {
        _gridButtons = new List<InventoryGridButton>();
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                GameObject obj = Instantiate<GameObject>(ButtonPrefab, Anchor);
                obj.transform.position = Anchor.position + Vector3.up * (height - 1 - j) * Anchor.rect.height + Vector3.right * i * Anchor.rect.width;

                obj.transform.position += new Vector3(-Anchor.rect.width * width / 2, -Anchor.rect.height * height / 2, 0);

                InventoryGridButton button = obj.GetComponent<InventoryGridButton>();
                button.Setup(i + j * width, this);
                _gridButtons.Add(button);
            }
        }
    }

    public void Select(int slot)
    {
        ItemButton button = HUD.Instance.HeldItemButton;
        if (button == null)
            return;

        InventoryItem success = _inventory.AddItem(button.Item, slot);
        if (success == null)
        {
            success = _inventory.AddItem(button.Item);
        }

        if (success != null && success.Slot >= 0)
        {
            PlaceButtonAtSlot(button, success);
            HUD.Instance.HoldItem(null);
        }
    }

    public void PlaceButtonAtSlot(ItemButton button, InventoryItem item)
    {
        Vector3 pos = _gridButtons[item.Slot].transform.position;

        float halfDown = (float)(button.Item.Data.Height / 2f) - 0.5f;
        float halfOver = (float)(button.Item.Data.Width / 2f) - 0.5f;

        button.gameObject.transform.position = pos + Vector3.down * halfDown * Anchor.rect.height + Vector3.right * halfOver * Anchor.rect.width;
        _itemButtons.Add(button.gameObject);
        button.transform.parent = Anchor;
        (button as InventoryItemButton).Setup(item, this);
        button.Reset();
    }

    public void Setup(Inventory inventory)
    {
        _inventory = inventory;

        SetupInventory();
    }

    public override void Setup(PartyMember member)
    {
        _inventory = member.Inventory;

        SetupInventory();
    }

    void SetupInventory()
    {
        foreach (var obj in _itemButtons)
            Destroy(obj);
        _itemButtons.Clear();

        List<InventoryItem> items = _inventory.Items;
        for (int i = 0; i < items.Count; i++)
        {
            InventoryItem item = items[i];

            float halfDown = (float)(item.Data.Height / 2f) - 0.5f;
            float halfOver = (float)(item.Data.Width / 2f) - 0.5f;

            Vector3 pos = _gridButtons[item.Slot].transform.position + Vector3.down * halfDown * Anchor.rect.height + Vector3.right * halfOver * Anchor.rect.width;
            InventoryItemButton button = HUD.Instance.CreateItemButton(item.Data, pos, Anchor);
            button.Setup(item, this);

            _itemButtons.Add(button.gameObject);
        }
    }

    public bool ClickItemButton(InventoryItemButton button)
    {
        bool success = false;
        if (HUD.Instance.HeldItemButton != null)
        {
            success = _inventory.TradeItem(HUD.Instance.HeldItemButton.Item, button.Item);
            if(success)
            {
                PlaceButtonAtSlot(HUD.Instance.HeldItemButton, HUD.Instance.HeldItemButton.Item);
                _itemButtons.Remove(button.gameObject);
                HUD.Instance.HoldItem(button);
            }
            return success;
        }

        success = _inventory.RemoveItem(button.Item);
        if (success)
        {
            _itemButtons.Remove(button.gameObject);
            HUD.Instance.HoldItem(button);
        }
        return success;
    }

    public void Hover(InventoryItem item)
    {

    }
}
