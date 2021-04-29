using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ClickEvent();
public delegate void ClickIndexEvent(int index);

public abstract class Menu : MonoBehaviour {

    [SerializeField]
    public string MenuTag;

    GameObject _contents;
    public GameObject Contents
    {
        get
        {
            if (_contents == null)
                _contents = transform.GetChild(0).gameObject;
            return _contents;
        }
    }

    void Awake()
    {
        if (Contents.activeSelf)
            Contents.SetActive(false);
    }

    void Start()
    {
        if (MenuManager.Instance != null)
            MenuManager.Instance.AddMenu(this);

        Init();
    }

    public void CloseMenu()
    {
        MenuManager.Instance.CloseMenu(MenuTag);
    }

    protected virtual void Init()
    {

    }

	public virtual void OnOpen ()
    {
        
	}

    public virtual void OnClose()
    {
        
    }
}
