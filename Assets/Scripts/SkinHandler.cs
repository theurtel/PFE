using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class SkinHandler : MonoBehaviour
{
    private TextMeshProUGUI txtButton;
    public ReadCSV readCSV;
    public DataHandler dataHandler;
    string path = "skins";

    [SerializeField] List<Button> buttonList = new List<Button>();

    public void Start()
    {
        readCSV.read(path);
        dataHandler.loadData();
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
    }
}
