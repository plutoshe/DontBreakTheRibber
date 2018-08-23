using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterPump.Core
{
    class Camera
    {
        public Matrix Transform { get; private set; }

        public void Follow(Vector2 target_position, Rectangle target_rectangle)
        {
            
            var position = Matrix.CreateTranslation(
                //-target_position.X - (target_rectangle.Width / 2),
                0,
                -target_position.Y - (target_rectangle.Height / 2), 0);
            
            var offset = Matrix.CreateTranslation(0, Game1.ScreenHeight / 2, 0);
            Console.WriteLine("pos: " + position.ToString());
            Console.WriteLine("offset: " + offset.ToString());
            
            Transform = position * offset;
           
            //var Transform = Matrix.CreateTranslation(100, 100, 0);
            
        }
    }
}
