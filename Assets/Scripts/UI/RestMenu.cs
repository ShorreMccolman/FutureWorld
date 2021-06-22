using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestMenu : Menu
{
    [SerializeField] Text Cost;
    [SerializeField] Text Time;
    [SerializeField] Text Day;
    [SerializeField] Text Month;
    [SerializeField] Text Year;

    bool isResting;

    public override void OnOpen()
    {
        isResting = false;
        UpdateDisplay();
        TimeManagement.Instance.OnTick += Tick;
        TimeManagement.Instance.SetTimeControl(TimeControl.Manual);
    }

    void Tick(float tick)
    {
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        System.DateTime dt = TimeManagement.Instance.GetDT();

        Cost.text = "1";
        Time.text = TimeManagement.Instance.GetTime(dt);
        Day.text = dt.Day.ToString();
        Month.text = dt.Month.ToString();
        Year.text = dt.Year.ToString();
    }

    public void OnRest()
    {
        if (isResting)
        {
            HUD.Instance.SendInfoMessage("You are already resting!", 2.0f);
            return;
        }

        bool success = Party.Instance.TryEat(1);

        if (success)
        {
            foreach (var member in Party.Instance.Members)
            {
                member.Rest(60 * 8);
            }

            isResting = true;
            TimeManagement.Instance.ProgressManually(60 * 8, OnFinishRest);
        }
    }

    public void OnRestUntilDawn()
    {
        System.DateTime dt = TimeManagement.Instance.GetDT();
        System.DateTime adjusted = dt.AddHours(19);

        float duration = 60 * 24 - (adjusted.Minute + adjusted.Hour * 60);

        foreach (var member in Party.Instance.Members)
        {
            member.Rest(duration);
        }

        isResting = true;
        TimeManagement.Instance.ProgressManually(duration, OnFinishRest);
    }

    public void OnFinishRest()
    {
        isResting = false;
        CloseMenu();
    }

    public void OnWaitUntilDawn()
    {
        if (isResting)
        {
            HUD.Instance.SendInfoMessage("You are already resting!", 2.0f);
            return;
        }

        System.DateTime dt = TimeManagement.Instance.GetDT();
        System.DateTime adjusted = dt.AddHours(19);

        float duration = 60 * 24 - (adjusted.Minute + adjusted.Hour * 60);
        TimeManagement.Instance.ProgressManually(duration);
    }

    public void OnWaitHour()
    {
        if (isResting)
        {
            HUD.Instance.SendInfoMessage("You are already resting!", 2.0f);
            return;
        }

        TimeManagement.Instance.ProgressManually(60);
    }

    public void OnWaitMinutes()
    {
        if (isResting)
        {
            HUD.Instance.SendInfoMessage("You are already resting!", 2.0f);
            return;
        }

        TimeManagement.Instance.ProgressManually(5);
    }

    public override void OnClose()
    {
        PartyController.Instance.SetControlState(ControlState.Previous);
        HUD.Instance.EnableSideMenu();
        TimeManagement.Instance.OnTick -= Tick;
        TimeManagement.Instance.CommitManualTick();
    }
}
