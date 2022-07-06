using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface StateHolder
{
    public abstract State State { get; }

    public string Name { get => GetType().Name; }

    public void LoadJson(string json)
    {
        JsonUtility.FromJsonOverwrite(json, State);
    }

    public string SaveJson()
    {
        return JsonUtility.ToJson(State);
    }

    public virtual void PostLoad() { }
}

public interface State
{

}
