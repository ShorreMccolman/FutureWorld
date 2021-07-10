using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterVitalsDisplay : MonoBehaviour {

    [SerializeField] Image Portrait;
    [SerializeField] Image ReadyIndicator;
    [SerializeField] Image SelectionIndicator;
    [SerializeField] Slider HealthSlider;
    [SerializeField] Image HealthFill;
    [SerializeField] Slider ManaSlider;
    [SerializeField] Image ManaFill;

    public PartyMember Member { get; private set; }

    public Sprite Sprite => Portrait.sprite;

    bool _clicking;

    public void Init(PartyMember member)
    {
        Member = member;

        member.Vitals.OnConditionChange += UpdateStatus;
        member.Vitals.OnExpressionChange += UpdateExpression;
        member.Vitals.OnHealthChange += UpdateHP;
        member.Vitals.OnManaChange += UpdateSP;
        Party.OnMemberChanged += IndicateSelection;

        UpdateSP(member.Vitals.CurrentHP, member.Vitals.Stats.EffectiveTotalHP);
        UpdateSP(member.Vitals.CurrentSP, member.Vitals.Stats.EffectiveTotalSP);
        UpdateExpression(member.Vitals.EffectiveExpression);
        UpdateStatus(member.Vitals.IsReady());
        IndicateSelection(Party.Instance.ActiveMember);

        PartyController.OnNearbyEnemiesChanged += NearbyEnemiesChanged;
    }

    private void UpdateStatus(bool isReady, PartyMemberState state = PartyMemberState.Concious)
    {
        Color color = Color.green;
        if (PartyController.Instance.AreEnemiesInRange())
            color = Color.red;
        else if (PartyController.Instance.AreEnemiesInArea())
            color = Color.yellow;

        ReadyIndicator.color = isReady ? color : Color.grey;
    }

    private void NearbyEnemiesChanged()
    {
        UpdateStatus(Member.Vitals.IsReady(), Member.Vitals.Condition);
    }

    void UpdateSP(int sp, int max)
    {
        float mana = (float)sp / (float)max;
        ManaSlider.value = mana;
        if (mana <= 0f)
            ManaFill.color = Color.clear;
        else if (mana <= 0.25f)
            ManaFill.color = Color.red;
        else if (mana <= 0.5f)
            ManaFill.color = Color.yellow;
        else
            ManaFill.color = Color.blue;
    }

    void UpdateHP(int hp, int max)
    {
        float health = (float)hp / (float)max;
        HealthSlider.value = health;
        if (health <= 0f)
            HealthFill.color = Color.clear;
        else if (health <= 0.25f)
            HealthFill.color = Color.red;
        else if (health <= 0.5f)
            HealthFill.color = Color.yellow;
        else
            HealthFill.color = Color.green;
    }

    void UpdateExpression(string expression)
    {
        if (string.IsNullOrEmpty(expression))
            expression = "neutral";
        Portrait.sprite = SpriteHandler.FetchSprite("Portraits", "Face" + Member.Profile.PortraitID + "_" + expression);
    }

    public void IndicateSelection(PartyMember member)
    {
        SelectionIndicator.color = member == Member ? Color.white : Color.grey;
    }

    public void MenuClick()
    {
        HUD.Instance.SelectCharacter(Member, _clicking);

        _clicking = true;
        CancelInvoke();
        Invoke("TimeoutClick", 0.3f);
    }

    void TimeoutClick()
    {
        _clicking = false;
    }
}
