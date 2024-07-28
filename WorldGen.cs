using System;
using System.IO;
using Microsoft.Xna.Framework;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace Eldham
{
    public class WorldGen
    {
        public const int CHUNK_SIZE = 128;
        private int seed;


        public WorldGen(int seed)
        {
            this.seed = seed;
        }

        public double[,,,] genData(Vector2 pos)
        {
            double[,,,] data = new double[5, 5, CHUNK_SIZE, CHUNK_SIZE];
            for (int chunkdx = -2; chunkdx <= 2; chunkdx++)
            {
                for (int chunkdy = -2; chunkdy <= 2; chunkdy++)
                {
                    Random random = new Random(seed + (int)(pos.X + chunkdx) + Int16.MaxValue / 2 * (int)(pos.Y + chunkdy));

                    for (int i = 0; i < CHUNK_SIZE; i++)
                    {
                        for (int j = 0; j < CHUNK_SIZE; j++)
                        {
                            data[chunkdx+2, chunkdy+2, i, j] = random.NextDouble();
                        }
                    }
                }
            }

            return data;
        }

        public Tile[,,,] genChunks(Vector2 pos, Vector2 plotSize)
        {

            Tile[,,,] chunks = new Tile[2 * (int)plotSize.X + 1, 2 * (int)plotSize.Y + 1, CHUNK_SIZE, CHUNK_SIZE];

            double[,,,] data = smoothData(genData(pos), 5);

            for (int chunkdx = -(int)plotSize.X; chunkdx <= (int)plotSize.X; chunkdx++)
            {
                for (int chunkdy = -(int)plotSize.Y; chunkdy <= (int)plotSize.Y; chunkdy++)
                {
                    for (int i = 0; i < CHUNK_SIZE; i++)
                    {
                        for (int j = 0; j < CHUNK_SIZE; j++)
                        {
                            chunks[chunkdx + 1, chunkdy + 1, i, j] = new Tile();
                            if (data[chunkdx + 2, chunkdy + 2, i, j] > 0.5f)
                            {
                                chunks[chunkdx + 1, chunkdy + 1, i, j].set(0);
                            }
                        }
                    }
                }
            }

            return chunks;
        }

        private double[,,,] smoothData(double[,,,] data, int iterations)
        {
            double[,,,] tempData = new double[5, 5, CHUNK_SIZE, CHUNK_SIZE];

            for (int iteration = 0; iteration < iterations; iteration++)
            {
                for (int chunkx = 0; chunkx < 3; chunkx++)
                {
                    for (int chunky = 0; chunky < 3; chunky++)
                    {
                        for (int i = 0; i < CHUNK_SIZE; i++)
                        {
                            for (int j = 0; j < CHUNK_SIZE; j++)
                            {
                                tempData[chunkx+1, chunky+1, i, j] = averageSurroundingData(data, chunkx+1, chunky+1, i, j);
                            }
                        }
                    }
                }
                data = (double[,,,])tempData.Clone();
            }

            return data;
        }

        private double averageSurroundingData(double[,,,] data, int xChunk, int yChunk, int x, int y)
        {
            double total = 0;
            int count = 0;
            for (int dx = -2; dx <= 2; dx++)
            {
                for (int dy = -2; dy <= 2; dy++)
                {
                    int xChunkIndex = xChunk;
                    int yChunkIndex = yChunk;

                    int xIndex = x + dx;
                    if (xIndex < 0)
                    {
                        xIndex += CHUNK_SIZE;
                        xChunkIndex -= 1;
                    }
                    if (xIndex >= CHUNK_SIZE)
                    {
                        xIndex -= CHUNK_SIZE;
                        xChunkIndex += 1;
                    }

                    int yIndex = y + dy;
                    if (yIndex < 0)
                    {
                        yIndex += CHUNK_SIZE;
                        yChunkIndex -= 1;
                    }
                    if (yIndex >= CHUNK_SIZE)
                    {
                        yIndex -= CHUNK_SIZE;
                        yChunkIndex += 1;
                    }

                    total += data[xChunkIndex, yChunkIndex, xIndex, yIndex];
                    count++;
                }
            }
            return total / count;
        }
    }
}
