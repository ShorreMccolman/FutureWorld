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

    Dictionary<string, Sprite> _usedSprites;

    bool _clicking;

    public void Init(PartyMember member)
    {
        Member = member;
        _usedSprites = new Dictionary<string, Sprite>();

        UpdateUI();
        SelectionIndicator.color = Color.grey;
    }

    public void UpdateUI()
    {
        ReadyIndicator.color = Member.Vitals.IsReady() ? Color.green : Color.grey;
        HealthSlider.value = Member.Vitals.GetHealthRatio();
        ManaSlider.value = Member.Vitals.GetManaRatio();
        SwapPortrait(Member.Vitals.EffectiveExpression);
    }

    void SwapPortrait(string expression)
    {
        if (_usedSprites.ContainsKey(expression))
        {
            Portrait.sprite = _usedSprites[expression];
        } 
        else
        {
            Sprite portrait = Resources.Load<Sprite>("Portraits/Face" + Member.Profile.PortraitID + "_" + expression);
            _usedSprites.Add(expression, portrait);
            Portrait.sprite = portrait;
        }
    }

    public void IndicateSelection(bool selected)
    {
        SelectionIndicator.color = selected ? Color.white : Color.grey;
    }

    public void MenuClick()
    {
        HUD.Instance.SelectCharacter(this, _clicking);

        _clicking = true;
        CancelInvoke();
        Invoke("TimeoutClick", 0.3f);
    }

    void TimeoutClick()
    {
        _clicking = false;
    }

}
