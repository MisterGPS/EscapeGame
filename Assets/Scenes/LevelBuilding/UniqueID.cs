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
        IDToGameobject.Add(ID, gameObject);
        GameobjectToID.Add(gameObject, ID);
    }
}
