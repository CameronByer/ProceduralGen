using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Eldham
{
    public class Main : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState keyboardState;
        MouseState mouseState;

        public static Vector2 screenSize;

        public WorldGen worldGen;
        Player player;
        Tile[,][,] chunks;
        double[,][,] data;
        List<Projectile> proj;

        public static Texture2D projTex;
        public static Texture2D rectBase;

        public static List<Texture2D> tileTextures = new List<Texture2D>();

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            screenSize = new Vector2(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);

            rectBase = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            rectBase.SetData<Color>(new Color[] { Color.Wheat });

            proj = new List<Projectile>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            tileTextures = new List<Texture2D>();

            tileTextures.Add(Content.Load<Texture2D>("block"));

            projTex = Content.Load<Texture2D>("proj");

            player = new Player(Content.Load<Texture2D>("square"));

            Random randseed = new Random();
            int seed = randseed.Next();
            Console.Out.WriteLine("" + seed);
            worldGen = new WorldGen(seed);

            chunks = new Tile[3, 3][,];
            data = new double[5, 5][,];

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    data[i, j] = worldGen.genData(new Vector2(i - 1, j - 1));
                }
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {

                    chunks[i, j] = worldGen.genChunk(new Vector2(i, j));
                }
            }
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            if (keyboardState.IsKeyDown(Keys.W))
            {
                proj.Add(new Projectile(projTex, new Vector2(mouseState.X + player.camera.X, mouseState.Y + player.camera.Y), new Vector2(0, 0)));
            }
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                int x = (int)Math.Floor((mouseState.X + player.camera.X) / Tile.SIZE);
                int y = (int)Math.Floor((mouseState.Y + player.camera.Y) / Tile.SIZE);
                int chunkx = (int)Math.Floor((float)x / WorldGen.CHUNK_SIZE) + 1;
                int chunky = (int)Math.Floor((float)y / WorldGen.CHUNK_SIZE) + 1;
                x = (x % WorldGen.CHUNK_SIZE + WorldGen.CHUNK_SIZE) % WorldGen.CHUNK_SIZE;
                y = (y % WorldGen.CHUNK_SIZE + WorldGen.CHUNK_SIZE) % WorldGen.CHUNK_SIZE;
                chunks[chunkx, chunky][x, y].set(0);
            }
            if (mouseState.RightButton == ButtonState.Pressed)
            {
                int x = (int)Math.Floor((mouseState.X + player.camera.X) / Tile.SIZE);
                int y = (int)Math.Floor((mouseState.Y + player.camera.Y) / Tile.SIZE);
                int chunkx = (int)Math.Floor((float)x / WorldGen.CHUNK_SIZE) + 1;
                int chunky = (int)Math.Floor((float)y / WorldGen.CHUNK_SIZE) + 1;
                x = (x % WorldGen.CHUNK_SIZE + WorldGen.CHUNK_SIZE) % WorldGen.CHUNK_SIZE;
                y = (y % WorldGen.CHUNK_SIZE + WorldGen.CHUNK_SIZE) % WorldGen.CHUNK_SIZE;
                chunks[chunkx, chunky][x, y].kill();
            }

            player.Update(gameTime, keyboardState, ref data, ref chunks, proj, worldGen);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            player.Draw(spriteBatch, chunks);

            foreach (Projectile p in proj)
            {
                p.Draw(spriteBatch, player.camera);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
