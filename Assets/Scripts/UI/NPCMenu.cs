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

    int _advanceStepCounter = -1;

    public void Setup(NPC npc)
    {
        _currentNPC = npc;

        DialogOptions = new List<OptionButton>();

        NPCLabel.text = _currentNPC.DisplayName;
        NPCPortrait.sprite = _currentNPC.Portrait;
        HUD.Instance.SendInfoMessage(_currentNPC.DisplayName);

        _advanceStepCounter = -1;

        DialogOptions = new List<OptionButton>();
        ShowMainOptions();

        DisplayDialog("Hello there! A mighty fine morning, if you ask me! I'm " + _currentNPC.Name + ".");
    }

    void ShowMainOptions()
    {
        foreach (var option in DialogOptions)
            Destroy(option.gameObject);
        DialogOptions.Clear();

        GameObject obj;
        DialogOptionButton UI;

        obj = Instantiate(DialogOptionPrefab, DialogAnchor);
        UI = obj.GetComponent<DialogOptionButton>();
        UI.Setup(_currentNPC.Topics[0].Header, 0, DisplayTopic);
        DialogOptions.Add(UI);

        obj = Instantiate(DialogOptionPrefab, DialogAnchor);
        UI = obj.GetComponent<DialogOptionButton>();
        UI.Setup("Join", ShowJoin);
        DialogOptions.Add(UI);

        obj = Instantiate(DialogOptionPrefab, DialogAnchor);
        UI = obj.GetComponent<DialogOptionButton>();
        UI.Setup(_currentNPC.Topics[1].Header, 1, DisplayTopic);
        DialogOptions.Add(UI);

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

        GameObject obj;
        DialogOptionButton UI;

        obj = Instantiate(DialogOptionPrefab, DialogAnchor);
        UI = obj.GetComponent<DialogOptionButton>();
        UI.Setup("More Information", ShowInfo);
        DialogOptions.Add(UI);

        obj = Instantiate(DialogOptionPrefab, DialogAnchor);
        UI = obj.GetComponent<DialogOptionButton>();
        UI.Setup("Hire", Hire);
        DialogOptions.Add(UI);

        StaggerOptions();

        DisplayDialog(_currentNPC.JoinText);
    }

    void ShowInfo()
    {
        DisplayDialog(_currentNPC.ActionText);
    }

    void Hire()
    {
        bool success = PartyController.Instance.Party.TryHire(_currentNPC);
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

        HUD.Instance.EnableSideMenu();
        CloseMenu();
    }
}
