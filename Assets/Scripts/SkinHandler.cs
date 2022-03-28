using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class SkinHandler : MonoBehaviour
{
    private TextMeshProUGUI txtButton;
    private TextMeshProUGUI txtGold;
    public CSVHandler readCSV;
    public DataHandler dataHandler;
    private string path = "skins";

    [SerializeField] 
    private List<Button> buttonList = new List<Button>();

    //COMMENT:26/03/2022:HEURTEL: when the Skin menu is loaded, checks for every skin if it's unlocked : if not, it displays its price on the button (else, the default value set in Unity is displayed)
    public void Start()
    {
        readCSV.read(path);
        dataHandler.loadData();
        //COMMENT:26/03/2022:HEURTEL: displays the player's money
        txtGold = GameObject.Find("/Canvas/gold").GetComponent<TextMeshProUGUI>();
        txtGold.text = dataHandler.getGold().ToString();
        int i=0;
        foreach(Button b in buttonList)
        {
            if(!dataHandler.isUnlocked(i))
            {
                txtButton = b.GetComponentInChildren<TextMeshProUGUI>();
                txtButton.text = readCSV.getPrice(i).ToString();
            }   
            ++i;
        }
    }

    //COMMENT:26/03/2022:HEURTEL: if the skin is unlocked, equips it, else if the player has enough gold to buy it, buys and equips it
    public void chooseSkin(int index)
    {
        readCSV.read(path);
        dataHandler.loadData();

        if(index >= readCSV.getSize())
            return ;

        if(dataHandler.isUnlocked(index))
        {
            dataHandler.changeSkin(readCSV.getName(index));
        }
        else if(dataHandler.getGold() >= readCSV.getPrice(index))
        {
            dataHandler.pay(readCSV.getPrice(index));
            dataHandler.unlockSkin(index);
            dataHandler.changeSkin(readCSV.getName(index));

            txtButton = buttonList[index].GetComponentInChildren<TextMeshProUGUI>();
            txtButton.text = "EQUIP";
        }
        txtGold.text = dataHandler.getGold().ToString();
    }
}
