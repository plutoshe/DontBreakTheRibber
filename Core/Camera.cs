﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DontBreakTheRubber.Core
{
    class Camera
    {
        public Matrix Transform { get; private set; }

        public void Follow(SpriteClass target)
        {

            var position = Matrix.CreateTranslation(
                //-target_position.X - (target_rectangle.Width / 2),
                0,
                -target.y - (target.textureHeight / 2), 0);

            var offset = Matrix.CreateTranslation(0, Game1.screenHeight / 2, 0);
            //Console.WriteLine("pos: " + position.ToString());
            //Console.WriteLine("offset: " + offset.ToString());

            Transform = position * offset;

            //var Transform = Matrix.CreateTranslation(100, 100, 0);

        }
    }
}
