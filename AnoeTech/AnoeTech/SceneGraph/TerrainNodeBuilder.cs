using System;
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
        private static int numberOfSectionsWide, numberOfSectionsTall;
        public static int totalWidth, totalHeight;
        public static float terrainStep, terrainScale;
        public static float minHeight, maxHeight;
        private static float[,] _heightMapData;
        private static VertexMultitextured[] vertices;
        public static int[] indices;


        public static List<SGNHeightmap> BuildHeightmaps(Texture2D heightMap, float step, float scale)
        {
            terrainStep = step;
            terrainScale = scale;
            numberOfSectionsWide = heightMap.Width / maxSectionSize;
            numberOfSectionsTall = heightMap.Height / maxSectionSize;
            totalWidth = heightMap.Width;
            totalHeight = heightMap.Height;

            _heightMapData = CreateHeightMatrix(heightMap, scale);                             // Create the initial giant heightmap
            vertices = new VertexMultitextured[totalWidth * totalHeight];
            StitchMatrices();                                                                  // Prep the matrix for division
            CalculateVertices(vertices);
            //CalculateIndices();
            //CalculateNormals();

            List<SGNHeightmap> finishedHeightmaps = new List<SGNHeightmap>();           // Create a List to hold all the heightmaps sections
            float[,] tempMatrix = new float[maxSectionSize, maxSectionSize];
            VertexMultitextured[] partial = new VertexMultitextured[maxSectionSize * maxSectionSize];
            
            for (int z = 0; z < numberOfSectionsTall; z++)
            {
                for (int x = 0; x < numberOfSectionsWide; x++)
                {
                    for (int innerX = 0; innerX < maxSectionSize; innerX++)
                        for (int innerZ = 0; innerZ < maxSectionSize; innerZ++)
                        {
                            tempMatrix[innerX, innerZ] = _heightMapData[innerX + z * maxSectionSize, innerZ + x * maxSectionSize];
                            partial[innerZ + (innerX * maxSectionSize)] = vertices[z * maxSectionSize + innerZ + x * maxSectionSize];
                        }
                    SGNHeightmap temp = new SGNHeightmap(new Vector3((z * maxSectionSize * step) - z * step, 0, (-x * maxSectionSize * step) + x * step), tempMatrix, step, scale);                   
                    temp.SetTextureCoords(partial);
                    finishedHeightmaps.Add(temp);
                    tempMatrix = new float[maxSectionSize, maxSectionSize];
                }
            }
            return finishedHeightmaps;
        }

        /// <summary>
        /// Takes the current height map data and Interpolates the values at the "seams" so that all the sections match up
        /// </summary>
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

        /// <summary>
        /// Create a 2D matrix of heights from a grayscale heightmap and a scaling factor.
        /// </summary>
        /// <param name="heightmap">An XNA.Textured2D file to be used as the heightmap</param>
        /// <param name="scale">The scaling factor to adjust the heights</param>
        /// <returns>A 2D matrix of floats representing the heights of the heightmap</returns>
        public static float[,] CreateHeightMatrix(Texture2D heightmap, float scale)
        {
            minHeight = float.MaxValue;                                                         // Set the minimum height to the maximum number of a float
            maxHeight = float.MinValue;                                                         // Set the maximum height to the minimum number of a float

            Color[] heightMapColors = new Color[heightmap.Width * heightmap.Height];            // Create a matrix to the hold the color data of the heightmap
            heightmap.GetData(heightMapColors);                                                 // Grab all the color data from the image

            float[,] _heightData = new float[heightmap.Width, heightmap.Height];                // Create a matrix to hold the heights
            for (int x = 0; x < heightmap.Width; x++)                                           // For every column
                for (int y = 0; y < heightmap.Height; y++)                                      // Read down the column
                {
                    _heightData[x, y] = heightMapColors[x + y * heightmap.Width].R * scale;     // Take the first byte of color information and multiply it by the scale and save it in the matrix of heights
                    if (_heightData[x, y] < minHeight) minHeight = _heightData[x, y];           // Keep track of the minimum height of the file
                    if (_heightData[x, y] > maxHeight) maxHeight = _heightData[x, y];           // Keep track of the maximum height of the file
                }
            return _heightData;                                                                 // Return the height matrix
        }

        private static void CalculateVertices(VertexMultitextured[] vertices)
        {
            
            
            for (int y = 0; y < totalWidth; y++)
                for (int x = 0; x < totalHeight; x++)
                {
                    vertices[x + y * totalWidth].Position = new Vector3(y*terrainStep, _heightMapData[x, y], -x*terrainStep);
                    vertices[x + y * totalWidth].TextureCoordinate.X = (float)x * terrainStep / 256.0f;
                    vertices[x + y * totalWidth].TextureCoordinate.Y = (float)y * terrainStep / 256.0f;

                    vertices[x + y * totalWidth].TexWeights.X = MathHelper.Clamp(1.0f - Math.Abs(_heightMapData[x, y] - 0) / (maxHeight / 5), 0, 1);
                    vertices[x + y * totalWidth].TexWeights.Y = MathHelper.Clamp(1.0f - Math.Abs(_heightMapData[x, y] - maxHeight * 0.2f) / (maxHeight / 4), 0, 1);
                    vertices[x + y * totalWidth].TexWeights.Z = MathHelper.Clamp(1.0f - Math.Abs(_heightMapData[x, y] - maxHeight * 0.6f) / (maxHeight / 5), 0, 1);
                    vertices[x + y * totalWidth].TexWeights.W = MathHelper.Clamp(1.0f - Math.Abs(_heightMapData[x, y] - maxHeight) / (maxHeight / 4), 0, 1);

                    float total = vertices[x + y * totalWidth].TexWeights.X;
                    total += vertices[x + y * totalWidth].TexWeights.Y;
                    total += vertices[x + y * totalWidth].TexWeights.Z;
                    total += vertices[x + y * totalWidth].TexWeights.W;

                    vertices[x + y * totalWidth].TexWeights.X /= total;
                    vertices[x + y * totalWidth].TexWeights.Y /= total;
                    vertices[x + y * totalWidth].TexWeights.Z /= total;
                    vertices[x + y * totalWidth].TexWeights.W /= total;
                }
        }

        private static void CalculateIndices()
        {
            indices = new int[(totalWidth - 1) * (totalHeight - 1) * 6];
            int counter = 0;
            for (int y = 0; y < totalHeight - 1; y++)
                for (int x = 0; x < totalWidth - 1; x++)
                {
                    int lowerLeft = x + y * totalWidth;
                    int lowerRight = (x + 1) + y * totalWidth;
                    int topLeft = x + (y + 1) * totalWidth;
                    int topRight = (x + 1) + (y + 1) * totalWidth;

                    indices[counter++] = topLeft;
                    indices[counter++] = lowerLeft;
                    indices[counter++] = lowerRight;


                    indices[counter++] = topLeft;
                    indices[counter++] = lowerRight;
                    indices[counter++] = topRight;

                }
        }

        private static void CalculateNormals()
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal = new Vector3(0, 0, 0);
            for (int i = 0; i < indices.Length / 3; i++)
            {
                int index1 = indices[i * 3];
                int index2 = indices[i * 3 + 1];
                int index3 = indices[i * 3 + 2];

                Vector3 side1 = vertices[index1].Position - vertices[index3].Position;
                Vector3 side2 = vertices[index1].Position - vertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                vertices[index1].Normal += normal;
                vertices[index2].Normal += normal;
                vertices[index3].Normal += normal;
            }
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal.Normalize();
        }
    }
}
