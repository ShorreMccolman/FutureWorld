using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemMenu : Menu {

	public void SaveGame()
    {
        MenuManager.Instance.OpenMenu("SaveMenu");
        MenuManager.Instance.CloseMenu(MenuTag);
    }

    public void NewGame()
    {

    }

    public void LoadGame()
    {
        MenuManager.Instance.OpenMenu("LoadMenu");
        MenuManager.Instance.CloseMenu(MenuTag);
    }

    public void Controls()
    {

    }

    public void QuitGame()
    {
        //GameController.Instance.QuitToMain();
    }

    public void Close()
    {
        MenuManager.Instance.CloseMenu(MenuTag);
    }
}
