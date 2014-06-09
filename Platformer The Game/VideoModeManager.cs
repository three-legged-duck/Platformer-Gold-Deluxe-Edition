using SFML.Window;

namespace Platformer_The_Game
{
    internal class VideoModeManager
    {
        private VideoMode[] modes;
        private int _pos;

        public VideoModeManager(Game game)
        {
            modes = VideoMode.FullscreenModes;
            for (int i = 0; i < modes.Length; i++)
            {
                if (modes[i].Width == game.W.Size.X && modes[i].Height == game.W.Size.Y)
                {
                    _pos = i;
                }
            }
        }

        private VideoMode GetCurrentVideoMode()
        {
            return modes[_pos];
        }

        public VideoMode GetNextVideoMode()
        {
            _pos += 1;
            if (_pos >= modes.Length) _pos = 0;
            return GetCurrentVideoMode();
        }
    }
}