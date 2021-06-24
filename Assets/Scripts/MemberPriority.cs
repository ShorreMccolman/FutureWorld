using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemberPriority
{
    List<CombatEntity> _queue;

    public MemberPriority()
    {
        _queue = new List<CombatEntity>();
    }

    public bool IsReady()
    { return _queue.Count > 0; }

    public void Add(CombatEntity member)
    {
        if (_queue.Contains(member))
            _queue.Remove(member);

        int index = 0;
        for (int i = 0; i < index; i++)
        {
            if (member.GetCooldown() > _queue[i].GetCooldown())
                index = i;
        }
        _queue.Insert(index, member);
    }

    public void Add(MemberPriority priority)
    {
        foreach (var member in priority._queue)
        {
            Add(member);
        }
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

        CombatEntity member = _queue[_queue.Count - 1];
        _queue.Remove(member);
        return member;
    }

    public void Flush(PartyMember member)
    {
        if (_queue.Contains(member))
            _queue.Remove(member);
    }
}