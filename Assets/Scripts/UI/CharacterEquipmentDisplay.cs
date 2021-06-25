using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CharacterEquipmentDisplay : Menu 
{
    [SerializeField] Image CharacterPortrait;
    [SerializeField] Image CharacterPortraitArm;
    [SerializeField] Image CharacterPortraitTwoArm;

    [SerializeField] RectTransform PrimaryAnchor;
    [SerializeField] RectTransform RangedAnchor;
    [SerializeField] RectTransform SecondaryAnchor;
    [SerializeField] RectTransform TwoHandAnchor;

    [SerializeField] RectTransform HelmetAnchor;
    [SerializeField] RectTransform BodyAnchor;
    [SerializeField] RectTransform CloakAnchor;
    [SerializeField] RectTransform BeltAnchor;
    [SerializeField] RectTransform BootsAnchor;

    [SerializeField] RectTransform AmuletAnchor;
    [SerializeField] RectTransform GauntletAnchor;
    [SerializeField] RectTransform[] RingAnchors;

    [SerializeField] Image LeftArmOverlay;
    [SerializeField] Image TwoArmOverlay;
    [SerializeField] Image LeftHandOverlay;
    [SerializeField] Image TwoHandOverlay;

    [SerializeField] GameObject Popup;

    [SerializeField] GameObject EquipmentItemButtonPrefab;

    List<GameObject> _itemButtons = new List<GameObject>();
    List<GameObject> _popupButtons = new List<GameObject>();

    PartyMember _member;

    bool _popupOpen = false;

    protected override void Init()
    {
        CharacterMenu.OnCharacterMenuOpen += OpenCharacterMenu;

        base.Init();
    }

    void OpenCharacterMenu(bool opened)
    {
        if (opened && !Contents.activeSelf)
        {
            Setup(Party.Instance.ActiveMember);
            Party.OnMemberChanged += Setup;
            Contents.SetActive(true);
        }
        else if (!opened && Contents.activeSelf)
        {
            _member.Equipment.OnEquipmentChanged -= Refresh;
            Party.OnMemberChanged -= Setup;
            Contents.SetActive(false);
        }
    }

    void Refresh()
    {
        Setup(_member);
    }

    public void Setup(PartyMember member)
    {
        if(_member != null)
        {
            _member.Equipment.OnEquipmentChanged -= Refresh;
        }

        _member = member;

        foreach (var obj in _itemButtons)
            Destroy(obj);
        _itemButtons.Clear();

        foreach (var obj in _popupButtons)
            Destroy(obj);
        _popupButtons.Clear();

        CreateWeaponButton(EquipSlot.Primary, PrimaryAnchor);
        CreateWeaponButton(EquipSlot.Ranged, RangedAnchor);
        CreateWeaponButton(EquipSlot.TwoHanded, TwoHandAnchor);
        CreateWeaponButton(EquipSlot.Secondary, SecondaryAnchor);
        CreateArmorButton(EquipSlot.Secondary, SecondaryAnchor);
        CreateArmorButton(EquipSlot.Body, BodyAnchor);
        CreateArmorButton(EquipSlot.Helmet, HelmetAnchor);
        CreateArmorButton(EquipSlot.Cloak, CloakAnchor);
        CreateArmorButton(EquipSlot.Belt, BeltAnchor);
        CreateArmorButton(EquipSlot.Boots, BootsAnchor);

        if (_popupOpen)
        {
            CreateArmorButton(EquipSlot.Amulet, AmuletAnchor, true);
            CreateArmorButton(EquipSlot.Gauntlets, GauntletAnchor, true);
            for (int i = 0; i < 6; i++)
            {
                CreateRingButton(i, RingAnchors[i]);
            }
        }

        RefreshOverlays();

        CharacterPortrait.sprite = SpriteHandler.FetchSprite("BodyPortraits", "Body" + member.Profile.PortraitID);
        CharacterPortraitArm.sprite = SpriteHandler.FetchSprite("BodyPortraits", "Arm" + member.Profile.PortraitID + "A");
        CharacterPortraitTwoArm.sprite = SpriteHandler.FetchSprite("BodyPortraits", "Arm" + member.Profile.PortraitID + "B");

        _member.Equipment.OnEquipmentChanged += Refresh;
    }

    void RefreshOverlays()
    {
        if (_member.Equipment.IsTwoHanding())
        {
            LeftArmOverlay.enabled = false;
            LeftHandOverlay.enabled = false;

            TwoArmOverlay.enabled = true;
            TwoHandOverlay.enabled = true;
        }
        else
        {
            LeftArmOverlay.enabled = true;
            LeftHandOverlay.enabled = !_member.Equipment.IsShielding();

            TwoArmOverlay.enabled = false;
            TwoHandOverlay.enabled = false;
        }
    }

    void CreateRingButton(int slot, RectTransform anchor)
    {
        InventoryItem item = null;

        if (slot == 0 && _member.Equipment.Ring0 != null)
        {
            item = _member.Equipment.Ring0;
        }
        else if (slot == 1 && _member.Equipment.Ring1 != null)
        {
            item = _member.Equipment.Ring1;
        }
        else if (slot == 2 && _member.Equipment.Ring2 != null)
        {
            item = _member.Equipment.Ring2;
        }
        else if (slot == 3 && _member.Equipment.Ring3 != null)
        {
            item = _member.Equipment.Ring3;
        }
        else if (slot == 4 && _member.Equipment.Ring4 != null)
        {
            item = _member.Equipment.Ring4;
        }
        else if (slot == 5 && _member.Equipment.Ring5 != null)
        {
            item = _member.Equipment.Ring5;
        }

        if (item == null)
            return;

        GameObject obj = Instantiate(EquipmentItemButtonPrefab, anchor);
        RectTransform rt = (RectTransform)obj.transform;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x * item.Data.Width, rt.sizeDelta.y * item.Data.Height);

        obj.transform.position = anchor.position;
        _itemButtons.Add(obj);

        EquipmentItemButton btn = obj.GetComponent<EquipmentItemButton>();
        btn.Setup(item, this, EquipSlot.Ring);
    }

    void CreateArmorButton(EquipSlot slot, RectTransform anchor, bool popupItem = false)
    {
        CreateButton(slot, _member.Equipment.Armor, anchor);
    }

    void CreateWeaponButton(EquipSlot slot, RectTransform anchor, bool popupItem = false)
    {
        CreateButton(slot, _member.Equipment.Weapons, anchor);
    }

    void CreateButton(EquipSlot slot, Dictionary<EquipSlot,InventoryItem> dict, RectTransform anchor, bool popupItem = false)
    {
        if (dict.ContainsKey(slot))
        {
            InventoryItem item = dict[slot];

            GameObject obj = Instantiate(EquipmentItemButtonPrefab, anchor);
            RectTransform rt = (RectTransform)obj.transform;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x * item.Data.Width, rt.sizeDelta.y * item.Data.Height);

            obj.transform.position = anchor.position;

            if (popupItem)
                _popupButtons.Add(obj);
            else
                _itemButtons.Add(obj);

            EquipmentItemButton btn = obj.GetComponent<EquipmentItemButton>();
            btn.Setup(item, this, slot);
        }
    }

    public bool PickupItem(EquipmentItemButton button, EquipSlot slot)
    {
        bool success = false;
        if (HUD.Instance.HeldItemButton != null)
        {
            if (slot == EquipSlot.Secondary)
                HUD.Instance.TryEquipSecondary();
            else
                HUD.Instance.TryEquip();
            RefreshOverlays();
            return success;
        }

        success = _member.Equipment.RemoveItem(button.Item, slot);
        if(success)
        {
            _itemButtons.Remove(button.gameObject);
            HUD.Instance.HoldItem(button);
        }
        RefreshOverlays();
        return success;
    }

    public void TogglePopup()
    {
        foreach (var obj in _popupButtons)
            Destroy(obj);
        _popupButtons.Clear();

        if (!_popupOpen)
        {
            CreateArmorButton(EquipSlot.Amulet, AmuletAnchor, true);
            CreateArmorButton(EquipSlot.Gauntlets, GauntletAnchor, true);
            for (int i = 0; i < 6; i++)
            {
                CreateRingButton(i, RingAnchors[i]);
            }
        }

        _popupOpen = !_popupOpen;
        Popup.SetActive(_popupOpen);

    }

}
