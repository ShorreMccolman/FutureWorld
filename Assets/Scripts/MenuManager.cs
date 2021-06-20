using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour 
{
    public static MenuManager Instance { get; private set; }
    void Awake() { Instance = this; }

    public delegate void MenuEvent(bool enabled);
    public MenuEvent OnMenuLock;

    Dictionary<string, Menu> _menuDict = new Dictionary<string, Menu>();

    Dictionary<string, Menu> _openMenuDict = new Dictionary<string, Menu>();

    public void SwapMenu(string menuTag)
    {
        foreach (var menu in _openMenuDict.Values)
        {
            menu.OnClose();
            menu.Contents.SetActive(false);
        }
        _openMenuDict.Clear();
        OpenMenu(menuTag);
    }

    public void CloseAllMenus()
    {
        foreach(var menu in _openMenuDict.Values)
        {
            menu.OnClose();
            menu.Contents.SetActive(false);
        }
        _openMenuDict.Clear();
        OnMenuLock?.Invoke(false);
    }

    public void CloseMenu(string menuTag)
    {
        if (_openMenuDict.ContainsKey(menuTag))
        {
            _openMenuDict[menuTag].OnClose();
            _menuDict[menuTag].Contents.SetActive(false);
            _openMenuDict.Remove(menuTag);
            OnMenuLock?.Invoke(false);
        }
    }

    public void OpenMenu(string menuTag, bool setMenuLock = false)
    {
        if(!_menuDict.ContainsKey(menuTag))
        {
            Debug.LogError("Did not find menu with tag " + menuTag);
            return;
        }

        if(!_openMenuDict.ContainsKey(menuTag))
        {
            _openMenuDict.Add(menuTag, _menuDict[menuTag]);
            _menuDict[menuTag].Contents.SetActive(true);
            _menuDict[menuTag].OnOpen();
        }

        if(setMenuLock)
        {
            OnMenuLock?.Invoke(true);
        }
    }

    public Menu GetMenu(string menuTag)
    {
        if (!_menuDict.ContainsKey(menuTag))
        {
            Debug.LogError("Did not find menu with tag " + menuTag);
            return null;
        }

        return _menuDict[menuTag];
    }
	
    public void AddMenu(Menu menu)
    {
        if (!_menuDict.ContainsKey(menu.MenuTag))
        {
            _menuDict.Add(menu.MenuTag, menu);
        }
        else
        {
            Debug.LogError("Manager already contains menu with tag " + menu.MenuTag);
        }
    }

    public bool IsMenuOpen()
    {
        return _openMenuDict.Count > 0;
    }

    public bool IsMenuOpen(List<string> tags)
    {
        foreach(var menu in _openMenuDict)
        {
            if (tags.Contains(menu.Key))
                return true;
        }
        return false;
    }

    public void ClearMenus()
    {
        _menuDict = new Dictionary<string, Menu>();
    }

}
