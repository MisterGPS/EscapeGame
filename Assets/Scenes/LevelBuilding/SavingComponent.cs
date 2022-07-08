using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

[RequireComponent(typeof(UniqueID))]
public class SavingComponent : MonoBehaviour
{
    StateHolder[] stateHolders;

    void Start()
    {
        stateHolders = GetComponents<StateHolder>();
    }

    public void Load(JObject json)
    {
        foreach (StateHolder holder in stateHolders)
        {
            string stateJsonString = (string)json[holder.Name];
            holder.LoadJson(stateJsonString);
        }
    }

    public JObject Save()
    {
        JObject json = new JObject();
        foreach (StateHolder holder in stateHolders)
        {
            json.Add(holder.Name, holder.SaveJson());
        }
        return json;
    }

    public void PreSave()
    {
        foreach (StateHolder holder in stateHolders)
        {
            holder.PreSave();
        }
    }

    public void PostSave()
    {
        foreach (StateHolder holder in stateHolders)
        {
            holder.PostSave();
        }
    }

    public void PreLoad()
    {
        foreach (StateHolder holder in stateHolders)
        {
            holder.PreLoad();
        }
    }

    public void PostLoad()
    {
        foreach (StateHolder holder in stateHolders)
        {
            holder.PostLoad();
        }
    }
}
