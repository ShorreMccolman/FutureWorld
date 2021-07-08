using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatPriority
{
    List<CombatEntity> _queue;
    public List<CombatEntity> Participants => _queue;

    public CombatPriority()
    {
        _queue = new List<CombatEntity>();
    }

    public bool IsReady()
    { return _queue.Count > 0; }

    public void Add(CombatEntity member)
    {
        if (!member.IsAlive())
            return;

        if (_queue.Contains(member))
            return;

        for (int i = 0; i < _queue.Count; i++)
        {
            if (member.GetCooldown() < _queue[i].GetCooldown())
            {
                _queue.Insert(i, member);
                return;
            }
        }
        _queue.Add(member);
    }

    public bool NextCooldown(out float cooldown)
    {
        if (_queue.Count == 0)
        {
            cooldown = 0;
            return false;
        }

        cooldown = _queue[0].GetCooldown();
        return cooldown <= 0;
    }

    public CombatEntity Get()
    {
        if (!IsReady())
            return null;

        CombatEntity member = _queue[0];
        _queue.Remove(member);
        return member;
    }

    public void Flush(CombatEntity member)
    {
        if (_queue.Contains(member))
            _queue.Remove(member);
    }

    public void Clear()
    {
        _queue.Clear();
    }
}