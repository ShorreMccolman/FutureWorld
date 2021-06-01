using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Xml;

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

    bool _isPaused;

    public delegate void UpdateDel();
    public UpdateDel EntityUpdate;
    
	void Start ()
    {
        Init();
        StartGameSequence();
	}

    void Init()
    {
        Debug.Log("Inititalizing Game Controller");
        DontDestroyOnLoad(this.gameObject);

        ItemDB = new ItemDatabase();
        SkillDB = new SkillDatabase();
        EnemyDB = new EnemyDatabase();
        EffectDB = new StatusEffectDatabase();
        QuestDB = new QuestDatabase();
        ResidencyDB = new ResidencyDatabase();
        MerchantDB = new MerchantDatabase();
        ProjectileDB = new ProjectileDatabase();
        SpellDB = new SpellDatabase();
        NPCDB = new NPCDatabase();
        TopicDB = new TopicDatabase();
        InteractableDB = new InteractableDatabase();
    }

    void Update()
    {
        if(!_isPaused && EntityUpdate != null)
            EntityUpdate();
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
        ScreenCapture.CaptureScreenshot(Application.dataPath + "/SaveData/screenshot_" + slot + ".png");
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
        yield return SceneManager.LoadSceneAsync("Game");
        while (PartyController.Instance == null)
            yield return null;
        Party party = new Party(characterData);
        PartyController.Instance.NewParty(party);
        Spawn[] spawns = FindObjectsOfType<Spawn>();
        for (int i=0;i< spawns.Length;i++)
        {
            spawns[i].Populate();
            spawns[i].Terminate();
        }

        TimeManagement.Instance.StartTiming(party);
    }

    public void LoadGame(int slot)
    {
        XmlDocument doc = GameStateManager.Instance.LoadGameState(slot);
        XmlNode gameData = doc.SelectSingleNode("GameData");
        StartCoroutine(LoadGameSequence(gameData));
    }

    IEnumerator LoadGameSequence(XmlNode gameData)
    {
        Party party = new Party(gameData.SelectSingleNode("Party"));
        yield return SceneManager.LoadSceneAsync(party.CurrentLocationID);
        while (PartyController.Instance == null)
            yield return null;

        yield return null;

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

        TimeManagement.Instance.StartTiming(party);
    }

    public void TriggerDeath()
    {
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        HUD.Instance.ShowLoad(true);
        Party party = PartyController.Instance.Party;

        yield return SceneManager.LoadSceneAsync(2);
        while (PartyController.Instance == null)
            yield return null;

        PartyController.Instance.ReviveParty(party);
        HUD.Instance.ShowLoad(false);
    }

    void PauseGame(bool shouldPause)
    {
        _isPaused = shouldPause;
    }
}
