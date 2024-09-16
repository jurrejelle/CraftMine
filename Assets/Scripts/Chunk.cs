
using System;
using UnityEngine;
public class Chunk{
    public const int CHUNKSIZE = 16;
    public int[,,] ChunkData;
    public Vector3Int ChunkPos;
    public void initialWorldGen(Vector3Int chunkPos){
        this.ChunkPos = chunkPos;
        ChunkData = new int[CHUNKSIZE,CHUNKSIZE,CHUNKSIZE];
        for(int x = 0; x < CHUNKSIZE; x++) { 
            for(int y = 0; y < CHUNKSIZE; y++) {
                for(int z = 0; z < CHUNKSIZE; z++) {
                    Vector3Int worldPos = World.ChunkPosToWorldPos(chunkPos, x, y, z);
                    if( worldPos.y < 36){
                        ChunkData[x,y,z] = 2;
                    }
                    if( worldPos.y == 36){
                        ChunkData[x,y,z] = 1;
                    }
                }
            }
        }
    }
    public void initialWorldGenNoise(Vector3Int chunkPos){
        this.ChunkPos = chunkPos;
        ChunkData = new int[CHUNKSIZE,CHUNKSIZE,CHUNKSIZE];
        for(int x = 0; x < CHUNKSIZE; x++) { 
            for(int y = 0; y < CHUNKSIZE; y++) {
                for(int z = 0; z < CHUNKSIZE; z++) {
                    Vector3Int worldPos = World.ChunkPosToWorldPos(chunkPos, x, y, z);
                    float perlinNoise = Mathf.PerlinNoise(worldPos.x/16.66f, worldPos.z/26.66f) * 64;
                    // Noise is number between 0 and 64
                    if( perlinNoise < worldPos.y) {
                        ChunkData[x,y,z] = 0;
                        continue;
                    }

                    if( worldPos.y < 36){
                        ChunkData[x,y,z] = 2;
                    }
                    if( worldPos.y == 36){
                        ChunkData[x,y,z] = 1;
                    }
                }
            }
        }
    }
    /*
    public bool IsAir(int x, int y, int z)
    {
        // Check if the block is out of bounds or marked as air (value == 0)
        if (x < 0 || y < 0 || z < 0 || x >= ChunkData.GetLength(0) || y >= ChunkData.GetLength(1) || z >= ChunkData.GetLength(2))
            return true;  // Treat out-of-bounds as air
        return ChunkData[x, y, z] == 0;
    }*/

    
    public int getAbsolutePos(Vector3Int worldPos){
        Vector3Int relativePos = worldPos - new Vector3Int(ChunkPos.x, ChunkPos.y, ChunkPos.z) * CHUNKSIZE;
        if (relativePos.x < 0 || relativePos.x >= CHUNKSIZE ||
            relativePos.y < 0 || relativePos.y >= CHUNKSIZE ||
            relativePos.z < 0 || relativePos.z >= CHUNKSIZE){
                throw new ArgumentException("getAbsolutePos called with position outside chunk: " + worldPos + "in chunk " + ChunkPos);
            }
        
        return getPos(relativePos);
    }

    public int getPos(Vector3Int relativePos){
        return ChunkData[relativePos.x, relativePos.y, relativePos.z];
    }
}