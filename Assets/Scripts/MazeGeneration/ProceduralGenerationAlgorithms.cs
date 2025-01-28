using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    public static HashSet<Vector3Int> SimpleRandomWalk(Vector3Int startPosition, int walkLength, List<Vector3Int> tileDatas)
    {
        HashSet<Vector3Int> path = new()
        {
            new() { x = startPosition.x, y = startPosition.y, z = (int)TileType.Floor }
        };
        var previousPosition = startPosition;

        for (int i = 0; i < walkLength; i++)
        {
            var dir=Direction2D.GetRandomDirection();
            var newPosition = previousPosition + dir ;
            bool isPositionIsAccessible = true;
            foreach (var tile in tileDatas)
            {
                if (newPosition.x == tile.x && newPosition.y == tile.y && tile.z == (int)TileType.StraightVerticalWall)
                {
                    isPositionIsAccessible = false;
                }
            }
            if (isPositionIsAccessible)
            {
                path.Add(new() { x = newPosition.x, y = newPosition.y, z = (int)TileType.Floor });
                previousPosition = newPosition;
            }
        }
        return path;
    }
}

public static class Direction2D
{
    public static List<Vector3Int> cardinalDirection2D = new(){
        new Vector3Int(0,1,0),//up
        new Vector3Int(0,-1,0),//down
        new Vector3Int(1,0,0),//right
        new Vector3Int(-1,0,0)//left
    };

    public static Vector3Int GetRandomDirection()
    {
        return cardinalDirection2D[Random.Range(0, cardinalDirection2D.Count)];
    }
}
