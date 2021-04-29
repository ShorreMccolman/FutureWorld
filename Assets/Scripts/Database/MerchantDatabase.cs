using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StoreType
{
    General,
    Weapon,
    Armor,
    Magic,
    Spell
}

public enum GeneralItemType
{
    Armor,
    Clothes,
    Herbs,
    Alchemy,
    Jewelery,
    Spells
}

[System.Serializable]
public class StoreHours
{
    public int OpenHour;
    public int CloseHour;

    public bool IsOpen(int hour)
    {
        if (OpenHour == CloseHour)
            return true;
        else if (OpenHour > CloseHour)
            return hour >= OpenHour || hour < CloseHour;
        else
            return hour >= OpenHour && hour < CloseHour;
    }

    public string GetOpenHours()
    {
        string result;

        bool isAM = true;
        int value = OpenHour;
        if (OpenHour >= 12) {
            isAM = false;
            value -= 12;
        }
        if (value == 0)
            value = 12;

        result = value + (isAM ? "am" : "pm");

        isAM = true;
        value = CloseHour;
        if (CloseHour >= 12)
        {
            isAM = false;
            value -= 12;
        }
        if (value == 0)
            value = 12;

        result += " to " + value + (isAM ? "am" : "pm");
        return result;
    }
}

[System.Serializable]
public class StoreInfo
{
    public TreasureLevel Level;

    public List<GeneralItemType> GeneralTypes;
    public List<WeaponType> WeaponTypes;
    public List<ArmorType> ArmorTypes;
    public List<SpellSchool> MagicTypes;

    public int CountForStoreType(StoreType type)
    {
        switch(type)
        {
            case StoreType.General:
                return GeneralTypes.Count;
            case StoreType.Weapon:
                return WeaponTypes.Count;
            case StoreType.Armor:
                return ArmorTypes.Count;
            case StoreType.Magic:
                return GeneralTypes.Count;
            case StoreType.Spell:
                return MagicTypes.Count;
        }
        return 0;
    }
}

[System.Serializable]
public class MerchantData
{
    public string ID;
    public string StoreName;

    public string MerchantName;
    public Sprite Sprite;

    public StoreType StoreType;

    public StoreInfo BuyInfo;
    public StoreInfo SpecialInfo;

    public bool CanIdentify;
    public bool CanRepair;

    public StoreHours Hours;
}

public class MerchantDatabase
{
    public static MerchantDatabase Instance;

    Dictionary<string, MerchantData> _merchantDict;

    public MerchantDatabase()
    {
        Instance = this;

        _merchantDict = new Dictionary<string, MerchantData>();

        MerchantDBObject[] resDBObjects = Resources.LoadAll<MerchantDBObject>("Database/Residencies");
        if (resDBObjects.Length == 0)
        {
            Debug.LogError("Failed to load ResidentDB");
            return;
        }

        foreach (var db in resDBObjects)
        {
            foreach(var merchant in db.Merchants)
                _merchantDict.Add(merchant.ID, merchant);
        }
    }

    public MerchantData GetMerchant(string ID)
    {
        if (!_merchantDict.ContainsKey(ID))
            return null;

        return _merchantDict[ID];
    }
}
