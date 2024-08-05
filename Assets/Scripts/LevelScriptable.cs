using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelScriptable", menuName = "ScriptableObjects/LvlScriptableObj", order = 1)]
public class LevelScriptable : ScriptableObject
{
    public List<LevelData> levels = new();
}

[System.Serializable]
public struct LevelData
{
    public Puzzle puzzle; //puzzle class referenced from game manager
    public PieceData[] pieceDatas; //array of piece data class 

    [Serializable]
    public class PieceData
    {
        public Vector3[] rotationValues;   //stored rotation values of specific pieces
        public GameObject[] prefabs;     //stored prefabs according to index
    }
}