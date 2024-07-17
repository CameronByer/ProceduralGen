using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Eldham
{
    class Entity
    {
        protected Texture2D texture;
        protected Vector2 dimensions;
        protected Vector2 pos;
        protected Vector2 speed;
        protected int maxHealth;
        protected int health;

        public Entity(Texture2D texture, Vector2 pos, Vector2 dimensions)
        {
            this.texture = texture;
            this.pos = pos;
            this.dimensions = dimensions;
            this.maxHealth = 100;
            this.health = 70;
        }

        protected void drawHealth(SpriteBatch spriteBatch, Vector2 camera, int width)
        {
            int red = (int)(width * (1 - ((float)health / maxHealth)));
            spriteBatch.Draw(Main.rectBase, new Rectangle((int)(pos.X - camera.X + (dimensions.X - width) / 2), (int)(pos.Y - camera.Y + dimensions.Y + 5), width, 4), Color.Red);
            spriteBatch.Draw(Main.rectBase, new Rectangle((int)(pos.X - camera.X + red + (dimensions.X - width) / 2), (int)(pos.Y - camera.Y + dimensions.Y + 5), width - red, 4), Color.Green);
        }
    }
}
