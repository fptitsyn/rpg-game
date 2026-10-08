using System;
using UnityEngine;

namespace SaveSystem
{
    public sealed class SaveData
    {
        [Serializable]
        public sealed class PlayerSaveData
        {
            public Vector3 position;
            public Quaternion rotation;
            public float health;
            public float mana;
        }

        [Serializable]
        public sealed class GameSaveData
        {
            public PlayerSaveData player;
        }
    }
}