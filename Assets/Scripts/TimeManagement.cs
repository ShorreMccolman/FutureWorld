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
    public TickEvent OnCombatTick;

    TimeControl _control;

    Party _party;

    System.DateTime _dt;

    float _manualDuration;
    float _combatDuration;
    bool _isComitting;
    float _timeToUpdate;
    bool _enemiesMoving;

    public static bool IsCombatMode;
    public MemberPriority CombatPriority;

    List<CombatEntity> ActiveEnemies;
    List<CombatEntity> ReadyMembers;

    public void StartTiming(Party party)
    {
        _party = party;
        _control = TimeControl.Auto;
        _isComitting = false;

        ReadyMembers = new List<CombatEntity>();

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
        else if (_control == TimeControl.Combat)
        {
            if(_enemiesMoving)
            {
                _enemiesMoving = false;
                List<CombatEntity> finished = new List<CombatEntity>();
                foreach(var enemy in ActiveEnemies)
                {
                    if(enemy.MoveCooldown <= 0)
                    {
                        finished.Add(enemy);
                    }
                }
                foreach (var enemy in finished)
                    ActiveEnemies.Remove(enemy);

                if (ActiveEnemies.Count > 0)
                    _enemiesMoving = true;
                else
                    CombatStep();
            }
            else if (_combatDuration > 0)
            {
                OnTick?.Invoke(_combatDuration);
                _combatDuration = 0;
                CombatStep();
            }
        }
    }

    public void ToggleCombatMode()
    {
        if(IsCombatMode)
        {
            SetTimeControl(TimeControl.Auto);
        } else
        {
            SetTimeControl(TimeControl.Combat);
        }
        HUD.Instance.UpdateDisplay();
    }

    public void SetTimeControl(TimeControl control)
    {
        _control = control;
        IsCombatMode = control == TimeControl.Combat;

        if(IsCombatMode)
        {
            CombatPriority = new MemberPriority();

            foreach(var member in PartyController.Instance.Members)
                CombatPriority.Add(member);

            foreach (var enemy in PartyController.Instance.GetActiveEnemies())
                CombatPriority.Add(enemy);

            CombatStep();
        }
    }

    public void EntityAttack(CombatEntity attacker)
    {
        ReadyMembers.Remove(attacker);
        CombatStep();
    }

    void CombatStep()
    {
        if(ReadyMembers.Count > 0)
        {
            return;
        }

        ActiveEnemies = new List<CombatEntity>();
        ReadyMembers = new List<CombatEntity>();

        float cooldown;
        while(CombatPriority.NextCooldown(out cooldown))
        {
            CombatEntity next = CombatPriority.Get();
            if(next is Enemy)
            {
                ActiveEnemies.Add(next);
            } 
            else
            {
                ReadyMembers.Add(next);
            }
        }

        if (ActiveEnemies.Count > 0)
        {
            _enemiesMoving = true;
            foreach (var enemy in ActiveEnemies)
            {
                enemy.CombatStep();
            }
        } 
        else if (ReadyMembers.Count > 0)
        {

        } 
        else
        {
            _combatDuration = cooldown;
        }
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
