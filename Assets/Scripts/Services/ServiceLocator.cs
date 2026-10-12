using SaveSystem;

namespace Services
{
    public sealed class ServiceLocator
    {
        public IAudioService AudioService { get; }
        public IGameSaveRepository SaveRepository { get; }

        public ServiceLocator(IAudioService audioService, IGameSaveRepository saveRepository)
        {
            AudioService = audioService;
            SaveRepository = saveRepository;
        }
    }
}