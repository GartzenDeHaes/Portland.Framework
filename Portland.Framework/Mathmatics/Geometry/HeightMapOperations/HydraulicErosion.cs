using System;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using Microsoft.Xna.Framework;
#endif

namespace Portland.Mathmatics.Geometry
{
	public partial class HeightMap
	{
      public int iterations = 50000;
      //[Range(0, 1)]
      public float inertia = 0.1f;
      public float gravity = 4f;
      public float minSlope = 0.01f;
      public float capacityFactor = 8f;
      //[Range(0, 1)]
      public float depositionFactor = 0.1f;
      //[Range(0, 1)]
      public float erosionFactor = 0.1f;
      //[Range(0, 1)]
      public float evaporationFactor = 0.05f;
      public float erosionRadius = 5f;
      public float depositionRadius = 5f;
      public int dropletLifetime = 30;

      ErosionInfo erosionInfo;
      ErosionBrush erosionBrush;
      DepositionBrush depositionBrush;

      System.Random rng;

      public void ErodeTerrain(int seed)
      {
         rng = new System.Random(seed);

         erosionInfo = InitializeErosionInfo();

         erosionBrush = InitializeErosionBrush(_map);
         depositionBrush = InitializeDepositionBrush(_map);

         for (int i = 0; i < iterations; i++)
         {
            Droplet droplet = new Droplet(_map, erosionInfo, erosionBrush, depositionBrush);
            droplet.Update();
         }
      }

      ErosionBrush InitializeErosionBrush(float[,] heightMap)
      {
         int width = heightMap.GetLength(0);
         int height = heightMap.GetLength(1);

         ErosionBrush erosionBrush = new ErosionBrush(width, height);

         Vector2 position = new Vector2();
         Vector2 v = new Vector2();

         float[] xOffsets = new float[(int)erosionRadius * (int)erosionRadius * 4];
         float[] yOffsets = new float[(int)erosionRadius * (int)erosionRadius * 4];

         for (int x = 0; x < width; x++)
         {
            for (int y = 0; y < height; y++)
            {
               if (x < 0 || x >= width || y < 0 || y >= height) continue;

               int index = 0;

               position.X = x;
               position.Y = y;

               float weightSum = 0f;
               int numVertices = 0;
               for (int i = -(int)erosionRadius; i <= erosionRadius; i++)
               {
                  int xCoord = x + i;
                  for (int j = -(int)erosionRadius; j <= erosionRadius; j++)
                  {
                     int yCoord = y + j;

                     v.X = xCoord;
                     v.Y = yCoord;

                     if (xCoord < 0 || xCoord >= width || yCoord < 0 || yCoord >= height) continue;

                     if ((v - position).Magnitude <= erosionRadius)
                     {
                        weightSum += MathF.Max(0f, (float)(erosionRadius - (v - position).Magnitude));
                        numVertices++;

                        xOffsets[index] = i;
                        yOffsets[index] = j;

                        index++;
                     }
                  }
               }

               erosionBrush.erosionBrushWeights[y * width + x] = new float[numVertices];
               erosionBrush.erosionBrushVertices[y * width + x] = new float[numVertices];

               for (int n = 0; n < numVertices; n++)
               {
                  v.X = x + xOffsets[n];
                  v.Y = y + yOffsets[n];

                  float weight = MathF.Max(0, (float)(erosionRadius - (v - position).Magnitude)) / weightSum;
                  erosionBrush.erosionBrushVertices[y * width + x][n] = (float)(v.Y * width + v.X);
                  erosionBrush.erosionBrushWeights[y * width + x][n] = weight;
               }
            }
         }

         return erosionBrush;
      }

      DepositionBrush InitializeDepositionBrush(float[,] heightMap)
      {
         int width = heightMap.GetLength(0);
         int height = heightMap.GetLength(1);

         DepositionBrush depositionBrush = new DepositionBrush(width, height);

         Vector2 position = new Vector2();
         Vector2 v = new Vector2();

         float[] xOffsets = new float[(int)depositionRadius * (int)depositionRadius * 4];
         float[] yOffsets = new float[(int)depositionRadius * (int)depositionRadius * 4];

         for (int x = 0; x < width; x++)
         {
            for (int y = 0; y < height; y++)
            {
               if (x < 0 || x >= width || y < 0 || y >= height) continue;

               int index = 0;

               position.X = x;
               position.Y = y;

               float weightSum = 0f;
               int numVertices = 0;
               for (int i = -(int)depositionRadius; i <= depositionRadius; i++)
               {
                  int xCoord = x + i;
                  for (int j = -(int)depositionRadius; j <= depositionRadius; j++)
                  {
                     int yCoord = y + j;

                     v.X = xCoord;
                     v.Y = yCoord;

                     if (xCoord < 0 || xCoord >= width || yCoord < 0 || yCoord >= height) continue;

                     if ((v - position).Magnitude <= depositionRadius)
                     {
                        weightSum += MathF.Max(0f, (float)(depositionRadius - (v - position).Magnitude));
                        numVertices++;

                        xOffsets[index] = i;
                        yOffsets[index] = j;

                        index++;
                     }
                  }
               }

               depositionBrush.depositionBrushWeights[y * width + x] = new float[numVertices];
               depositionBrush.depositionBrushVertices[y * width + x] = new float[numVertices];

               for (int n = 0; n < numVertices; n++)
               {
                  v.X = x + xOffsets[n];
                  v.X = y + yOffsets[n];

                  float weight = MathF.Max(0, (float)(depositionRadius - (v - position).Magnitude)) / weightSum;
                  depositionBrush.depositionBrushVertices[y * width + x][n] = (float)(v.Y * width + v.X);
                  depositionBrush.depositionBrushWeights[y * width + x][n] = weight;
               }
            }
         }

         return depositionBrush;
      }

      ErosionInfo InitializeErosionInfo()
      {
         ErosionInfo erosionInfo;

         erosionInfo.inertia = inertia;
         erosionInfo.gravity = gravity;
         erosionInfo.minSlope = minSlope;
         erosionInfo.capacityFactor = capacityFactor;
         erosionInfo.depositionFactor = depositionFactor;
         erosionInfo.erosionFactor = erosionFactor;
         erosionInfo.evaporationFactor = evaporationFactor;
         erosionInfo.erosionRadius = erosionRadius;
         erosionInfo.dropletLifetime = dropletLifetime;
         erosionInfo.rng = rng;

         return erosionInfo;
      }

      class Droplet
      {
         Vector2 position;
         Vector2 direction;
         float height;
         float speed = 1f;
         float water = 1f;
         float sediment = 0f;


         float[,] heightMap;
         int mapWidth;
         int mapHeight;

         ErosionInfo erosionInfo;
         ErosionBrush erosionBrush;
         DepositionBrush depositionBrush;

         public Droplet(float[,] map, ErosionInfo info, ErosionBrush eBrush, DepositionBrush dBrush)
         {
            heightMap = map;
            erosionInfo = info;
            erosionBrush = eBrush;
            depositionBrush = dBrush;

            mapWidth = heightMap.GetLength(0);
            mapHeight = heightMap.GetLength(1);

            position = new Vector2(
                erosionInfo.rng.Next(0, mapWidth - 1),
                erosionInfo.rng.Next(0, mapHeight - 1)
            );

            direction = new Vector2(0f, 0f).Normalized;
         }

         public void Update()
         {
            for (int i = 0; i < erosionInfo.dropletLifetime; i++)
            {
               // Update direction and get current height
               direction = GetNewDirection();
               height = GetNewHeight();

               position += direction;

               // Check whether droplet has stopped moving or flowed over the edge of the map
               if ((direction.X == 0 && direction.Y == 0) || position.X < 0 || position.X >= mapWidth - 1 || position.Y < 0 || position.Y >= mapHeight - 1)
               {
                  break;
               }

               // Calculate new height using bilinear interpolation
               float heightDiff = GetNewHeight() - height;

               // Update sediment carrying capacity
               float sedimentCapacity = MathF.Max(-heightDiff, erosionInfo.minSlope) * speed * water * erosionInfo.capacityFactor;

               // If height diff > 0, the new position is uphill
               // If travelling uphill, or carrying more sediment than capacity, deposit sediment
               if (sediment > sedimentCapacity || heightDiff > 0)
               {
                  float sedimentDeposited = (sediment - sedimentCapacity) * erosionInfo.depositionFactor;
                  sediment -= sedimentDeposited;

                  // Deposit sediment to surrounding vertices
                  DepositSediment(sedimentDeposited);
               }
               // If height diff < 0, the new position is downhill
               else
               {
                  // Erode a fraction of the sediment carrying capacity
                  float sedimentEroded = MathF.Min((sedimentCapacity - sediment) * erosionInfo.erosionFactor, -heightDiff);
                  sediment += sedimentEroded;

                  // Erode sediment from surrounding vertices
                  ErodeSediment(sedimentEroded);
               }

               // Update speed
               speed = MathF.Max(MathF.Sqrt(speed * speed + heightDiff * erosionInfo.gravity), 0f);

               // Evaporate water
               water = water * (1 - erosionInfo.evaporationFactor);
            }
         }

         void DepositSediment(float sedimentDeposited)
         {
            int coordX = (int)position.X;
            int coordY = (int)position.Y;

            int brushIndex = coordY * mapWidth + coordX;

            for (int i = 0; i < depositionBrush.depositionBrushVertices[brushIndex].Length; i++)
            {
               int nodeIndex = (int)depositionBrush.depositionBrushVertices[brushIndex][i];
               int x = nodeIndex % mapWidth;
               int y = nodeIndex / mapWidth;

               float weight = depositionBrush.depositionBrushWeights[brushIndex][i];

               heightMap[x, y] += sedimentDeposited * weight;
            }
         }

         void ErodeSediment(float sedimentEroded)
         {
            int coordX = (int)position.X;
            int coordY = (int)position.Y;

            int brushIndex = coordY * mapWidth + coordX;

            for (int i = 0; i < erosionBrush.erosionBrushVertices[brushIndex].Length; i++)
            {
               int nodeIndex = (int)erosionBrush.erosionBrushVertices[brushIndex][i];
               int x = nodeIndex % mapWidth;
               int y = nodeIndex / mapWidth;

               float weight = erosionBrush.erosionBrushWeights[brushIndex][i];

               heightMap[x, y] -= sedimentEroded * weight;
            }
         }

         float GetNewHeight()
         {
            int coordX = (int)position.X;
            int coordY = (int)position.Y;

            float offsetX = (float)position.X - coordX;
            float offsetY = (float)position.Y - coordY;

            float heightNW = heightMap[coordX, coordY];
            float heightNE = heightMap[coordX + 1, coordY];
            float heightSW = heightMap[coordX, coordY + 1];
            float heightSE = heightMap[coordX + 1, coordY + 1];

            return heightNW * (1 - offsetX) * (1 - offsetY) + heightNE * offsetX * (1 - offsetY) + heightSW * (1 - offsetX) * offsetY + heightSE * offsetX * offsetY;
         }

         Vector2 GetNewDirection()
         {
            Vector2 gradient = GetGradient();

            return (direction * erosionInfo.inertia - gradient * (1 - erosionInfo.inertia)).Normalized;
         }

         Vector2 GetGradient()
         {
            int coordX = (int)position.X;
            int coordY = (int)position.Y;

            float offsetX = (float)position.X - coordX;
            float offsetY = (float)position.Y - coordY;

            return new Vector2(
                (heightMap[coordX + 1, coordY] - heightMap[coordX, coordY]) * (1 - offsetX) - (heightMap[coordX + 1, coordY + 1] - heightMap[coordX, coordY + 1]) * offsetX,
                (heightMap[coordX, coordY + 1] - heightMap[coordX, coordY]) * (1 - offsetY) - (heightMap[coordX + 1, coordY + 1] - heightMap[coordX + 1, coordY]) * offsetY
            );
         }
      }

      class ErosionBrush
      {
         public float[][] erosionBrushWeights;
         public float[][] erosionBrushVertices;

         public ErosionBrush(int width, int height)
         {
            erosionBrushWeights = new float[width * height][];
            erosionBrushVertices = new float[width * height][];
         }
      }

      class DepositionBrush
      {
         public float[][] depositionBrushWeights;
         public float[][] depositionBrushVertices;

         public DepositionBrush(int width, int height)
         {
            depositionBrushWeights = new float[width * height][];
            depositionBrushVertices = new float[width * height][];
         }
      }

      struct ErosionInfo
      {
         public float inertia;
         public float gravity;
         public float minSlope;
         public float capacityFactor;
         public float depositionFactor;
         public float erosionFactor;
         public float evaporationFactor;
         public float erosionRadius;
         public int dropletLifetime;

         public System.Random rng;
      }
   }
}
