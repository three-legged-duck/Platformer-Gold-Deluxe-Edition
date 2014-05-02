using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
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
                        game.State = CreateMiscOptionsMenu(game, options);
                        break;
                    case 3:
                        game.State = returnState;
                        break;
                }
            };
            return options;
        }

        public static MenuState CreateVideoOptionsMenu(Game game, IState returnState)
        {
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
                        options.MenuBtns[0].DisplayedString = GetVideoModeOptionText(game);
                        break;
                    case 1:
                        if (game.Settings.WindowWidth == 1920 && game.Settings.WindowHeight == 1080)
                        {
                            game.Settings.WindowWidth = 1366;
                            game.Settings.WindowHeight = 768;
                        }
                        else if (game.Settings.WindowWidth == 1366 && game.Settings.WindowHeight == 768)
                        {
                            game.Settings.WindowWidth = 800;
                            game.Settings.WindowHeight = 600;                            
                        }
                        else if (game.Settings.WindowWidth == 800 && game.Settings.WindowHeight == 600)
                        {
                            game.Settings.WindowWidth = 1920;
                            game.Settings.WindowHeight = 1080;
                        }
                        game.RecreateWindow();
                        options.MenuBtns[1].DisplayedString = GetResolutionOptionText(game);
                        break;
                    case 2:
                        game.Settings.DrawTextures = !game.Settings.DrawTextures;
                        options.MenuBtns[2].DisplayedString = GetOptionText("drawTextures", game.Settings.DrawTextures,
                            game);
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
                        options.MenuBtns[0].DisplayedString = Utils.GetString("language", game) + " : " +
                                                              (game.Settings.Language == Utils.Language.English
                                                                  ? Utils.Language.English.ToString()
                                                                  : Utils.Language.French.ToString());
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
            return Utils.GetString("resolution",g) +" : " + g.Settings.WindowWidth + "x" + g.Settings.WindowHeight;
        }
    }
}
