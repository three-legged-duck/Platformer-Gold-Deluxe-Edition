using SFML.Window;

namespace Platformer_The_Game
{
    internal interface IEntity
    {
        Vector2f Pos { get; set; }
        // Vector2i Size { get; set; }
        void Update();
        void Draw();
    }
}