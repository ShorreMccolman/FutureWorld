using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersistentUI : MonoBehaviour
{
    static PersistentUI Instance;
    void Awake(){ Instance = this; }

    [SerializeField] GameObject LoadingScreen;
    [SerializeField] Slider LoadBar;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        LoadingScreen.SetActive(false);
        LoadBar.value = 0;
    }

    public static void ProgressBar(float progress)
    {
        Instance.LoadBar.value = progress;
    }

    public static void ShowLoad()
    {
        Instance.LoadingScreen.SetActive(true);
        Instance.LoadBar.value = 0;
    }

    public static void HideLoad()
    {
        Instance.LoadingScreen.SetActive(false);
    }

}
