using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public Material blockMaterial; 

    private Vector3 blockSize = Vector3.one;  // Size of each block (can be changed)
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private MeshRenderer meshRenderer;
    private Dictionary<int, int> blockToTexture;
    private World world;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = blockMaterial;
        blockToTexture = new Dictionary<int, int>();
        // Skipping 0 since it's air
        blockToTexture[1] = 15*64 + 0; // Dirt
        blockToTexture[2] = 15*64 + 2; // Cobble

        world = new World();
        world.initialWorldGen();
        GenerateMesh();
        
    }

    void GenerateMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        // TODO: one chunk per object, keep in dict, only update right chunks?
        foreach(Chunk chunk in world.chunks.Values)
        {
            GenChunk(chunk, vertices, triangles, uvs);
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;  // Update the mesh collider
    }

    void GenChunk(Chunk chunk, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs){
        int[,,] chunkData = chunk.ChunkData;
        Vector3Int chunkPos = chunk.ChunkPos;
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
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 3);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);

        AddUVs(blockId, uvs);
    }

    void AddUVs(int blockId, List<Vector2> uvs)
    {
        int blockTextureIndex = blockToTexture[blockId];
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
}