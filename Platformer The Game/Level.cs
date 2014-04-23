using System;
using System.Collections.Generic;
using System.IO;

namespace Platformer_The_Game
{
    /*class LevelJsonIntermediate
    {
        public ICollection
    }*/

    internal class Level
    {
        public delegate void LoadProgressHandler(LoadProgressArgs args);

        public HashSet<IEntity> entities = new HashSet<IEntity>();

        private Level(HashSet<IEntity> entities)
        {
            this.entities = entities;
        }


        public event LoadProgressHandler LoadProgress;

        public static Level LoadLevel(string levelName)
        {
            FileStream f = File.Open(levelName, FileMode.Open);
            while (f.CanRead)
            {
                ReadHeader(f);
                long x = f.ReadVarInt();
                long y = f.ReadVarInt();
//                new Platform(new Vector2f(x, y), new Vector2i(10, 10));
            }
            return null;
        }

        private static void ReadHeader(Stream stream)
        {
            while (stream.CanRead)
            {
                string myStr = stream.ReadString();
            }
        }

        public class LoadProgressArgs : EventArgs
        {
            private int percents;
        }
    }
}