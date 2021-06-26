using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ClickEvent();
public delegate void ClickIndexEvent(int index);

public abstract class Menu : MonoBehaviour {

    [SerializeField]
    public string MenuTag;

    GameObject _contents;

    public GameObject Contents => _contents;
    public bool IsOpen => _contents.activeSelf;

    void Awake()
    {
        _contents = transform.GetChild(0).gameObject;

        if (_contents.activeSelf)
            _contents.SetActive(false);
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
