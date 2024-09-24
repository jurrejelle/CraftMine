using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
public class World {
    public Dictionary<Vector3Int,Chunk> chunks = new();
    public const int CHUNKSIZE = 16;
    public const int WORLDSIZE = 4;
    public WorldManager manager;
    public World(WorldManager manager){
        this.manager = manager;
    }
    public void InitialWorldGen(){
        for(int chunkX = -WORLDSIZE; chunkX <= WORLDSIZE; chunkX++){
            for(int chunkY = 0; chunkY <= 4; chunkY++){
                for(int chunkZ = -WORLDSIZE; chunkZ <= WORLDSIZE; chunkZ++){
                    Vector3Int chunkPos = new Vector3Int(chunkX, chunkY, chunkZ);
                    GenChunk(chunkPos);
                }
            }
        }
    }
    public void InitialWorldGenSpecificChunks(){
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
                Chunk currentChunk = new Chunk(this, chunkPos);
                currentChunk.InitialWorldGenNoise(chunkPos);
                currentChunk.CreateObject();
                chunks[chunkPos] = currentChunk;

    }

    public void UpdateAllChunks(){
        foreach(Chunk chunk in chunks.Values){
            chunk.UpdateChunk();
        }
    }
    public void UpdateBlock(Vector3Int worldPos){
        HashSet<Vector3Int> chunksToUpdate = new();
        Vector3Int[] dirs = new Vector3Int[]{ new(0,0,1), new(0,0,-1),
                                              new(0,1,0), new(0,-1,0),
                                              new(1,0,0), new(-1,0,0)};
        foreach (Vector3Int dir in dirs){
            Vector3Int curPos = worldPos + dir;
            chunksToUpdate.Add(WorldPosToChunkPos(curPos));
        }
        foreach (Vector3Int chunkPosToUpdate in chunksToUpdate){
            UpdateChunkByChunk(chunkPosToUpdate);
        }
    }
    
    public void UpdateChunk(Vector3Int worldPos){
        Vector3Int chunkPos = WorldPosToChunkPos(worldPos);
        UpdateChunkByChunk(chunkPos);
    }

    public void UpdateChunkByChunk(Vector3Int chunkPos){
        if (!chunks.ContainsKey(chunkPos)){
            GenChunk(chunkPos);
        };
        chunks[chunkPos].UpdateChunk();
    }

    public bool DestroyBlock(Vector3Int worldPos){
        if(GetBlock(worldPos) == 0){
            return false;
        }
        else { 
            SetBlock(worldPos, 0);
            UpdateBlock(worldPos);
            return true;
        }
    }
    public bool PlaceBlock(Vector3Int worldPos, int blockId){
        if(GetBlock(worldPos) != 0){
            return false;
        }
        else { 
            SetBlock(worldPos, blockId);
            UpdateBlock(worldPos);
            return true;
        }
    }
    public bool IsAir(Vector3Int chunkPos, int x, int y, int z){
        return GetBlock(chunkPos, x, y, z) == 0;
    }
    public int GetBlock(Vector3Int chunkPos, int x, int y, int z){
        return GetBlock(ChunkPosToWorldPos(chunkPos) + new Vector3Int(x, y, z));
    }
    public int GetBlock(Vector3Int chunkPos, Vector3Int relativePos){
        return GetBlock(ChunkPosToWorldPos(chunkPos) + relativePos);
    }
    public int GetBlock(int x, int y, int z){
        return GetBlock(new Vector3Int(x,y,z));
    }
    public int GetBlock(Vector3Int worldPos){
        if (worldPos.y < 0) return 0;
        Vector3Int chunkPos = WorldPosToChunkPos(worldPos);
        if (!chunks.ContainsKey(chunkPos)) return 0;
        return chunks[chunkPos].GetBlockAbsolute(worldPos);
    }

    public bool SetBlock(Vector3Int worldPos, int blockId){
        Vector3Int chunkPos = WorldPosToChunkPos(worldPos);
        if (!chunks.ContainsKey(chunkPos)) return false;
        return chunks[chunkPos].SetBlockAbsolute(worldPos, blockId);
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