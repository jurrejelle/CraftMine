using UnityEngine;

public static class Extensions
{
    // Extension method to floor a Vector3 and convert it to a Vector3Int
    public static Vector3Int floor(this Vector3 vector)
    {
        return new Vector3Int(
            Mathf.FloorToInt(vector.x),
            Mathf.FloorToInt(vector.y),
            Mathf.FloorToInt(vector.z)
        );
    }
}