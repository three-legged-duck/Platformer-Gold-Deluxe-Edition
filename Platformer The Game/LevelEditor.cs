using SFML.Graphics;
using Gwen.Control;
using SFML.Window;
using Platformer_The_Game.GwenExtensions;
using System.Diagnostics;
using System;

namespace Platformer_The_Game
{
    internal class LevelEditor : IState
    {
        private Game _game;

        public string BgMusicName { get { return null; } }

        private View _view;

        // Gwen
        private readonly Canvas _gwenCanvas;
        private Gwen.Input.SFML _gwenInput;
        private ScrollControl entitySettingsPage;

        // Item management
        private bool _itemSelected;
        private Sprite _currentItem;
        
        // Placed item management
        private IEntity _placedSelected;


        // Level management
        private Level level = Level.CreateLevel();
        private Sprite backgroundSprite = new Sprite();

        // Entity Management
        int _currentArg;

        public LevelEditor(Game g)
        {
            _game = g;
            _view = new View(new Vector2f(0, 0), new Vector2f(_game.W.Size.X, _game.W.Size.Y));
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
            entities.SetSize(_gwenCanvas.Width, 200);
            entities.SetPosition(0, _gwenCanvas.Height - 200);
            Gwen.Texture tex = new Gwen.Texture(skin.Renderer);
            tex.Load(@"res\images\plateformes.png");

            var platforms = entities.AddPage(Utils.GetString("platforms",_game));
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

            var ents = entities.AddPage(Utils.GetString(Utils.GetString("entities",_game),_game));
            page = new ScrollControl(ents.Page);
            page.Dock = Gwen.Pos.Fill;

            ents = entities.AddPage(Utils.GetString("entitySettings", _game));
            page = entitySettingsPage = new ScrollControl(ents.Page);
            page.Dock = Gwen.Pos.Fill;
            NumericUpDown updown = new NumericUpDown(entitySettingsPage);
            updown.SetPosition(0, 0);
            updown.Value = updown.Min = 0;
            updown.ValueChanged += delegate(Base sender, EventArgs eventargs)
            {
                _currentArg = (int)((NumericUpDown)sender).Value;
            };
            TextBox argBox = new TextBox(page);
            argBox.SetPosition(200, 0);
            argBox.SubmitPressed += delegate(Base sender, EventArgs eventargs)
            {
                if (_placedSelected != null)
                {
                    var args = _placedSelected.Args;
                    args[_currentArg] = (sender as TextBox).Text;
                    _placedSelected.Initialize(args);
                }
            };

            var settingsPage = entities.AddPage(Utils.GetString("levelSettings", _game));
            page = new ScrollControl(settingsPage.Page);
            page.Dock = Gwen.Pos.Fill;
            TextBox background = new TextBox(page);
            background.SetPosition(0, 0);
            background.SubmitPressed += delegate(Base sender, EventArgs eventargs)
            {
                level.background = (sender as TextBox).Text;
                reloadBackground();
            };

            Button btn = new Button(_gwenCanvas);
            btn.Text = Utils.GetString("quit", _game);
            btn.Released += (sender, arguments) => {
                level.Save("customLevel");
                _game.State = Utils.CreateMainMenu(_game);
            };
        }

        private void reloadBackground()
        {
            backgroundSprite.Texture = _game.ResMan.GetTexture(level.background);
            backgroundSprite.Scale = new Vector2f(_game.W.DefaultView.Size.X / backgroundSprite.GetGlobalBounds().Width,
                _game.W.DefaultView.Size.Y / backgroundSprite.GetGlobalBounds().Height);
        }

        private void reloadEntitySettingsPage()
        {
            entitySettingsPage.Children.Clear();
            if (_placedSelected != null)
            {
                var argstype = _placedSelected.ArgsType;
                for (var i = 0; i < argstype.Length; i++)
                {
                    var _i = i;
                    var lbl = new Label(entitySettingsPage);
                    lbl.Text = argstype[i];
                    lbl.SetPosition(0, 50 * i);
                    var txtBox = new TextBox(entitySettingsPage);
                    txtBox.SetPosition(200, 50 * i);
                    txtBox.Text = _placedSelected.Args[i];
                    txtBox.SubmitPressed += delegate(Base sender, EventArgs args)
                    {
                        var arg = _placedSelected.Args;
                        arg[_i] = (sender as TextBox).Text;
                        _placedSelected.Initialize(arg);
                    };
                }
            }
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
            _game.W.SetView(_game.W.DefaultView);
            _game.W.Draw(backgroundSprite);
            _gwenCanvas.RenderCanvas();
            if (_itemSelected)
            {
                _game.W.Draw(_currentItem);
            }
            _game.W.SetView(_view);
            foreach (var ent in level.entities)
            {
                ent.Draw();
            }
            if (_placedSelected != null)
            {
                var shape = new RectangleShape();
                shape.OutlineColor = Color.Red;
                shape.OutlineThickness = 3;
                shape.Size = _placedSelected.Size;
                shape.Position = _placedSelected.Pos;
                shape.FillColor = Color.Transparent;
                _game.W.Draw(shape);
            }
            _game.W.SetView(_game.W.DefaultView);
        }
        // Useless... kinda.
        public void OnEvent(Settings.Action ev)
        {
            switch (ev)
            {
                case Settings.Action.Left:
                    _view.Move(new Vector2f(-1, 0));
                    break;
                case Settings.Action.Right:
                    _view.Move(new Vector2f(1, 0));
                    break;
                case Settings.Action.Up:
                    _view.Move(new Vector2f(0, -1));
                    break;
                case Settings.Action.Down:
                    _view.Move(new Vector2f(0, 1));
                    break;
            }

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
            else if (e.Y < _game.W.Size.Y - 200)
            {
                if (_itemSelected != false)
                {
                    _placedSelected = new Platform(_game, _game.W.MapPixelToCoords(new Vector2i(e.X - 15, e.Y - 15), _view));
                    reloadEntitySettingsPage();
                    level.entities.Add(_placedSelected);
                }
                else
                {
                    var mouseCoords = _game.W.MapPixelToCoords(new Vector2i(e.X, e.Y), _view);
                    foreach (IEntity ent in level.entities)
                    {
                        if (ent.Pos.X - 15 < mouseCoords.X && mouseCoords.X < ent.Pos.X + 15
                         && ent.Pos.Y - 15 < mouseCoords.Y && mouseCoords.Y < ent.Pos.Y + 15)
                        {
                            _placedSelected = ent;
                            reloadEntitySettingsPage();
                            break;
                        }
                    }
                }
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