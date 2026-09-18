namespace SaveSystem
{
    public class GameSaveInteractor
    {
        private readonly IGameSaveRepository _repository;
        private readonly IPlayerSaveState _player;

        public GameSaveInteractor(IGameSaveRepository repository, IPlayerSaveState player)
        {
            _repository = repository;
            _player = player;
        }

        public void Save()
        {
            _repository.Save(new SaveData.GameSaveData { Player = _player.Capture() });
        }

        public bool Load()
        {
            if (!_repository.TryLoad(out SaveData.GameSaveData data)) return false;
            
            _player.Restore(data.Player);
            return true;
        }
    }
}