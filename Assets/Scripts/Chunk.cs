
using UnityEngine;
public class Chunk{
    public const int CHUNKSIZE = 16;
    public int[,,] ChunkData;
    public Vector2Int ChunkPos;
    public void initialWorldGen(Vector2Int chunkPos){
        this.ChunkPos = chunkPos;
        ChunkData = new int[CHUNKSIZE,CHUNKSIZE,CHUNKSIZE];
        for(int x = 0; x < CHUNKSIZE; x++) { 
            for(int y = 0; y < CHUNKSIZE; y++) {
                for(int z = 0; z < CHUNKSIZE; z++) {
                    if( y < 8){
                        ChunkData[x,y,z] = 2;
                    }
                    if( y == 8){
                        ChunkData[x,y,z] = 1;
                    }
                }
            }
        }
    }
    
    public bool IsAir(int x, int y, int z)
    {
        // Check if the block is out of bounds or marked as air (value == 0)
        if (x < 0 || y < 0 || z < 0 || x >= ChunkData.GetLength(0) || y >= ChunkData.GetLength(1) || z >= ChunkData.GetLength(2))
            return true;  // Treat out-of-bounds as air
        return ChunkData[x, y, z] == 0;
    }

    
    public int getAbsolutePos(Vector3Int worldPos){
        Vector3Int relativePos = worldPos - new Vector3Int(ChunkPos.x, 0, ChunkPos.y) * CHUNKSIZE;
        return getPos(relativePos);
    }

    public int getPos(Vector3Int relativePos){
        return ChunkData[relativePos.x, relativePos.y, relativePos.z];
    }
}