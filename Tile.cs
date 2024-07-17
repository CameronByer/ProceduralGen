using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Eldham
{
    public class Tile
    {
        public static int SIZE = 16;

        Vector2 dimensions;
        bool solid;
        bool active;
        Texture2D texture;

        public Tile(Vector2 dimensions, bool solid)
        {
            this.dimensions = dimensions;
            this.solid = solid;
            this.active = false;
        }

        public Tile()
        {
            active = false;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 camera, Vector2 pos)
        {
            if (active)
            {
                spriteBatch.Draw(texture, pos - camera, Color.White);
            }
        }

        public static Tile[,] grid(int width, int height)
        {
            Tile[,] tiles = new Tile[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tiles[x, y] = new Tile();
                }
            }
            return tiles;
        }

        public void set(int type)
        {
            active = true;
            solid = true;
            texture = Main.tileTextures[type];
        }

        public void kill()
        {
            active = false;
        }

        public bool isSolid()
        {
            return active && solid;
        }
    }
}

