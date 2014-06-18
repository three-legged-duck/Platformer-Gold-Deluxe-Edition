using SFML.Window;
using SFML.Graphics;

namespace Platformer_The_Game
{
    internal interface IEntity
    {
        void Initialize(params string[] args);
        string[] Args { get; }
        string[] ArgsType { get; }

        Vector2f Size { get; }
        Vector2f Pos { get; set; }
        void Update();
        void Draw();
    }

}