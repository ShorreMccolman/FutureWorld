using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerSettings
{
    public enum EWindowMode
    {
        Windowed,
        Fullscreen
    }

    public float MusicVolume { get; private set; }
    public float SfxVolume { get; private set; }
    public EWindowMode WindowMode { get; private set; }
	
    public PlayerSettings()
    {
        MusicVolume = 1.0f;
        SfxVolume = 1.0f;
        WindowMode = EWindowMode.Fullscreen;
    }
}
