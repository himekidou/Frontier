using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class UserDBMgr : MonoBehaviour
{

    // Use this for initialization
    public string GameMoney;
    public string RealMoney;
    public string Level;
    public string Exp;
    public string RankPoint;
    public string NormalGame1vs1Count;
    public string NormalGame1vs1vs1Count;
    public string NormalGame1vs1vs1vs1Count;
    public string NormalGame2vs2Count;
    public string RankGame1vs1Count;
    public string RankGame1vs1vs1Count;
    public string RankGame1vs1vs1vs1Count;
    public string RankGame2vs2Count;
    public string NormalGame1vs1Win;
    public string NormalGame1vs1vs1Win;
    public string NormalGame1vs1vs1vs1Win;
    public string NormalGame2vs2Win;
    public string RankGame1vs1Win;
    public string RankGame1vs1vs1Win;
    public string RankGame1vs1vs1vs1Win;
    public string RankGame2vs2Win;
    public string UserNickName;
    public string IntroduceMyself;
    public string[] ExpTabel;
    public string SceneCheck;

    private TouchScreenKeyboard mobileKeys;
    
    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void Update()
    {
        SetUIManager();
    }

    public void OnInputEvent()
    {
        mobileKeys = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false);
    }

    public void SetUIManager()
    {
        SceneCheck = SceneManager.GetActiveScene().name;

        if(SceneCheck == "lobby")
        {
            GameObject.Find("NickName_Lobby").GetComponent<TextMeshProUGUI>().text = UserNickName;
            GameObject.Find("Level_Lobby").GetComponent<TextMeshProUGUI>().text = Level;
            GameObject.Find("RealMoney_Lobby").GetComponent<TextMeshProUGUI>().text = RealMoney;
            GameObject.Find("GameMoney_Lobby").GetComponent<TextMeshProUGUI>().text = GameMoney;
        }
        else if(SceneCheck == "MyInformation")
        {
            int NormalWinRate;
            int RankWinRate;


            try
            {
                NormalWinRate = (Convert.ToInt32(NormalGame1vs1Win) / Convert.ToInt32(NormalGame1vs1Count));
            }
            catch (System.DivideByZeroException)
            {
                NormalWinRate = 0;
            }
            try
            {
                RankWinRate = (Convert.ToInt32(RankGame1vs1Win) / Convert.ToInt32(RankGame1vs1Count));
            }
            catch (System.DivideByZeroException)
            {
                RankWinRate = 0;
            }
            GameObject.Find("NormalWinPercent_Information").GetComponent<TextMeshProUGUI>().text = NormalWinRate.ToString() + "%";
            GameObject.Find("RankWinPercent_Information").GetComponent<TextMeshProUGUI>().text = RankWinRate.ToString() + "%";
        
            GameObject.Find("NormalCount_Information").GetComponent<TextMeshProUGUI>().text = NormalGame1vs1Count;
            GameObject.Find("NormalWin_Information").GetComponent<TextMeshProUGUI>().text = NormalGame1vs1Win;
            
            GameObject.Find("RankCount_Information").GetComponent<TextMeshProUGUI>().text = RankGame1vs1Count;
            GameObject.Find("RankWin_Information").GetComponent<TextMeshProUGUI>().text = RankGame1vs1Win;
            
            GameObject.Find("NickName_Information").GetComponent<TextMeshProUGUI>().text = UserNickName;
            GameObject.Find("Level_Information").GetComponent<TextMeshProUGUI>().text = "Lv." + Level;
            GameObject.Find("Exp_Information").GetComponent<TextMeshProUGUI>().text = ("Exp: " + Convert.ToInt32(Exp) + "/" + Convert.ToInt32(ExpTabel[Convert.ToInt32(Level) - 1]));
            /*
            if (mobileKeys.done)
            {
                GameObject.Find("IntroduceMyself_Information").GetComponent<FirebaseMgr>();
            }
            */
        }

    }

    

}
