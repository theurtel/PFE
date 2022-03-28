using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataHandler : MonoBehaviour
{
    private int score;
    private int gold;
    private string previousStreakDate;
    //COMMENT:25/03/2022:HEURTEL: name of the current skin sprite
    private string currentSkin;  
    //COMMENT:25/03/2022:HEURTEL: unlockedSkins is a string where each character's index matches the index of the skin in skin.csv - if the char is 1, the skin is unlocked, if it's 0, the skin is locked       
    private string unlockedSkins;       

    public CSVHandler readCSV;

    //COMMENT:25/03/2022:HEURTEL: load Data from the PlayerPrefs and assign the values to the respective attributes
    public void loadData()
    {
        score = PlayerPrefs.GetInt("Score");
        gold = PlayerPrefs.GetInt("Gold");
        previousStreakDate = PlayerPrefs.GetString("Date");

        //COMMENT:25/03/2022:HEURTEL: if there is no existing "Skin" key (it's the first time the app is started), assign the default skin
        if(!PlayerPrefs.HasKey("Skin"))
            PlayerPrefs.SetString("Skin", "default");
        currentSkin = PlayerPrefs.GetString("Skin");

        //COMMENT:25/03/2022:HEURTEL: if it's the first time the app is started, assign the value 1 for the first skin of the CSV (default skin) and 0 for the others
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


    //COMMENT:25/03/2022:HEURTEL: this method says if the user already had his daily bonus
    public bool isNewDay(string currentDate)
    {
        if(previousStreakDate != currentDate)
            return true;
        return false;
    }

    //COMMENT:25/03/2022:HEURTEL: this method is called when the user gets his daily bonus, and saves the current date as the date he got his last bonus
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
