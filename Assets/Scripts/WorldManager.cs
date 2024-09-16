using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public int[,,] worldData;  // Your 3D array (fill it elsewhere)
    public Vector3 blockSize = Vector3.one;  // Size of each block (can be changed)

    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private const int CHUNKSIZE = 16;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        PopulateWorldData();
        GenerateMesh();
    }
    void PopulateWorldData(){
        worldData = new int[CHUNKSIZE,CHUNKSIZE,CHUNKSIZE];
        for(int x=0; x < CHUNKSIZE; x++) { 
            for(int y=0; y < CHUNKSIZE; y++) {
                for(int z=0; z < CHUNKSIZE; z++) {
                    if( y < 8){
                        worldData[x,y,z] = 1;
                    }
                }
            }
        }
    }

    void GenerateMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int x = 0; x < worldData.GetLength(0); x++)
        {
            for (int y = 0; y < worldData.GetLength(1); y++)
            {
                for (int z = 0; z < worldData.GetLength(2); z++)
                {
                    if (worldData[x, y, z] == 1)  // If the block is filled
                    {
                        Vector3 blockPosition = new Vector3(x, y, z);

                        // Check if the block has exposed sides, add faces for exposed sides
                        if (IsAir(x, y + 1, z)) AddFace(vertices, triangles, blockPosition, Vector3.up);    // Top face
                        if (IsAir(x, y - 1, z)) AddFace(vertices, triangles, blockPosition, Vector3.down);  // Bottom face
                        if (IsAir(x + 1, y, z)) AddFace(vertices, triangles, blockPosition, Vector3.right); // Right face
                        if (IsAir(x - 1, y, z)) AddFace(vertices, triangles, blockPosition, Vector3.left);  // Left face
                        if (IsAir(x, y, z + 1)) AddFace(vertices, triangles, blockPosition, Vector3.forward); // Front face
                        if (IsAir(x, y, z - 1)) AddFace(vertices, triangles, blockPosition, Vector3.back);   // Back face
                    }
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;  // Update the mesh collider
    }

    bool IsAir(int x, int y, int z)
    {
        // Check if the block is out of bounds or marked as air (value == 0)
        if (x < 0 || y < 0 || z < 0 || x >= worldData.GetLength(0) || y >= worldData.GetLength(1) || z >= worldData.GetLength(2))
            return true;  // Treat out-of-bounds as air
        return worldData[x, y, z] == 0;
    }

    void AddFace(List<Vector3> vertices, List<int> triangles, Vector3 blockPosition, Vector3 direction)
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
    }
}