using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Xml;
using System.IO;

[System.Serializable]
public class PlayerFileInfo
{
    public int Slot;
    public string Title;

    public int month;
    public int day;
    public int hour;
    public int minute;

    public PlayerFileInfo(int slot, string title)
    {
        Slot = slot;
        Title = title;
        System.DateTime dt = System.DateTime.Now;

        month = dt.Month;
        day = dt.Day;
        hour = dt.Hour;
        minute = dt.Minute;
    }
}

public class GameController : MonoBehaviour {
    public static GameController Instance { get; private set; }
    void Awake() { Instance = this; }

    public PlayerSettings PlayerSettings { get; private set; }

    ItemDatabase ItemDB;
    SkillDatabase SkillDB;
    ChestDatabase ChestDB;
    EnemyDatabase EnemyDB;
    StatusEffectDatabase EffectDB;
    QuestDatabase QuestDB;
    ResidencyDatabase ResidencyDB;
    MerchantDatabase MerchantDB;
    ProjectileDatabase ProjectileDB;
    SpellDatabase SpellDB;
    NPCDatabase NPCDB;
    TopicDatabase TopicDB;
    InteractableDatabase InteractableDB;
    ProfilePopupInfoDatabase ProfileInfoDB;

    bool _isPaused;
    
	void Start ()
    {
        Init();
        StartGameSequence();
	}

    void Init()
    {
        DontDestroyOnLoad(this.gameObject);

        ItemDB = new ItemDatabase();
        SkillDB = new SkillDatabase();
        EnemyDB = new EnemyDatabase();
        ChestDB = new ChestDatabase();
        EffectDB = new StatusEffectDatabase();
        QuestDB = new QuestDatabase();
        ResidencyDB = new ResidencyDatabase();
        MerchantDB = new MerchantDatabase();
        ProjectileDB = new ProjectileDatabase();
        SpellDB = new SpellDatabase();
        NPCDB = new NPCDatabase();
        TopicDB = new TopicDatabase();
        InteractableDB = new InteractableDatabase();
        ProfileInfoDB = new ProfilePopupInfoDatabase();
    }

    void StartGameSequence()
    {
        LoadPlayerSettings();
        StartCoroutine(LoadMenuSequence());
    }

    void LoadPlayerSettings()
    {
        PlayerSettings = FileManager.LoadBinary<PlayerSettings>("PlayerSettings");
        if(PlayerSettings == null)
        {
            PlayerSettings = new PlayerSettings();
        }

        // Apply Player Settings
    }

    public void SaveGame(int slot, string title)
    {
        string directory = Application.persistentDataPath + "/SaveData";

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        ScreenCapture.CaptureScreenshot(directory + "/screenshot_" + slot + ".png");
        PlayerFileInfo info = new PlayerFileInfo(slot, title);
        FileManager.SaveBinary<PlayerFileInfo>(info, "info_" + slot);

        GameStateManager.Instance.SaveGameState(slot);
    }

    IEnumerator LoadMenuSequence()
    {
        yield return SceneManager.LoadSceneAsync("Main");
        while (MenuManager.Instance == null)
            yield return null;

        yield return null;
        MenuManager.Instance.OpenMenu("MainMenu");
    }

    public void StartNewGame(CharacterData[] characterData)
    {
        StartCoroutine(NewGameSequence(characterData));
    }

    IEnumerator NewGameSequence(CharacterData[] characterData)
    {
        PersistentUI.ShowLoad();
        SoundManager.Instance.FadeMusic();
        yield return new WaitForSeconds(1.0f);

        AsyncOperation async = SceneManager.LoadSceneAsync("Game");
        while (!async.isDone)
        {
            PersistentUI.ProgressBar(async.progress);
            yield return null;
        }

        Party party = new Party(characterData);
        PartyController.Instance.NewParty(party);
        Spawn[] spawns = FindObjectsOfType<Spawn>();
        for (int i=0;i< spawns.Length;i++)
        {
            spawns[i].Populate();
            spawns[i].Terminate();
        }

        yield return null;
        SpriteHandler.PurgeTemp();
        yield return null;

        TimeManagement.Instance.StartTiming(party);
        PersistentUI.HideLoad();
    }

    public void LoadGame(int slot)
    {
        XmlDocument doc = GameStateManager.Instance.LoadGameState(slot);
        XmlNode gameData = doc.SelectSingleNode("GameData");
        StartCoroutine(LoadGameSequence(gameData));
    }

    IEnumerator LoadGameSequence(XmlNode gameData)
    {
        PersistentUI.ShowLoad();
        SoundManager.Instance.FadeMusic();
        yield return new WaitForSeconds(1.0f);

        Party party = new Party(gameData.SelectSingleNode("Party"));
        AsyncOperation async = SceneManager.LoadSceneAsync(party.CurrentLocationID);
        while (!async.isDone)
        {
            PersistentUI.ProgressBar(async.progress);
            yield return null;
        }

        PartyController.Instance.LoadParty(party);
        Spawn[] spawns = FindObjectsOfType<Spawn>();
        for (int i = 0; i < spawns.Length; i++)
        {
            spawns[i].Terminate();
        }

        XmlNodeList drops = gameData.SelectNodes("ItemDrop");
        DropController.Instance.LoadDrops(drops);
        XmlNodeList chests = gameData.SelectNodes("Chest");
        DropController.Instance.LoadChests(chests);
        XmlNodeList residences = gameData.SelectNodes("Residency");
        DropController.Instance.LoadResidences(residences);
        XmlNodeList merchants = gameData.SelectNodes("Merchant");
        DropController.Instance.LoadMerchants(merchants);
        XmlNodeList enemies = gameData.SelectNodes("Enemy");
        DropController.Instance.LoadEnemies(enemies);
        XmlNodeList projectiles = gameData.SelectNodes("Projectile");
        DropController.Instance.LoadProjectiles(projectiles);
        XmlNodeList interactables = gameData.SelectNodes("Interactable");
        DropController.Instance.LoadInteractables(interactables);

        yield return null;
        yield return null;

        TimeManagement.Instance.StartTiming(party);
        PersistentUI.HideLoad();
    }

    public void TriggerDeath()
    {
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        Party party = Party.Instance;

        PersistentUI.ShowLoad();
        SoundManager.Instance.FadeMusic();
        yield return new WaitForSeconds(1.0f);

        AsyncOperation async = SceneManager.LoadSceneAsync("Game");
        while (!async.isDone)
        {
            PersistentUI.ProgressBar(async.progress);
            yield return null;
        }

        PartyController.Instance.ReviveParty(party);
        yield return null;
        yield return null;

        TimeManagement.Instance.StartTiming(party);
        PersistentUI.HideLoad();
    }

    public void QuitToMain()
    {
        SceneManager.LoadSceneAsync("Main");
    }

    void PauseGame(bool shouldPause)
    {
        _isPaused = shouldPause;
    }
}
