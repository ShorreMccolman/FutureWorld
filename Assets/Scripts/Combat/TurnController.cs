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
    CombatEntity _activeCombatEntity;

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
            MenuManager.OnMenusClosed += ResetActiveMember;

            _turnEntities = new List<CombatEntity>();
            StartCoroutine(Combat());
        }
        else
        {
            TimeManagement.Instance.SetTimeControl(TimeControl.Auto);
            Vitals.OnMemberKnockout -= OnKnockout;
            EnemyEntity.OnEnemyDeath -= OnKnockout;
            MenuManager.OnMenusClosed -= ResetActiveMember;

            StopAllCoroutines();
        }
    }

    void ResetActiveMember()
    {
        Party.Instance.SetActiveMember(_activeCombatEntity as PartyMember);
    }

    void OnKnockout(CombatEntity entity)
    {
        _combatPriority.Flush(entity);
    }

    void DetermineCombatPriority()
    {
        _combatPriority = new CombatPriority();
        _turnEntities = new List<CombatEntity>();

        foreach(var member in Party.Instance.Members)
            if(member.IsConcious())
                _turnEntities.Add(member);

        _turnEntities.AddRange(PartyController.Instance.GetActiveEnemies());

        foreach (var ent in _turnEntities)
        {
            _combatPriority.Add(ent);
        }
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
            if (_turnEntities.Count == 0)
            {
                DetermineCombatPriority();

                if (_turnEntities.Find(x => x is Enemy) != null)
                {
                    OnEnemyMoveToggled?.Invoke(true);
                    yield return new WaitForSeconds(2.0f);
                    OnEnemyMoveToggled?.Invoke(false);
                }
            }

            _activeCombatEntity = _combatPriority.Get();
            _waiting = true;
            TimeManagement.Instance.ProgressManuallySeconds(_activeCombatEntity.GetCooldown(), FinishWaiting);
            while(_waiting)
                yield return null;

            _activeCombatEntity.ActivateTurn();
            while(_activeCombatEntity.WaitForTurn() && _activeCombatEntity.IsConcious())
            {
                yield return null;
            }

            if (_turnEntities.Contains(_activeCombatEntity))
                _turnEntities.Remove(_activeCombatEntity);
            _combatPriority.Add(_activeCombatEntity);

            yield return null;
        }
    }
}
