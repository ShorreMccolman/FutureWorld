﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductItemButton : ItemButton
{
    StoreFrontUI _ui;

    public void Setup(InventoryItem item, StoreFrontUI ui, bool alignBottom, bool alignTop, bool randomY)
    {
        _ui = ui;
        Setup(item);

        if(alignBottom)
            Image.rectTransform.position += Vector3.up * Image.rectTransform.sizeDelta.y / 4;
        else if(alignTop)
            Image.rectTransform.position += Vector3.down * Image.rectTransform.sizeDelta.y / 4;
        else if(randomY)
        {
            float range = 15 * (10f - item.Data.Height);
            float rand = Random.Range(-range, range);

            Image.rectTransform.position += Vector3.up * rand;
        }
    }

    protected override void OnHover()
    {
        _ui.GetMenu().HoverItem(Item);
    }

    protected override void OnHoverExit()
    {
        _ui.GetMenu().HoverItem(null);
    }

    protected override void OnLeftDown()
    {
        if (!_holding)
        {
            bool success = _ui.GetMenu().Barter(Item);
            if (success)
            {
                _ui.GetMenu().HoverItem(null);
                Destroy(this.gameObject);
            }
        }
    }
}
