using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogStep
{
    public string Display;

    [TextArea(minLines: 1, maxLines: 15)]
    public string FirstDialog;
    [TextArea(minLines: 1, maxLines: 15)]
    public string NoRequiredItemDialog;

    public string ItemRequired;

    public bool StartQuest;
    public bool ProgressQuest;
    public bool CompleteQuest;

    public string ItemReceived;
    public int GoldReceived;
    public int ExpReceived;

    public bool MembershipOffer;
    public bool ExpertiseOffer;
}

[System.Serializable]
public class MembershipOffer
{
    public string GuildID;
    public string GuildName;
    public int Cost;
}

[System.Serializable]
public class ExpertiseOffer
{
    public string SkillID;
    public SkillProficiency Proficiency;
    public int Cost;
}

[System.Serializable]
public class DialogOption
{
    public QuestLine QuestLine;
    public List<DialogStep> Steps;
    public MembershipOffer Membership;
    public ExpertiseOffer Expertise;
}

[System.Serializable]
public class Services
{
    public int RoomRentalCost;

    public int FoodQuantity;
    public int FoodCost;

    public int DrinkCost;
    public int TipCost;
    public string[] Tips;

    public int TrainingCost;
    public int MaxTrainingLevel;

    public List<string> Skills;
    public int SkillCost;

    public bool IsBank;

    public bool IsTemple;
    public float HealingCostMultiplier;

    public bool IsBounty;

    public bool IsTransport;
}

[System.Serializable]
public class ResidentData
{
    public string ShortName;
    public string DisplayName;
    public string FirstName;
    public Sprite Sprite;

    public string GuildID;

    public bool IsService;
    public Services Services;
    public List<DialogOption> Options;
}

[CreateAssetMenu]
public class ResidencyDBObject : ScriptableObject
{
    public string ID;
    public string DisplayName;
    public StoreHours Hours;
    public List<ResidentData> Residents;
}
