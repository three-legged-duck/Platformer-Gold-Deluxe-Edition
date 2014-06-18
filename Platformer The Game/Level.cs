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

        public string background;
        public int startScore;
        public ISet<IEntity> entities = new HashSet<IEntity>();
        public Vector2f startPos;
        public Vector2f endPos;

        public event LoadProgressHandler LoadProgress;

        public static Level CreateLevel()
        {
            Level lvl = new Level();
            return lvl;
        }
        const int VERSION = 3;
        public static Level LoadLevel(Game game, string levelName)
        {
            Level level = new Level();
            FileStream f = File.Open(@"res\levels\" + levelName, FileMode.Open);
            var version = f.ReadVarInt();
            if (version == 1)
            {
                LoadVersion1(game, level, f);
            }
            else if (version == 2)
            {
                LoadVersion2(game, level, f);
            }
            else if (version == 3)
            {
                LoadVersion3(game, level, f);
            }
            f.Close();
            return level;
        }

        private static void LoadVersion1(Game game, Level level, FileStream f)
        {
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
                  throw new Exception("Entity is not an entity");
              }
              var info = t.GetConstructor(new Type[] { typeof(Game), typeof(Vector2f), typeof(string[]) });
              long x = f.ReadVarInt();
              long y = f.ReadVarInt();
              long argCount = f.ReadVarInt();
              var args = new string[argCount];
              for (int j = 0; j < argCount; j++)
              {
                 args[j] = f.ReadString();
              }
              level.entities.Add((IEntity)info.Invoke(new object[] { game, new Vector2f(x, y), args }));
            }
            #endregion entities
        }

        private static void LoadVersion2(Game game, Level level, FileStream f)
        {
            level.startPos = new Vector2f((float)f.ReadVarInt(), (float)f.ReadVarInt());
            LoadVersion1(game, level, f);
        }

        private static void LoadVersion3(Game game, Level level, FileStream f)
        {
            level.endPos = new Vector2f((float)f.ReadVarInt(), (float)f.ReadVarInt());
            LoadVersion2(game, level, f);
        }

        public void Save(string levelName)
        {
            if (!Directory.Exists(@"res\levels")) Directory.CreateDirectory(@"res\levels");
            var f = File.Open(@"res\levels\" + levelName, FileMode.Create);
            f.WriteVarInt(VERSION);
            f.WriteVarInt((int)this.endPos.X);
            f.WriteVarInt((int)this.endPos.Y);
            f.WriteVarInt((int)this.startPos.X);
            f.WriteVarInt((int)this.startPos.Y);
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