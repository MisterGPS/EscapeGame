using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class UniqueID : MonoBehaviour
{
    [SerializeField]
    private string id = Guid.NewGuid().ToString();

    public string ID => id;

    [ContextMenu("Generate new ID")]
    private void RegenerateGUID() => id = Guid.NewGuid().ToString();

    public static Dictionary<string, GameObject> IDToGameobject { get; } = new Dictionary<string, GameObject>();
    public static Dictionary<GameObject, string> GameobjectToID { get; } = new Dictionary<GameObject, string>();

    void Start()
    {
        if (IDToGameobject.ContainsKey(ID))
        {
            Debug.LogError(string.Format("The ID {0} for the gameobject {1} is already taken. This will cause unexpected behavior when saving and loading. Please Generate a new ID.", ID, gameObject));
        }
        IDToGameobject.Add(ID, gameObject);
        GameobjectToID.Add(gameObject, ID);
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null && ID != GameManager.Instance.GetComponent<UniqueID>().ID)
        {
            IDToGameobject.Remove(ID);
            GameobjectToID.Remove(gameObject);
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void clearDictionarys()
    {
        IDToGameobject.Clear();
        GameobjectToID.Clear();
    }
}
