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
    [SerializeField] StoreFrontUI GeneralStore;
    [SerializeField] StoreFrontUI WeaponStore;
    [SerializeField] StoreFrontUI ArmorStore;
    [SerializeField] StoreFrontUI MagicStore;
    [SerializeField] StoreFrontUI SpellStore;
    [SerializeField] PlayerMechandiseUI PlayerInventoryDisplay;

    Merchant _currentMerchant;
    bool _inventoryMenuOpen;

    ItemEvent OnHover;

    public void Setup(Merchant merchant)
    {
        _currentMerchant = merchant;

        StoreLabel.text = merchant.Data.StoreName;
        MerchantLabel.text = merchant.DisplayName;
        MerchantSprite.sprite = merchant.Data.Sprite;
        HoverDialog.text = "";

        DialogOptions = new List<OptionButton>();

        Party.OnMemberChanged += OnMemberChanged;

        OnHover = null;
        SoundManager.Instance.SetMusicVolume(0.25f);
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

    void OnMemberChanged(PartyMember member)
    {
        if (PlayerInventoryDisplay.gameObject.activeSelf)
            PlayerInventoryDisplay.DisplayInventory(member.Inventory);
        else if(!_inventoryMenuOpen)
            SetupDialog();
    }

    void SetupDialog()
    {
        foreach (var option in DialogOptions)
            Destroy(option.gameObject);
        DialogOptions.Clear();

        _inventoryMenuOpen = false;

        _currentMerchant.TryUpdateStock(Party.Instance.CurrentTime);

        if (!Party.Instance.QuestLog.IsMemberOfGuild(_currentMerchant.Data.GuildID))
        {
            AddButton("You must be a member of this guild to study here.");
        }
        else if (!Party.Instance.ActiveMember.IsConcious())
        {
            AddButton(Party.Instance.ActiveMember.Profile.CharacterName + " is in no condition to do anything");
        } 
        else
        {
            if (_currentMerchant.Data.BuyInfo.CountForStoreType(_currentMerchant.Data.StoreType) > 0)
            {
                string buyLabel = "Buy";

                List<Skill> availableSkills = new List<Skill>();
                if (_currentMerchant.Data.StoreType == StoreType.Spell)
                {
                    List<string> schools = new List<string>();
                    foreach (var school in _currentMerchant.Data.BuyInfo.MagicTypes)
                    {
                        schools.Add(school.ToString());
                    }
                    availableSkills = Party.Instance.ActiveMember.DetermineLearnableSkills(schools);
                    buyLabel = "Buy Spells";
                }

                if(availableSkills.Count > 0)
                {
                    AddButton("Skill Cost: " + _currentMerchant.Data.SkillCost);
                }

                AddButton(buyLabel, SetupBuy);

                if(availableSkills.Count > 0)
                {
                    for(int i=0;i<availableSkills.Count;i++)
                    {
                        AddButton(availableSkills[i].DisplayName, i, LearnSkill);
                    }
                } 
                else if (_currentMerchant.Data.StoreType != StoreType.Spell)
                {
                    AddButton("Sell", SetupSell);
                }
            }

            if (_currentMerchant.Data.CanIdentify)
            {
                AddButton("Identify", SetupIdentify);
            }

            if (_currentMerchant.Data.CanRepair)
            {
                AddButton("Repair", SetupRepair);
            }

            if (_currentMerchant.Data.SpecialInfo.CountForStoreType(_currentMerchant.Data.StoreType) > 0)
            {
                AddButton("Special", SetupSpecial);
            }
        }

        StaggerOptions();
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

    public void LearnSkill(int index)
    {
        List<string> schools = new List<string>();
        foreach (var school in _currentMerchant.Data.BuyInfo.MagicTypes)
        {
            schools.Add(school.ToString());
        }
        List<Skill> available = Party.Instance.ActiveMember.DetermineLearnableSkills(schools);

        Skill skill = available[index];

        if(Party.Instance.TryPay(_currentMerchant.Data.SkillCost))
        {
            Party.Instance.ActiveMember.Skillset.LearnSkill(skill);
            Party.Instance.ActiveMember.Vitals.Express(GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);
            SetupDialog();
        }
    }

    public void SetupBuy()
    {
        foreach (var option in DialogOptions)
            Destroy(option.gameObject);
        DialogOptions.Clear();

        InfoMessageReceiver.Send("Select the Item to Buy", 0, true);

        OnHover = HoverItemBuy;

        _inventoryMenuOpen = true;

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

        InfoMessageReceiver.Send("Select the Item to Sell", 0, true);

        OnHover = HoverItemSell;

        _inventoryMenuOpen = true;

        PlayerInventoryDisplay.Init(this, BarterSell, true);
        PlayerInventoryDisplay.DisplayInventory(Party.Instance.ActiveMember.Inventory);
    }

    public void SetupIdentify()
    {
        foreach (var option in DialogOptions)
            Destroy(option.gameObject);
        DialogOptions.Clear();

        InfoMessageReceiver.Send("Select the Item to Identify", 0, true);

        OnHover = HoverItemIdentify;

        _inventoryMenuOpen = true;

        PlayerInventoryDisplay.Init(this, BarterIdentify);
        PlayerInventoryDisplay.DisplayInventory(Party.Instance.ActiveMember.Inventory);
    }

    public void SetupRepair()
    {
        foreach (var option in DialogOptions)
            Destroy(option.gameObject);
        DialogOptions.Clear();

        InfoMessageReceiver.Send("Select the Item to Repair", 0, true);

        OnHover = HoverItemRepair;

        _inventoryMenuOpen = true;

        PlayerInventoryDisplay.Init(this, BarterRepair);
        PlayerInventoryDisplay.DisplayInventory(Party.Instance.ActiveMember.Inventory);
    }

    public void SetupSpecial()
    {
        foreach (var option in DialogOptions)
            Destroy(option.gameObject);
        DialogOptions.Clear();

        InfoMessageReceiver.Send("Select the Item to Buy", 0, true);

        OnHover = HoverItemBuy;

        _inventoryMenuOpen = true;

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
            _currentMerchant.ConfirmPurchase(item);
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

    public void Back()
    {
        foreach (var option in DialogOptions)
            Destroy(option.gameObject);
        DialogOptions.Clear();

        if (_inventoryMenuOpen)
        {
            InfoMessageReceiver.ReleaseLock();

            PlayerInventoryDisplay.gameObject.SetActive(false);

            HoverDialog.text = "";
            OnHover = null;

            CloseStores();
            SetupDialog();
        }
        else
            CloseMenu();
    }

    public override void OnClose()
    {
        base.OnClose();

        foreach (var option in DialogOptions)
            Destroy(option.gameObject);
        DialogOptions.Clear();

        if (_inventoryMenuOpen)
        {
            InfoMessageReceiver.ReleaseLock();

            PlayerInventoryDisplay.gameObject.SetActive(false);

            HoverDialog.text = "";
            OnHover = null;

            CloseStores();
            _inventoryMenuOpen = false;
        }

        Party.OnMemberChanged -= OnMemberChanged;
        SoundManager.Instance.SetMusicVolume(1f);
        SoundManager.Instance.PlayUISound("Close");
    }
}
