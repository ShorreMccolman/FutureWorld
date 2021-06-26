using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemButton : ItemButton { 

    ISlotable _menu;

    bool _holdOnClick;

	public void Setup(InventoryItem item, ISlotable menu, bool holdOnClick = true)
    {
        _menu = menu;
        _holdOnClick = holdOnClick;

        if (!holdOnClick)
        {
            if (item.IsBroken)
                Image.color = Color.red;
            else if (!item.IsIdentified)
                Image.color = Color.green;
        }
        else
            Image.color = Color.white;

        Setup(item);
    }

    public void ForceHold()
    {
        Image.raycastTarget = false;
        _holding = true;
    }

    protected override void OnHover()
    {
        _menu.Hover(Item);
    }

    protected override void OnHoverExit()
    {
        _menu.Hover(null);
    }

    protected override void OnLeftDown()
    {
        if (!_holding)
        {
            bool success = _menu.ClickItemButton(this);
            if (success)
            {
                _menu.Hover(null);
                if (_holdOnClick)
                {
                    Image.raycastTarget = false;
                    _holding = true;
                }
            }
        }
    }

    protected override void OnRightDown()
    {
        bool success = HUD.Instance.TryCombine(Item);
        if (success)
        {
            Popups.Supress();
        }
    }

    public static InventoryItemButton Create(Item item, Vector3 position, RectTransform parent)
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/InventoryItemButton"), parent);

        RectTransform rt = (RectTransform)obj.transform;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x * item.Width, rt.sizeDelta.y * item.Height);

        float halfDown = (float)(item.Height / 2f) - 0.5f;
        float halfOver = (float)(item.Width / 2f) - 0.5f;

        obj.transform.position = position;

        InventoryItemButton btn = obj.GetComponent<InventoryItemButton>();
        return btn;
    }
}
