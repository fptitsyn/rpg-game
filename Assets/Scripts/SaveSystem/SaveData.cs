using System;
using UnityEngine;

namespace SaveSystem
{
    public class SaveData
    {
        [Serializable]
        public sealed class PlayerSaveData
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public float Health;
            public float Mana;
        }

        [Serializable]
        public sealed class GameSaveData
        {
            public PlayerSaveData Player;
        }
    }
}