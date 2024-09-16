using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
public class World {
    public Dictionary<Vector3Int,Chunk> chunks = new();
    public const int CHUNKSIZE = 16;
    public const int WORLDSIZE = 8;
    public void initialWorldGen(){
        for(int chunkX = -WORLDSIZE; chunkX <= WORLDSIZE; chunkX++){
            for(int chunkY = 0; chunkY <= 4; chunkY++){
                for(int chunkZ = -WORLDSIZE; chunkZ <= WORLDSIZE; chunkZ++){
                    Vector3Int chunkPos = new Vector3Int(chunkX, chunkY, chunkZ);
                    GenChunk(chunkPos);
                }
            }
        }
    }
    public void initialWorldGenSpecificChunks(){
        foreach( Vector3Int chunkPos in new Vector3Int[]{
            new Vector3Int(-1,0,-1), new Vector3Int(-1,0,0), new Vector3Int(-1,0,1),
            new Vector3Int( 0,0,-1), new Vector3Int( 0,0,0), new Vector3Int( 0,0,1),
            new Vector3Int( 1,0,-1), new Vector3Int( 1,0,0), new Vector3Int( 1,0,1),
            new Vector3Int(-1,1,0),
            new Vector3Int( 0,1,-1), new Vector3Int( 0,1,0), new Vector3Int( 0,1,1),
            new Vector3Int( 1,1,0),
            new Vector3Int(0, 2, 0)
        }){
                    GenChunk(chunkPos);
        }
    }

    private void GenChunk(Vector3Int chunkPos){
                Chunk currentChunk = new Chunk();
                currentChunk.initialWorldGenNoise(chunkPos);

                chunks[chunkPos] = currentChunk;

    }
    public bool IsAir(Vector3Int chunkPos, int x, int y, int z){
        return Get(chunkPos, x, y, z) == 0;
    }
    public int Get(Vector3Int chunkPos, int x, int y, int z){
        return Get(ChunkPosToWorldPos(chunkPos) + new Vector3Int(x, y, z));
    }
    public int Get(Vector3Int chunkPos, Vector3Int relativePos){
        return Get(ChunkPosToWorldPos(chunkPos) + relativePos);
    }
    public int Get(int x, int y, int z){
        return Get(new Vector3Int(x,y,z));
    }
    public int Get(Vector3Int worldPos){
        if (worldPos.y < 0) return 0;
        Vector3Int chunkPos = WorldPosToChunkPos(worldPos);
        if (!chunks.ContainsKey(chunkPos)) return 0;
        return chunks[chunkPos].getAbsolutePos(worldPos);
    }
    public static int WorldPosToChunkPos(int coord){
        return (coord >= 0) ? (coord / 16) : ((coord + 1) / 16) - 1;
    }
    public static Vector3Int WorldPosToChunkPos(Vector3Int worldPos){
        return new Vector3Int(WorldPosToChunkPos(worldPos.x), WorldPosToChunkPos(worldPos.y), WorldPosToChunkPos(worldPos.z));
    }

    public static Vector3Int ChunkPosToWorldPos(Vector3Int chunkPos){
        return chunkPos * CHUNKSIZE;
    }
    public static Vector3Int ChunkPosToWorldPos(Vector3Int chunkPos, int x, int y, int z){
        return chunkPos * CHUNKSIZE + new Vector3Int(x,y,z);
    }

}