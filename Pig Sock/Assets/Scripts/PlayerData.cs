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
    public int finalScore;
    public Card bestCard;

    public PlayerData()
    {
        name = "SockFan" + Random.Range(101, 999).ToString();
    }

    public PlayerData(string name, Type type)
    {
        this.name = name;
        this.type = type;
    }

}
