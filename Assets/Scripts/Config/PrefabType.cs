using System;
using UnityEngine;

namespace Config
{
    [Serializable]
    public enum PrefabType
    {
        None = 0,
        Cell = 1,
		CreateWindow = 2,
        MainUi = 3,
        PauseWindow = 4,
        Field = 5,
        Camera = 6
    }
}