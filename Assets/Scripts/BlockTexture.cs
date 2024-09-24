using System;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;

public class BlockTexture {
    private int[] indices = null;
    private int blockId = -1;
    private static Vector3[] directions = new Vector3[]{Vector3.forward, Vector3.right,
                                                        Vector3.back, Vector3.left,
                                                        Vector3.up, Vector3.down };
    public BlockTexture(int blockId, int index) {
        indices = new int[6];
        this.blockId = blockId;
        Array.Fill(indices, index);
    }
    public BlockTexture(int blockId, int[] indices) {
        this.blockId = blockId;
        if (indices.Length != 6) {
            throw new ArgumentException("BlockTexture created with index array not length 6");
        }
        this.indices = indices;
    }
    public BlockTexture(int blockId, int front, int left, int back, int right, int top, int bottom){
        this.blockId = blockId;
        indices = new int[]{front, left, back, right, top, bottom};
    }

    public BlockTexture(int blockId, int sides, int top, int bottom){
        this.blockId = blockId;
        indices = new int[]{sides, sides, sides, sides, top, bottom};
    }

    public int GetTexture(Vector3 direction){
        if (indices == null || indices.Length != 6) {
            throw new ArgumentException($"GetTexture called on invalid BlockTexture with id {blockId}");
        }
        int index = ArrayUtility.IndexOf(directions, direction);
        return indices[index];

    }
}