using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public enum Type
    {
        Human,
        CPU,
        Off
    }

    public Type type;
    public string name;
}
