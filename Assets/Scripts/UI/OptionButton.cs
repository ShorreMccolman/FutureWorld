using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionButton : MonoBehaviour, IInfoMessenger
{
    [SerializeField] protected InputField Input;

    public Text Label;

    protected int _option;
    protected string _infoMessage;

    protected ClickEvent OnCommit;
    protected ClickIndexEvent OnIndexClick;

    public void Setup(string label, int option, ClickIndexEvent onIndexClick)
    {
        _option = option;
        _infoMessage = label;

        Label.text = label;
        OnCommit = null;
        OnIndexClick = onIndexClick;
    }

    public void SetupInput(string label, ClickIndexEvent onInputClick)
    {
        _infoMessage = "";
        Label.text = label;
        OnIndexClick = onInputClick;
        OnCommit = null;

        EventSystem.current.SetSelectedGameObject(Input.gameObject);
    }

    public void Setup(string label, ClickEvent onClick)
    {
        _infoMessage = "";
        Label.text = label;
        OnCommit = onClick;
        OnIndexClick = null;
    }

    public string GetInfoMessage()
    {
        return _infoMessage;
    }
}
