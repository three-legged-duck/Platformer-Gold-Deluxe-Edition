using SFML.Graphics;
using Gwen.Control;
using SFML.Window;
using Platformer_The_Game.GwenExtensions;
using System.Diagnostics;
using System;
using System.Linq;

namespace Platformer_The_Game
{
    internal class LevelEditor : IState
    {
        enum SelectMode
        {
            Normal,
            Item
        }
        private Game _game;

        public string BgMusicName { get { return null; } }

        private View _view;

        // Gwen
        private readonly Canvas _gwenCanvas;
        private Gwen.Input.SFML _gwenInput;
        private ScrollControl _entitySettingsPage;
        private Button[] _typeButtons;

        SelectMode _mode = SelectMode.Normal;
        SelectMode mode
        {
            get { return _mode; }
            set
            {
                if (value == SelectMode.Normal)
                {
                    foreach (var btn in _typeButtons)
                    {
                        btn.ToggleState = false;
                    }
                }
                _mode = value;
            }
        }
        Vector2i AbsoluteMousePos;

        // Item management
        private string _currentItem;
        
        // Placed item management
        private IEntity _placedSelected;


        // Level management
        private Level level = Level.CreateLevel();
        private Sprite backgroundSprite = new Sprite();
        private Player player;

        // Entity Management
        int _currentArg;

        public LevelEditor(Game g)
        {
            player = new Player(g, null, level.startPos);
            _game = g;
            AbsoluteMousePos = new Vector2i();
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
            
            TabControl tabs = new TabControl(_gwenCanvas);
            tabs.SetSize(_gwenCanvas.Width, 200);
            tabs.SetPosition(0, _gwenCanvas.Height - 200);
            Gwen.Texture tex = new Gwen.Texture(skin.Renderer);
            tex.Load(@"res\images\plateformes.png");

            /*var platforms = tabs.AddPage(Utils.GetString("platforms",_game));
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
                        mode = SelectMode.Item;
                        _currentItem.TextureRect = new IntRect(sender.X, sender.Y, 31, 31);
                        _currentItem.Color = new Color(255, 255, 255, 127);
                    };
                }
            }*/

            var ents = tabs.AddPage(Utils.GetString(Utils.GetString("entities",_game),_game));
            var page = new ScrollControl(ents.Page);
            page.Dock = Gwen.Pos.Fill;
            var type = typeof(IEntity);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => !p.IsAbstract && !p.IsInterface && type.IsAssignableFrom(p));
            _typeButtons = new Button[types.Count()];
            var i = 0;
            foreach (Type t in types)
            {
                Type _t = t;
                var btn = _typeButtons[i] = new Button(page);
                btn.Text = t.Name;
                btn.SetToolTipText(t.FullName);
                btn.SetPosition((i % 3) * btn.Bounds.Right, (i / 3) * btn.Bounds.Height);
                btn.IsToggle = true;
                btn.ToggledOn += (sender, args) =>
                {
                    foreach (var bleigh in _typeButtons)
                    {
                        if (bleigh != btn) bleigh.ToggleState = false;
                    }
                    _currentItem = _t.FullName;
                    mode = SelectMode.Item;
                };
                btn.ToggledOff += (sender, args) =>
                {
                    mode = SelectMode.Normal;
                };
                i++;
            }

            ents = tabs.AddPage(Utils.GetString("entitySettings", _game));
            page = _entitySettingsPage = new ScrollControl(ents.Page);
            page.Dock = Gwen.Pos.Fill;
            NumericUpDown updown = new NumericUpDown(_entitySettingsPage);
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

            var settingsPage = tabs.AddPage(Utils.GetString("levelSettings", _game));
            page = new ScrollControl(settingsPage.Page);
            page.Dock = Gwen.Pos.Fill;
            Label lbl = new Label(page);
            lbl.Text = "Background Image : ";
            lbl.SetPosition(0, 0);
            TextBox background = new TextBox(page);
            background.SetPosition(lbl.Width, 0);
            background.SubmitPressed += delegate(Base sender, EventArgs eventargs)
            {
                level.background = (sender as TextBox).Text;
                reloadBackground();
            };
            lbl = new Label(page);
            lbl.Text = "Start Position : ";
            lbl.SetPosition(0, background.Bounds.Height + 5);
            TextBoxNumeric startPosX = new TextBoxNumeric(page);
            startPosX.SetPosition(lbl.Width, background.Bounds.Height + 5);
            startPosX.SubmitPressed += (sender, arguments) =>
            {
                level.startPos = new Vector2f(Convert.ToInt32(startPosX.Text), level.startPos.Y);
            };
            TextBoxNumeric startPosY = new TextBoxNumeric(page);
            startPosY.SetPosition(lbl.Width + startPosX.Width, background.Bounds.Height + 5);
            startPosY.SubmitPressed += (sender, arguments) =>
            {
                level.startPos = new Vector2f(level.startPos.X, Convert.ToInt32(startPosY.Text));
            };

            Button exitbtn = new Button(_gwenCanvas);
            exitbtn.Text = Utils.GetString("quit", _game);
            exitbtn.Released += (sender, arguments) => {
                level.Save("customLevel");
                _game.State = Utils.CreateMainMenu(_game);
            };
            var testBtn = new Button(_gwenCanvas);
            testBtn.Text = "Test";
            testBtn.SetPosition(exitbtn.Bounds.Width + 1, 0);
            testBtn.Released += (sender, arguments) =>
            {
                level.Save("customLevel");
                _game.State = new GameState("customlevel", level);
            };
            var openCustomLvl = new Button(_gwenCanvas);
            openCustomLvl.Text = "Open CustomLevel";
            openCustomLvl.SetPosition(testBtn.Bounds.Right + 1, 0);
            openCustomLvl.Released += (sender, arguments) =>
            {
                level = Level.LoadLevel(_game, "customLevel");
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
            _entitySettingsPage.Children.Clear();
            if (_placedSelected != null)
            {
                var argstype = _placedSelected.ArgsType;
                for (var i = 0; i < argstype.Length; i++)
                {
                    var _i = i;
                    var lbl = new Label(_entitySettingsPage);
                    lbl.Text = argstype[i];
                    lbl.SetPosition(0, 50 * i);
                    var txtBox = new TextBox(_entitySettingsPage);
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
            //_currentItem = new Sprite(new Texture(@"res\images\plateformes.png"));
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
            if (mode == SelectMode.Item)
            {
               // _game.W.Draw(_currentItem);
            }
            _game.W.SetView(_view);
            foreach (var ent in level.entities)
            {
                ent.Draw();
            }
            player.Pos = level.startPos;
            player.Draw();
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
            var mousePos = _game.W.MapPixelToCoords(AbsoluteMousePos, _view);
            Text posText = new Text("X : " +  mousePos.X + ", Y : " + mousePos.Y, 
                _game.MenuFont, _game.W.Size.Y / 40) { Position = new Vector2f(_game.W.Size.X - 200, 0) };
            _game.W.Draw(posText);
        }
        // Useless... kinda.
        public void OnEvent(Settings.Action ev)
        {
            switch (ev)
            {
                case Settings.Action.Left:
                    _view.Move(new Vector2f(-10, 0));
                    break;
                case Settings.Action.Right:
                    _view.Move(new Vector2f(10, 0));
                    break;
                case Settings.Action.Up:
                    _view.Move(new Vector2f(0, -10));
                    break;
                case Settings.Action.Down:
                    _view.Move(new Vector2f(0, 10));
                    break;
            }

        }

        void window_TextEntered(object sender, TextEventArgs e)
        {
            _gwenInput.ProcessMessage(e);
        }

        void window_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            AbsoluteMousePos = new Vector2i(e.X, e.Y);
            //_currentItem.Position = new Vector2f(e.X - 15, e.Y - 15);
            _gwenInput.ProcessMessage(e);
        }

        void window_MouseWheelMoved(object sender, MouseWheelEventArgs e)
        {
            _gwenInput.ProcessMessage(e);
        }

        void window_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            var mouseCoords = _game.W.MapPixelToCoords(new Vector2i(e.X, e.Y), _view);
            if (e.Button == Mouse.Button.Right && mode == SelectMode.Item)
            {
                mode = SelectMode.Normal;
            }
            else if (e.Button == Mouse.Button.Right)
            {
                foreach (IEntity ent in level.entities)
                {
                    if (ent.CollisionSprite.GetGlobalBounds().Intersects(
                        new FloatRect(mouseCoords.X - 10, mouseCoords.Y - 10, 20, 20))) {
                        if (_placedSelected == ent) _placedSelected = null;
                        level.entities.Remove(ent);
                        break;
                    }
                }
            }
            else if (mode == SelectMode.Item)
            {
                if (e.Y < _gwenCanvas.Height - 200)
                {
                    Type t = Type.GetType(_currentItem, false);
                    var info = t.GetConstructor(new Type[] { typeof(Game), typeof(Vector2f) });
                    var coords = _game.W.MapPixelToCoords(new Vector2i(e.X - 15, e.Y - 15), _view);
                    _placedSelected = (IEntity)info.Invoke(new object[] { _game, coords });
                    reloadEntitySettingsPage();
                    level.entities.Add(_placedSelected);
                }
            }
            else
            {
                foreach (IEntity ent in level.entities)
                {
                    if (ent.CollisionSprite.GetGlobalBounds().Intersects(
                        new FloatRect(mouseCoords.X - 10, mouseCoords.Y - 10, 20, 20)))
                    {
                        _placedSelected = ent;
                        break;
                    }
                }
                reloadEntitySettingsPage();
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
