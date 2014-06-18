using SFML.Window;
using SFML.Graphics;

namespace Platformer_The_Game
{
    internal interface IEntity
    {
        void Initialize(params string[] args);
        string[] Args { get; }
        string[] ArgsType { get; }

        /// <summary>
        /// Sprite used for the collision checks. Note that this
        /// is not necessarely the sprite shown on the screen.
        /// </summary>
        Sprite CollisionSprite { get; }
        Vector2f Size { get; }
        Vector2f Pos { get; set; }
        void Update();
        void Draw();
    }

}