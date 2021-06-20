using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemMenu : Menu {

	public void SaveGame()
    {
        MenuManager.Instance.SwapMenu("SaveMenu");
    }

    public void NewGame()
    {
        GameController.Instance.QuitToMain();
    }

    public void LoadGame()
    {
        MenuManager.Instance.SwapMenu("LoadMenu");
    }

    public void Controls()
    {

    }

    public void QuitGame()
    {
        GameController.Instance.QuitToMain();
    }

    public void Close()
    {
        MenuManager.Instance.CloseMenu(MenuTag);
    }
}
