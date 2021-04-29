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
    bool _wasClicked;

    string currentExpression;

    public void Init(PartyMember member)
    {
        Member = member;
        currentExpression = "X";

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
                    if(currentExpression != expression)
                    {
                        currentExpression = expression;
                        Portrait.sprite = Resources.Load<Sprite>("Portraits/Face" + Member.Profile.PortraitID + "_" + expression);
                    }
                }
                else if (currentExpression != Member.Vitals.Expression)
                {
                    currentExpression = Member.Vitals.Expression;
                    Portrait.sprite = Resources.Load<Sprite>("Portraits/Face" + Member.Profile.PortraitID + "_" + Member.Vitals.Expression);
                }
                break;
            case PartyMemberState.Unconcious:
                if (currentExpression != GameConstants.EXPRESSION_UNCONCIOUS)
                {
                    currentExpression = GameConstants.EXPRESSION_UNCONCIOUS;
                    Portrait.sprite = Resources.Load<Sprite>("Portraits/Face" + Member.Profile.PortraitID + "_" + currentExpression);
                }
                break;
            case PartyMemberState.Dead:
                if (currentExpression != GameConstants.EXPRESSION_DEAD)
                {
                    currentExpression = GameConstants.EXPRESSION_DEAD;
                    Portrait.sprite = Resources.Load<Sprite>("Portraits/Face" + Member.Profile.PortraitID + "_" + currentExpression);
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
        bool shouldOpen = _wasClicked || HUD.Instance.CharacterMenuOpen;
        if (shouldOpen)
        {
            _wasClicked = false;
            CancelInvoke("Unclick");
            HUD.Instance.SelectCharacter(this, shouldOpen);
        }
        else
        {
            HUD.Instance.SelectCharacter(this, shouldOpen);
            _wasClicked = true;
            Invoke("Unclick", 0.2f);
        }
    }

    public void HoverPortrait()
    {
        HUD.Instance.SendInfoMessage(Member.Profile.CharacterName + " the " + Member.Profile.Class.ToString() + ": " + Member.EffectiveStatusCondition());
    }

    public void HoverVitals()
    {
        string hp = Member.Vitals.CurrentHP + " / " + Member.Vitals.EffectiveTotalHP + " Hit Points";
        string mp = Member.Vitals.CurrentMP + " / " + Member.Vitals.EffectiveTotalMP + " Spell Points";

        HUD.Instance.SendInfoMessage(hp + "     " + mp);
    }

    public void Unhover()
    {
        HUD.Instance.SendInfoMessage("");
    }

    void Unclick()
    {
        _wasClicked = false;
    }
}
