﻿using System;
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

        public string background;
        public int startScore;
        public ISet<IEntity> entities = new HashSet<IEntity>();

        public event LoadProgressHandler LoadProgress;

        public static Level CreateLevel()
        {
            Level lvl = new Level();
            return lvl;
        }

        public static Level LoadLevel(Game game, string levelName)
        {
            Level level = new Level();
            FileStream f = File.Open(@"levels\" + levelName, FileMode.Open);

            #region header
            level.background = f.ReadString();
            level.startScore = (int)f.ReadVarInt();
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
            f.Close();
            return level;
        }

        public void Save(string levelName)
        {
            if (!Directory.Exists(@"levels")) Directory.CreateDirectory(@"levels");
            var f = File.Open(@"levels\" + levelName, FileMode.Create);
            f.WriteString(this.background);
            f.WriteVarInt(this.startScore);
            f.WriteVarInt(this.entities.Count);
            foreach (var ent in entities)
            {
                f.WriteString(ent.GetType().FullName);
                f.WriteVarInt((int)ent.Pos.X);
                f.WriteVarInt((int)ent.Pos.Y);
                f.WriteVarInt(ent.Args.Length);
                foreach (var arg in ent.Args)
                {
                    f.WriteString(arg);
                }
            }
            f.Flush();
            f.Close();
        }

        public class LoadProgressArgs : EventArgs
        {
            private int percents;
        }
    }
}