using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Threading;
using SFML.Graphics;
using SFML.Window;

namespace Platformer_The_Game
{
    internal class EndOfLevelState : IState
    {
        public EventHandler<MouseButtonEventArgs> MouseClickHandler;
        public EventHandler<MouseMoveEventArgs> MouseMoveHandler;

        private readonly Sprite _backgroundSprite;
        private Font _font;
        private bool _initialized;
        private Game _game;
        private List<Text> _menuBtns = new List<Text>();
        private Text[] _leaderboardNames = new Text[11];
        private Text[] _leaderboardScores = new Text[11];
        private Text _scoreText;
        private View _view;
        private IState _nextState;
        private int _nextmillis, _selectedPos, _playerScore;
        private uint _scoreCharacterSize;
        private string _apiUrl, _currentLevel;
        private HighscoresList _leaderboard;

        public string BgMusicName
        {
            get { return "eddsworldCreditsTheme.ogg"; }
        }

        // ReSharper disable InconsistentNaming
        public class Highscore
        {
            public string name { get; set; }
            public int score { get; set; }
        }

        public class HighscoresList
        {
            public List<Highscore> highscores { get; set; }
        }

        // ReSharper restore InconsistentNaming

        // ReSharper disable PossibleLossOfFraction
        public EndOfLevelState(Font font, IState nextState, string currentLevel, int playerScore)
        {
            _playerScore = playerScore;
            _currentLevel = currentLevel;
            _nextState = nextState;
            _font = font;
            MouseClickHandler = OnMousePressed;
            MouseMoveHandler = OnMouseMoved;
            Image backgroundImage = new Image(@"res\images\menuBg.png");
            Texture backgroundTexture = new Texture(backgroundImage);
            _backgroundSprite = new Sprite(backgroundTexture);
        }

        public void Initialize(Game game)
        {
            _game = game;
            _view = game.W.DefaultView;
            game.W.MouseButtonPressed += MouseClickHandler;
            game.W.MouseMoved += MouseMoveHandler;
            _nextmillis = Environment.TickCount + 150;
            _scoreCharacterSize = _game.MenuTextSize/2;
            if (_initialized) return;
            _menuBtns.Add(new Text(Utils.GetString("nextLevel", _game), _font));
            _menuBtns.Add(new Text(Utils.GetString("backMain", _game), _font));
            foreach (Text btn in _menuBtns)
            {
                btn.CharacterSize = game.MenuTextSize;
            }
            _leaderboardNames[0] = new Text(Utils.GetString("username", _game), _font, _scoreCharacterSize);
            _leaderboardScores[0] = new Text("Score", _font, _scoreCharacterSize);
            for (int i = 1; i < _leaderboardNames.Length; i++)
            {
                _leaderboardNames[i] = new Text("-", _font, _scoreCharacterSize);
            }
            for (int i = 1; i < _leaderboardScores.Length; i++)
            {
                _leaderboardScores[i] = new Text("-", _font, _scoreCharacterSize);
            }

            //Doesn't need to be updated each frame because the user cannot change the resolution during this state
            for (int i = 0; i < _menuBtns.Count; i++)
            {
                Text text = _menuBtns[i];
                text.CharacterSize = _game.MenuTextSize;
                uint iHeight = text.CharacterSize;
                text.Position = new Vector2f((_view.Size.X/40),
                    _view.Size.Y - (_view.Size.Y/10) - _menuBtns.Count*iHeight/2 + i*iHeight);
            }
            for (int i = 0; i < _leaderboardNames.Length; i++)
            {
                Text text = _leaderboardNames[i];
                text.Position = new Vector2f(_view.Size.X - (_view.Size.X/3),
                    _view.Size.Y - (_view.Size.Y/2) - _leaderboardNames.Length*_scoreCharacterSize/2 +
                    i*_scoreCharacterSize);
            }
            for (int i = 0; i < _leaderboardScores.Length; i++)
            {
                Text text = _leaderboardScores[i];
                text.Position = new Vector2f(_view.Size.X - (_view.Size.X/3) + 300,
                    _view.Size.Y - (_view.Size.Y/2) - _leaderboardScores.Length*_scoreCharacterSize/2 +
                    i*_scoreCharacterSize);
            }
            _scoreText = new Text(String.Format(Utils.GetString("yourScore", _game), _playerScore), _font,
                _scoreCharacterSize)
            {
                Position = new Vector2f((_view.Size.X/40),
                    _view.Size.Y - (_view.Size.Y/2) - _leaderboardNames.Length*_scoreCharacterSize/2)
            };

            if (_game.Settings.LocalLeaderboards)
            {
                _apiUrl = "http://localhost:5000/";
            }
            else
            {
                _apiUrl = "http://platformer.fr/api/";
            }

            _backgroundSprite.Scale = new Vector2f(_view.Size.X/_backgroundSprite.GetLocalBounds().Width,
                _view.Size.Y/_backgroundSprite.GetLocalBounds().Height);
            Thread scoresThread = new Thread(SetAndGetHighScores);
            scoresThread.Start();

            _initialized = true;
        }

        public void Update()
        {
        }

        public void Draw()
        {
            _game.W.SetView(_view);
            _game.W.Draw(_backgroundSprite);
            for (int i = 0; i < _menuBtns.Count; i++)
            {
                Text t = _menuBtns[i];
                if (i == _selectedPos)
                {
                    t.Color = new Color(t.Color.R, t.Color.G, t.Color.B, 255);
                }
                else
                {
                    t.Color = new Color(t.Color.R, t.Color.G, t.Color.B, 150);
                }
                _game.W.Draw(t);
            }
            foreach (Text name in _leaderboardNames)
            {
                _game.W.Draw(name);
            }
            foreach (Text score in _leaderboardScores)
            {
                _game.W.Draw(score);
            }
            _game.W.Draw(_scoreText);
        }

        public void Uninitialize()
        {
            _game.W.MouseButtonPressed -= MouseClickHandler;
            _game.W.MouseMoved -= MouseMoveHandler;
        }

        public void OnEvent(Settings.Action action)
        {
            if (_nextmillis > Environment.TickCount)
            {
                return;
            }
            switch (action)
            {
                case Settings.Action.Up:
                    _selectedPos = _selectedPos - 1;
                    break;
                case Settings.Action.Down:
                    _selectedPos = _selectedPos + 1;
                    break;
                case Settings.Action.Use:
                    if (_selectedPos == 0)
                    {
                        _game.State = _nextState;
                    }
                    else
                    {
                        _game.State = Utils.CreateMainMenu(_game);
                    }
                    break;
            }

            if (_selectedPos < 0)
            {
                _selectedPos = _menuBtns.Count - 1;
            }
            if (_selectedPos >= _menuBtns.Count)
            {
                _selectedPos = 0;
            }

            _nextmillis = Environment.TickCount + 150;
        }

        public void OnMousePressed(object sender, MouseButtonEventArgs args)
        {
            if (args.Button != Mouse.Button.Left)
            {
                return;
            }
            for (int i = 0; i < _menuBtns.Count; i++)
            {
                Vector2f mousePos = _game.W.MapPixelToCoords(new Vector2i(args.X, args.Y));
                FloatRect realRect = getRealRect(_menuBtns[i].GetGlobalBounds());
                if (realRect.Contains(mousePos.X, mousePos.Y))
                {
                    OnEvent(Settings.Action.Use);
                }
            }
        }

        public void OnMouseMoved(object sender, MouseMoveEventArgs args)
        {
            for (int i = 0; i < _menuBtns.Count; i++)
            {
                Vector2f mousePos = _game.W.MapPixelToCoords(new Vector2i(args.X, args.Y));
                FloatRect realRect = getRealRect(_menuBtns[i].GetGlobalBounds());
                if (realRect.Contains(mousePos.X, mousePos.Y))
                {
                    _selectedPos = i;
                }
            }
        }

        private FloatRect getRealRect(FloatRect fr)
        {
            return new FloatRect(fr.Left, fr.Top,
                fr.Width, fr.Height);
        }

        private void SetAndGetHighScores()
        {
            try
            {
                WebClient client = new WebClient();

                string sendResult =
                    client.DownloadString(_apiUrl + "insert/" + _currentLevel + "/" +
                                          _game.Settings.Username + "/" + _playerScore);
                if (sendResult.Contains("-1"))
                {
                    _scoreText.DisplayedString += "\n" +
                                                  String.Format(Utils.GetString("scoreTooLow", _game), _playerScore);
                }
                else
                {
                    _scoreText.DisplayedString += "\n" +
                                                  String.Format(Utils.GetString("scoreAdded", _game), _playerScore);
                }

                string jsonResult =
                    client.DownloadString(_apiUrl + "leaderboard/" + _currentLevel);
                _leaderboard = JsonConvert.DeserializeObject<HighscoresList>(jsonResult);
                _leaderboard.highscores = _leaderboard.highscores.OrderByDescending(o => o.score).ToList();
                for (int i = 0; i < _leaderboard.highscores.Count; i++)
                {
                    _leaderboardNames[i + 1].DisplayedString = _leaderboard.highscores[i].name;
                    _leaderboardScores[i + 1].DisplayedString = _leaderboard.highscores[i].score.ToString("####");
                }
            }
            catch (Exception e)
            {
                _scoreText.DisplayedString += "\n" + Utils.GetString("scoresError", _game) + "\n" + e.Message;
            }
        }
    }
}

// ReSharper restore PossibleLossOfFraction