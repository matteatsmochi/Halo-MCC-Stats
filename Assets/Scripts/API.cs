using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using SimpleJSON;

public class API : MonoBehaviour
{
    public gametypes gameData;
    
    public Bot bot;

    public UIStuff ui;
    
    float sessionKills = 0;
    float sessionDeaths = 0;
    float sessionKD = 0;
    float sessionWins = 0;
    float sessionLosses = 0;

    bool dontadd = false;

    bool init = true;
    Stats previousGame = new Stats();

    string[] AllGametypes;

    void Awake()
    {
        gameData = new gametypes();
        AllGametypes = new string[18] { "CTF", "SLAYER", "ODDBALL", "KOTH", "JUGGERNAUGHT", "INFECTION", "FLOOD", "RACE", "EXTRACTION", "DOMINION", "REGICIDE", "GRIFBALL", "RICOCHET", "SANDBOX", "VIP", "TERRITORIES", "ASSAULT", "PAYBACK" };
        
    }

    
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log(sessionWins + " " + sessionLosses);
        }
    }
    
    string GetGamesJSON(string gt)
    {
        string WEBSERVICE_URL = "https://www.halowaypoint.com/en-us/games/halo-the-master-chief-collection/xbox-one/game-history?gamertags="+ PlayerPrefs.GetString("gamertag") + "&gameVariant=" + UppercaseFirst(gt.ToLower()) + "&game=" + gameData.Game +"&view=DataOnly";
            try
            {
                var webRequest = WebRequest.Create(WEBSERVICE_URL);
                if (webRequest != null)
                {
                    webRequest.Method = "GET";
                    webRequest.Timeout = 20000;
                    webRequest.ContentType = "application/json";
                    //webRequest.Headers.Add("Cookie", "Auth=" + cookieAuth);
                    webRequest.Headers.Add("Cookie", "Auth=" + PlayerPrefs.GetString("cookie"));
                    using (Stream s = webRequest.GetResponse().GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(s))
                        {
                            var jsonResponse = sr.ReadToEnd();

                            return jsonResponse;
                        }
                    }
                }
                return "";
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex);
                return "";
            }
    }
    
    
    bool VerifyReturnJSON(string json)
    {
        if (json.Contains("<!DOCTYPE html>") || json.Length == 0) //check if an error is in the message
        {
            //something went wrong
            ui.cookieIMG.color = Color.red;
            return false;
        }
        else
        {
            //success
            ui.cookieIMG.color = Color.green;
            return true;
        }
    }

    IEnumerator UpdateAllJSON()
    {
        foreach (var item in System.Enum.GetValues(typeof(gametypes.GameType)))
        {
            string tempJSON = GetGamesJSON(item.ToString());
            Debug.Log(tempJSON);
            if (VerifyReturnJSON(tempJSON))
            {
                if (System.Enum.TryParse(item.ToString(), out gametypes.GameType type))
                {
                    gameData.JSONData[type] = tempJSON;
                }
            }
            yield return new WaitForSeconds(0.2f);
        }

        
        if (!init)
        {
            CheckNewAgainstOld();
        }
        else
        {
            AllGametypes[0] = gameData.JSONData[gametypes.GameType.CTF];
            AllGametypes[1] = gameData.JSONData[gametypes.GameType.SLAYER];
            AllGametypes[2] = gameData.JSONData[gametypes.GameType.ODDBALL];
            AllGametypes[3] = gameData.JSONData[gametypes.GameType.KOTH];
            AllGametypes[4] = gameData.JSONData[gametypes.GameType.JUGGERNAUGHT];
            AllGametypes[5] = gameData.JSONData[gametypes.GameType.INFECTION];
            AllGametypes[6] = gameData.JSONData[gametypes.GameType.FLOOD];
            AllGametypes[7] = gameData.JSONData[gametypes.GameType.RACE];
            AllGametypes[8] = gameData.JSONData[gametypes.GameType.EXTRACTION];
            AllGametypes[9] = gameData.JSONData[gametypes.GameType.DOMINION];
            AllGametypes[10] = gameData.JSONData[gametypes.GameType.REGICIDE];
            AllGametypes[11] = gameData.JSONData[gametypes.GameType.GRIFBALL];
            AllGametypes[12] = gameData.JSONData[gametypes.GameType.RICOCHET];
            AllGametypes[13] = gameData.JSONData[gametypes.GameType.SANDBOX];
            AllGametypes[14] = gameData.JSONData[gametypes.GameType.VIP];
            AllGametypes[15] = gameData.JSONData[gametypes.GameType.TERRITORIES];
            AllGametypes[16] = gameData.JSONData[gametypes.GameType.ASSAULT];
            AllGametypes[17] = gameData.JSONData[gametypes.GameType.PAYBACK];
        }
        
        init = false;
    }
    
    
    void CheckNewAgainstOld()
    {
        int i = 0;
        foreach (var item in System.Enum.GetValues(typeof(gametypes.GameType)))
        {
            
            System.Enum.TryParse(item.ToString(), out gametypes.GameType type);
            if (gameData.JSONData[type] != AllGametypes[i])
            {
                //we have a new game played!
                //update new to old
                ui.jsonText.text = gameData.JSONData[type].ToString();
                ParseGameData(gameData.JSONData[type], item.ToString(), i);
                //AllGametypes[i] = gameData.JSONData[type];
            }

            i ++;
        }
    Debug.Log("End of search.");

    }
    
    void ParseGameData(string json, string gt, int agIndex)
    {
        Stats newestGame = new Stats();

        JSONNode n = JSON.Parse(json);

        if (n[0]["Stats"][0]["DateTime"].Value != "")
        {
            //we successfully got a json. 
        
            //the game is new, we need to issue an update
            newestGame.GameId = n[0]["Stats"][0]["GameId"].AsInt;
            newestGame.Score = n[0]["Stats"][0]["Score"].AsInt;
            newestGame.Kills = n[0]["Stats"][0]["Kills"].AsInt;
            newestGame.Deaths = n[0]["Stats"][0]["Deaths"].AsInt;
            newestGame.Assists = n[0]["Stats"][0]["Assists"].AsInt;
            newestGame.Headshots = n[0]["Stats"][0]["Headshots"].AsInt;
            newestGame.DateTime = n[0]["Stats"][0]["DateTime"].Value;
            newestGame.MapID = n[0]["Stats"][0]["MapID"].AsInt;
            newestGame.LocalizedMapName = n[0]["Stats"][0]["LocalizedMapName"].Value;
            newestGame.Won = n[0]["Stats"][0]["Won"].AsBool;
            newestGame.Medals = n[0]["Stats"][0]["Medals"].AsInt;
            newestGame.GameType = n[0]["Stats"][0]["GameType"].AsInt;
            newestGame.KD = n[0]["Stats"][0]["KD"].AsFloat;

            //ui.jsonText.text = "new game found";
            ui.newgameIMG.color = Color.green;
            AllGametypes[agIndex] = json;

            //new game is assigned
            //update session variables

            if (newestGame.Kills != previousGame.Kills && newestGame.Deaths != previousGame.Deaths && newestGame.Assists != previousGame.Assists)
            {
                //only accept if this game does not exactly equal the old game but under a different search return
            
                sessionKills += newestGame.Kills;
                sessionDeaths += newestGame.Deaths;
                
                Debug.Log("sessionKD: " + sessionKD);
                sessionKills += newestGame.Kills;
                sessionDeaths += newestGame.Deaths;
                if (sessionDeaths == 0)
                {
                    sessionKD = sessionKills;
                }
                else
                {
                    sessionKD = sessionKills/sessionDeaths;
                }
                
                Debug.Log("sessionKD (after): " + sessionKD);
                

                //UI to do something with this
                //Twitch Chat bot message

                if (newestGame.Won)
                {
                    //ui.AddWin();
                    if (!dontadd) 
                    {
                        sessionWins += 1;
                        dontadd = true;
                        StartCoroutine(OKToAdd());
                    }
                    if (PlayerPrefs.GetInt("bot") == 1)
                    {
                        bot.TwitchMessage("PogChamp " + PlayerPrefs.GetString("gamertag") + " just Won a game of " + newestGame.LocalizedMapName + " " + gt + " (Kills: " + newestGame.Kills + ", Deaths: " + newestGame.Deaths + ", Assists: " + newestGame.Assists + ")");
                    }
                }
                else
                {
                    //ui.AddLoss();
                    if (!dontadd) 
                    {
                        sessionLosses += 1;
                        dontadd = true;
                        StartCoroutine(OKToAdd());
                    }
                    if (PlayerPrefs.GetInt("bot") == 1)
                    {
                        bot.TwitchMessage("NotLikeThis " + PlayerPrefs.GetString("gamertag") + " just Lost a game of " + newestGame.LocalizedMapName + " " + gt + " (Kills: " + newestGame.Kills + ", Deaths: " + newestGame.Deaths + ", Assists: " + newestGame.Assists + ")");
                    }
                }

                ui.UpdateKD(sessionKD);
                ui.RecieveWLT(sessionWins, sessionLosses, 0);
                

                //make newestgame the previous game
                previousGame = newestGame;
            }
            
        }

    }
    
    public void Init()
    {
        if (PlayerPrefs.GetString("twitch") != "" && PlayerPrefs.GetInt("bot") == 1)
        {
            bot.ConnectToTwitchChat(PlayerPrefs.GetString("twitch"));
        }
        
        if (PlayerPrefs.GetString("cookie") != "")
        {
            UpdateVoid();
        }

        ui.RemoveInitButton();
        
    }

    void UpdateVoid()
    {
        ui.jsonText.text = "Looking for new games";
        ui.newgameIMG.color = Color.yellow; 
        StartCoroutine(UpdateAllJSON());
        StartCoroutine(UpdateDelay(1));
    }

    IEnumerator UpdateDelay(int s)
    {
        yield return new WaitForSeconds(s * 60);
        UpdateVoid();
    }

    IEnumerator OKToAdd()
    {
        yield return new WaitForSeconds(30);
        dontadd = false;
    }
    
  

    string UppercaseFirst(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        return char.ToUpper(s[0]) + s.Substring(1);
    }
}
