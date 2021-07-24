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

public class HUD : Menu {
    public static HUD Instance;
    void Awake() { Instance = this; }

    [SerializeField] Text FPS;
    [SerializeField] GameObject DebugMenu;
    [SerializeField] GameObject SideMenu;

    [SerializeField] Transform Compass;

    [SerializeField] CharacterVitalsDisplay[] CharacterVitalsUI;

    [SerializeField] HireButton[] HireButtons;

    [SerializeField] Text FoodLabel;
    [SerializeField] Text GoldLabel;

    [SerializeField] Image Vignette;

    [SerializeField] Sprite HoldSprite;
    [SerializeField] Sprite WaitSprite;
    [SerializeField] Image CombatIndicator;

    [SerializeField] ProfileMenu ProfileMenu;
    [SerializeField] SkillsMenu SkillsMenu;
    [SerializeField] InventoryMenu InventoryMenu;
    [SerializeField] AwardsMenu AwardsMenu;

    [SerializeField] Transform HeldItemAnchor;

    Party _party;
    CharacterMenu _selectedMenu;
    bool _frozen;

    public ItemButton HeldItemButton { get; private set; }

    public static event System.Action<PartyMember> OnSpellsPressed;

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
            member.Vitals.OnVitalsChanged += UpdateCurrentMenu;
            member.Profile.OnProfileChanged += UpdateCurrentMenu;
        }

        for (int i = 0; i < CharacterVitalsUI.Length; i++)
        {
            CharacterVitalsUI[i].Init(party.Members[i]);
        }

        _selectedMenu = ProfileMenu;

        Vignette.enabled = false;
        CombatIndicator.enabled = false;

        FoodLabel.text = _party.CurrentFood.ToString();
        GoldLabel.text = _party.CurrentGold.ToString();

        Party.OnFundsChanged += UpdateFunds;
        Party.OnFoodChanged += UpdateFood;
        Party.OnHiresChanged += UpdateHires;
        MenuManager.OnMenuOpened += MenuOpened;
        MenuManager.OnMenusClosed += MenusClosed;
        TurnController.OnTurnBasedToggled += UpdateCombatIndicator;
        TurnController.OnEnemyMoveToggled += UpdateCombatIndicatorWait;
        TimeManagement.OnTimeFreezeChanged += Freeze;
    }

    void Freeze(bool enable)
    {
        Debug.LogError(enable);
        _frozen = enable;
    }

    void Update()
    {
        if (_party != null)
            UpdateCompass();

        if (Input.GetKeyDown(KeyCode.F1))
        {
            DebugMenu.SetActive(!DebugMenu.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            FPS.enabled = !FPS.enabled;
        }

        float fps = 1 / Time.unscaledDeltaTime;
        FPS.text = "" + fps;
    }

    void MenuOpened(bool useVignette, bool hideSidemenu)
    {
        Vignette.enabled = useVignette;
        SideMenu.SetActive(!hideSidemenu);
    }

    void MenusClosed()
    {
        Vignette.enabled = false;
        SideMenu.SetActive(true);
    }

    void UpdateCurrentMenu()
    {
        if(_selectedMenu.IsOpen)
            _selectedMenu.Setup(_party.ActiveMember);
    }

    void UpdateFood(int food)
    {
        FoodLabel.text = food.ToString();
    }

    void UpdateFunds(int gold, int balance)
    {
        GoldLabel.text = gold.ToString();
    }

    void UpdateHires(List<NPC> hires)
    {
        HireButtons[0].gameObject.SetActive(false);
        HireButtons[1].gameObject.SetActive(false);
        for (int i = 0; i < hires.Count; i++)
        {
            HireButtons[i].Setup(hires[i]);
        }
    }

    void UpdateCombatIndicator(bool enable)
    {
        CombatIndicator.enabled = enable;
        if (enable)
            CombatIndicator.sprite = HoldSprite;
    }

    void UpdateCombatIndicatorWait(bool enable)
    {
        CombatIndicator.sprite = enable ? WaitSprite : HoldSprite;
    }

    public void OpenSystem()
    {
        if (HeldItemButton != null)
            return;

        MenuManager.Instance.SwapMenu("System", true, false);
    }

    public void OpenQuests()
    {
        MenuManager.Instance.SwapMenu("Quests", true, false);
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
        if (_party.ActiveMember != null)
        {
            OnSpellsPressed?.Invoke(_party.ActiveMember);
        }
    }

    public void OpenRest()
    {
        MenuManager.Instance.SwapMenu("Rest", false, true);
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

    public void Close()
    {
        SoundManager.Instance.PlayUISound("Button");
        MenuManager.Instance.CloseMenu(_selectedMenu.MenuTag);
    }

    void SwapMenu(CharacterMenu menu)
    {
        MenuManager.Instance.CloseMenu(_selectedMenu.MenuTag);
        _selectedMenu = menu;
        _selectedMenu.Setup(_party.ActiveMember);
        SoundManager.Instance.PlayUISound("Button");
        MenuManager.Instance.OpenMenu(menu.MenuTag, true, false);
    }

    public void SelectCharacter(PartyMember member, bool openMenu)
    {
        if (_frozen)
            return;

        if (HeldItemButton != null)
        {
            InventoryItem item = member.Inventory.AddItem(HeldItemButton.Item);
            if (item != null)
            {
                if (member.Equals(_party.ActiveMember))
                    InventoryMenu.PlaceButtonAtSlot(HeldItemButton, item);
                else
                    Destroy(HeldItemButton.gameObject);
                HeldItemButton = null;
            }
            return;
        }

        if (!TurnController.Instance.IsTurnBasedEnabled || _selectedMenu.IsOpen)
        {
            if (_party.ActiveMember != member) {
                _party.SetActiveMember(member);
            } 
            else
            {
                openMenu = true;
            }
        }

        if (openMenu)
        {
            if (_party.ActiveMember != member)
            {
                _party.SetActiveMember(member);
            }
            OpenCharacterMenu();
        }
    }

    void OpenCharacterMenu()
    {
        if (MenuManager.Instance.IsMenuOpen(new List<string>() { "Merchant", "Residence", "NPC", "Rest", "System", "Quests", "Spells" }))
            return;

        MenuManager.Instance.OpenMenu(_selectedMenu.MenuTag, true, false);
        _selectedMenu.Setup(_party.ActiveMember);
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

        InventoryItem returned = _party.ActiveMember.Equipment.ReplaceRing(HeldItemButton.Item, slot);
        EquipSwap(returned);
    }

    public void TryEquipSecondary()
    {
        if (HeldItemButton == null)
            return;

        if (!_party.ActiveMember.Skillset.CanDualWield(HeldItemButton.Item.Data))
        {
            TryEquip();
        }
        else
        {
            if (_party.ActiveMember.Equipment.CanEquip(HeldItemButton.Item))
            {
                InventoryItem returned = _party.ActiveMember.Equipment.Replace(HeldItemButton.Item, true);
                EquipSwap(returned);

                _selectedMenu.Setup(_party.ActiveMember);
            }
        }
    }

    public void TryRead()
    {
        if (HeldItemButton != null && HeldItemButton.Item.Data is Scroll && Input.GetMouseButton(0))
        {
            Popups.ShowScroll(HeldItemButton.Item.Data as Scroll);
        }
    }

    public void TryEquip()
    {
        if (HeldItemButton == null)
            return;

        if(HeldItemButton.Item.IsConsumable())
        {
            TryConsume(_party.ActiveMember);
            return;
        }

        if (!_party.ActiveMember.Skillset.CanEquip(HeldItemButton.Item.Data))
            return;

        if (_party.ActiveMember.Equipment.CanEquip(HeldItemButton.Item))
        {
            InventoryItem returned = _party.ActiveMember.Equipment.Replace(HeldItemButton.Item);
            EquipSwap(returned);

            _selectedMenu.Setup(_party.ActiveMember);
        }
    }

    void EquipSwap(InventoryItem returned)
    {
        if (returned != null)
        {
            InventoryItemButton invButton = InventoryItemButton.Create(returned.Data, HeldItemButton.transform.position, (RectTransform)HeldItemAnchor);
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
    }

    public void TryConsume(PartyMember member)
    {
        bool success = member.TryConsume(HeldItemButton.Item);
        if(success)
        {
            member.Vitals.Express(GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);
            Destroy(HeldItemButton.gameObject);
            HeldItemButton = null;
        }
    }

    public void GiveHoldItem(InventoryItem item)
    {
        if(HeldItemButton != null)
        {
            _party.ActiveMember.Inventory.AddItem(HeldItemButton.Item);
            Destroy(HeldItemButton.gameObject);
        }

        InventoryItemButton invButton = InventoryItemButton.Create(item.Data, Input.mousePosition, (RectTransform)HeldItemAnchor);
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

            InventoryItemButton invButton = InventoryItemButton.Create(button.Item.Data, pos, (RectTransform)parent);
            invButton.Setup(button.Item, InventoryMenu);
            invButton.ForceHold();
            button = invButton;

            _selectedMenu.Setup(_party.ActiveMember);
        }

        HeldItemButton = button;
        if(button != null)
            HeldItemButton.transform.parent = HeldItemAnchor;
    }

    public void DropItem()
    {
        if (HeldItemButton == null)
            return;

        DropController.Instance.DropItem(HeldItemButton.Item, _party.DropPosition);
        Destroy(HeldItemButton.gameObject);
        HeldItemButton = null;
    }

    public bool PurchaseItem(InventoryItem item, int price)
    {
        bool success = _party.TryPay(price);
        if (success)
        {
            if (_party.ActiveMember != null)
            {
                InventoryItem result = _party.ActiveMember.Inventory.AddItem(item);
                if (item != null)
                    return true;
            }

            if (HeldItemButton != null)
            {
                _party.TryPay(-price);
                return false;
            }

            InventoryItemButton invButton = InventoryItemButton.Create(item.Data, HeldItemAnchor.transform.position, (RectTransform)HeldItemAnchor);
            invButton.Setup(item, InventoryMenu);
            invButton.ForceHold();

            HeldItemButton = invButton;
            HeldItemButton.transform.parent = HeldItemAnchor;

            _party.ActiveMember.Vitals.Express(GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);
        }
        else
        {
            _party.ActiveMember.Vitals.Express(GameConstants.EXPRESSION_SAD, GameConstants.EXPRESSION_SAD_DURATION);
        }

        return success;
    }

    public bool TryHold(InventoryItem item)
    {
        if (HeldItemButton != null)
            return false;

        InventoryItemButton invButton = InventoryItemButton.Create(item.Data, HeldItemAnchor.transform.position, (RectTransform)HeldItemAnchor);
        invButton.Setup(item, InventoryMenu);
        invButton.ForceHold();

        HeldItemButton = invButton;
        HeldItemButton.transform.parent = HeldItemAnchor;
        PartyController.Instance.SetControlState(ControlState.MouseControl, true);
        return true;
    }

    public bool TryCombine(InventoryItem item)
    {
        if (HeldItemButton != null && item.ID.Contains("bottle"))
        {
            List<InventoryItem> results = ItemDatabase.Instance.TryCombine(item.Data as Consumable, HeldItemButton.Item.Data as Consumable);
            if (results.Count > 0)
            {
                int slot = item.Slot;
                _party.ActiveMember.Inventory.RemoveItem(item);
                Destroy(HeldItemButton.gameObject);
                HeldItemButton = null;

                for (int i = 0; i < results.Count; i++)
                {
                    if (i == 0)
                        _party.ActiveMember.Inventory.AddItem(results[i], slot);
                    else
                        _party.ActiveMember.Inventory.AddItem(results[i]);
                }

                _selectedMenu.Setup(_party.ActiveMember);
                return true;
            }
        }
        return false;
    }

    public void UpdateCompass()
    {
        float rot = _party.Entity.transform.eulerAngles.y % 360;
        float translation = -rot * 256f / 360f;
        Compass.localPosition = new Vector3(translation, Compass.localPosition.y, Compass.localPosition.z);
    }
}
