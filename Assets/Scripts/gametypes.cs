using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gametypes
{
    
    public string Game;
    //public Dictionary<GameType, bool> isEnabled = new Dictionary<GameType, bool>();
    public Dictionary<GameType, string> JSONData = new Dictionary<GameType, string>();

    public gametypes()
    {
        foreach (var item in System.Enum.GetValues(typeof(GameType)))
        {
            //isEnabled.Add((GameType)item, true);
            JSONData.Add((GameType)item, "hello");
        }

        Game = "All";
    }


    public void ToggleGametype(string gametype, bool isEnabled)
    {
        if (System.Enum.TryParse(gametype, out GameType type))
        {
            //this.isEnabled[type] = isEnabled;
        }
    }


    
    public enum GameType
    {
        CTF,
        SLAYER,
        ODDBALL,
        KOTH,
        JUGGERNAUGHT,
        INFECTION,
        FLOOD,
        RACE,
        EXTRACTION,
        DOMINION,
        REGICIDE,
        GRIFBALL,
        RICOCHET,
        SANDBOX,
        VIP,
        TERRITORIES,
        ASSAULT,
        PAYBACK
    }
}


