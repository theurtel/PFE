using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataHandler : MonoBehaviour
{
    private int score;
    private int gold;
    private string previousStreakDate;
    private string currentSkin;
    private string unlockedSkins;

    public ReadCSV readCSV;


    public void loadData()
    {
        score = PlayerPrefs.GetInt("Score");
        gold = PlayerPrefs.GetInt("Gold");
        previousStreakDate = PlayerPrefs.GetString("Date");

        if(!PlayerPrefs.HasKey("Skin"))
            PlayerPrefs.SetString("Skin", "default");
        currentSkin = PlayerPrefs.GetString("Skin");

        if(!PlayerPrefs.HasKey("UnlockedSkins"))
        {
            readCSV.read("skins");
            string skins = "1";
            for(int i=1 ; i<readCSV.getSize(); ++i)
            {
                skins += '0';
            }
            PlayerPrefs.SetString("UnlockedSkins", skins);
        }
        unlockedSkins = PlayerPrefs.GetString("UnlockedSkins");
    }

    public void resetData()
    {
        score=0;
        PlayerPrefs.SetInt("Score", score);
        gold=0;
        PlayerPrefs.SetInt("Gold", gold);
        previousStreakDate=null;
        PlayerPrefs.SetString("Date", previousStreakDate);
        currentSkin="default";
        PlayerPrefs.SetString("Skin", currentSkin);

        readCSV.read("skins");
        unlockedSkins = "1";
        for(int i=1 ; i<readCSV.getSize(); ++i)
        {
            unlockedSkins += '0';
        }
        PlayerPrefs.SetString("UnlockedSkins", unlockedSkins);
    }



    public bool isNewDay(string currentDate)
    {
        if(previousStreakDate != currentDate)
            return true;
        return false;
    }

    public void saveDate(string currentDate)
    {
        previousStreakDate = currentDate;
        PlayerPrefs.SetString("Date", previousStreakDate);
    }



    public string getSkin()
    {
        return currentSkin;
    }

    public void changeSkin(string skin)
    {
        PlayerPrefs.SetString("Skin", skin);
    }



    public int getScore()
    {
        return score;
    }

    public void winScore()
    {
        score += 10 ;
    }

    public void loseScore()
    {
        score -= 10 ;
    }

    public void saveScore()
    {
        PlayerPrefs.SetInt("Score", score);
    }



    public int getGold()
    {
        return gold;
    }
    
    public void addGold(int nb)
    {
        gold += nb;
    }

    public void pay(int price)
    {
        gold -= price;
        PlayerPrefs.SetInt("Gold", gold);
    }

    public void saveGold()
    {
        PlayerPrefs.SetInt("Gold", gold);
    }



    public bool isUnlocked(int index)
    {
        char c = unlockedSkins[index];
        if(c == '1')
           return true;
        return false;
    }

    public void unlockSkin(int index)
    {
        string skins = "";
        int i=0;
        foreach (char c in unlockedSkins)
        {
            if(i==index)
            {
                skins += '1';
            }
            else
            {
                skins += unlockedSkins[i];
            }
            ++i;
        }
        unlockedSkins = skins;
        PlayerPrefs.SetString("UnlockedSkins", unlockedSkins);
    }
}
