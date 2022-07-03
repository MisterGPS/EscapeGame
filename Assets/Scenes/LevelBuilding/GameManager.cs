using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance { get; private set; }

    public static string SaveFile { get; private set; }

    public static CacheDiscardList LoadCacheDiscardList { get; } = new CacheDiscardList();

    private GameState state;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        Instantiate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Instantiate()
    {
        SaveFile = Application.persistentDataPath + "\\save.json";
        SetTime();
        timeStringCache = new(() => {
            StringBuilder sb = new();
            sb.Append(timeCode[0]);
            sb.Append(timeCode[1]);
            sb.Append(":");
            sb.Append(timeCode[2]);
            sb.Append(timeCode[3]);
            return sb.ToString();
        }, LoadCacheDiscardList);
    }

    // Saving and loading generated puzzles
    public void Load()
    {
        string json = File.ReadAllText(SaveFile);
        Debug.Log(json);
        JsonUtility.FromJsonOverwrite(json, state);
        LoadCacheDiscardList.DiscardAll();
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(state, true);
        Debug.Log(json);
        File.WriteAllText(SaveFile, json);
    }

    public static void ShuffleList<T>(List<T> list)
    {
        int count = list.Count;
        int last = count - 1;
        for (var i = 0; i < count; i++)
        {
            int r = Random.Range(i, count);
            T tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }

    // Time; Clock Puzzle
    public int[] timeCode { get => state.timeCode; private set => state.timeCode = value; }
    public string timeString { get => timeStringCache.Value; }
    private LazyCache<string> timeStringCache;
    public bool bClockWiresSolved { get; set; }
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
    private struct GameState
    {
        public int[] timeCode;
    }
}
