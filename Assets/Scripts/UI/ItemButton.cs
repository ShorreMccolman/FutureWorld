using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Image Image;

    public InventoryItem Item { get; private set; }

    protected bool _holding = false;

    protected virtual void OnLeftDown() { }
    protected virtual void OnLeftUp() { }
    protected virtual void OnRightUp() { }
    protected virtual void OnRightDown() { }
    protected virtual void OnHover() { }
    protected virtual void OnHoverExit() { }

    protected void Setup(InventoryItem item, bool useEquip = false)
    {
        Item = item;
        Image.sprite = item.Data.equipSprite != null && useEquip ? item.Data.equipSprite : item.Data.sprite;
        Image.SetNativeSize();
        if(useEquip)
            Image.rectTransform.localPosition += Item.Data.EquipOffset;
    }

    void Update()
    {
        if (_holding)
        {
            if (Item.IsBroken)
                Image.color = Color.red;
            else if (!Item.IsIdentified)
                Image.color = Color.green;

            RectTransform rectT = (RectTransform)transform;

            transform.position = Input.mousePosition + Vector3.down * rectT.rect.height * 0.4f + Vector3.right * rectT.rect.width * 0.4f;
        }
    }

    public void Reset(InventoryItem item)
    {
        Item = item;
        Reset();
    }

    public void Reset()
    {
        Image.color = Color.white;
        _holding = false;
        Image.raycastTarget = true;
    }

    public void UpdateColor()
    {
        if (Item.IsBroken)
            Image.color = Color.red;
        else if (!Item.IsIdentified)
            Image.color = Color.green;
        else
            Image.color = Color.white;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftDown();
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {

        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            bool success = HUD.Instance.TryConsume(Item);
            if (!success)
            {
                HUD.Instance.UpdatePopup(Item, true);
            }
            OnRightDown();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HUD.Instance.UpdatePopup(Item, Input.GetMouseButton(1));
        OnHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HUD.Instance.HidePopup();
        OnHoverExit();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftUp();
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {

        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            HUD.Instance.HidePopup();
            OnRightUp();
        }
    }
}
