using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SharpDX.Direct2D1.Effects;

namespace Eldham
{
    public class WorldGen
    {
        public static int CHUNK_SIZE = 128;
        private int seed;


        public WorldGen(int seed)
        {
            this.seed = seed;
        }

        public double[,] genData(Vector2 pos)
        {
            Random random = new Random(seed + (int)pos.X + Int32.MaxValue / 2 * (int)pos.Y);

            double[,] data = new double[CHUNK_SIZE, CHUNK_SIZE];
            for (int i = 0; i < CHUNK_SIZE; i++)
            {
                for (int j = 0; j < CHUNK_SIZE; j++)
                {
                    data[i, j] = random.NextDouble();
                }
            }
            return data;
        }

        public Tile[,] genChunk2(Vector2 pos, Vector2 relPos, double[,][,] data)
        {
            Tile[,] chunk = Tile.grid(CHUNK_SIZE, CHUNK_SIZE);
            double[,][,] data2 = new double[5, 5][,];
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    data2[i, j] = new double[CHUNK_SIZE, CHUNK_SIZE];
                }
            }
            for (int i = 0; i < CHUNK_SIZE; i++)
            {
                for (int j = 0; j < CHUNK_SIZE; j++)
                {
                    for (int x = -2; x <= 2; x++)
                    {
                        for (int y = -2; y <= 2; y++)
                        {
                            int xindex = i + x;
                            int yindex = j + y;
                            int datax = (int)relPos.X;
                            int datay = (int)relPos.Y;
                            if (xindex < 0)
                            {
                                xindex += CHUNK_SIZE;
                                datax -= 1;
                            }
                            if (yindex < 0)
                            {
                                yindex += CHUNK_SIZE;
                                datay -= 1;
                            }
                            if (xindex >= CHUNK_SIZE)
                            {
                                xindex -= CHUNK_SIZE;
                                datax += 1;
                            }
                            if (yindex >= CHUNK_SIZE)
                            {
                                yindex -= CHUNK_SIZE;
                                datay += 1;
                            }
                            data2[datax, datay][i, j] += data[(int)relPos.X, (int)relPos.Y][xindex, yindex];
                        }
                    }
                }
            }
            return chunk;
        }

        public Tile[,] genChunk(Vector2 pos)
        {
            Console.Out.WriteLine(pos);

            Random random = new Random(seed + (int)pos.X + Int32.MaxValue / 2 * (int)pos.Y); //TOPLEFT
            Random rand2 = new Random(seed + (int)pos.X + 1 + Int32.MaxValue / 2 * (int)pos.Y); //TOPRIGHT
            Random rand3 = new Random(seed + (int)pos.X + Int32.MaxValue / 2 * (int)(pos.Y + 1)); //BOTLEFT
            Random rand4 = new Random(seed + (int)pos.X + 1 + Int32.MaxValue / 2 * (int)(pos.Y + 1)); //BOTRIGHT

            Tile[,] chunk = Tile.grid(CHUNK_SIZE, CHUNK_SIZE);
            double[,] data = new double[CHUNK_SIZE, CHUNK_SIZE];
            double[,] data2 = new double[CHUNK_SIZE, CHUNK_SIZE];
            double[,] data3 = new double[CHUNK_SIZE, CHUNK_SIZE];
            int border = 3;

            for (int i = 0; i <= CHUNK_SIZE / 2; i++)
            {
                for (int j = 0; j < border; j++)
                {
                    data[i, j] = random.NextDouble();
                    data[j, i] = random.NextDouble();
                    data[CHUNK_SIZE - i - 1, j] = rand2.NextDouble();
                    data[CHUNK_SIZE - j - 1, i] = rand2.NextDouble();
                    data[i, CHUNK_SIZE - j - 1] = rand3.NextDouble();
                    data[j, CHUNK_SIZE - i - 1] = rand3.NextDouble();
                    data[CHUNK_SIZE - i - 1, CHUNK_SIZE - j - 1] = rand4.NextDouble();
                    data[CHUNK_SIZE - j - 1, CHUNK_SIZE - i - 1] = rand4.NextDouble();
                }
            }
            for (int i = border; i < CHUNK_SIZE - border; i++)
            {
                for (int j = border; j < CHUNK_SIZE - border; j++)
                {
                    data[i, j] = random.NextDouble();
                }
            }
            for (int i = 0; i < CHUNK_SIZE; i++)
            {
                for (int j = 0; j < CHUNK_SIZE; j++)
                {
                    for (int x = -2; x <= 2; x++)
                    {
                        for (int y = -2; y <= 2; y++)
                        {
                            int xindex = i + x;
                            int yindex = j + y;
                            if (xindex < 0)
                            {
                                xindex = -xindex;
                            }
                            if (yindex < 0)
                            {
                                yindex = -yindex;
                            }
                            if (xindex >= CHUNK_SIZE)
                            {
                                xindex = -xindex + (CHUNK_SIZE * 2) - 1;
                            }
                            if (yindex >= CHUNK_SIZE)
                            {
                                yindex = -yindex + (CHUNK_SIZE * 2) - 1;
                            }
                            data2[i, j] += data[xindex, yindex];
                        }
                    }
                }
            }
            for (int i = 0; i < CHUNK_SIZE; i++)
            {
                for (int j = 0; j < CHUNK_SIZE; j++)
                {
                    for (int x = -2; x <= 2; x++)
                    {
                        for (int y = -2; y <= 2; y++)
                        {
                            int xindex = i + x;
                            int yindex = j + y;
                            if (xindex < 0)
                            {
                                xindex = -xindex;
                            }
                            if (yindex < 0)
                            {
                                yindex = -yindex;
                            }
                            if (xindex >= CHUNK_SIZE)
                            {
                                xindex = -xindex + (CHUNK_SIZE * 2) - 1;
                            }
                            if (yindex >= CHUNK_SIZE)
                            {
                                yindex = -yindex + (CHUNK_SIZE * 2) - 1;
                            }
                            data3[i, j] += data2[xindex, yindex];
                        }
                    }
                }
            }
            for (int i = 0; i < CHUNK_SIZE; i++)
            {
                for (int j = 0; j < CHUNK_SIZE; j++)
                {
                    double total = 0;
                    for (int x = -2; x <= 2; x++)
                    {
                        for (int y = -2; y <= 2; y++)
                        {
                            int xindex = i + x;
                            int yindex = j + y;
                            if (xindex < 0)
                            {
                                xindex = -xindex;
                            }
                            if (yindex < 0)
                            {
                                yindex = -yindex;
                            }
                            if (xindex >= CHUNK_SIZE)
                            {
                                xindex = -xindex + (CHUNK_SIZE * 2) - 1;
                            }
                            if (yindex >= CHUNK_SIZE)
                            {
                                yindex = -yindex + (CHUNK_SIZE * 2) - 1;
                            }
                            total += data3[xindex, yindex];
                        }
                    }
                    if (total <= Math.Pow(24.8, 3) / 2f)
                    {
                        chunk[i, j].set(0);
                    }
                }
            }
            return chunk;
        }
    }
}
