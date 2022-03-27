using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Unity.IO;
using System.Linq;


public class ReadCSV : MonoBehaviour
{
    private List<string> names = new List<string>();
    private List<bool> healths = new List<bool>();
    private List<int> ids = new List<int>();
    private List<int> prices = new List<int>();
    private int size=0;

    public void read(string path)
    {
        size=0;
        using(var reader = new StreamReader(new MemoryStream((Resources.Load(path) as TextAsset).bytes)))
        {
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                names.Add(values[0]);
                healths.Add(bool.Parse(values[1]));
                ids.Add(Int32.Parse(values[2]));
                prices.Add(Int32.Parse(values[3]));
                size+=1;
            }
        }
    }

    public int getSize()
    {
        return size;
    }

    public string getName(int id)
    {
        return names[id];
    }

    public bool isHealthy(int id)
    {
        return healths[id];
    }

    public int getPrice(int id)
    {
        return prices[id];
    }
}
