using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResidenceMenu : ConversationMenu
{
    [SerializeField] Text ResidencyLabel;
    [SerializeField] Transform ResidentsAnchor;
    [SerializeField] GameObject ResidentOptionPrefab;
    Residency _currentResidency;
    List<ResidentOptionUI> Options;

    [SerializeField] Text ResidentLabel;
    [SerializeField] Image ResidentSprite;
    [SerializeField] Transform ResidentDialogObject;
    Resident _currentResident;

    [SerializeField] RectTransform DialogBox;
    [SerializeField] Text Dialog;

    bool _hasAchievedSpecialCondition;
    int _advanceStepCounter = -1;
    bool _hasTrained;

    public void Setup(Residency residency)
    {
        _currentResidency = residency;

        _hasAchievedSpecialCondition = false;

        DialogOptions = new List<OptionButton>();

        ResidentsAnchor.gameObject.SetActive(true);
        ResidentDialogObject.gameObject.SetActive(false);

        ResidencyLabel.text = _currentResidency.DisplayName;
        HUD.Instance.SendInfoMessage(_currentResidency.DisplayName);

        Options = new List<ResidentOptionUI>();
        int yOffset = 100 * (_currentResidency.Residents.Count - 1);
        for (int i = 0; i < _currentResidency.Residents.Count; i++)
        {
            GameObject obj = Instantiate(ResidentOptionPrefab, ResidentsAnchor);
            obj.transform.position = ResidentsAnchor.position + Vector3.down * 200 * i + Vector3.up * yOffset;

            ResidentOptionUI UI = obj.GetComponent<ResidentOptionUI>();
            UI.Setup(this, _currentResidency.Residents[i]);
            Options.Add(UI);
        }

        if (_currentResidency.Residents.Count == 1)
            SelectResident(_currentResidency.Residents[0]);

        DialogBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

        Party.MemberChanged += OnMemberChanged;
        SoundManager.Instance.SetMusicVolume(0.25f);
    }

    public void SelectResident(Resident resident)
    {
        _currentResident = resident;
        _advanceStepCounter = -1;

        foreach (var option in DialogOptions)
            Destroy(option.gameObject);
        DialogOptions.Clear();

        ResidentsAnchor.gameObject.SetActive(false);
        ResidentDialogObject.gameObject.SetActive(true);

        ResidentLabel.text = resident.Data.DisplayName;
        ResidentSprite.sprite = resident.Data.Sprite;

        DialogOptions = new List<OptionButton>();

        GameObject obj;
        DialogOptionButton UI;

        int yOffset = 30 * (_currentResident.Data.Options.Count);
        for (int i = 0; i < _currentResident.Data.Options.Count; i++)
        {
            DialogOption option = _currentResident.Data.Options[i];
            DialogStep step = option.Steps[resident.OptionProgress[i]];

            if (step.MembershipOffer && Party.Instance.QuestLog.IsMemberOfGuild(option.Membership.GuildID))
                continue;

            obj = Instantiate(DialogOptionPrefab, DialogAnchor);

            UI = obj.GetComponent<DialogOptionButton>();

            string label = step.Display;
            if (step.MembershipOffer)
                label = "Join " + option.Membership.GuildName;
            else if (step.ExpertiseOffer)
            {
                Skill skill = SkillDatabase.Instance.GetSkill(option.Expertise.SkillID);
                label = GameConstants.LabelForProficiency[option.Expertise.Proficiency] + " " + skill.TrainingName;
            }

            UI.Setup(label, i, AttemptAdvanceStep);
            DialogOptions.Add(UI);
        }

        if (!Party.Instance.QuestLog.IsMemberOfGuild(resident.Data.GuildID))
        { 
            obj = Instantiate(DialogOptionPrefab, DialogAnchor);

            UI = obj.GetComponent<DialogOptionButton>();
            UI.Setup("You must be a member of this guild to study here.", null);
            DialogOptions.Add(UI);
        } 
        else if (resident.Data.IsService)
        {
            if (!Party.Instance.ActiveMember.IsConcious() && !resident.Data.Services.IsTemple)
            {
                obj = Instantiate(DialogOptionPrefab, DialogAnchor);

                UI = obj.GetComponent<DialogOptionButton>();
                UI.Setup(Party.Instance.ActiveMember.Profile.CharacterName + " is in no condition to do anything", null);
                DialogOptions.Add(UI);
            } 
            else
            {
                if(resident.Data.Services.IsTransport)
                {
                    obj = Instantiate(DialogOptionPrefab, DialogAnchor);

                    UI = obj.GetComponent<DialogOptionButton>();
                    UI.Setup("Sorry, come back another day.", null);
                    DialogOptions.Add(UI);
                }
                if (resident.Data.Services.IsBank)
                {
                    obj = Instantiate(DialogOptionPrefab, DialogAnchor);

                    UI = obj.GetComponent<DialogOptionButton>();
                    UI.Setup("Deposit", StartDeposit);
                    DialogOptions.Add(UI);

                    obj = Instantiate(DialogOptionPrefab, DialogAnchor);

                    UI = obj.GetComponent<DialogOptionButton>();
                    UI.Setup("Withdraw", StartWithdraw);
                    DialogOptions.Add(UI);

                    obj = Instantiate(DialogOptionPrefab, DialogAnchor);

                    UI = obj.GetComponent<DialogOptionButton>();
                    UI.Setup("Balance: " + Party.Instance.CurrentBalance, null);
                    DialogOptions.Add(UI);
                }
                if (resident.Data.Services.IsTemple)
                {
                    OfferHealingToActiveMember();

                    obj = Instantiate(DialogOptionPrefab, DialogAnchor);

                    UI = obj.GetComponent<DialogOptionButton>();
                    UI.Setup("Donate", Donate);
                    DialogOptions.Add(UI);
                }
                if(resident.Data.Services.IsBounty)
                {
                    resident.TryUpdateBounty(Party.Instance.CurrentTime);

                    obj = Instantiate(DialogOptionPrefab, DialogAnchor);

                    UI = obj.GetComponent<DialogOptionButton>();
                    UI.Setup("Bounty Hunt", SetupBounty);
                    DialogOptions.Add(UI);
                }
                if (resident.Data.Services.Skills.Count > 0)
                {
                    OfferSkillsToActiveMember();
                }
                if (resident.Data.Services.MaxTrainingLevel > 0)
                {
                    OfferTrainingToActiveMember();
                }
                if (resident.Data.Services.RoomRentalCost > 0)
                {
                    obj = Instantiate(DialogOptionPrefab, DialogAnchor);

                    UI = obj.GetComponent<DialogOptionButton>();
                    UI.Setup("Rent Room for " + resident.Data.Services.RoomRentalCost + " gold", BuyRoom);
                    DialogOptions.Add(UI);
                }
                if (resident.Data.Services.FoodCost > 0)
                {
                    obj = Instantiate(DialogOptionPrefab, DialogAnchor);

                    UI = obj.GetComponent<DialogOptionButton>();
                    UI.Setup("Fill Packs to " + resident.Data.Services.FoodQuantity + " days for " + _currentResident.Data.Services.FoodCost + " gold", BuyFood);
                    DialogOptions.Add(UI);
                }
                if (resident.Data.Services.DrinkCost > 0)
                {
                    obj = Instantiate(DialogOptionPrefab, DialogAnchor);

                    UI = obj.GetComponent<DialogOptionButton>();
                    UI.Setup("Have a Drink", BuyDrink);
                    DialogOptions.Add(UI);
                }
                if (resident.Data.Services.TipCost > 0)
                {
                    obj = Instantiate(DialogOptionPrefab, DialogAnchor);

                    UI = obj.GetComponent<DialogOptionButton>();
                    UI.Setup("Tip Barkeep", BuyTip);
                    DialogOptions.Add(UI);
                }
            }
        }

        StaggerOptions();
    }

    void OnMemberChanged()
    {
        if (_currentResident != null)
        {
            if(_advanceStepCounter < 0)
                SelectResident(_currentResident);
            else
            {
                AttemptAdvanceStep(_advanceStepCounter);
            }
        }
    }

    void SetupBounty()
    {
        if (_currentResident.BountyID == "complete")
        {
            DisplayDialog("Someone has already claimed the bounty this month. Come back next month for a new bounty.");
        } 
        else
        {
            EnemyData data = EnemyDatabase.Instance.GetEnemyData(_currentResident.BountyID);

            if(Party.Instance.HasKilled(data.ID))
            {
                DisplayDialog("Congratulations on defeating the " + data.DisplayName + "! Here is the " + data.Level * 100 + " gold reward. Come back next month for a new bounty.");
                Party.Instance.CollectGold(data.Level * 100);
                _currentResident.CompleteBounty();
            }
            else
            {
                DisplayDialog("This month's bounty is on a " + data.DisplayName + ". Kill it and return before the end of the month to collect the " + data.Level * 100 + " gold reward");
            }
        }

        _advanceStepCounter = 1;

        foreach (var dialog in DialogOptions)
            Destroy(dialog.gameObject);
        DialogOptions.Clear();
    }

    void OfferHealingToActiveMember()
    {
        int price = Party.Instance.ActiveMember.PriceOfHealing();
        if (price > 0)
        {
            GameObject obj = Instantiate(DialogOptionPrefab, DialogAnchor);

            OptionButton UI = obj.GetComponent<DialogOptionButton>();
            UI.Setup("Heal " + price + " Gold", TryHeal);
            DialogOptions.Add(UI);
        }
    }

    void OfferSkillsToActiveMember()
    {
        List<Skill> available = Party.Instance.ActiveMember.DetermineLearnableSkills(_currentResident.Data.Services.Skills);

        if(available.Count == 0)
        {
            GameObject obj = Instantiate(DialogOptionPrefab, DialogAnchor);

            OptionButton UI = obj.GetComponent<DialogOptionButton>();
            UI.Setup("Sorry but there is nothing more that I can teach you.", null);
            DialogOptions.Add(UI);
        }
        else
        {
            GameObject obj = Instantiate(DialogOptionPrefab, DialogAnchor);

            OptionButton UI = obj.GetComponent<DialogOptionButton>();
            UI.Setup("Skill Cost: " + _currentResident.Data.Services.SkillCost, null);
            DialogOptions.Add(UI);

            for (int i=0;i<available.Count;i++)
            {
                obj = Instantiate(DialogOptionPrefab, DialogAnchor);

                UI = obj.GetComponent<DialogOptionButton>();
                UI.Setup(available[i].DisplayName, i, LearnSkill);
                DialogOptions.Add(UI);
            }
        }

        StaggerOptions();
    }

    void OfferTrainingToActiveMember()
    {
        int level = Party.Instance.ActiveMember.Profile.Level;
        if(level >= _currentResident.Data.Services.MaxTrainingLevel)
        {
            GameObject obj = Instantiate(DialogOptionPrefab, DialogAnchor);

            OptionButton UI = obj.GetComponent<DialogOptionButton>();
            UI.Setup("Sorry but your training surpasses mine.", null);
            DialogOptions.Add(UI);
        }
        else
        {
            bool canTrain = Party.Instance.ActiveMember.Profile.CanTrainLevel();
            if(canTrain)
            {
                GameObject obj = Instantiate(DialogOptionPrefab, DialogAnchor);

                OptionButton UI = obj.GetComponent<DialogOptionButton>();
                UI.Setup("Train to level " + (Party.Instance.ActiveMember.Profile.Level + 1) + " for " + _currentResident.Data.Services.TrainingCost + " gold", TrainLevel);
                DialogOptions.Add(UI);
            }
            else
            {
                GameObject obj = Instantiate(DialogOptionPrefab, DialogAnchor);

                OptionButton UI = obj.GetComponent<DialogOptionButton>();
                UI.Setup(Party.Instance.ActiveMember.Profile.GetTrainingDetails(), null);
                DialogOptions.Add(UI);
            }
        }

        StaggerOptions();
    }

    public void AttemptAdvanceStep(int option)
    {
        QuestLog Log = Party.Instance.QuestLog;
        DialogOption Option = _currentResident.Data.Options[option];
        DialogStep Step = Option.Steps[_currentResident.OptionProgress[option]];

        if(Step.MembershipOffer)
        {
            DisplayDialog(Step.FirstDialog);

            foreach (var dialog in DialogOptions)
                Destroy(dialog.gameObject);
            DialogOptions.Clear();

            GameObject obj = Instantiate(DialogOptionPrefab, DialogAnchor);

            OptionButton UI = obj.GetComponent<DialogOptionButton>();
            UI.Setup("Join " + Option.Membership.GuildName + " for " + Option.Membership.Cost + " gold", option, JoinGuild);
            DialogOptions.Add(UI);
            _advanceStepCounter = option;

            StaggerOptions();
        }
        else if (Step.ExpertiseOffer)
        {
            DisplayDialog(Step.FirstDialog);

            foreach (var dialog in DialogOptions)
                Destroy(dialog.gameObject);
            DialogOptions.Clear();

            GameObject obj = Instantiate(DialogOptionPrefab, DialogAnchor);
            OptionButton UI = obj.GetComponent<DialogOptionButton>();

            string label = "";
            ClickIndexEvent clickEvent = null;
            if(!Party.Instance.ActiveMember.Skillset.KnowsSkill(Option.Expertise.SkillID))
            {
                if(Option.Expertise.Proficiency == SkillProficiency.Expert)
                    label = "You must know the skill before you can become an expert in it!";
                else
                    label = "You must know the skill before you can become a master in it!";
            }
            else if (Party.Instance.ActiveMember.Skillset.KnowsSkillAtProficiency(Option.Expertise.SkillID, Option.Expertise.Proficiency))
            {
                if (Option.Expertise.Proficiency == SkillProficiency.Expert)
                    label = "You are already an expert in this skill.";
                else
                    label = "You are already a master in this skill.";
            }
            else if (Party.Instance.ActiveMember.Skillset.CanTrain(Option.Expertise.SkillID, Option.Expertise.Proficiency))
            {
                label = "Become " + Option.Expertise.Proficiency.ToString() + " in " + SkillDatabase.Instance.GetSkill(Option.Expertise.SkillID).DisplayName + " for " + Option.Expertise.Cost + " gold";
                clickEvent = TrainSkill;
            }
            else
            {
                label = "You do not meet the requirements and cannot be taught until you do.";
            }

            UI.Setup(label, option, clickEvent);
            DialogOptions.Add(UI);
            _advanceStepCounter = option;

            StaggerOptions();
        }
        else if(Option.Steps.Count == 1)
        {
            DisplayDialog(Step.FirstDialog);
        }
        else
        {
            if (!string.IsNullOrEmpty(Step.ItemRequired))
            {
                if (!Party.Instance.CheckForItem(Step.ItemRequired))
                {
                    DisplayDialog(Step.NoRequiredItemDialog);
                    return;
                }
            }

            if (Step.StartQuest)
                Log.AcceptQuest(Option.QuestLine);
            else if (Step.ProgressQuest)
                Log.ProgressQuest(Option.QuestLine);
            else if (Step.CompleteQuest)
                Log.CompleteQuest(Option.QuestLine);

            if(!string.IsNullOrEmpty(Step.ItemReceived))
            {
                InventoryItem item = ItemDatabase.Instance.GetInventoryItem(Step.ItemReceived);
                HUD.Instance.GiveHoldItem(item);
            }

            DisplayDialog(Step.FirstDialog);
            _currentResident.ProgressOption(option);
        }
    }

    public void LearnSkill(int option)
    {
        bool success = Party.Instance.TryPay(_currentResident.Data.Services.SkillCost);

        if (success)
        {
            List<Skill> available = Party.Instance.ActiveMember.DetermineLearnableSkills(_currentResident.Data.Services.Skills);
            Party.Instance.ActiveMember.Skillset.LearnSkill(available[option]);

            HUD.Instance.ExpressSelectedMember(GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);

            SelectResident(_currentResident);
        }
    }

    public void JoinGuild(int option)
    {
        bool success = Party.Instance.TryPay(_currentResident.Data.Options[option].Membership.Cost);

        if (success)
        {
            Party.Instance.QuestLog.JoinGuild(_currentResident.Data.Options[option].Membership.GuildID);

            DisplayDialog("");
            HUD.Instance.ExpressMembers(GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);

            SelectResident(_currentResident);
        }
    }

    public void TrainSkill(int option)
    {
        bool success = Party.Instance.TryPay(_currentResident.Data.Options[option].Expertise.Cost);

        if (success)
        {
            Party.Instance.ActiveMember.Skillset.TrainSkill(_currentResident.Data.Options[option].Expertise.SkillID);

            DisplayDialog("");
            HUD.Instance.ExpressSelectedMember(GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);

            SelectResident(_currentResident);
        }
    }

    public void TryHeal()
    {
        int price = Mathf.RoundToInt(Party.Instance.ActiveMember.PriceOfHealing() * _currentResident.Data.Services.HealingCostMultiplier);
        bool success = Party.Instance.TryPay(price);
        if(success)
        {
            Party.Instance.ActiveMember.FullHeal();
            HUD.Instance.UpdateDisplay();

            SelectResident(_currentResident);
        }
    }

    public void Donate()
    {
        bool success = Party.Instance.TryPay(Mathf.RoundToInt(10f * _currentResident.Data.Services.HealingCostMultiplier));
        
        if(success)
        {
            HUD.Instance.SendInfoMessage("Thanks!", 2.0f);
        }
    }

    public void TrainLevel()
    {
        bool success = Party.Instance.TryPay(_currentResident.Data.Services.TrainingCost * (Party.Instance.ActiveMember.Profile.Level + 1));

        if (success)
        {
            foreach (var member in Party.Instance.Members)
                member.Rest(0);

            Party.Instance.ActiveMember.Profile.TrainLevel();
            Party.Instance.ActiveMember.Vitals.ForceFullHeal();

            _hasTrained = true;

            DisplayDialog("");
            HUD.Instance.ExpressMembers(GameConstants.EXPRESSION_HAPPY, GameConstants.EXPRESSION_HAPPY_DURATION);

            SelectResident(_currentResident);
        }
    }

    public void StartDeposit()
    {
        foreach (var option in DialogOptions)
            Destroy(option.gameObject);
        DialogOptions.Clear();

        GameObject obj = Instantiate(InputOptionPrefab, DialogAnchor);

        OptionButton UI = obj.GetComponent<InputOptionButton>();
        UI.SetupInput("Deposit\nHow Much?", CommitDeposit);
        DialogOptions.Add(UI);

        obj = Instantiate(DialogOptionPrefab, DialogAnchor);

        UI = obj.GetComponent<DialogOptionButton>();
        UI.Setup("Balance: " + Party.Instance.CurrentBalance, null);
        DialogOptions.Add(UI);

        StaggerOptions();
    }

    public void CommitDeposit(int value)
    {
        Party.Instance.Deposit(value);
        HUD.Instance.UpdateDisplay();
        HUD.Instance.ExpressSelectedMember(GameConstants.EXPRESSION_UNSURE, GameConstants.EXPRESSION_UNSURE_DURATION);

        SelectResident(_currentResident);
    }

    public void StartWithdraw()
    {
        foreach (var option in DialogOptions)
            Destroy(option.gameObject);
        DialogOptions.Clear();

        GameObject obj = Instantiate(InputOptionPrefab, DialogAnchor);

        OptionButton UI = obj.GetComponent<InputOptionButton>();
        UI.SetupInput("Withdraw\nHow Much?", CommitWithdraw);
        DialogOptions.Add(UI);

        obj = Instantiate(DialogOptionPrefab, DialogAnchor);

        UI = obj.GetComponent<DialogOptionButton>();
        UI.Setup("Balance: " + Party.Instance.CurrentBalance, null);
        DialogOptions.Add(UI);

        StaggerOptions();
    }

    public void CommitWithdraw(int value)
    {
        Party.Instance.Withdraw(value);
        HUD.Instance.UpdateDisplay();

        SelectResident(_currentResident);
    }

    public void BuyRoom()
    {
        if (Party.Instance.TryPay(_currentResident.Data.Services.RoomRentalCost))
        {
            CloseMenu();
            MenuManager.Instance.OpenMenu("Rest", true);

            RestMenu menu = MenuManager.Instance.GetMenu("Rest") as RestMenu;
            menu.OnRestUntilDawn();
        }
    }

    public void BuyFood()
    {
        if(Party.Instance.CurrentFood >= _currentResident.Data.Services.FoodQuantity)
        {
            DisplayDialog("");
            HUD.Instance.SendInfoMessage("Your packs are already full!", 2.0f);
        }
        else
        {
            if (Party.Instance.TryPay(_currentResident.Data.Services.FoodCost))
            {
                DisplayDialog("");
                Party.Instance.FillFoodTo(_currentResident.Data.Services.FoodQuantity);
            }
        }
    }

    public void BuyDrink()
    {
        if (Party.Instance.TryPay(_currentResident.Data.Services.DrinkCost))
        {
            DisplayDialog("");
            HUD.Instance.SendInfoMessage("Hic...", 2.0f);
            _hasAchievedSpecialCondition = true;
        }
    }

    public void BuyTip()
    {
        if(_hasAchievedSpecialCondition)
        {
            if (Party.Instance.TryPay(_currentResident.Data.Services.TipCost))
            {
                DisplayDialog(_currentResident.Data.Services.Tips[0]);
            }
        }
        else
        {
            DisplayDialog("");
            HUD.Instance.SendInfoMessage("Have a Drink first...", 2.0f);
        }
    }

    public override void DisplayDialog(string dialog)
    {
        Dialog.text = dialog;
        if(string.IsNullOrEmpty(dialog))
            DialogBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
        else
            DialogBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Dialog.preferredHeight + 10);
    }

    public void HoverBack()
    {
        HUD.Instance.SendInfoMessage("End Conversation");
    }

    public void Back()
    {
        if(_advanceStepCounter >= 0 && _currentResident != null)
        {
            SelectResident(_currentResident);
        }
        else if(ResidentDialogObject.gameObject.activeSelf && _currentResidency.Residents.Count > 1)
        {
            ResidentsAnchor.gameObject.SetActive(true);
            ResidentDialogObject.gameObject.SetActive(false);

            foreach (var option in DialogOptions)
                Destroy(option.gameObject);

            _currentResident = null;

            DialogOptions.Clear();
            DialogBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
        }
        else
        {
            CloseMenu();
        }
    }

    public override void OnClose()
    {
        base.OnClose();

        foreach (var option in DialogOptions)
            Destroy(option.gameObject);
        DialogOptions.Clear();

        foreach (var option in Options)
            Destroy(option.gameObject);
        Options.Clear();

        if(_hasTrained)
        {
            System.DateTime dt = TimeManagement.Instance.GetDT();
            System.DateTime adjusted = dt.AddHours(15);

            float duration = 24 * 7 * 60 + 60 * 24 - (adjusted.Minute + adjusted.Hour * 60);
            TimeManagement.Instance.ProgressInstant(duration);
            foreach (var member in Party.Instance.Members)
            {
                member.Status.RemoveCondition(StatusEffectOption.Weak);
                member.Status.AddCondition(StatusEffectOption.Rested, GameConstants.REST_DURATION);
            }
            HUD.Instance.UpdateDisplay();

            _hasTrained = false;
        }

        HUD.Instance.EnableSideMenu();
        Party.MemberChanged -= OnMemberChanged;
        SoundManager.Instance.SetMusicVolume(1f);
        SoundManager.Instance.PlayUISound("Close");
    }
}
