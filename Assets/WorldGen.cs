using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Terra.Graph;
using Terra.Graph.Noise;
using Terra.CoherentNoise;
using JetBrains.Annotations;

public class WorldGen : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float tileSize = 1f;
    List<Texture2D> tileTextures; // Array of tile textures to be combined into a texture atlas
    public Material baseMaterial; // The material to which the texture atlas will be applied
    public Texture2D textureAtlas;
    public Vector2[] uvAtlas;

    public NoiseGraph terrainGraph;
    public float heightFactor;
    public float spreadFactor;

    public float humidityScale;
    public float temperatureScale;

    public List<Biome> biomes;
    public Biome nullBiome;

    public static WorldGen chunk;

    public Vector3 generationOffset;

    public int LOD;

    public static WorldGen instance;


    public static int seed;
    void Start()
    {
        instance = this;
        tileTextures = new List<Texture2D>();
        for(int i = 0; i < biomes.Count; i++)
        {
            biomes[i].index = i;
            tileTextures.Add(biomes[i].floorMaterial);
        }
     //   //seed = Random.Range(-99999, 99999);
     //
        CreateTextureAtlas();
        GenerateGrid();
    }

    void CreateTextureAtlas()
    {
        // Combine tile textures into a texture atlas
        textureAtlas = new Texture2D(1024, 1024); // Adjust size as needed
        Rect[] rects = textureAtlas.PackTextures(tileTextures.ToArray(), 0, 1024);
        textureAtlas.filterMode = FilterMode.Point;

        uvAtlas = new Vector2[tileTextures.Count * 4];
        for (int i = 0; i < rects.Length; i++)
        {
            uvAtlas[i * 4 + 0] = new Vector2(rects[i].xMin, rects[i].yMin);
            uvAtlas[i * 4 + 1] = new Vector2(rects[i].xMax, rects[i].yMin);
            uvAtlas[i * 4 + 2] = new Vector2(rects[i].xMin, rects[i].yMax);
            uvAtlas[i * 4 + 3] = new Vector2(rects[i].xMax, rects[i].yMax);
        }

        // Assign the texture atlas to the base material
        baseMaterial.mainTexture = textureAtlas;
    }

    void GenerateGrid()
    {
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = baseMaterial;

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[gridWidth * gridHeight * 4];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[gridWidth * gridHeight * 6];
        int vertexIndex = 0;
        int triangleIndex = 0;

        Generator gen = terrainGraph.GetEndGenerator();

        tileSize *= LOD;
        Debug.Log(generationOffset.ToString());
        for (int x = 0; x < gridWidth / LOD; x++)
        {
            for (int y = 0; y < gridHeight / LOD; y++)
            {
                Vector3 tileOrigin = new Vector3(x * tileSize, 0, y * tileSize);

                // Add vertices for the tile
                vertices[vertexIndex + 0] = tileOrigin + new Vector3(0, gen.GetValue((tileOrigin + new Vector3(generationOffset.x, 0, generationOffset.y)) * spreadFactor) * heightFactor, 0);
                vertices[vertexIndex + 1] = tileOrigin + new Vector3(tileSize, gen.GetValue((tileOrigin + new Vector3(generationOffset.x, 0, generationOffset.y) + new Vector3(tileSize, 0,0)) * spreadFactor) * heightFactor, 0);
                vertices[vertexIndex + 2] = tileOrigin + new Vector3(0, gen.GetValue((tileOrigin + new Vector3(generationOffset.x, 0, generationOffset.y) + new Vector3(0, 0, tileSize)) * spreadFactor) * heightFactor, tileSize);
                vertices[vertexIndex + 3] = tileOrigin + new Vector3(tileSize, gen.GetValue((tileOrigin + new Vector3(generationOffset.x, 0, generationOffset.y) + new Vector3(tileSize, 0, tileSize)) * spreadFactor) * heightFactor, tileSize);

                Biome b = GetBiome(new Vector3(generationOffset.x, 0, generationOffset.y) + tileOrigin, gen);

                // Add UVs for the tile
                int textureIndex = b.index; // Example logic for assigning textures
                uv[vertexIndex + 0] = uvAtlas[textureIndex * 4];
                uv[vertexIndex + 1] = uvAtlas[textureIndex * 4 + 1];
                uv[vertexIndex + 2] = uvAtlas[textureIndex * 4 + 2];
                uv[vertexIndex + 3] = uvAtlas[textureIndex * 4 + 3];

                // Add triangles for the tile
                triangles[triangleIndex + 0] = vertexIndex + 0;
                triangles[triangleIndex + 1] = vertexIndex + 2;
                triangles[triangleIndex + 2] = vertexIndex + 1;
                triangles[triangleIndex + 3] = vertexIndex + 1;
                triangles[triangleIndex + 4] = vertexIndex + 2;
                triangles[triangleIndex + 5] = vertexIndex + 3;

                foreach (Feature f in b.features)
                {
                  //  CreateInstancedFeature(f.mesh, f.materials, tileOrigin, f.scale, f.amount, gen);
                    if (EvoUtils.PercentChance(f.chance))
                    {
                        Vector3 offset = new Vector3(Random.Range(0f, tileSize), 0, Random.Range(0f, tileSize));
                        GameObject feature = Instantiate(f.varients[Random.Range(0, f.varients.Length)], new Vector3(generationOffset.x, 0, generationOffset.y) + tileOrigin + new Vector3(0, gen.GetValue((tileOrigin + new Vector3(generationOffset.x, 0, generationOffset.y) + offset) * spreadFactor) * heightFactor, 0) + offset, Quaternion.Euler(f.rotateOffset + new Vector3(Random.Range(-f.rotateRange.x, f.rotateRange.x), Random.Range(-f.rotateRange.y, f.rotateRange.y), Random.Range(-f.rotateRange.z, f.rotateRange.z))));
                        feature.transform.parent = transform;
                    } 
                }

                vertexIndex += 4;
                triangleIndex += 6;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        GetComponent<MeshCollider>().sharedMesh = mesh;
        meshFilter.mesh = mesh;
    }

    void CreateInstancedFeature(Mesh mesh, Material[] materials, Vector3 position, Vector3 scale, int amount, Generator gen)
    {
        GPUInstancer instancer = gameObject.AddComponent<GPUInstancer>();

        instancer.mesh = mesh;
        instancer.materials = materials;
        instancer.instances = amount;


        int addedMatricies = 0;

        instancer.batches.Add(new List<Matrix4x4>());
        for (int i = 0; i < amount; i++)
        {
            if (addedMatricies < 1000)
            {
                instancer.batches[instancer.batches.Count - 1].Add(Matrix4x4.TRS(position + new Vector3(Random.Range(0, gridWidth * tileSize), gen.GetValue((position + new Vector3(generationOffset.x, 0, generationOffset.y) + position) * spreadFactor) * heightFactor, Random.Range(0, gridWidth * tileSize)), Quaternion.Euler(new Vector3(-90,0,0)), scale * Random.Range(0.5f, 1.5f)));
            }
            else
            {
                instancer.batches.Add(new List<Matrix4x4>());
                addedMatricies = 0;
            }
        }
    }

    public Biome GetBiome(Vector3 pos, Generator gen)
    {
        Biome optimalBiome = null;
        foreach (Biome b in biomes)
        {
            if (GetTemperature(pos, gen) <= b.maxTemp && GetTemperature(pos, gen) >= b.minTemp && GetHumidity(pos) <= b.maxHumid && GetHumidity(pos) >= b.minHumid && gen.GetValue(pos * spreadFactor) * heightFactor <= b.maxHeight && gen.GetValue(pos * spreadFactor) * heightFactor >= b.minHeight)
            {
                optimalBiome = b;
                break;
            }
        }
        if (optimalBiome != null)
        {
            return optimalBiome;
        }
        return nullBiome;
    }

    public float GetTemperature(Vector3 pos, Generator gen)
    {
        return Mathf.Clamp01(Mathf.PerlinNoise((pos.x + seed) * temperatureScale, (pos.z + seed) * temperatureScale) - (gen.GetValue(pos * spreadFactor) * heightFactor) / 10000);
    }
    public float GetHumidity(Vector3 pos)
    {
        return (Mathf.PerlinNoise((pos.x - seed) * humidityScale, (pos.z - seed) * humidityScale));
    }
}