using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum CharacterMenuSection
{
    Profile = 0,
    Skills = 1,
    Inventory = 2,
    Awards = 3
}

public delegate void SelectedMemberChangeEvent(PartyMember member);

public class HUD : Menu {
    public static HUD Instance;
    void Awake() { Instance = this; }

    [SerializeField] GameObject DebugMenu;
    [SerializeField] GameObject SideMenu;

    [SerializeField] Image LoadingScreen;

    [SerializeField] Transform Compass;

    [SerializeField] CharacterVitalsDisplay[] CharacterVitalsUI;
    [SerializeField] GameObject[] CharacterMenuObjects;

    [SerializeField] Text InfoMessageLabel;

    [SerializeField] Text FoodLabel;
    [SerializeField] Text GoldLabel;

    [SerializeField] Image Vignette;
    [SerializeField] Image CombatIndicator;

    [SerializeField] Image CharacterPortrait;
    [SerializeField] Image CharacterPortraitArm;
    [SerializeField] Image CharacterPortraitTwoArm;

    [SerializeField] ProfileMenu ProfileMenu;
    [SerializeField] SkillsMenu SkillsMenu;
    [SerializeField] InventoryMenu InventoryMenu;
    [SerializeField] AwardsMenu AwardsMenu;
    [SerializeField] InventoryMenu ChestMenu;
    [SerializeField] ResidenceMenu ResidenceMenu;
    [SerializeField] MerchantMenu MerchantMenu;
    [SerializeField] NPCMenu NPCMenu;
    [SerializeField] SpellsMenu SpellsMenu;
    [SerializeField] CharacterEquipmentDisplay EquipmentDisplay;

    [SerializeField] ItemInfoPopup ItemInfoPopup;
    [SerializeField] ScrollPopup ScrollInfoPopup;

    [SerializeField] Transform HeldItemAnchor;

    public SelectedMemberChangeEvent SelectNewMember;

    public bool CharacterMenuOpen { get; private set; }
    public bool OtherMenuOpen { get; private set; }

    CharacterMenu _selectedMenu;
    public PartyMember SelectedMember {
        get {
            return _party.ActiveMember;
        }
    }
    public PartyMember ForceSelectedMember {
                get {

            if (_party.ActiveMember == null)
                return _party.Members[0];
            return _party.ActiveMember;
        }
   }

    Party _party;
    CharacterMenuSection _section;

    Dictionary<PartyMember, CharacterVitalsDisplay> partyMembersDisplayDict;

    bool _showingTempMessage;
    string _currentInfoMessage;
    bool _infoMessageLock;

    public ItemButton HeldItemButton { get; private set; }

    void Update()
    {
        if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(0))
            HidePopup();

        UpdateCompass();

        if(Input.GetKeyDown(KeyCode.F1))
        {
            DebugMenu.SetActive(!DebugMenu.activeSelf);
        }
    }

    public void InitParty(Party party)
    {
        _party = party;

        foreach (var member in party.Members)
        {
            if (member.Vitals.IsReady())
            {
                _party.SetActiveMember(member);
                break;
            }
        }

        partyMembersDisplayDict = new Dictionary<PartyMember, CharacterVitalsDisplay>();
        for (int i = 0; i < CharacterVitalsUI.Length; i++)
        {
            CharacterVitalsUI[i].Init(party.Members[i]);
            partyMembersDisplayDict.Add(party.Members[i], CharacterVitalsUI[i]);
        }
        foreach (var cvd in CharacterVitalsUI)
            cvd.IndicateSelection(cvd.Member == SelectedMember);

        _currentInfoMessage = "";
        ResetInfoMessage();

        Vignette.enabled = false;

        foreach (var obj in CharacterMenuObjects)
            obj.SetActive(false);
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        foreach (var vitals in CharacterVitalsUI)
        {
            vitals.UpdateUI();
            vitals.IndicateSelection(vitals.Member == _party.ActiveMember);
        }

        FoodLabel.text = _party.CurrentFood.ToString();
        GoldLabel.text = _party.CurrentGold.ToString();

        CombatIndicator.gameObject.SetActive(TimeManagement.IsCombatMode);
    }

    public void System()
    {
        if (HeldItemButton != null)
            return;

        if(_selectedMenu != null)
            MenuManager.Instance.CloseMenu(_selectedMenu.MenuTag);

        MenuManager.Instance.OpenMenu("System");
        foreach (var obj in CharacterMenuObjects)
            obj.SetActive(false);
    }

    public void OpenQuests()
    {
        if (OtherMenuOpen || CharacterMenuOpen)
            return;

        MenuManager.Instance.OpenMenu("Quests");
    }

    public void OpenNotes()
    {

    }

    public void OpenMaps()
    {

    }

    public void OpenCalendar()
    {

    }

    public void OpenSpells()
    {
        if (OtherMenuOpen || CharacterMenuOpen)
            return;

        if (SelectedMember != null)
        {
            MenuManager.Instance.OpenMenu("Spells");
            SpellsMenu.Setup(SelectedMember);
        }
    }

    public void EnableSideMenu()
    {
        SideMenu.SetActive(true);
        OtherMenuOpen = false;
    }

    public void OpenRest()
    {
        if (OtherMenuOpen || CharacterMenuOpen)
            return;

        SideMenu.SetActive(false);
        MenuManager.Instance.OpenMenu("Rest");
    }

    public void Profile()
    {
        SwapMenu(ProfileMenu);
    }

    public void Skills()
    {
        SwapMenu(SkillsMenu);
    }

    public void Inventory()
    {
        SwapMenu(InventoryMenu);
    }

    public void Awards()
    {
        SwapMenu(AwardsMenu);
    }

    void SwapMenu(CharacterMenu menu)
    {
        if (_selectedMenu != null)
            MenuManager.Instance.CloseMenu(_selectedMenu.MenuTag);

        Vignette.enabled = true;
        _selectedMenu = menu;
        _selectedMenu.Setup(SelectedMember);
        MenuManager.Instance.OpenMenu(_selectedMenu.MenuTag);
    }

    public void Close()
    {
        if (_selectedMenu != null)
            MenuManager.Instance.CloseMenu(_selectedMenu.MenuTag);
        foreach (var obj in CharacterMenuObjects)
            obj.SetActive(false);
        CharacterMenuOpen = false;
        Vignette.enabled = false;
    }

    public void CloseAll()
    {
        MenuManager.Instance.CloseAllMenus();
        foreach (var obj in CharacterMenuObjects)
            obj.SetActive(false);
        CharacterMenuOpen = false;
        Vignette.enabled = false;
        OtherMenuOpen = false;
        PartyController.Instance.SetControlState(ControlState.Previous);
    }

    public void ReadyEvent(PartyMember member)
    {
        if(SelectedMember == null)
        {
            SelectCharacter(member, false);
        }
        else if (SelectedMember == member)
        {
            foreach (var cvd in CharacterVitalsUI)
                cvd.IndicateSelection(cvd.Member == member);
        }

        partyMembersDisplayDict[member].UpdateUI();
    }

    public void ToggleSelectedCharacter()
    {
        if (SelectedMember == null)
            return;

        int member = -1;
        for (int i=0;i<4;i++)
        {
            if(_party.Members[i].Equals(SelectedMember))
            {
                member = i;
            }
        }

        int index = member;
        for(int i=1;i<4;i++)
        {
            int cur = (index + i) % 4;
            if(_party.Members[cur].Vitals.IsReady())
            {
                _party.SetActiveMember(_party.Members[cur]);
                foreach (var cvd in CharacterVitalsUI)
                    cvd.IndicateSelection(cvd.Member == SelectedMember);

                if(CharacterMenuOpen)
                {
                    _selectedMenu.Setup(SelectedMember);
                    EquipmentDisplay.Setup(SelectedMember);
                    RefreshCharacterSprite();
                }

                return;
            }
        }
    }

    void RefreshCharacterSprite()
    {
        CharacterPortrait.sprite = Resources.Load<Sprite>("BodyPortraits/Body" + SelectedMember.Profile.PortraitID);
        CharacterPortraitArm.sprite = Resources.Load<Sprite>("BodyPortraits/Arm" + SelectedMember.Profile.PortraitID + "A");
        CharacterPortraitTwoArm.sprite = Resources.Load<Sprite>("BodyPortraits/Arm" + SelectedMember.Profile.PortraitID + "B");
    }

    public void SelectCharacter(PartyMember member, bool openMenu)
    {
        SelectCharacter(partyMembersDisplayDict[member], openMenu);
    }

    public void SelectCharacter(CharacterVitalsDisplay display, bool openMenu)
    {
        if (_selectedMenu == null)
        {
            _selectedMenu = ProfileMenu;
        }

        if (HeldItemButton != null)
        {
            InventoryItem item = display.Member.Inventory.AddItem(HeldItemButton.Item);
            if (item != null)
            {
                if (display.Member.Equals(SelectedMember))
                    InventoryMenu.PlaceButtonAtSlot(HeldItemButton, item);
                else
                    Destroy(HeldItemButton.gameObject);
                HeldItemButton = null;
            }
            return;
        }

        if (display.Member.Vitals.IsReady())
        {
            if (SelectedMember != display.Member) {
                _party.SetActiveMember(display.Member);
                SelectNewMember?.Invoke(display.Member);
            }
            foreach (var cvd in CharacterVitalsUI)
                cvd.IndicateSelection(cvd == display);
        }

        if (openMenu)
        {
            if (SelectedMember != display.Member)
            {
                _party.SetActiveMember(display.Member);
                SelectNewMember?.Invoke(display.Member);
            }
            OpenMenu();
        }
    }

    void OpenMenu()
    {
        if (MenuManager.Instance.IsMenuOpen(new List<string>() { "Merchant", "Residence" }))
            return;

        CharacterMenuOpen = true;
        MenuManager.Instance.OpenMenu(_selectedMenu.MenuTag);
        _selectedMenu.Setup(SelectedMember);
        Vignette.enabled = true;
        foreach (var obj in CharacterMenuObjects)
            obj.SetActive(true);
        RefreshCharacterSprite();
        EquipmentDisplay.Setup(SelectedMember);
    }

    public InventoryItemButton CreateItemButton(Item item, Vector3 position, RectTransform parent)
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

    public void TryEquipRing(int slot)
    {
        if (HeldItemButton == null)
            return;

        Armor ring = HeldItemButton.Item.Data as Armor;
        if (ring == null)
        {
            TryEquip();
            return;
        }

        if (ring.EquipSlot != EquipSlot.Ring)
        {
            TryEquip();
            return;
        }

        InventoryItem returned = SelectedMember.Equipment.ReplaceRing(HeldItemButton.Item, slot);
        EquipSwap(returned);
    }

    public void TryEquipSecondary()
    {
        if (HeldItemButton == null)
            return;

        if (!SelectedMember.Skillset.CanDualWield(HeldItemButton.Item.Data))
        {
            TryEquip();
        }
        else
        {
            if (SelectedMember.Equipment.CanEquip(HeldItemButton.Item))
            {
                InventoryItem returned = SelectedMember.Equipment.Replace(HeldItemButton.Item, true);
                EquipSwap(returned);

                _selectedMenu.Setup(SelectedMember);
            }
        }
    }

    public void TryRead()
    {
        if (HeldItemButton != null && HeldItemButton.Item.Data is Scroll && Input.GetMouseButton(0))
        {
            ScrollInfoPopup.gameObject.SetActive(true);
            ScrollInfoPopup.UpdateUI(HeldItemButton.Item.Data as Scroll);
        }
    }

    public void TryEquip()
    {
        if (HeldItemButton == null)
            return;

        if(HeldItemButton.Item.IsConsumable())
        {
            TryConsume(SelectedMember);
            return;
        }

        if (!SelectedMember.Skillset.CanEquip(HeldItemButton.Item.Data))
            return;

        if (SelectedMember.Equipment.CanEquip(HeldItemButton.Item))
        {
            InventoryItem returned = SelectedMember.Equipment.Replace(HeldItemButton.Item);
            EquipSwap(returned);

            _selectedMenu.Setup(SelectedMember);
        }
    }

    void EquipSwap(InventoryItem returned)
    {
        if (returned != null)
        {
            InventoryItemButton invButton = CreateItemButton(returned.Data, HeldItemButton.transform.position, (RectTransform)HeldItemAnchor);
            invButton.Setup(returned, InventoryMenu);
            invButton.ForceHold();

            Destroy(HeldItemButton.gameObject);
            HeldItemButton = invButton;
            HeldItemButton.transform.parent = HeldItemAnchor;
        }
        else
        {
            Destroy(HeldItemButton.gameObject);
            HeldItemButton = null;
        }

        EquipmentDisplay.Setup(SelectedMember);
    }

    public void TryConsume(PartyMember member)
    {
        bool success = member.TryConsume(HeldItemButton.Item);
        if(success)
        {
            UpdateDisplay();
            ExpressMember(member, GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);
            Destroy(HeldItemButton.gameObject);
            HeldItemButton = null;
        }
    }

    public void GiveHoldItem(InventoryItem item)
    {
        if(HeldItemButton != null)
        {
            SelectedMember.Inventory.AddItem(HeldItemButton.Item);
            Destroy(HeldItemButton.gameObject);
        }

        InventoryItemButton invButton = HUD.Instance.CreateItemButton(item.Data, Input.mousePosition, (RectTransform)HeldItemAnchor);
        invButton.Setup(item, InventoryMenu);
        invButton.ForceHold();
        HeldItemButton = invButton;
        HeldItemButton.transform.parent = HeldItemAnchor;
    }

    public void HoldItem(ItemButton button)
    {
        if(button != null && button.Item.Data is Gold)
        {
            _party.CollectGold(button.Item);
            Destroy(button.gameObject);
            return;
        }

        if(button is EquipmentItemButton)
        {
            Vector3 pos = button.transform.position;
            Transform parent = button.transform.parent;
            Destroy(button.gameObject);

            InventoryItemButton invButton = HUD.Instance.CreateItemButton(button.Item.Data, pos, (RectTransform)parent);
            invButton.Setup(button.Item, InventoryMenu);
            invButton.ForceHold();
            button = invButton;

            _selectedMenu.Setup(SelectedMember);
        }

        HeldItemButton = button;
        if(button != null)
            HeldItemButton.transform.parent = HeldItemAnchor;
    }

    public void DropItem()
    {
        if (HeldItemButton == null || CharacterMenuOpen || OtherMenuOpen)
            return;

        DropController.Instance.DropItem(HeldItemButton.Item, _party.DropPosition);
        Destroy(HeldItemButton.gameObject);
        HeldItemButton = null;
    }

    public bool SellItem(InventoryItem item, int price)
    {
        bool success = _party.TryPay(-price);
        if(success)
        {
            success = SelectedMember.Inventory.RemoveItem(item);
        }

        return success;
    }

    public bool IdentifyItem(InventoryItem item, int price)
    {
        bool success = _party.TryPay(price);
        if (success)
        {
            success = item.TryIdentify(100000);
        }

        return success;
    }

    public bool RepairItem(InventoryItem item, int price)
    {
        bool success = _party.TryPay(price);
        if (success)
        {
            success = item.TryRepair(100000);
        }

        return success;
    }

    public bool PurchaseItem(InventoryItem item, int price)
    {
        bool success = _party.TryPay(price);
        if (success)
        {
            if (SelectedMember != null)
            {
                InventoryItem result = SelectedMember.Inventory.AddItem(item);
                if (item != null)
                    return true;
            }

            if (HeldItemButton != null)
            {
                _party.TryPay(-price);
                return false;
            }

            InventoryItemButton invButton = HUD.Instance.CreateItemButton(item.Data, HeldItemAnchor.transform.position, (RectTransform)HeldItemAnchor);
            invButton.Setup(item, InventoryMenu);
            invButton.ForceHold();

            HeldItemButton = invButton;
            HeldItemButton.transform.parent = HeldItemAnchor;
            PartyController.Instance.SetControlState(ControlState.MouseControl, true);
        }

        return success;
    }

    public bool PickupCorpse(Enemy enemy)
    {
        return true;
    }

    public bool PickupDrop(ItemDrop drop)
    {
        if(drop.Item.Data is Gold)
        {
            _party.CollectGold(drop.Item);
            return true;
        }

        if (SelectedMember != null)
        {
            InventoryItem item = SelectedMember.Inventory.AddItem(drop.Item);
            if (item != null)
                return true;
        }

        if (HeldItemButton != null)
            return false;

        InventoryItemButton invButton = HUD.Instance.CreateItemButton(drop.Item.Data, HeldItemAnchor.transform.position, (RectTransform)HeldItemAnchor);
        InventoryItem invItem = drop.Item;
        invButton.Setup(invItem, InventoryMenu);
        invButton.ForceHold();

        HeldItemButton = invButton;
        HeldItemButton.transform.parent = HeldItemAnchor;
        PartyController.Instance.SetControlState(ControlState.MouseControl, true);
        return true;
    }

    public void InspectChest(Chest chest)
    {
        PartyController.Instance.SetControlState(ControlState.MenuLock);
        MenuManager.Instance.OpenMenu("Chest");
        ChestMenu.Setup(chest.Inventory);
        OtherMenuOpen = true;
    }

    public void EnterResidence(Residency residency)
    {
        PartyController.Instance.SetControlState(ControlState.MenuLock);
        MenuManager.Instance.OpenMenu("Residence");
        ResidenceMenu.Setup(residency);
        OtherMenuOpen = true;
        SideMenu.SetActive(false);
    }

    public void EnterResidence(Merchant merchant)
    {
        PartyController.Instance.SetControlState(ControlState.MenuLock);
        MenuManager.Instance.OpenMenu("Merchant");
        MerchantMenu.Setup(merchant);
        OtherMenuOpen = true;
        SideMenu.SetActive(false);
    }

    public void Converse(NPC npc)
    {
        PartyController.Instance.SetControlState(ControlState.MenuLock);
        MenuManager.Instance.OpenMenu("NPC");
        NPCMenu.Setup(npc);
        OtherMenuOpen = true;
        SideMenu.SetActive(false);
    }

    public bool TryConsume(InventoryItem item)
    {
        if (HeldItemButton != null && item.ID.Contains("bottle"))
        {
            List<InventoryItem> results = ItemDatabase.Instance.TryCombine(item.Data as Consumable, HeldItemButton.Item.Data as Consumable);
            if (results.Count > 0)
            {
                int slot = item.Slot;
                SelectedMember.Inventory.RemoveItem(item);
                Destroy(HeldItemButton.gameObject);
                HeldItemButton = null;

                for (int i = 0; i < results.Count; i++)
                {
                    if (i == 0)
                        SelectedMember.Inventory.AddItem(results[i], slot);
                    else
                        SelectedMember.Inventory.AddItem(results[i]);
                }

                _selectedMenu.Setup(SelectedMember);
                return true;
            }
        }
        return false;
    }

    public void UpdatePopup(InventoryItem item, bool showPopup)
    {
        if (showPopup)
        {
            ItemInfoPopup.gameObject.SetActive(true);

            if (item.IsBroken)
            {
                bool success = ForceSelectedMember.TryRepair(item);
                if (success)
                {
                    ExpressSelectedMember(GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);
                }
            }
            if (!item.IsIdentified)
            {
                bool success = ForceSelectedMember.TryIdentify(item);
                if (success)
                {
                    ExpressSelectedMember(GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);
                }
            }
        }
        ItemInfoPopup.UpdateUI(item);
    }

    public void HidePopup()
    {
        ItemInfoPopup.gameObject.SetActive(false);
        ScrollInfoPopup.gameObject.SetActive(false);
    }

    public void UpdateCompass()
    {
        float rot = _party.Entity.transform.eulerAngles.y % 360;

        float translation = -rot * 256f / 360f;

        Compass.localPosition = new Vector3(translation, Compass.localPosition.y, Compass.localPosition.z);
    }

    public void ShowLoad(bool isShowing)
    {
        LoadingScreen.gameObject.SetActive(isShowing);
    }

    public void ReleaseInfoLock()
    {
        _infoMessageLock = false;
        _currentInfoMessage = "";
        InfoMessageLabel.text = "";
    }

    public void ExpressMembers(string expression, float duration)
    {
        foreach(var member in _party.Members)
            member.Vitals.SetExpression(expression, duration);
        UpdateDisplay();
    }

    public void ExpressMember(PartyMember member, string expression, float duration)
    {
        member.Vitals.SetExpression(expression, duration);
        UpdateDisplay();
    }

    public void ExpressSelectedMember(string expression, float duration)
    {
        ExpressMember(SelectedMember, expression, duration);
    }

    public void SendInfoMessage(string msg, float duration = 0, bool setLock = false)
    {
        if(setLock)
            _infoMessageLock = true;

        if (duration == 0)
        {
            if (setLock || !_infoMessageLock)
            {
                _currentInfoMessage = msg;
                if (!_showingTempMessage)
                    ResetInfoMessage();
            }
        }
        else
        {
            CancelInvoke("ResetInfoMessage");

            InfoMessageLabel.color = Color.yellow;
            InfoMessageLabel.text = msg;
            _showingTempMessage = true;

            Invoke("ResetInfoMessage", duration);
        }
    }

    void ResetInfoMessage()
    {
        InfoMessageLabel.color = Color.white;
        InfoMessageLabel.text = _currentInfoMessage;
        _showingTempMessage = false;
    }
}
