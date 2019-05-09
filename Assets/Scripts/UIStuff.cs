using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIStuff : MonoBehaviour
{
    API api;
    
    public Image srbar;
    public Image WinsImage;
    public Image LossesImage;
    public Image TiesImage;
    public Image RecordImage;


    public TextMeshProUGUI Wins;
    public TextMeshProUGUI Losses;
    public TextMeshProUGUI Ties;
    public TextMeshProUGUI Record;
    public TextMeshProUGUI KD;

    public TMP_InputField gamertagIF;
    public TMP_InputField twitchIF;
    public Button cookieBTN;
    public TMP_InputField cookieIF;
    public Image cookieIMG;
    public Toggle botBool;
    public Button initButton;

    public TMP_Dropdown gameDD;

    public TextMeshProUGUI jsonText;
    public Image newgameIMG;

    float apiWins;
    float apiLosses;
    float apiTies;
    
    float adjustedWins;
    float adjustedLosses;
    float adjustedTies;

    float averageKD;

    void Awake()
    {        
        api = GetComponent<API>();

    }

    void Start()
    {
        if (!PlayerPrefs.HasKey("gamertag"))
        {PlayerPrefs.SetString("gamertag", "");}
        else
        {gamertagIF.text = PlayerPrefs.GetString("gamertag");}

        if (!PlayerPrefs.HasKey("twitch"))
        {PlayerPrefs.SetString("twitch", "");}
        else
        {twitchIF.text = PlayerPrefs.GetString("twitch");}

        if (!PlayerPrefs.HasKey("cookie"))
        {PlayerPrefs.SetString("cookie", "");}
        else
        {cookieIF.text = PlayerPrefs.GetString("cookie");}

        if (!PlayerPrefs.HasKey("bot"))
        {PlayerPrefs.SetInt("bot", 0);}
        else
        {
            if (PlayerPrefs.GetInt("bot") == 1)
                botBool.isOn = true;
            else
                botBool.isOn = false;
        }
        PlayerPrefs.Save();
    }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.A))
    //     {
    //         aaa();
    //     }
    //     else if (Input.GetKeyDown(KeyCode.S))
    //     {
    //         bbb();
    //     }
    //     else if (Input.GetKeyDown(KeyCode.D))
    //     {
    //         ccc();
    //     }
    //     else if (Input.GetKeyDown(KeyCode.F))
    //     {
    //         ddd();
    //     }
    // }
    
    public void UpdateGamertag()
    {
        PlayerPrefs.SetString("gamertag", gamertagIF.text);
        PlayerPrefs.Save();
    }

    public void UpdateTwitch()
    {
        PlayerPrefs.SetString("twitch", twitchIF.text);
        PlayerPrefs.Save();
    }

    public void UpdateCookie()
    {
        PlayerPrefs.SetString("cookie", cookieIF.text);
        PlayerPrefs.Save();
    }

    public void OpenCookie()
    {
        Application.OpenURL("https://login.live.com/oauth20_authorize.srf?client_id=000000004C0BD2F1&scope=xbox.basic+xbox.offline_access&response_type=code&redirect_uri=https:%2f%2fwww.halowaypoint.com%2fauth%2fcallback&locale=en-us&display=touch&state=https%253a%252f%252fwww.halowaypoint.com%252fen-us");
    }

    public void UpdateBot()
    {
        if (botBool.isOn)
        {
            PlayerPrefs.SetInt("bot", 1);
        }
        else
        {
            PlayerPrefs.SetInt("bot", 0);
        }
        
        PlayerPrefs.Save();
    }
    

    public void UpdateColor(string newColor)
    {

    }

    // public void aaa ()
    // {
    //     RecieveWLT(3,4,0);
    // }

    // public void bbb ()
    // {
    //     RecieveWLT(0,4,0);
    // }

    // public void ccc ()
    // {
    //     RecieveWLT(16,2,0);
    // }

    // public void ddd ()
    // {
    //     RecieveWLT(69,420,0);
    // }
    
    public void RecieveWLT(float wins, float losses, float ties)
    {
        apiWins = wins;
        apiLosses = losses;
        apiTies = ties;
        UpdateWLT();
    }
    
    void UpdateWLT()
    {
        RemoveTodaysRecord();

        float usableWins = adjustedWins + apiWins;
        float usableLosses = adjustedLosses + apiLosses;
        float usableTies = adjustedTies + apiTies;
        
        
        float winprct = usableWins / (usableWins + usableLosses + usableTies);
        float lossprct = usableLosses / (usableWins + usableLosses + usableTies);
        float tieprct = usableTies / (usableWins + usableLosses + usableTies);

        string appendW = "W:";
        string appendL = "L:";
        string appendT = "T:";
        
        if (winprct < 0.15f) appendW = "";
        if (lossprct < 0.15f) appendL = "";
        if (tieprct < 0.15f) appendT = "";
        
        Wins.text = appendW + usableWins.ToString();
        Losses.text = appendL + usableLosses.ToString();
        Ties.text = appendT + usableTies.ToString();

        if (usableWins != 0)
        {
            Wins.DOFade(1, 0.7f);
        }
        else
        {
            Wins.DOFade(0, 0.7f);
        }

        if (usableLosses != 0)
        {
            Losses.DOFade(1, 0.7f);
        }
        else
        {
            Losses.DOFade(0, 0.7f);
        }

        if (usableTies != 0)
        {
            Ties.DOFade(1, 0.7f);
        }
        else
        {
            Ties.DOFade(0, 0.7f);
        }

        Sequence seq = DOTween.Sequence();
            seq.Append(WinsImage.rectTransform.DOSizeDelta(new Vector2(winprct * 250, 24), 1));
            seq.Join(WinsImage.rectTransform.DOAnchorPos(new Vector3(winprct * 125, 0, 0), 1));

            seq.Join(LossesImage.rectTransform.DOSizeDelta(new Vector2(lossprct * 250, 24), 1));
            seq.Join(LossesImage.rectTransform.DOAnchorPos(new Vector3((lossprct * 125) + (winprct * 250), 0, 0), 1));

            seq.Join(TiesImage.rectTransform.DOSizeDelta(new Vector2(tieprct * 250, 24), 1));
            seq.Join(TiesImage.rectTransform.DOAnchorPos(new Vector3((tieprct * 125) + (lossprct * 250) + (winprct * 250), 0, 0), 1));
            
            
    }
    

    public void AddWin()
    {
        adjustedWins ++;
        UpdateWLT();
    }

    public void AddLoss()
    {
        adjustedLosses ++;
        UpdateWLT();
    }

    public void AddTie()
    {
        adjustedTies ++;
        UpdateWLT();
    }

    public void SubWin()
    {
        if (adjustedWins > 0)
        {
            adjustedWins --;
            UpdateWLT();
        }
    }

    public void SubLoss()
    {
        if (adjustedLosses > 0)
        {
            adjustedLosses --;
            UpdateWLT();
        } 
    }

    public void SubTie()
    {
        if (adjustedTies > 0)
        {
            adjustedTies --;
            UpdateWLT();
        }
    }

    public void RemoveTodaysRecord()
    {
        Sequence seq = DOTween.Sequence();
            seq.Append(RecordImage.rectTransform.DOSizeDelta(new Vector2(0, 24), 2f));
            seq.Join(RecordImage.rectTransform.DOAnchorPos(new Vector3(0, 0, 0), 2f));
            seq.Join(Record.DOFade(0, 0.7f));
    }

    public void RemoveInitButton()
    {
        initButton.gameObject.SetActive(false);
    }

    public void UpdateKD(float kd)
    {
        KD.text = "overall KD: " + kd.ToString("0.##");
    }


    public void UpdateGameDD(int index)
    {
        api.gameData.Game = GetGameFromNumber(index);
    }

    public string GetGameFromNumber(int index)
    {
        switch (index)
        {
            case 0:
                return "All";
            case 1:
                return "HaloCombatEvolved";
            case 2:
                return "Halo2";
            case 3:
                return "Halo2Anniversary";
            case 4:
                return "Halo3";
            case 5:
                return "Halo4";
        }
        return null;
    }


}
