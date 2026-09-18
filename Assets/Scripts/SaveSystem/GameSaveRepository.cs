using System.IO;
using UnityEngine;

namespace SaveSystem
{
    public interface IGameSaveRepository
    {
        void Save(SaveData.GameSaveData data);
        bool TryLoad(out SaveData.GameSaveData data);
    }

    public sealed class FileGameSaveRepository : IGameSaveRepository
    {
        private readonly string _path;
        public static int CurrentSaveNumber = 1;

        public FileGameSaveRepository(string path)
        {
            _path = path;
        }
        
        public void Save(SaveData.GameSaveData data)
        {
            File.WriteAllText(_path, JsonUtility.ToJson(data, true));
            CurrentSaveNumber++;
        }

        public bool TryLoad(out SaveData.GameSaveData data)
        {
            if (!File.Exists(_path))
            {
                data = null;
                return false;
            }

            data = JsonUtility.FromJson<SaveData.GameSaveData>(File.ReadAllText(_path));
            return data != null;
        }
    }
}