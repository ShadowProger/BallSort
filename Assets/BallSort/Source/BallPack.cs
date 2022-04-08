using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BallPack", menuName = "Settings/BallPack")]
public class BallPack : ScriptableObject
{
    public string packName;
    public GameObject ballPrefab;
    public List<Sprite> ballSprites;
}
