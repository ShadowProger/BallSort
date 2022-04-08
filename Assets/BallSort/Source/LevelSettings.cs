using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class LevelSettings
{
    public int containerSize;
    public List<LevelSettings.Container> containers = new List<Container>();
    public Status status;

    public void Load(XmlNode node)
    {
        containers.Clear();
        containerSize = int.Parse(node.Attributes["size"].Value);
        string level = node.InnerText;
        var strCons = level.Split('|');

        foreach (var strCon in strCons)
        {
            Container con = new Container();
            if (!string.IsNullOrEmpty(strCon))
            {
                con.balls = Array.ConvertAll(strCon.Split(','), int.Parse).ToList();
            }
            containers.Add(con);
        }
    }

    [Serializable]
    public class Container
    {
        public List<int> balls = new List<int>();
    }

    public enum Status 
    { 
        Closed,
        Open,
        Complete
    }
}