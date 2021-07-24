using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TimeControl
{
    Auto,
    Manual
}

public class TimeManagement : MonoBehaviour
{
    public static TimeManagement Instance;
    void Awake()
    { Instance = this; }

    [SerializeField] Transform LightAnchor;
    [SerializeField] Light DayLight;
    [SerializeField] Light NightLight;
    [SerializeField] Material DaySky;
    [SerializeField] Material NightSky;

    [SerializeField] Texture2D TargetCursorTex;

    event System.Action OnFinish;
    public static event System.Action<float> OnTick;

    public static event System.Action<TimeControl> OnControlChanged;
    public static event System.Action<bool> OnTimeFreezeChanged;

    TimeControl _control;
    Party _party;
    System.DateTime _dt;

    float _manualDuration;
    bool _isComitting;

    float _manualPace = 6 * 60 * 60;

    Dictionary<Animator, float> _animationSpeedDict = new Dictionary<Animator, float>();

    public void StartTiming(Party party)
    {
        _party = party;
        _control = TimeControl.Auto;
        _isComitting = false;

        InitEnvironment(party.CurrentTime);
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

                float tick = Time.deltaTime * _manualPace;
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
        if (control == _control)
            return;

        _control = control;
        OnControlChanged?.Invoke(control);
    }

    public void ProgressManuallySeconds(float seconds, System.Action finishEvent = null)
    {
        _manualPace = 2 * 30;
        if (seconds <= 0)
        {
            finishEvent?.Invoke();
            return;
        }

        _manualDuration = seconds;
        OnFinish = finishEvent;
    }

    public void ProgressManually(float minutes, System.Action finishEvent = null)
    {
        _manualPace = 6 * 60 * 60;
        if (minutes <= 0)
        {
            finishEvent?.Invoke();
            return;
        }

        _manualDuration = minutes * 60;
        OnFinish = finishEvent;
    }

    public void ProgressInstant(float minutes)
    {
        OnTick?.Invoke(minutes * 60);
    }

    void InitEnvironment(float time)
    {
        System.DateTime dt = TimeManagement.Instance.GetDT(time);

        float rot = ((float)dt.TimeOfDay.TotalSeconds - 60f * 60f * 13f) * 360f / (24f * 60f * 60f);

        LightAnchor.eulerAngles = new Vector3(45, 0, rot);

        //bool isDay = dt.Hour > 6 && dt.Hour < 21;
        //Light light = isDay ? DayLight : NightLight;
        //Material box = isDay ? DayBox : NightBox;
        //if (RenderSettings.sun != light)
        //{
        //    RenderSettings.sun = light;
        //    RenderSettings.skybox = box;
        //    RenderSettings.ambientIntensity = isDay ? 0.8f : 0.5f;
        //}

        OnTick += UpdateEnvironment;
    }

    void UpdateEnvironment(float tick)
    {
        float hour = GetCurrentHourFractional();

        bool isDay = hour > 5 && hour < 21;
        Material box = isDay ? DaySky : NightSky;
        if (RenderSettings.skybox != box)
        {
            RenderSettings.skybox = box;
        }

        float rot = tick * 360f / (24f * 60f * 60f);
        LightAnchor.Rotate(new Vector3(0, 0, rot));
    }

    public void CommitManualTick()
    {
        if (_manualDuration > 0)
            _isComitting = true;
        else
            _control = TimeControl.Auto;
    }

    public bool ShouldRefresh(RefreshPeriod period, float lastUpdate, out float currentTime)
    {
        System.DateTime date = GetDT();
        System.DateTime update = TimeManagement.Instance.GetDT(lastUpdate);

        bool restock = false;
        switch (period)
        {
            case RefreshPeriod.Daily:
                restock = date.Year > update.Year || date.DayOfYear > update.DayOfYear;
                break;
            case RefreshPeriod.Even:
                if (date.DayOfYear % 2 == 0)
                {
                    restock = date.DayOfYear - update.DayOfYear > 1 || date.Year > update.Year;
                }
                else
                {
                    restock = date.DayOfYear > update.DayOfYear || date.Year > update.Year;
                }
                break;
            case RefreshPeriod.Weekly:
                int difference = 7 - (int)date.DayOfWeek;
                restock = date.DayOfYear - update.DayOfYear >= difference;
                break;
            case RefreshPeriod.Monthly:
                restock = date.Year > update.Year || date.Month > update.Month;
                break;
            case RefreshPeriod.Yearly:
                restock = date.Year > update.Year;
                break;
            case RefreshPeriod.YearToDate:
                System.TimeSpan span = new System.TimeSpan(date.Ticks - update.Ticks);
                restock = span.Days >= 365;
                break;
            case RefreshPeriod.Permanent:
                restock = false;
                break;
        }

        currentTime = _party.CurrentTime;
        return restock;
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

    public System.DateTime GetDT(float time)
    {
        System.DateTime date = new System.DateTime();
        date = date.AddSeconds(time);
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

    public void FreezeTime()
    {
        Animator[] anims = FindObjectsOfType<Animator>();
        foreach (var anim in anims)
        {
            _animationSpeedDict.Add(anim, anim.speed);
            anim.speed = 0f;
        }
        Time.timeScale = 0f;
        OnTimeFreezeChanged?.Invoke(true);

        Cursor.SetCursor(TargetCursorTex, new Vector2(32f, 32f), CursorMode.Auto);
    }

    public void UnfreezeTime()
    {
        foreach (var anim in _animationSpeedDict.Keys)
        {
            anim.speed = _animationSpeedDict[anim];
        }
        _animationSpeedDict.Clear();
        Time.timeScale = 1f;
        OnTimeFreezeChanged?.Invoke(false);
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
