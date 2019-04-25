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
    public TMP_InputField emailIF;
    public TMP_InputField passwordIF;
    public Toggle botBool;
    public Button initButton;

    public TMP_Dropdown gameDD;
    public RectTransform content;


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
        
        var values = System.Enum.GetValues(typeof(gametypes.GameType));
        float i = 22;
        int c = 1;
        content.sizeDelta = new Vector2(content.sizeDelta.x, values.Length * i);

        foreach (var item in values)
        {
            DefaultControls.Resources resources = new DefaultControls.Resources();
            GameObject newToggle = DefaultControls.CreateToggle(resources);

            newToggle.transform.SetParent(content, false);
            //i = newToggle.transform.lossyScale.y;
            newToggle.transform.localPosition = new Vector2(10, 5 + (c * i));
            
            c++;
            
        }

        
        Debug.Log(i);

        
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

        if (!PlayerPrefs.HasKey("email"))
        {PlayerPrefs.SetString("email", "");}
        else
        {emailIF.text = PlayerPrefs.GetString("email");}

        if (!PlayerPrefs.HasKey("password"))
        {PlayerPrefs.SetString("password", "");}
        else
        {passwordIF.text = PlayerPrefs.GetString("password");}

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

    public void UpdateEmail()
    {
        PlayerPrefs.SetString("email", emailIF.text);
        PlayerPrefs.Save();
    }

    public void UpdatePassword()
    {
        PlayerPrefs.SetString("password", passwordIF.text);
        PlayerPrefs.Save();
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

    public void aaa ()
    {
        RecieveWLT(3,4,0);
    }

    public void bbb ()
    {
        RecieveWLT(0,4,2);
    }

    public void ccc ()
    {
        RecieveWLT(16,2,1);
    }
    
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
        adjustedWins += 1;
        UpdateWLT();
    }

    public void AddLoss()
    {
        adjustedLosses += 1;
        UpdateWLT();
    }

    public void AddTie()
    {
        adjustedTies += 1;
        UpdateWLT();
    }

    public void SubWin()
    {
        if (adjustedWins > 0)
        {
            adjustedWins -= 1;
            UpdateWLT();
        }
    }

    public void SubLoss()
    {
        if (adjustedLosses > 0)
        {
            adjustedLosses -= 1;
            UpdateWLT();
        } 
    }

    public void SubTie()
    {
        if (adjustedTies > 0)
        {
            adjustedTies -= 1;
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
        averageKD = (averageKD + kd) / 2;
        KD.text = "overall KD: " + averageKD.ToString("0.##");
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
