using SaveSystem;

namespace Services
{
    public sealed class GameServices
    {
        public IAudioService AudioService { get; }
        public IGameSaveRepository SaveRepository { get; }

        public GameServices(IAudioService audioService, IGameSaveRepository saveRepository)
        {
            AudioService = audioService;
            SaveRepository = saveRepository;
        }
    }
}