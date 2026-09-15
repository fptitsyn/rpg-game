namespace Services
{
    public sealed class GameServices
    {
        public IAudioService AudioService { get; }

        public GameServices(IAudioService audioService)
        {
            AudioService = audioService;
        }
    }
}