using SFML.Window;

namespace Platformer_The_Game
{
    internal interface IEntity
    {
        void Initialize(params string[] args);
        Vector2f Pos { get; set; }
        // Vector2i Size { get; set; }
        void Update();
        void Draw();
    }

}