
using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
public class Chunk{
    private static Material material = null;
    public const int CHUNKSIZE = 16;
    public int[,,] chunkData;
    public Vector3Int chunkPos;
    public GameObject chunkObject;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private World world;

    public Chunk(World world, Vector3Int chunkPos){
        this.world = world;
        this.chunkPos = chunkPos;
    }

    public void CreateObject(){
        GameObject chunkObject = new GameObject("Chunk("+chunkPos.x+","+chunkPos.y+","+chunkPos.z+")");
        chunkObject.transform.SetParent(world.manager.transform);
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshCollider = chunkObject.AddComponent<MeshCollider>();
        if(material == null){
            Debug.Log("Loading material");
            material = Resources.Load<Material>("Textures/blockMaterial");
            Debug.Log(material);
        }
        meshRenderer.material = material;
    }
    public void Unload(){
        if(chunkObject == null){
            throw new Exception("UnloadChunk called without existing ChunkObject");
        }
        UnityEngine.Object.Destroy(chunkObject);

    }

    private int CreateBlockData(Vector3Int input){
        if( input.y == 36){
            return 1;
        }
        if( input.y < 36 && input.y >= 34){
            return 2;
        }
        if( input.y < 34){
            return 3;
        }
        return 0;

    }
    public void InitialWorldGen(Vector3Int chunkPos){
        this.chunkPos = chunkPos;
        chunkData = new int[CHUNKSIZE,CHUNKSIZE,CHUNKSIZE];
        for(int x = 0; x < CHUNKSIZE; x++) { 
            for(int y = 0; y < CHUNKSIZE; y++) {
                for(int z = 0; z < CHUNKSIZE; z++) {
                    Vector3Int worldPos = World.ChunkPosToWorldPos(chunkPos, x, y, z);
                    chunkData[x,y,z] = CreateBlockData(worldPos);
                }
            }
        }
    }
    public void InitialWorldGenNoise(Vector3Int chunkPos){
        this.chunkPos = chunkPos;
        chunkData = new int[CHUNKSIZE,CHUNKSIZE,CHUNKSIZE];
        for(int x = 0; x < CHUNKSIZE; x++) { 
            for(int y = 0; y < CHUNKSIZE; y++) {
                for(int z = 0; z < CHUNKSIZE; z++) {
                    Vector3Int worldPos = World.ChunkPosToWorldPos(chunkPos, x, y, z);
                    float perlinNoise = Mathf.PerlinNoise(worldPos.x/16.66f, worldPos.z/26.66f) * 64;
                    // Noise is number between 0 and 64
                    if( perlinNoise < worldPos.y) {
                        chunkData[x,y,z] = 0;
                        continue;
                    }
                    chunkData[x,y,z] = CreateBlockData(worldPos);
                }
            }
        }
    }

    public void UpdateChunk(){
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        GenerateMesh(vertices, triangles, uvs);

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }
    void GenerateMesh(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs){
        // TODO: greedy meshing??? 
        for (int x = 0; x < chunkData.GetLength(0); x++)
        {
            for (int y = 0; y < chunkData.GetLength(1); y++)
            {
                for (int z = 0; z < chunkData.GetLength(2); z++)
                {
                    int blockId = chunkData[x, y, z];
                    if (blockId != 0)  // If the block is filled
                    {
                        Vector3 blockPosition = new Vector3(x, y, z) + World.ChunkPosToWorldPos(chunkPos);

                        // Check if the block has exposed sides, add faces for exposed sides
                        if (world.IsAir(chunkPos, x, y + 1, z)) AddFace(vertices, triangles, uvs, blockPosition, blockId, Vector3.up);    // Top face
                        if (world.IsAir(chunkPos, x, y - 1, z)) AddFace(vertices, triangles, uvs, blockPosition, blockId, Vector3.down);  // Bottom face
                        if (world.IsAir(chunkPos, x + 1, y, z)) AddFace(vertices, triangles, uvs, blockPosition, blockId, Vector3.right); // Right face
                        if (world.IsAir(chunkPos, x - 1, y, z)) AddFace(vertices, triangles, uvs, blockPosition, blockId, Vector3.left);  // Left face
                        if (world.IsAir(chunkPos, x, y, z + 1)) AddFace(vertices, triangles, uvs, blockPosition, blockId, Vector3.forward); // Front face
                        if (world.IsAir(chunkPos, x, y, z - 1)) AddFace(vertices, triangles, uvs, blockPosition, blockId, Vector3.back);   // Back face
                    }
                }
            }
        }
    }
 void AddFace(List<Vector3> vertices, 
                 List<int> triangles, 
                 List<Vector2> uvs, 
                 Vector3 blockPosition, 
                 int blockId, 
                 Vector3 direction)
    {
        int vertexIndex = vertices.Count;

        // Define the four vertices of the face based on the direction
        Vector3[] faceVertices = new Vector3[4];

        if (direction == Vector3.up || direction == Vector3.down) // Top or bottom face
        {
            float yOffset = (direction == Vector3.up) ? 1f : 0f;
            faceVertices[0] = blockPosition + new Vector3(0f, yOffset, 0f);
            faceVertices[1] = blockPosition + new Vector3(1f, yOffset, 0f);
            faceVertices[2] = blockPosition + new Vector3(1f, yOffset, 1f);
            faceVertices[3] = blockPosition + new Vector3(0f, yOffset, 1f);
        }
        else if (direction == Vector3.forward || direction == Vector3.back) // Front or back face
        {
            float zOffset = (direction == Vector3.forward) ? 1f : 0f;
            faceVertices[0] = blockPosition + new Vector3(0f, 0f, zOffset);
            faceVertices[1] = blockPosition + new Vector3(1f, 0f, zOffset);
            faceVertices[2] = blockPosition + new Vector3(1f, 1f, zOffset);
            faceVertices[3] = blockPosition + new Vector3(0f, 1f, zOffset);
        }
        else if (direction == Vector3.right || direction == Vector3.left) // Right or left face
        {
            float xOffset = (direction == Vector3.right) ? 1f : 0f;
            faceVertices[0] = blockPosition + new Vector3(xOffset, 0f, 0f);
            faceVertices[1] = blockPosition + new Vector3(xOffset, 0f, 1f);
            faceVertices[2] = blockPosition + new Vector3(xOffset, 1f, 1f);
            faceVertices[3] = blockPosition + new Vector3(xOffset, 1f, 0f);
        }

        vertices.AddRange(faceVertices);

        // Add the two triangles that make up the face
        if( direction == Vector3.back ||
            direction == Vector3.right ||
            direction == Vector3.up ){
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 3);
            triangles.Add(vertexIndex + 2);
        } else { 
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 3);
        }

        AddUVs(blockId, direction, uvs);
    }

    void AddUVs(int blockId, Vector3 direction, List<Vector2> uvs)
    {
        // TODO: Rework access of blockToTexture (global dictionary?)
        int blockTextureIndex = world.manager.blockToTexture[blockId].GetTexture(direction);
        // Texture atlas dimensions
        float atlasWidth = 1024f;
        float atlasHeight = 512;
        float tileSizeX = 16f / atlasWidth;   // Width of a single texture in UV space
        float tileSizeY = 16f / atlasHeight;  // Height of a single texture in UV space

        // Calculate the row and column based on the blockType index
        int tilesPerRow = (int)(atlasWidth / 16f);  // How many textures fit horizontally
        int row = blockTextureIndex / tilesPerRow;
        int col = blockTextureIndex % tilesPerRow;

        // Calculate the UV coordinates for the top-left corner of the texture
        float uMin = col * tileSizeX;
        float vMin = 1f - (row + 1) * tileSizeY;  // 1 minus (row + 1) because UV space goes from 0 to 1, top to bottom

        // Define the UVs for one face of the block
        Vector2[] uvFace = new Vector2[4];
        uvFace[0] = new Vector2(uMin, vMin);                    // Bottom-left
        uvFace[1] = new Vector2(uMin + tileSizeX, vMin);        // Bottom-right
        uvFace[2] = new Vector2(uMin + tileSizeX, vMin + tileSizeY); // Top-right
        uvFace[3] = new Vector2(uMin, vMin + tileSizeY);        // Top-left

        // Add the UVs for this face
        uvs.AddRange(uvFace);
    }

    /*
    public bool IsAir(int x, int y, int z)
    {
        // Check if the block is out of bounds or marked as air (value == 0)
        if (x < 0 || y < 0 || z < 0 || x >= ChunkData.GetLength(0) || y >= ChunkData.GetLength(1) || z >= ChunkData.GetLength(2))
            return true;  // Treat out-of-bounds as air
        return ChunkData[x, y, z] == 0;
    }*/

    
    public int GetBlockAbsolute(Vector3Int worldPos){
        Vector3Int relativePos = worldPos - new Vector3Int(chunkPos.x, chunkPos.y, chunkPos.z) * CHUNKSIZE;
        if (relativePos.x < 0 || relativePos.x >= CHUNKSIZE ||
            relativePos.y < 0 || relativePos.y >= CHUNKSIZE ||
            relativePos.z < 0 || relativePos.z >= CHUNKSIZE){
                throw new ArgumentException("GetAbsolutePos called with position outside chunk: " + worldPos + "in chunk " + chunkPos);
            }
        
        return GetBlock(relativePos);
    }

    public int GetBlock(Vector3Int relativePos){
        return chunkData[relativePos.x, relativePos.y, relativePos.z];
    }

    public bool SetBlockAbsolute(Vector3Int worldPos, int blockId){
        Vector3Int relativePos = worldPos - new Vector3Int(chunkPos.x, chunkPos.y, chunkPos.z) * CHUNKSIZE;
        if (relativePos.x < 0 || relativePos.x >= CHUNKSIZE ||
            relativePos.y < 0 || relativePos.y >= CHUNKSIZE ||
            relativePos.z < 0 || relativePos.z >= CHUNKSIZE){
                throw new ArgumentException("SetBlockAbsolute called with position outside chunk: " + worldPos + "in chunk " + chunkPos);
            }
        
        return SetBlock(relativePos, blockId);
    }

    public bool SetBlock(Vector3Int relativePos, int blockId){
        chunkData[relativePos.x, relativePos.y, relativePos.z] = blockId;
        return true;
    }
}