using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour, StateHolder
{
    // Singleton
    public static GameManager Instance { get; private set; }

    public static string SaveFile { get; private set; }

    private static PlayerController PlayerController { get; set; }
    public static PlayerController GetPlayerController()
    {
        PlayerController = PlayerController != null ? PlayerController : FindObjectOfType<PlayerController>();
        return PlayerController;
    }
    
    private static AudioManager AudioManager { get; set; }
    public static AudioManager GetAudioManager()
    {
        AudioManager = AudioManager != null ? AudioManager : FindObjectOfType<AudioManager>();
        return AudioManager;
    }

    public static InputController InputController { get; private set; }

    public static CacheDiscardList LoadCacheDiscardList { get; } = new CacheDiscardList();

    public State State => gameState;
    private GameState gameState = new GameState();

    private SavingComponent[] saveList;

    public bool bMainMenu = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
        }
    }

    private void Start()
    {
        Instantiate();
        InputController = GetComponent<InputController>();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void Instantiate()
    {
        SaveFile = Application.persistentDataPath + "\\save.json";
        SetTime();
        timeStringCache = new LazyCache<string>(() => {
            StringBuilder sb = new();
            sb.Append(timeCode[0]);
            sb.Append(timeCode[1]);
            sb.Append(":");
            sb.Append(timeCode[2]);
            sb.Append(timeCode[3]);
            return sb.ToString();
        }, LoadCacheDiscardList);
        saveList = FindObjectsOfType<SavingComponent>();
    }

    // Saving and loading generated puzzles
    public void Load()
    {
        foreach (SavingComponent savingComponent in saveList)
        {
            savingComponent.PreLoad();
        }
        JObject json = JObject.Parse(File.ReadAllText(SaveFile));
        Debug.Log(json);
        foreach (JProperty jProperty in json.Properties())
        {
            string id = jProperty.Name;
            JObject jObject = jProperty.Value as JObject;
            if (UniqueID.IDToGameobject.ContainsKey(id))
            {
                GameObject gameObject = UniqueID.IDToGameobject[id];
                SavingComponent savingComponent = gameObject.GetComponent<SavingComponent>();
                if (savingComponent != null)
                {
                    savingComponent.Load(jObject);
                }
                else
                {
                    Debug.LogWarning(string.Format("Save file contains data for ID {0} but the GameObject with that ID has no saveable data.\nDiscarding...", id));
                }
            }
            else
            {
                Debug.LogWarning(string.Format("Save file contains data for ID {0} but there is no GameObject with that ID.\nDiscarding...", id));
            }
        }
        LoadCacheDiscardList.DiscardAll();
        foreach (SavingComponent savingComponent in saveList)
        {
            savingComponent.PostLoad();
        }
    }

    public void Save()
    {
        foreach (SavingComponent savingComponent in saveList)
        {
            savingComponent.PreSave();
        }
        JObject json = new JObject();
        foreach (SavingComponent savingComponent in saveList)
        {
            JObject jObject = savingComponent.Save();
            json.Add(savingComponent.GetComponent<UniqueID>().ID, jObject);
        }
        Debug.Log(json);
        File.WriteAllText(SaveFile, json.ToString());
        foreach (SavingComponent savingComponent in saveList)
        {
            savingComponent.PostSave();
        }
    }

    public void NewGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Room1");
        bMainMenu = false;
        StartCoroutine(SaveNextTick());
    }
    
    public void LoadLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Room1");
        bMainMenu = false;
        StartCoroutine(LoadNextTick());
    }

    public void LoadMainMenuWithSave()
    {
        //Hier direkt saven
        Save();
        LoadMainMenu();
    }

    public void LoadMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        bMainMenu = true;
        PlayerController = null;
        InputController.DisableInput();
    }

    public void LoadCredits()
    {
        PlayerController = null;
        InputController.DisableInput();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Credits");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public static void ShuffleList<T>(List<T> list)
    {
        int count = list.Count;
        int last = count - 1;
        for (int i = 0; i < count; i++)
        {
            int r = Random.Range(i, count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }

    private int saveTicks = 0;
    IEnumerator SaveNextTick()
    {
        //Two two avoid catching destroyed GameManager
        while (saveTicks < 2)
        {
            saveTicks++;
            yield return new WaitForFixedUpdate();
        }
        saveList = new SavingComponent[0];
        saveList = FindObjectsOfType<SavingComponent>();
        Save();
        saveTicks = 0;
        PlayerController.ActivateInput();
        yield return null;
    }

    private int loadTicks = 0;
    IEnumerator LoadNextTick()
    {
        //Two two avoid catching destroyed GameManager
        while (loadTicks < 2)
        {
            loadTicks++;
            yield return new WaitForFixedUpdate();
        }
        saveList = FindObjectsOfType<SavingComponent>();
        Load();
        loadTicks = 0;
        PlayerController.ActivateInput();
        yield return null;
    }

    // Time; Clock Puzzle
    public int[] timeCode { get => gameState.timeCode; private set => gameState.timeCode = value; }
    public string timeString { get => timeStringCache.Value; }
    private LazyCache<string> timeStringCache;
    private void SetTime()
    {
        int totalMinutes = Random.Range(0, 1440);
        int hours = Mathf.FloorToInt(totalMinutes / 60);
        int minutes = totalMinutes % 60;

        int zahl1 = Mathf.FloorToInt(hours / 10);
        int zahl2 = hours % 10;
        int zahl3 = Mathf.FloorToInt(minutes / 10);
        int zahl4 = minutes % 10;

        timeCode = new int[] { zahl1, zahl2, zahl3, zahl4 };
    }

    /**
     * Saves all the variables that represent the current state of the game and need to persist between sessions.
     */
    [System.Serializable]
    private class GameState : State
    {
        public int[] timeCode;
    }
}
