using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterVitalsDisplay : MonoBehaviour {

    [SerializeField] Image Portrait;
    [SerializeField] Image ReadyIndicator;
    [SerializeField] Image SelectionIndicator;
    [SerializeField] Slider HealthSlider;
    [SerializeField] Slider ManaSlider;

    public PartyMember Member { get; private set; }

    public Sprite Sprite { get { return Portrait.sprite; } }

    string _currentExpression;

    bool _clicking;

    public void Init(PartyMember member)
    {
        Member = member;
        _currentExpression = "X";

        UpdateUI();
        SelectionIndicator.color = Color.grey;
    }

    public void UpdateUI()
    {
        ReadyIndicator.color = Member.Vitals.IsReady() ? Color.green : Color.yellow;
        HealthSlider.value = Member.Vitals.GetHealthRatio();
        ManaSlider.value = Member.Vitals.GetManaRatio();
        switch(Member.Vitals.Condition)
        {
            case PartyMemberState.Good:
                string expression;
                if(Member.Status.ExpressionOverride(out expression))
                {
                    if(_currentExpression != expression)
                    {
                        _currentExpression = expression;
                        Portrait.sprite = Resources.Load<Sprite>("Portraits/Face" + Member.Profile.PortraitID + "_" + expression);
                    }
                }
                else if (_currentExpression != Member.Vitals.Expression)
                {
                    _currentExpression = Member.Vitals.Expression;
                    Portrait.sprite = Resources.Load<Sprite>("Portraits/Face" + Member.Profile.PortraitID + "_" + Member.Vitals.Expression);
                }
                break;
            case PartyMemberState.Unconcious:
                if (_currentExpression != GameConstants.EXPRESSION_UNCONCIOUS)
                {
                    _currentExpression = GameConstants.EXPRESSION_UNCONCIOUS;
                    Portrait.sprite = Resources.Load<Sprite>("Portraits/Face" + Member.Profile.PortraitID + "_" + _currentExpression);
                }
                break;
            case PartyMemberState.Dead:
                if (_currentExpression != GameConstants.EXPRESSION_DEAD)
                {
                    _currentExpression = GameConstants.EXPRESSION_DEAD;
                    Portrait.sprite = Resources.Load<Sprite>("Portraits/Face" + Member.Profile.PortraitID + "_" + _currentExpression);
                }
                break;
        }
    }

    public void IndicateSelection(bool selected)
    {
        SelectionIndicator.color = selected ? new Color(1.0f, 1.0f, 8.0f) : Color.grey;
    }

    public void MenuClick()
    {
        bool shouldOpen = HUD.Instance.CharacterMenuOpen || Member == Party.Instance.ActiveMember || _clicking;
        HUD.Instance.SelectCharacter(this, shouldOpen);

        _clicking = true;
        CancelInvoke();
        Invoke("TimeoutClick", 0.3f);
    }

    void TimeoutClick()
    {
        _clicking = false;
    }

}
