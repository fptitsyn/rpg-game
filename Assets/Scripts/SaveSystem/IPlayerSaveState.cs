namespace SaveSystem
{
    public interface IPlayerSaveState
    {
        SaveData.PlayerSaveData Capture();
        void Restore(SaveData.PlayerSaveData data);
    }
}