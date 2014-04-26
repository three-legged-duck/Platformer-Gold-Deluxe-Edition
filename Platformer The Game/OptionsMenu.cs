using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                        game.State = CreateBindOptionsMenu(game,options);
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
                GetOptionText("fullscreen", game.Settings.Fullscreen, game),
                GetOptionText("drawTextures", game.Settings.DrawTextures, game),
                GetOptionText("drawHitboxes", game.Settings.DrawHitbox, game),
                Utils.GetString("back", game)
            };
            var options = new MenuState(game.MenuFont, "menuBg.bmp", true, menuItems);

            options.ItemSelected += delegate(object sender, MenuState.ItemSelectedEventArgs args)
            {
                switch (args.SelectedPos)
                {
                    case 0:

                        game.Settings.Fullscreen = !game.Settings.Fullscreen;
                        game.RecreateWindow();
                        options.MenuBtns[0].DisplayedString = GetOptionText("fullscreen", game.Settings.Fullscreen, game);
                        break;
                    case 1:
                        game.Settings.DrawTextures = !game.Settings.DrawTextures;
                        options.MenuBtns[1].DisplayedString = GetOptionText("drawTextures", game.Settings.DrawTextures,
                            game);
                        break;
                    case 2:
                        game.Settings.DrawHitbox = !game.Settings.DrawHitbox;
                        options.MenuBtns[2].DisplayedString = GetOptionText("drawHitboxes", game.Settings.DrawHitbox,
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
                        game.State = returnState;
                        break;
                }
            };
            return options;
        }

        private static MenuState CreateBindOptionsMenu(Game game, IState returnState)
        {
            string[] menuItems =
            {
                Utils.GetString("back", game)
            };
            var options = new MenuState(game.MenuFont, "menuBg.bmp", true, menuItems);

            options.ItemSelected += delegate(object sender, MenuState.ItemSelectedEventArgs args)
            {
                switch (args.SelectedPos)
                {
                    case 0:
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
    }
}
