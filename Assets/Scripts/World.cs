using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
public class World {
    public Dictionary<Vector2Int,Chunk> chunks = new();
    public const int CHUNKSIZE = 16;
    public void initialWorldGen(){
        for(int chunkX = -1; chunkX <= 1; chunkX++){
            for(int chunkY = -1; chunkY <= 1; chunkY++){
                Vector2Int chunkPos = new Vector2Int(chunkX, chunkY);
                GenChunk(chunkPos);
            }
        }
    }

    private void GenChunk(Vector2Int chunkPos){
                Chunk currentChunk = new Chunk();
                currentChunk.initialWorldGen(chunkPos);

                chunks[chunkPos] = currentChunk;
                Debug.Log("Chunk" + chunkPos + "Generated");

    }
    public bool IsAir(Vector2Int chunkPos, int x, int y, int z){
        return Get(chunkPos, x, y, z) == 0;
    }
    public int Get(Vector2Int chunkPos, int x, int y, int z){
        return Get(ChunkPosToWorldPos(chunkPos) + new Vector3Int(x, y, z));
    }
    public int Get(Vector2Int chunkPos, Vector3Int relativePos){
        return Get(ChunkPosToWorldPos(chunkPos) + relativePos);
    }
    public int Get(int x, int y, int z){
        return Get(new Vector3Int(x,y,z));
    }
    public int Get(Vector3Int worldPos){
        if (worldPos.y < 0) return 0;
        Vector2Int chunkPos = WorldPosToChunkPos(worldPos);
        if (!chunks.ContainsKey(chunkPos)) return 0;
        return chunks[chunkPos].getAbsolutePos(worldPos);
    }
    public static int WorldPosToChunkPos(int coord){
        return (coord >= 0) ? (coord / 16) : ((coord + 1) / 16) - 1;
    }
    public static Vector2Int WorldPosToChunkPos(Vector3Int worldPos){
        return new Vector2Int(WorldPosToChunkPos(worldPos.x), WorldPosToChunkPos(worldPos.z));
    }

    public static Vector3Int ChunkPosToWorldPos(Vector2Int chunkPos){
        return new Vector3Int(chunkPos.x * CHUNKSIZE,0, chunkPos.y * CHUNKSIZE);
    }

}