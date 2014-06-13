using SFML.Graphics;
using Gwen.Control;
using SFML.Window;
using Platformer_The_Game.GwenExtensions;
using System.Diagnostics;

namespace Platformer_The_Game
{
    internal class LevelEditor : IState
    {
        private Game _game;

        public string BgMusicName { get { return null; } }
        
        // Gwen
        private readonly Canvas _gwenCanvas;
        private Gwen.Input.SFML _gwenInput;

        // Item management
        private bool _itemSelected;
        private Sprite _currentItem;
        

        // Level management

        public LevelEditor(Game g)
        {
            _game = g;
            var gwen = new Gwen.Renderer.SFML(_game.W);
            var skin = new Gwen.Skin.TexturedBase(gwen, @"res\images\DefaultSkin.png");

            // set default font
            var defaultFont = new Gwen.Font(gwen) { Size = 10, FaceName = "Arial Unicode MS" };

            // try to load, fallback if failed
            if (gwen.LoadFont(defaultFont))
            {
                gwen.FreeFont(defaultFont);
            }
            else // try another
            {
                defaultFont.FaceName = "Arial";
                if (gwen.LoadFont(defaultFont))
                {
                    gwen.FreeFont(defaultFont);
                }
                else // try default
                {
                    defaultFont.FaceName = "OpenSans.ttf";
                }
            }

            skin.SetDefaultFont(defaultFont.FaceName);
            defaultFont.Dispose();

            _gwenCanvas = new Canvas(skin);
            _gwenCanvas.SetSize((int)_game.W.Size.X, (int)_game.W.Size.Y);
            _gwenCanvas.ShouldDrawBackground = false;
            _gwenCanvas.KeyboardInputEnabled = true;

            _gwenInput = new Gwen.Input.SFML();
            _gwenInput.Initialize(_gwenCanvas, _game.W);
            
            TabControl entities = new TabControl(_gwenCanvas);
            var platforms = entities.AddPage(Utils.GetString("platforms",_game));

            entities.SetSize(_gwenCanvas.Width, 200);
            entities.SetPosition(0, _gwenCanvas.Height - 200);
            Gwen.Texture tex = new Gwen.Texture(skin.Renderer);
            tex.Load(@"res\images\plateformes.png");

            var page = new ScrollControl(platforms.Page);
            page.Dock = Gwen.Pos.Fill;
            for (int y = 0; y < tex.Height; y += 32)
            {
                for (int x = 0; x < tex.Width; x += 32)
                {
                    ClippedImage img = new ClippedImage(page, tex);
                    img.SetTextureRect(x, y, 31, 31);
                    img.SetSize(31, 31);
                    img.SetPosition(x, y);
                    img.Clicked += delegate(Base sender, ClickedEventArgs args)
                    {
                        Debug.WriteLine("{0},{1}", sender.Width, sender.Height);
                        _itemSelected = true;
                        _currentItem.TextureRect = new IntRect(sender.X, sender.Y, 31, 31);
                        _currentItem.Color = new Color(255, 255, 255, 127);
                    };
                }
            }
            Button btn = new Button(_gwenCanvas);
            btn.Text = "Exit";
            btn.Released += (sender, arguments) => {
                _game.State = Utils.CreateMainMenu(_game);
            };
        }

        public void Initialize(Game game)
        {
            RenderWindow mWindow = game.W;
            _currentItem = new Sprite(new Texture(@"res\images\plateformes.png"));
            mWindow.KeyPressed += window_KeyPressed;
            mWindow.KeyReleased += window_KeyReleased;
            mWindow.MouseButtonPressed += window_MouseButtonPressed;
            mWindow.MouseButtonReleased += window_MouseButtonReleased;
            mWindow.MouseWheelMoved += window_MouseWheelMoved;
            mWindow.MouseMoved += window_MouseMoved;
            mWindow.TextEntered += window_TextEntered;
        }

        public void Update()
        {
        }

        public void Draw()
        {
            _gwenCanvas.RenderCanvas();
            if (_itemSelected)
            {
                _game.W.Draw(_currentItem);
            }
        }
        // Useless... kinda.
        public void OnEvent(Settings.Action ev)
        {

        }

        void window_TextEntered(object sender, TextEventArgs e)
        {
            _gwenInput.ProcessMessage(e);
        }

        void window_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            _currentItem.Position = new Vector2f(e.X - 15, e.Y - 15);
            _gwenInput.ProcessMessage(e);
        }

        void window_MouseWheelMoved(object sender, MouseWheelEventArgs e)
        {
            _gwenInput.ProcessMessage(e);
        }

        void window_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == Mouse.Button.Right)
            {
                _itemSelected = false;
            }
            if (e.Y < _game.W.Size.Y - 200)
            {
                // Add to level
                // Draw on level.
            }
            _gwenInput.ProcessMessage(new Gwen.Input.SFMLMouseButtonEventArgs(e, true));
            
        }

        void window_MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            _gwenInput.ProcessMessage(new Gwen.Input.SFMLMouseButtonEventArgs(e, false));
        }

        void window_KeyReleased(object sender, KeyEventArgs e)
        {
            _gwenInput.ProcessMessage(new Gwen.Input.SFMLKeyEventArgs(e, false));
        }

        void window_KeyPressed(object sender, KeyEventArgs e)
        {
            _gwenInput.ProcessMessage(new Gwen.Input.SFMLKeyEventArgs(e, true));
        }

        void window_Resized(object sender, SizeEventArgs e)
        {
            _game.W.SetView(new View(new FloatRect(0f, 0f, e.Width, e.Height)));
            _gwenCanvas.SetSize((int)e.Width, (int)e.Height);
        }

        public void Uninitialize()
        {
            _game.W.KeyPressed -= window_KeyPressed;
            _game.W.KeyReleased -= window_KeyReleased;
            _game.W.MouseButtonPressed -= window_MouseButtonPressed;
            _game.W.MouseButtonReleased -= window_MouseButtonReleased;
            _game.W.MouseWheelMoved -= window_MouseWheelMoved;
            _game.W.MouseMoved -= window_MouseMoved;
            _game.W.TextEntered -= window_TextEntered;
        }
    }
}