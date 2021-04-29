using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void ItemEvent(InventoryItem Item);
public delegate bool ItemCheckEvent(InventoryItem item);

public class MerchantMenu : ConversationMenu
{
    [SerializeField] Text StoreLabel;

    [SerializeField] Text MerchantLabel;
    [SerializeField] Image MerchantSprite;

    [SerializeField] Text HoverDialog;

    Merchant _currentMerchant;

    List<DialogOptionButton> DialogOptions;

    [SerializeField] StoreFrontUI GeneralStore;
    [SerializeField] StoreFrontUI WeaponStore;
    [SerializeField] StoreFrontUI ArmorStore;
    [SerializeField] StoreFrontUI MagicStore;
    [SerializeField] StoreFrontUI SpellStore;

    [SerializeField] PlayerMechandiseUI PlayerInventoryDisplay;

    ItemEvent OnHover;

    bool menuOpen = false;

    public void Setup(Merchant merchant)
    {
        _currentMerchant = merchant;

        StoreLabel.text = merchant.Data.StoreName;
        MerchantLabel.text = merchant.Data.MerchantName;
        MerchantSprite.sprite = merchant.Data.Sprite;
        HoverDialog.text = "";

        DialogOptions = new List<DialogOptionButton>();

        OnHover = null;
        CloseStores();
        SetupDialog();
    }

    void CloseStores()
    {
        PlayerInventoryDisplay.ClearInventory();

        GeneralStore.gameObject.SetActive(false);
        WeaponStore.gameObject.SetActive(false);
        ArmorStore.gameObject.SetActive(false);
        MagicStore.gameObject.SetActive(false);
        SpellStore.gameObject.SetActive(false);
    }

    void SetupDialog()
    {
        menuOpen = false;

        GameObject obj;
        DialogOptionButton UI;
        if (_currentMerchant.Data.BuyInfo.CountForStoreType(_currentMerchant.Data.StoreType) > 0)
        {
            obj = Instantiate(DialogOptionPrefab, DialogAnchor);

            UI = obj.GetComponent<DialogOptionButton>();
            UI.Setup("Buy", SetupBuy);
            DialogOptions.Add(UI);


            obj = Instantiate(DialogOptionPrefab, DialogAnchor);
            UI = obj.GetComponent<DialogOptionButton>();
            UI.Setup("Sell", SetupSell);
            DialogOptions.Add(UI);
        }

        if (_currentMerchant.Data.CanIdentify)
        {
            obj = Instantiate(DialogOptionPrefab, DialogAnchor);
            UI = obj.GetComponent<DialogOptionButton>();
            UI.Setup("Identify", SetupIdentify);
            DialogOptions.Add(UI);
        }

        if (_currentMerchant.Data.CanRepair)
        {
            obj = Instantiate(DialogOptionPrefab, DialogAnchor);
            UI = obj.GetComponent<DialogOptionButton>();
            UI.Setup("Repair", SetupRepair);
            DialogOptions.Add(UI);
        }

        if (_currentMerchant.Data.SpecialInfo.CountForStoreType(_currentMerchant.Data.StoreType) > 0)
        {
            obj = Instantiate(DialogOptionPrefab, DialogAnchor);

            UI = obj.GetComponent<DialogOptionButton>();
            UI.Setup("Special", SetupSpecial);
            DialogOptions.Add(UI);
        }

        int yOffset = 25 * DialogOptions.Count;
        for (int i = 0; i < DialogOptions.Count; i++)
            DialogOptions[i].transform.position = DialogAnchor.position + Vector3.down * 60 * i + Vector3.up * yOffset;
    }

    public void HoverItem(InventoryItem item)
    {
        OnHover?.Invoke(item);
    }

    void HoverItemBuy(InventoryItem item)
    {
        string text = "";
        if (item != null)
        {
            int value = item.EffectiveValue;
            if (_currentMerchant.Data.StoreType == StoreType.General)
                value *= 2;
            else
                value = Mathf.RoundToInt(value * 1.5f);

            text = "An excellent choice! This " + item.EffectiveName + " is of the finest quality. I am willing to virtually give it away for " + value + " gold.";
        }

        HoverDialog.text = text;
    }

    void HoverItemSell(InventoryItem item)
    {
        string text = "";
        if (item != null)
        {
            bool isValid = _currentMerchant.IsItemValidForStore(item);

            if (item.EffectiveValue == 0)
            {
                text = "I'm a simple " + GameConstants.MerchantTitleForStoreType[_currentMerchant.Data.StoreType] + " and that " + item.Data.DisplayName + " is beyond my meager knowledge.";
            }
            else if (isValid)
            {
                int value = item.EffectiveValue;
                if (_currentMerchant.Data.StoreType == StoreType.General)
                    value = Mathf.RoundToInt(value / 8f);
                else
                    value = Mathf.RoundToInt(value / 3.5f);
                value = Mathf.Max(1, value);

                text = "Hmph. Looks like junk to me. <yawn> I suppose I could give you oh, say, " + value + " gold pieces for it.";
            } else
                text = "Sorry, I am a " + GameConstants.MerchantTitleForStoreType[_currentMerchant.Data.StoreType] + ". I'm not interested in such things.";
        }

        HoverDialog.text = text;
    }

    void HoverItemRepair(InventoryItem item)
    {
        string text = "";
        if (item != null && item.IsBroken)
        {
            bool isValid = _currentMerchant.IsItemValidForStore(item);

            if (isValid)
            {
                int value = item.EffectiveValue;
                value = Mathf.RoundToInt(value * 0.2f);

                text = "This " + item.EffectiveName + " is nearly beyond repair. It will take a superhuman effort to fix it! I'll have to charge " + value + " gold.";
            } else
                text = "Sorry, I can't repair a " + item.EffectiveName + " because I am a " + GameConstants.MerchantTitleForStoreType[_currentMerchant.Data.StoreType] + ". I don't know anything about those.";
        }

        HoverDialog.text = text;
    }

    void HoverItemIdentify(InventoryItem item)
    {
        string text = "";
        if (item != null)
        {
            if (item.IsIdentified)
            {
                text = item.Data.DisplayName;
            }
            else
            {
                bool isValid = _currentMerchant.IsItemValidForStore(item);

                if (isValid) {
                    int value = item.EffectiveValue;
                    value = Mathf.RoundToInt(value * 0.2f);

                    text = "I'll tell you what it is for " + value + " gold pieces.";
                } else
                    text = "Sorry, I can't identify a " + item.EffectiveName + " because I am a " + GameConstants.MerchantTitleForStoreType[_currentMerchant.Data.StoreType] + ". I don't know anything about those.";
            }
        }

        HoverDialog.text = text;
    }

    public void SetupBuy()
    {
        foreach (var option in DialogOptions)
            Destroy(option.gameObject);
        DialogOptions.Clear();

        HUD.Instance.SendInfoMessage("Select the Item to Buy", 0, true);

        OnHover = HoverItemBuy;

        menuOpen = true;

        switch (_currentMerchant.Data.StoreType)
        {
            case StoreType.General:
                GeneralStore.gameObject.SetActive(true);
                GeneralStore.Setup(this, _currentMerchant.BuyItems);
                break;
            case StoreType.Weapon:
                WeaponStore.gameObject.SetActive(true);
                WeaponStore.Setup(this, _currentMerchant.BuyItems);
                break;
            case StoreType.Armor:
                ArmorStore.gameObject.SetActive(true);
                ArmorStore.Setup(this, _currentMerchant.BuyItems);
                break;
            case StoreType.Magic:
                MagicStore.gameObject.SetActive(true);
                MagicStore.Setup(this, _currentMerchant.BuyItems);
                break;
            case StoreType.Spell:
                SpellStore.gameObject.SetActive(true);
                SpellStore.Setup(this, _currentMerchant.BuyItems);
                break;
        }
    }

    public void SetupSell()
    {
        foreach (var option in DialogOptions)
            Destroy(option.gameObject);
        DialogOptions.Clear();

        HUD.Instance.SendInfoMessage("Select the Item to Sell", 0, true);

        OnHover = HoverItemSell;

        menuOpen = true;

        HUD.Instance.SelectNewMember += OnMemberChanged;

        PlayerInventoryDisplay.Init(this, BarterSell, true);
        PlayerInventoryDisplay.DisplayInventory(HUD.Instance.SelectedMember.Inventory);
    }

    public void SetupIdentify()
    {
        foreach (var option in DialogOptions)
            Destroy(option.gameObject);
        DialogOptions.Clear();

        HUD.Instance.SendInfoMessage("Select the Item to Identify", 0, true);

        OnHover = HoverItemIdentify;

        menuOpen = true;

        HUD.Instance.SelectNewMember += OnMemberChanged;

        PlayerInventoryDisplay.Init(this, BarterIdentify);
        PlayerInventoryDisplay.DisplayInventory(HUD.Instance.SelectedMember.Inventory);
    }

    public void SetupRepair()
    {
        foreach (var option in DialogOptions)
            Destroy(option.gameObject);
        DialogOptions.Clear();

        HUD.Instance.SendInfoMessage("Select the Item to Repair", 0, true);

        OnHover = HoverItemRepair;

        menuOpen = true;

        HUD.Instance.SelectNewMember += OnMemberChanged;

        PlayerInventoryDisplay.Init(this, BarterRepair);
        PlayerInventoryDisplay.DisplayInventory(HUD.Instance.SelectedMember.Inventory);
    }

    public void SetupSpecial()
    {
        foreach (var option in DialogOptions)
            Destroy(option.gameObject);
        DialogOptions.Clear();

        HUD.Instance.SendInfoMessage("Select the Item to Buy", 0, true);

        OnHover = HoverItemBuy;

        menuOpen = true;

        switch (_currentMerchant.Data.StoreType)
        {
            case StoreType.General:
                GeneralStore.gameObject.SetActive(true);
                GeneralStore.Setup(this, _currentMerchant.SpecialItems);
                break;
            case StoreType.Weapon:
                WeaponStore.gameObject.SetActive(true);
                WeaponStore.Setup(this, _currentMerchant.SpecialItems);
                break;
            case StoreType.Armor:
                ArmorStore.gameObject.SetActive(true);
                ArmorStore.Setup(this, _currentMerchant.SpecialItems);
                break;
            case StoreType.Magic:
                MagicStore.gameObject.SetActive(true);
                MagicStore.Setup(this, _currentMerchant.SpecialItems);
                break;
            case StoreType.Spell:
                SpellStore.gameObject.SetActive(true);
                SpellStore.Setup(this, _currentMerchant.SpecialItems);
                break;
        }
    }

    public bool Barter(InventoryItem item)
    {
        int value = item.EffectiveValue;
        if (_currentMerchant.Data.StoreType == StoreType.General)
            value *= 2;
        else
            value = Mathf.RoundToInt(value * 1.5f);

        bool success = HUD.Instance.PurchaseItem(item, value);
        if(success)
        {
            HUD.Instance.ExpressSelectedMember(GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);
            _currentMerchant.ConfirmPurchase(item);
        }
        else
        {
            HUD.Instance.ExpressSelectedMember(GameConstants.EXPRESSION_SAD, GameConstants.EXPRESSION_SAD_DURATION);
        }
        return success;
    }

    public bool BarterSell(InventoryItem item)
    {
        if (!_currentMerchant.IsItemValidForStore(item))
            return false;

        int value = item.EffectiveValue;
        if (value == 0)
            return false;

        if (_currentMerchant.Data.StoreType == StoreType.General)
            value = Mathf.RoundToInt(value / 8f);
        else
            value = Mathf.RoundToInt(value / 3.5f);

        bool success = HUD.Instance.SellItem(item, value);
        return success;
    }

    public bool BarterIdentify(InventoryItem item)
    {
        if (!_currentMerchant.IsItemValidForStore(item))
            return false;

        int value = item.EffectiveValue;
        if (value == 0)
            return false;

        value = Mathf.RoundToInt(value * 0.2f);

        bool success = HUD.Instance.IdentifyItem(item, value);
        return success;
    }

    public bool BarterRepair(InventoryItem item)
    {
        if (!_currentMerchant.IsItemValidForStore(item))
            return false;

        int value = item.EffectiveValue;
        if (value == 0)
            return false;

        value = Mathf.RoundToInt(value * 0.2f);

        bool success = HUD.Instance.RepairItem(item, value);
        return success;
    }

    void OnMemberChanged(PartyMember member)
    {
        PlayerInventoryDisplay.DisplayInventory(member.Inventory);
    }

    public void Back()
    {
        foreach (var option in DialogOptions)
            Destroy(option.gameObject);

        if (menuOpen)
        {
            HUD.Instance.ReleaseInfoLock();

            PlayerInventoryDisplay.gameObject.SetActive(false);

            HoverDialog.text = "";
            OnHover = null;

            CloseStores();
            SetupDialog();
        }
        else 
            HUD.Instance.CloseAll();

        HUD.Instance.SelectNewMember -= OnMemberChanged;
    }

    public override void OnClose()
    {
        foreach (var option in DialogOptions)
            Destroy(option.gameObject);

        if (menuOpen)
        {
            HUD.Instance.ReleaseInfoLock();

            PlayerInventoryDisplay.gameObject.SetActive(false);

            HoverDialog.text = "";
            OnHover = null;

            CloseStores();
            SetupDialog();
        }

        HUD.Instance.SelectNewMember -= OnMemberChanged;
    }
}
