using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Eldham
{
    class Projectile : Entity
    {
        public Projectile(Texture2D texture, Vector2 pos, Vector2 speed) : base(texture, pos, new Vector2(texture.Width, texture.Height))
        {
            this.pos = pos;
            this.speed = speed;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 camera)
        {
            spriteBatch.Draw(texture, pos - camera, Color.White);
        }

        public Vector2 getPos()
        {
            return pos;
        }
        public Vector2 getDim()
        {
            return dimensions;
        }
    }
}
