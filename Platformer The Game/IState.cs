namespace Platformer_The_Game
{
    internal interface IState
    {
        string BgMusicName { get; }
        void Initialize(Game game);
        void Update();
        void Draw();
        void Uninitialize();
        void OnEvent(Settings.Action a);
    }
}