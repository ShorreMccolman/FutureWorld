using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    public static TurnController Instance;
    void Awake()
    { Instance = this; }

    bool _waiting;

    bool _isTurnBased;
    public bool IsTurnBasedEnabled => _isTurnBased;

    CombatPriority _combatPriority;
    List<CombatEntity> _turnEntities;

    public static event System.Action<bool> OnTurnBasedToggled;
    public static event System.Action<bool> OnEnemyMoveToggled;

    public void ToggleCombatMode()
    {
        _isTurnBased = !_isTurnBased;
        OnTurnBasedToggled?.Invoke(_isTurnBased);
        if (_isTurnBased)
        {
            TimeManagement.Instance.SetTimeControl(TimeControl.Manual);
            Vitals.OnMemberKnockout += OnKnockout;
            EnemyEntity.OnEnemyDeath += OnKnockout;

            DetermineCombatPriority();
        }
        else
        {
            TimeManagement.Instance.SetTimeControl(TimeControl.Auto);
            Vitals.OnMemberKnockout -= OnKnockout;
            EnemyEntity.OnEnemyDeath -= OnKnockout;

            StopAllCoroutines();
        }
    }

    void OnKnockout(CombatEntity entity)
    {
        _combatPriority.Flush(entity);
    }

    void DetermineCombatPriority()
    {
        _combatPriority = new CombatPriority();
        _turnEntities = new List<CombatEntity>();

        while(Party.Instance.Priority.IsReady())
            _turnEntities.Add(Party.Instance.Priority.Get());

        foreach(var member in Party.Instance.Members)
            if(!_turnEntities.Contains(member))
                _turnEntities.Add(member);

        _turnEntities.AddRange(PartyController.Instance.GetActiveEnemies());

        foreach (var ent in _turnEntities)
        {
            _combatPriority.Add(ent);
        }

        StartCoroutine(Combat());
    }

    void PartyMemberRevived(PartyMember member)
    {
        _combatPriority.Add(member);
    }

    void FinishWaiting()
    {
        _waiting = false;
    }

    IEnumerator Combat()
    {
        while(true)
        {
            CombatEntity next = _combatPriority.Get();
            _waiting = true;
            TimeManagement.Instance.ProgressManuallySeconds(next.GetCooldown(), FinishWaiting);
            while(_waiting)
                yield return null;

            next.ActivateTurn();
            while(next.WaitForTurn() && next.IsAlive())
            {
                yield return null;
            }
            Party.Instance.SetActiveMember(null);

            if (_turnEntities.Contains(next))
                _turnEntities.Remove(next);
            _combatPriority.Add(next);

            if(_turnEntities.Count == 0)
            {
                OnEnemyMoveToggled?.Invoke(true);
                yield return new WaitForSeconds(2.0f);
                OnEnemyMoveToggled?.Invoke(false);
                _turnEntities.AddRange(_combatPriority.Participants);
            }
        }
    }
}
