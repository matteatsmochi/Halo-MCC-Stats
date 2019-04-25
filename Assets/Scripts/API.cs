using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using SimpleJSON;

public class API : MonoBehaviour
{
    public gametypes gameData;
    
    public Bot bot;
    bool canChat;

    public UIStuff ui;
    
    bool success;
    string AuthCookie;

    float sessionKills;
    float sessionDeaths;
    float sessionKD;
    float sessionWins;
    float sessionLosses;

    Stats previousGame = new Stats();
    SkillRank ranks = new SkillRank();
    

    void Awake()
    {
        gameData = new gametypes();

        
    }
    
    void Start()
    {
        Debug.Log("API Start");
        
        
    }


    void Update()
    {
        
    }

    
    

    void CheckForNewGames()
    {
        Stats newestGame = new Stats();

        JSONNode n = JSON.Parse(NewestJSONStats());

        if (n["Stats"][0]["GameId"].Value != "")
        {
            //we successfully got a json. check if the game is new
            if (n["Stats"][0]["DateTime"].Value != previousGame.DateTime)
            {
                //the game is new, we need to issue an update
                newestGame.GameId = n["Stats"][0]["GameId"].AsInt;
                newestGame.Score = n["Stats"][0]["Score"].AsInt;
                newestGame.Kills = n["Stats"][0]["Kills"].AsInt;
                newestGame.Deaths = n["Stats"][0]["Deaths"].AsInt;
                newestGame.Assists = n["Stats"][0]["Assists"].AsInt;
                newestGame.Headshots = n["Stats"][0]["Headshots"].AsInt;
                newestGame.DateTime = n["Stats"][0]["DateTime"].Value;
                newestGame.MapID = n["Stats"][0]["MapID"].AsInt;
                newestGame.LocalizedMapName = n["Stats"][0]["LocalizedMapName"].Value;
                newestGame.Won = n["Stats"][0]["Won"].AsBool;
                newestGame.Medals = n["Stats"][0]["Medals"].AsInt;
                newestGame.GameType = n["Stats"][0]["GameType"].AsInt;
                newestGame.KD = n["Stats"][0]["KD"].AsFloat;

                //new game is assigned
                //update session variables

                sessionKills += newestGame.Kills;
                sessionDeaths += newestGame.Deaths;
                sessionKD = (sessionKD + newestGame.KD) / 2;
                
                if (newestGame.Won)
                {
                    sessionWins ++;
                }
                else if (!newestGame.Won)
                {
                    sessionLosses ++;
                }

                //UI to do something with this
                //Twitch Chat bot message

                if (newestGame.Won)
                {
                    ui.AddWin();
                    if (canChat)
                    {
                        bot.TwitchMessage("PogChamp" + PlayerPrefs.GetString("gamertag") + " just Won a game on " + UppercaseFirst(newestGame.LocalizedMapName.ToLower()) + " (Kills: " + newestGame.Kills + ", Deaths: " + newestGame.Deaths + ", Assists: " + newestGame.Assists + ")");
                    }
                }
                else
                {
                    ui.AddLoss();
                    if (canChat)
                    {
                        bot.TwitchMessage("NotLikeThis" + PlayerPrefs.GetString("gamertag") + " just Lost a game on " + UppercaseFirst(newestGame.LocalizedMapName.ToLower()) + " (Kills: " + newestGame.Kills + ", Deaths: " + newestGame.Deaths + ", Assists: " + newestGame.Assists + ")");
                    }
                }

                ui.UpdateKD(newestGame.KD);
                
                

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
        
        if (PlayerPrefs.GetString("email") != "" && PlayerPrefs.GetString("password") != "")
        {
            //get auth
            RetrieveCookie();

            //get ranks
            

            //get recent games
            InitPreviousGame();

            //start cookie refresh loop
            UpdateCookie();

            
            
        }

        //move this to cookie success
        ui.RemoveInitButton();
        
        
        

    }
    
    void InitPreviousGame()
    {
        JSONNode n = JSON.Parse(NewestJSONStats());

        if (n["Stats"][0]["GameId"].Value != "")
        {
            //the json is valid
            previousGame.GameId = n["Stats"][0]["GameId"].AsInt;
            previousGame.Score = n["Stats"][0]["Score"].AsInt;
            previousGame.Kills = n["Stats"][0]["Kills"].AsInt;
            previousGame.Deaths = n["Stats"][0]["Deaths"].AsInt;
            previousGame.Assists = n["Stats"][0]["Assists"].AsInt;
            previousGame.Headshots = n["Stats"][0]["Headshots"].AsInt;
            previousGame.DateTime = n["Stats"][0]["DateTime"].Value;
            previousGame.MapID = n["Stats"][0]["MapID"].AsInt;
            previousGame.LocalizedMapName = n["Stats"][0]["LocalizedMapName"].Value;
            previousGame.Won = n["Stats"][0]["Won"].AsBool;
            previousGame.Medals = n["Stats"][0]["Medals"].AsInt;
            previousGame.GameType = n["Stats"][0]["GameType"].AsInt;
            previousGame.KD = n["Stats"][0]["KD"].AsFloat;
        }
        
    }
    
    string NewestJSONStats()
    {
        return null;
    }

    void RetrieveJSONRank()
    {

    }
    
    void RetrieveCookie()
    {
        
        
        //set success as true or false
    }
    
    void UpdateCookie()
    {
        if (!success)
        {
            StartCoroutine(UpdateCookieDelay(1));
        }
        else
        {
            StartCoroutine(UpdateCookieDelay(5));
        }
    }

    IEnumerator UpdateCookieDelay(float delay)
    {
        yield return new WaitForSeconds(delay * 60);
        RetrieveCookie();
        UpdateCookie();
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
