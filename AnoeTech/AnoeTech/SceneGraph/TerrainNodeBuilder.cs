﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace AnoeTech
{
    static class TerrainNodeBuilder
    {
        public static int maxSectionSize = 128;
        private static int numberOfSectionsWide, numberOfSectionsTall, totalWidth, totalHeight;
        private static float[,] _heightMapData;

        public static List<SGNHeightmap> BuildHeightmaps(Texture2D heightMap, float step, float scale)
        {
            _heightMapData = CreateHeightMatrix(heightMap, step, scale);                             // Create the initial giant heightmap
            numberOfSectionsWide = heightMap.Width / maxSectionSize;
            numberOfSectionsTall = heightMap.Height / maxSectionSize;
            totalWidth = heightMap.Width;
            totalHeight = heightMap.Height;
            StitchMatrices();
            
            List<SGNHeightmap> finishedHeightmaps = new List<SGNHeightmap>();           // Create a List to hold all the heightmaps sections
            float[,] tempMatrix = new float[maxSectionSize, maxSectionSize];
            
            for (int z = 0; z < numberOfSectionsTall; z++)
            {
                for (int x = 0; x < numberOfSectionsWide; x++)
                {
                    for (int innerX = 0; innerX < maxSectionSize; innerX++)
                        for (int innerZ = 0; innerZ < maxSectionSize; innerZ++)
                            tempMatrix[innerX, innerZ] = _heightMapData[innerX + z * maxSectionSize, innerZ + x * maxSectionSize];
                    finishedHeightmaps.Add(new SGNHeightmap(new Vector3((z * maxSectionSize * step) - z * step, 0, (-x * maxSectionSize * step) + x * step), tempMatrix, step, scale));
                    tempMatrix = new float[maxSectionSize, maxSectionSize];
                }
            }
            return finishedHeightmaps;
        }

        private static void StitchMatrices()
        {
            for (int z = maxSectionSize - 1; z < totalHeight - 1; z += maxSectionSize)
                for (int x = 0; x < totalWidth; x++)
                {
                    float average = (_heightMapData[x, z] + _heightMapData[x, z+1])/2;
                    _heightMapData[x, z] = _heightMapData[x, z+1] = average;
                }

            for (int x = maxSectionSize - 1; x < totalWidth - 1; x += maxSectionSize)
                for (int z = 0; z < totalHeight; z++)
                {
                    float average = (_heightMapData[x, z] + _heightMapData[x+1, z]) / 2;
                    _heightMapData[x, z] = _heightMapData[x+1, z] = average;
                }
        }

        public static float[,] CreateHeightMatrix(Texture2D heightmap, float step, float scale)
        {
            float minimumHeight = float.MaxValue;
            float maximumHeight = float.MinValue;

            Color[] heightMapColors = new Color[heightmap.Width * heightmap.Height];
            heightmap.GetData(heightMapColors);

            float[,] _heightData = new float[heightmap.Width, heightmap.Height];
            for (int x = 0; x < heightmap.Width; x++)
                for (int y = 0; y < heightmap.Height; y++)
                {
                    _heightData[x, y] = heightMapColors[x + y * heightmap.Width].R * scale;
                    if (_heightData[x, y] < minimumHeight) minimumHeight = _heightData[x, y];
                    if (_heightData[x, y] > maximumHeight) maximumHeight = _heightData[x, y];
                }

            /*for (int x = 0; x < heightmap.Width; x++)
                for (int y = 0; y < heightmap.Height; y++)
                    _heightData[x, y] *= scale;// (_heightData[x, y] - minimumHeight) / (maximumHeight - minimumHeight) * scale;
            */
            return _heightData;
        }
    }
}
