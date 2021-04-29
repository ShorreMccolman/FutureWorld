using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TimeControl
{
    Auto,
    Manual,
    Combat
}

public class TimeManagement : MonoBehaviour
{
    public static TimeManagement Instance;
    void Awake()
    { Instance = this; }

    [SerializeField] Light MainLight;

    [SerializeField] Material DayBox;
    [SerializeField] Material NightBox;

    public delegate void FinishEvent();
    public FinishEvent OnFinish;
    public delegate void TickEvent(float tick);
    public TickEvent OnTick;

    TimeControl _control;

    Party _party;

    System.DateTime _dt;

    float _manualDuration;
    bool _isComitting;
    float _timeToUpdate;

    public void StartTiming(Party party)
    {
        _party = party;
        _control = TimeControl.Auto;
        _isComitting = false;

        _timeToUpdate = 0;
        OnTick += UpdateEnvironment;
    }

    void Update()
    {
        if (_control == TimeControl.Auto)
        {
            float tick = Time.deltaTime * 30.0f;

            OnTick?.Invoke(tick);
        }
        else if (_control == TimeControl.Manual)
        {
            if (_manualDuration > 0)
            {
                bool isFinished = false;

                float tick = Time.deltaTime * 6 * 60 * 60;
                if (_manualDuration - tick <= 0 || _isComitting)
                {
                    tick = _manualDuration;
                    isFinished = true;
                }

                _manualDuration -= tick;
                OnTick?.Invoke(tick);
                if (isFinished)
                {
                    OnFinish?.Invoke();
                    OnFinish = null;
                    if (_isComitting)
                        _control = TimeControl.Auto;
                    _isComitting = false;
                }
            }
        }
    }

    public void SetTimeControl(TimeControl control)
    {
        _control = control;
    }

    public void ProgressManually(float minutes, FinishEvent finishEvent = null)
    {
        _manualDuration = minutes * 60;
        OnFinish = finishEvent;
    }

    void UpdateEnvironment(float tick)
    {
        _timeToUpdate -= tick;

        if (_timeToUpdate <= 0)
        {
            float hour = GetCurrentHourFractional();

            if (hour < 3 || hour == 23)
            {
                MainLight.intensity = 0.2f;
            }
            else if (hour < 6)
            {
                MainLight.intensity = 1f - (6 - hour) * 0.8f / 3f;
            }
            else if (hour < 20)
            {
                MainLight.intensity = 1f;
            }
            else
            {
                MainLight.intensity = 0.2f + (23 - hour) * 0.8f / 3f;
            }

            Material box = hour > 4 && hour < 21 ? DayBox : NightBox;
            if (RenderSettings.skybox != box)
                RenderSettings.skybox = box;

            _timeToUpdate = 10 * 60;
        }
    }

    public void CommitManualTick()
    {
        if (_manualDuration > 0)
            _isComitting = true;
        else
            _control = TimeControl.Auto;
    }

    public string GetFullDateAndTime()
    {
        System.DateTime date = GetDT();

        int hour = date.Hour % 12;
        if (hour == 0)
            hour = 12;

        string minute = date.Minute.ToString();
        if (date.Minute < 10)
            minute = "0" + date.Minute;

        string time = hour + ":" + minute + (date.Hour < 12 ? " am" : " pm");
        string label = time + "  " + date.DayOfWeek + " " + date.Day + " " + GameConstants.monthDict[date.Month] + " " + date.Year.ToString();
        return label;
    }

    public System.DateTime GetDT()
    {
        System.DateTime date = new System.DateTime();
        date = date.AddSeconds(_party.CurrentTime);
        date = date.AddHours(9);
        date = date.AddYears(2998);
        return date;
    }

    public float GetCurrentHourFractional()
    {
        System.DateTime dt = GetDT();
        return dt.Hour + dt.Minute / 60f;
    }

    public int GetCurrentHour()
    {
        return GetDT().Hour;
    }

    public string GetTime(System.DateTime date)
    {
        int hour = date.Hour % 12;
        if (hour == 0)
            hour = 12;

        string minute = date.Minute.ToString();
        if (date.Minute < 10)
            minute = "0" + date.Minute;

        string time = hour + ":" + minute + (date.Hour < 12 ? " am" : " pm");
        return time;
    }
}
