using System;
using System.Collections.Generic;
using System.IO;
using SFML.Window;

namespace Platformer_The_Game
{
    /*class LevelJsonIntermediate
    {
        public ICollection
    }*/

    internal class Level
    {
        public delegate void LoadProgressHandler(LoadProgressArgs args);

        private Level()
        {
        }

        Vector2f size;
        string background;
        int parTime;
        public ISet<IEntity> entities = new HashSet<IEntity>();

        public event LoadProgressHandler LoadProgress;

        public static Level CreateLevel(Vector2f size)
        {
            Level lvl = new Level();
            lvl.size = size;
            return lvl;
        }

        public static Level LoadLevel(Game game, string levelName)
        {
            Level level = new Level();
            FileStream f = File.Open(levelName, FileMode.Open);

            #region header
            level.size = new Vector2f(f.ReadVarInt(), f.ReadVarInt());
            level.background = f.ReadString();
            level.parTime = (int)f.ReadVarInt();
            #endregion header
            #region entities
            var entCount = f.ReadVarInt();
            for (int i = 0; i < entCount; i++)
            {
              Type t = Type.GetType(f.ReadString(), false);
              if (t == null || t.GetInterface("IEntity") == null)
              {
                // Throw an exception
                return null;
              }
              var info = t.GetConstructor(new Type[] { typeof(Game), typeof(Vector2f), typeof(string[]) });
              long x = f.ReadVarInt();
              long y = f.ReadVarInt();
              long argCount = f.ReadVarInt();
              var args = new string[argCount];
              for (i = 0; i < argCount; i++)
              {
                 args[i] = f.ReadString();
              }
              level.entities.Add((IEntity)info.Invoke(new object[] { game, new Vector2f(x, y), args }));
            }
            #endregion entities
            return level;
        }

        public class LoadProgressArgs : EventArgs
        {
            private int percents;
        }
    }
}