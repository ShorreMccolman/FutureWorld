using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCMenu : ConversationMenu
{
    [SerializeField] Text NPCLabel;
    [SerializeField] Image NPCPortrait;
    [SerializeField] Transform ResidentDialogObject;
    [SerializeField] RectTransform DialogBox;
    [SerializeField] Text Dialog;

    NPC _currentNPC;
    int _advanceStepCounter;
    bool _isHire;

    protected override void Init()
    {
        NPC.OnNPCConverse += Setup;

        base.Init();
    }

    public void Setup(NPC npc, bool isHire)
    {
        MenuManager.Instance.OpenMenu(MenuTag, false, true);

        _currentNPC = npc;

        DialogOptions = new List<OptionButton>();

        NPCLabel.text = _currentNPC.DisplayName;
        NPCPortrait.sprite = _currentNPC.Portrait;

        _advanceStepCounter = -1;
        _isHire = isHire;

        DialogOptions = new List<OptionButton>();
        ShowMainOptions();

        DisplayDialog("Hello there! A mighty fine morning, if you ask me! I'm " + _currentNPC.Name + ".");
    }

    void ShowMainOptions()
    {
        foreach (var option in DialogOptions)
            Destroy(option.gameObject);
        DialogOptions.Clear();

        AddButton(_currentNPC.Topics[0].Header, 0, DisplayTopic);

        if (_isHire)
        {
            AddButton("Dismiss " + _currentNPC.Name, Dismiss, true);
        }
        else
        {
            AddButton("Join", ShowJoin, true);
        }

        AddButton(_currentNPC.Topics[1].Header, 1, DisplayTopic);

        StaggerOptions();
    }

    void DisplayTopic(int index)
    {
        DisplayDialog(_currentNPC.Topics[index].Body);
    }

    void ShowJoin()
    {
        _advanceStepCounter = 0;

        foreach (var option in DialogOptions)
            Destroy(option.gameObject);
        DialogOptions.Clear();

        AddButton("More Information", ShowInfo, true);
        AddButton("Hire", Hire, true);

        StaggerOptions();

        DisplayDialog(_currentNPC.JoinText);
    }

    void ShowInfo()
    {
        DisplayDialog(_currentNPC.ActionText);
    }

    void Hire()
    {
        bool success = Party.Instance.TryHire(_currentNPC);
        if (success)
        {
            _currentNPC.Parent.Entity.Kill();

            Back();
        }
        else
        {
            DisplayDialog(_currentNPC.JoinText);
        }
    }

    void Dismiss()
    {
        bool success = Party.Instance.DismissHire(_currentNPC);
        if (success)
            Back();
    }

    public override void DisplayDialog(string dialog)
    {
        Dialog.text = dialog;
        if (string.IsNullOrEmpty(dialog))
            DialogBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
        else
            DialogBox.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Dialog.preferredHeight + 10);
    }

    public void Back()
    {
        foreach (var option in DialogOptions)
            Destroy(option.gameObject);
        DialogOptions.Clear();

        CloseMenu();
    }
}
