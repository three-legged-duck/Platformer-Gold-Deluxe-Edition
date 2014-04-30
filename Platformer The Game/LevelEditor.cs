using SFML.Graphics;
using Gwen.Control;
using SFML.Window;
using Platformer_The_Game.GwenExtensions;

namespace Platformer_The_Game
{
    internal class LevelEditor : IState
    {
        private Game _game;

        public string BgMusicName { get { return null; } }
        private readonly Canvas _gwenCanvas;
        private Gwen.Input.SFML _gwenInput;
        private object ItemSelected;
        public LevelEditor(Game g)
        {
            this._game = g;
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
            var platforms = entities.AddPage("platforms");

            entities.SetSize(_gwenCanvas.Width, 200);
            entities.SetPosition(0, _gwenCanvas.Height - 200);
            Gwen.Texture tex = new Gwen.Texture(skin.Renderer);
            tex.Load(@"res\images\plateformes.png");

            var page = new ScrollControl(platforms.Page);
            page.Dock = Gwen.Pos.Fill;
            for (int y = 0; y < tex.Height; y += 30)
            {
                for (int x = 0; x < tex.Width; x += 30)
                {
                    ClippedImage img = new ClippedImage(page, tex);
                    img.SetTextureRect(x, y, 30, 30);
                    img.SetSize(30, 30);
                    img.SetPosition(x, y);
                    img.Clicked += new Base.GwenEventHandler<ClickedEventArgs>(img_Clicked);
                }
            }
            //Button btn = new Button(_gwenCanvas);
            //btn.Text = "Exit";
            //btn.Clicked += btn_Clicked;
        }

        void img_Clicked(Base sender, ClickedEventArgs arguments)
        {
            ClippedImage img = sender as ClippedImage;
            
        }

        void btn_Clicked(Base sender, ClickedEventArgs arguments)
        {
            _game.State = Utils.CreateMainMenu(_game);
        }

        public void Initialize(Game game)
        {
            RenderWindow m_Window = game.W;
            m_Window.KeyPressed += window_KeyPressed;
            m_Window.KeyReleased += window_KeyReleased;
            m_Window.MouseButtonPressed += window_MouseButtonPressed;
            m_Window.MouseButtonReleased += window_MouseButtonReleased;
            m_Window.MouseWheelMoved += window_MouseWheelMoved;
            m_Window.MouseMoved += window_MouseMoved;
            m_Window.TextEntered += window_TextEntered;
        }

        public void Update()
        {
        }

        public void Draw()
        {
            _gwenCanvas.RenderCanvas();
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
            _gwenInput.ProcessMessage(e);
        }

        void window_MouseWheelMoved(object sender, MouseWheelEventArgs e)
        {
            _gwenInput.ProcessMessage(e);
        }

        void window_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
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