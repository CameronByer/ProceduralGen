
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;

namespace Eldham
{
    class Player : Entity
    {
        float maxHorSpeed;
        float maxVertSpeed;
        float gravity;
        bool canJump;
        float friction;

        public Vector2 camera;
        public Vector2 curchunk;

        public Player(Texture2D texture) : base(texture, new Vector2(0, 0), new Vector2(32, 48))
        {
            maxHorSpeed = 8f;
            maxVertSpeed = 12f;
            gravity = 0.4f;
            canJump = false;

            camera = new Vector2(0, 0);
            curchunk = new Vector2(1, 1);
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, ref double[,][,] data, ref Tile[,][,] chunks, List<Projectile> proj, WorldGen worldGen)
        {
            friction = 0.95f;
            if (canJump)
            {
                friction = 0.85f;
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                speed.X -= 0.1f;
                if (speed.X < 0)
                {
                    friction = 1f;
                }
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                speed.X += 0.1f;
                if (speed.X > 0)
                {
                    friction = 1f;
                }
            }
            if (keyboardState.IsKeyDown(Keys.Space) && canJump)
            {
                speed.Y = -9f;
            }
            speed.X *= friction;
            capSpeed();
            updateGrav();
            updateTileCollision(chunks);
            pos += speed;
            updateChunks(ref data, ref chunks, worldGen);
            updateProjCollision(proj);

            camera = pos - new Vector2(Main.screenSize.X / 2 + Tile.SIZE, Main.screenSize.Y / 2 + Tile.SIZE);
        }

        public void Draw(SpriteBatch spriteBatch, Tile[,][,] chunks)
        {
            spriteBatch.Draw(texture, pos - camera, Color.White);

            int px = (int)Math.Floor(pos.X / Tile.SIZE);
            int py = (int)Math.Floor(pos.Y / Tile.SIZE);
            int xtiles = 1 + (int)Math.Floor(Main.screenSize.X / Tile.SIZE);
            int ytiles = 1 + (int)Math.Floor(Main.screenSize.Y / Tile.SIZE);
            int pwidth = (int)Math.Floor(dimensions.X / 2 / Tile.SIZE);
            int pheight = (int)Math.Floor(dimensions.Y / 2 / Tile.SIZE);

            for (int x = 0; x < xtiles; x++)
            {
                for (int y = 0; y < ytiles; y++)
                {
                    int blockx = px + x - xtiles / 2 - pwidth;
                    int blocky = py + y - ytiles / 2 - pheight;
                    int chunkx = 1 + (int)(Math.Floor((float)blockx / WorldGen.CHUNK_SIZE) - Math.Floor((float)px / WorldGen.CHUNK_SIZE));
                    int chunky = 1 + (int)(Math.Floor((float)blocky / WorldGen.CHUNK_SIZE) - Math.Floor((float)py / WorldGen.CHUNK_SIZE));
                    blockx = (blockx % WorldGen.CHUNK_SIZE + WorldGen.CHUNK_SIZE) % WorldGen.CHUNK_SIZE;
                    blocky = (blocky % WorldGen.CHUNK_SIZE + WorldGen.CHUNK_SIZE) % WorldGen.CHUNK_SIZE;
                    chunks[chunkx, chunky][blockx, blocky].Draw(spriteBatch, camera, new Vector2(Tile.SIZE * (blockx + WorldGen.CHUNK_SIZE * (chunkx - 1)), Tile.SIZE * (blocky + WorldGen.CHUNK_SIZE * (chunky - 1))));
                }
            }

            this.drawHealth(spriteBatch, camera, 50);
        }

        void capSpeed()
        {
            if (speed.X >= maxHorSpeed)
            {
                speed.X = maxHorSpeed;
            }
            if (speed.X <= -maxHorSpeed)
            {
                speed.X = -maxHorSpeed;
            }
            if (speed.Y >= maxVertSpeed)
            {
                speed.Y = maxVertSpeed;
            }
            if (speed.Y <= -maxVertSpeed)
            {
                speed.Y = -maxVertSpeed;
            }
        }

        void updateChunks(ref double[,][,] data, ref Tile[,][,] chunks, WorldGen worldGen)
        {
            Vector2 change = -new Vector2((float)Math.Floor((pos.X - speed.X) / Tile.SIZE / WorldGen.CHUNK_SIZE), (float)Math.Floor((pos.Y - speed.Y) / Tile.SIZE / WorldGen.CHUNK_SIZE));
            change = new Vector2((float)Math.Floor(pos.X / Tile.SIZE / WorldGen.CHUNK_SIZE), (float)Math.Floor(pos.Y / Tile.SIZE / WorldGen.CHUNK_SIZE));
            curchunk += change;
            pos -= change * Tile.SIZE * WorldGen.CHUNK_SIZE;
            double[,][,] newdata = new double[5, 5][,];
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (i + change.X >= 0 && i + change.X <= 4 && j + change.Y >= 0 && j + change.Y <= 4)
                    {
                        newdata[i, j] = data[i + (int)change.X, j + (int)change.Y];
                    }
                    else
                    {
                        newdata[i, j] = worldGen.genData(new Vector2(i + curchunk.X - 2, j + curchunk.Y - 2));
                    }
                }
            }
            Tile[,][,] newchunks = new Tile[3, 3][,];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i + change.X >= 0 && i + change.X <= 2 && j + change.Y >= 0 && j + change.Y <= 2)
                    {
                        newchunks[i, j] = chunks[i + (int)change.X, j + (int)change.Y];
                    }
                    else
                    {
                        Console.Out.WriteLine("" + i + " " + j);
                        newchunks[i, j] = worldGen.genChunk(new Vector2(i + curchunk.X - 1, j + curchunk.Y - 1));//worldGen.genChunk2(new Vector2(i + curchunk.X - 1, j + curchunk.Y - 1), new Vector2(i + 1, j + 1),  data);
                    }
                }
            }
            /*
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i + change.X >= 0 && i + change.X <= 2 && j + change.Y >= 0 && j + change.Y <= 2)
                    {
                        newchunks[i, j] = chunks[i + (int)change.X, j + (int)change.Y];
                    }
                    else
                    {
                        newchunks[i, j] = worldGen.genChunk(new Vector2(i + curchunk.X - 1, j + curchunk.Y - 1));
                    }
                }
            }*/
            chunks = newchunks;
        }

        void updateTileCollision(Tile[,][,] chunks)
        {
            Vector2 newSpeed = new Vector2(0f, 0f);
            Vector2 newPos = pos + speed;
            int left, top, right, bottom;
            bool col;

            for (int i = 1; i <= 5; i++)
            {
                left = (int)Math.Floor(newPos.X / Tile.SIZE);
                top = (int)Math.Floor(newPos.Y / Tile.SIZE);
                right = (int)Math.Floor((newPos.X + dimensions.X) / Tile.SIZE);
                bottom = (int)Math.Floor((newPos.Y + dimensions.Y) / Tile.SIZE);
                col = false;
                for (int x = left; x <= right; x++)
                {
                    for (int y = top; y <= bottom; y++)
                    {
                        int chunkx = (int)(Math.Floor((float)x / WorldGen.CHUNK_SIZE) + 1);
                        int chunky = (int)(Math.Floor((float)y / WorldGen.CHUNK_SIZE) + 1);
                        int blockx = (x % WorldGen.CHUNK_SIZE + WorldGen.CHUNK_SIZE) % WorldGen.CHUNK_SIZE;
                        int blocky = (y % WorldGen.CHUNK_SIZE + WorldGen.CHUNK_SIZE) % WorldGen.CHUNK_SIZE;

                        if (chunks[chunkx, chunky][blockx, blocky].isSolid())
                        {
                            col = true;
                        }
                    }
                }
                if (col)
                {
                    newPos -= new Vector2(speed.X / (float)Math.Pow(2, i), speed.Y / (float)Math.Pow(2, i));
                }
                else
                {
                    newSpeed += new Vector2(speed.X / (float)Math.Pow(2, i), speed.Y / (float)Math.Pow(2, i));
                }
            }
            col = false;
            newPos = new Vector2(pos.X + newSpeed.X, pos.Y + speed.Y);
            left = (int)(newPos.X / Tile.SIZE);
            top = (int)(newPos.Y / Tile.SIZE);
            right = (int)Math.Floor((newPos.X + dimensions.X) / Tile.SIZE);
            bottom = (int)Math.Floor((newPos.Y + dimensions.Y) / Tile.SIZE);
            for (int x = left; x <= right; x++)
            {
                for (int y = top; y <= bottom; y++)
                {
                    int chunkx = (int)(Math.Floor((float)x / WorldGen.CHUNK_SIZE) + 1);
                    int chunky = (int)(Math.Floor((float)y / WorldGen.CHUNK_SIZE) + 1);
                    int blockx = (x % WorldGen.CHUNK_SIZE + WorldGen.CHUNK_SIZE) % WorldGen.CHUNK_SIZE;
                    int blocky = (y % WorldGen.CHUNK_SIZE + WorldGen.CHUNK_SIZE) % WorldGen.CHUNK_SIZE;
                    if (chunks[chunkx, chunky][blockx, blocky].isSolid())
                    {
                        col = true;
                    }
                }
            }
            canJump = col && speed.Y >= 0;
            if (!col)
            {
                newSpeed.Y = speed.Y;
            }
            col = false;
            newPos = new Vector2(pos.X + speed.X, pos.Y + newSpeed.Y);
            left = (int)(newPos.X / Tile.SIZE);
            top = (int)(newPos.Y / Tile.SIZE);
            right = (int)Math.Floor((newPos.X + dimensions.X) / Tile.SIZE);
            bottom = (int)Math.Floor((newPos.Y + dimensions.Y) / Tile.SIZE);
            for (int x = left; x <= right; x++)
            {
                for (int y = top; y <= bottom; y++)
                {
                    int chunkx = (int)(Math.Floor((float)x / WorldGen.CHUNK_SIZE) + 1);
                    int chunky = (int)(Math.Floor((float)y / WorldGen.CHUNK_SIZE) + 1);
                    int blockx = (x % WorldGen.CHUNK_SIZE + WorldGen.CHUNK_SIZE) % WorldGen.CHUNK_SIZE;
                    int blocky = (y % WorldGen.CHUNK_SIZE + WorldGen.CHUNK_SIZE) % WorldGen.CHUNK_SIZE;
                    if (chunks[chunkx, chunky][blockx, blocky].isSolid())
                    {
                        col = true;
                    }
                }
            }
            if (!col)
            {
                newSpeed.X = speed.X;
            }
            speed = newSpeed;
        }

        void updateGrav()
        {
            if (!canJump)
            {
                speed.Y += gravity;
            }
        }

        void updateProjCollision(List<Projectile> proj)
        {
            Rectangle playerRect = new Rectangle(pos.ToPoint(), dimensions.ToPoint());
            Rectangle projRect;
            List<Projectile> collisions = new List<Projectile>();
            foreach (Projectile p in proj)
            {
                projRect = new Rectangle(p.getPos().ToPoint(), p.getDim().ToPoint());
                if (playerRect.Intersects(projRect))
                {
                    collisions.Add(p);
                }
            }
            foreach (Projectile p in collisions)
            {
                health -= 5;
                proj.Remove(p);
            }
        }
    }
}
