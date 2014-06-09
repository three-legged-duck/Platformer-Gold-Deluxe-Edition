using SFML.Window;

namespace Platformer_The_Game
{
    class OptionsMenu
    {
        public static MenuState CreateOptionsMenu(Game game, IState returnState)
        {
            string[] menuItems =
            {
                Utils.GetString("binding", game),
                Utils.GetString("video", game),
                Utils.GetString("sound", game),
                Utils.GetString("misc", game),
                Utils.GetString("back", game)
            };
            var options = new MenuState(game.MenuFont, "menuBg.bmp", true, menuItems);

            options.ItemSelected += delegate(object sender, MenuState.ItemSelectedEventArgs args)
            {
                switch (args.SelectedPos)
                {
                    case 0:
                        game.State = new KeyBindingState(options);
                        break;
                    case 1:
                        game.State = CreateVideoOptionsMenu(game, options);
                        break;
                    case 2:
                        game.State = CreateSoundOptionsMenu(game, options);
                        break;
                    case 3:
                        game.State = CreateMiscOptionsMenu(game, options);
                        break;
                    case 4:
                        game.State = returnState;
                        break;
                }
            };
            return options;
        }

        public static MenuState CreateSoundOptionsMenu(Game game, IState returnState)
        {
            string[] menuItems =
            {
                GetOptionText("musicVolume", game.Settings.MusicVolume, game),
                GetOptionText("fxVolume", game.Settings.FxVolume, game),
                                Utils.GetString("back", game)
            };
            var options = new MenuState(game.MenuFont, "menuBg.bmp", true, menuItems);

            options.ItemSelected += delegate(object sender, MenuState.ItemSelectedEventArgs args)
            {
                switch (args.SelectedPos)
                {
                    case 0:
                        game.Settings.MusicVolume = ChangeVolumeValue(game.Settings.MusicVolume);
                       options.ModifyElement(0,GetOptionText("musicVolume", game.Settings.MusicVolume, game));
                        game.ReloadMusicVolume();
                        break;
                    case 1:
                        game.Settings.FxVolume = ChangeVolumeValue(game.Settings.FxVolume);
                        options.ModifyElement(1,GetOptionText("fxVolume", game.Settings.FxVolume, game));
                        break;
                    case 2:
                        game.State = returnState;
                        break;
                }
            };
            return options;
        }

        public static MenuState CreateVideoOptionsMenu(Game game, IState returnState)
        {
            VideoModeManager vmm = new VideoModeManager(game);
            string[] menuItems =
            {
                GetVideoModeOptionText(game),
                GetResolutionOptionText(game),
                GetOptionText("drawTextures", game.Settings.DrawTextures, game),
                Utils.GetString("back", game)
            };
            var options = new MenuState(game.MenuFont, "menuBg.bmp", true, menuItems);

            options.ItemSelected += delegate(object sender, MenuState.ItemSelectedEventArgs args)
            {
                switch (args.SelectedPos)
                {
                    case 0:
                        switch (game.Settings.WindowType)
                        {
                            case Styles.Fullscreen:
                                game.Settings.WindowType = Styles.None;
                                break;
                            case Styles.None:
                                game.Settings.WindowType = Styles.Close;
                                break;
                            default:
                                game.Settings.WindowType = Styles.Fullscreen;
                                break;
                        }
                        game.RecreateWindow();
                        options.ModifyElement(0,GetVideoModeOptionText(game));
                        break;
                    case 1:
                        VideoMode newVideoMode = vmm.GetNextVideoMode();
                        game.Settings.VideoModeWidth = newVideoMode.Width;
                        game.Settings.VideoModeHeight = newVideoMode.Height;
                        game.RecreateWindow();
                        options.ModifyElement(1,GetResolutionOptionText(game));
                        break;
                    case 2:
                        game.Settings.DrawTextures = !game.Settings.DrawTextures;
                        options.ModifyElement(2,GetOptionText("drawTextures", game.Settings.DrawTextures,game));
                        break;
                    case 3:
                        game.State = returnState;
                        break;
                }
            };
            return options;
        }

        public static MenuState CreateMiscOptionsMenu(Game game, IState returnState)
        {
            string[] menuItems =
            {
                Utils.GetString("language", game) + " : " +
                (game.Settings.Language == Utils.Language.English ? Utils.Language.English.ToString() : Utils.Language.French.ToString()),
                Utils.GetString("settingsReset",game),
                Utils.GetString("back", game)
            };
            var options = new MenuState(game.MenuFont, "menuBg.bmp", true, menuItems);

            options.ItemSelected += delegate(object sender, MenuState.ItemSelectedEventArgs args)
            {
                switch (args.SelectedPos)
                {
                    case 0:
                        game.Settings.Language = game.Settings.Language == Utils.Language.English ? Utils.Language.French : Utils.Language.English;
                        options.ModifyElement(0,Utils.GetString("language", game) + " : " +
                                                              (game.Settings.Language == Utils.Language.English
                                                                  ? Utils.Language.English.ToString()
                                                                  : Utils.Language.French.ToString()));
                        break;
                    case 1:
                        game.Settings = new Settings();
                        break;
                    case 2:
                        game.State = returnState;
                        break;
                }
            };
            return options;
        }

        private static string GetOptionText(string key, bool value, Game g)
        {
            return Utils.GetString(key, g) + " : " + (value ? Utils.GetString("yes", g) : Utils.GetString("no", g));
        }

        private static string GetOptionText(string key, float value, Game g)
        {
            return Utils.GetString(key, g) + " : " + value.ToString("0");
        }
        private static string GetVideoModeOptionText(Game g)
        {
            string vMode;
            switch (g.Settings.WindowType)
            {
                case Styles.Fullscreen:
                    vMode = "fullscreen";
                    break;
                case Styles.None:
                    vMode = "borderless";
                    break;
                default:
                    vMode = "windowed";
                    break;
            }
            return Utils.GetString("videoMode", g) + " : " + Utils.GetString(vMode,g);
        }

        private static string GetResolutionOptionText(Game g)
        {
            return Utils.GetString("resolution",g) +" : " + g.Settings.VideoModeWidth + "x" + g.Settings.VideoModeHeight;
        }

        private static float ChangeVolumeValue(float f)
        {
            f += 25;
            if (f > 100)
            {
                f = 0;
            }
            return f;
        }
    }
}
