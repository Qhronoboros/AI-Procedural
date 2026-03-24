// Source: https://ijmrap.com/wp-content/uploads/2022/07/IJMRAP-V5N2P27Y22.pdf
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private GameObject pathPrefab;
    [SerializeField] private int _spriteSize = 32;
    
    [SerializeField] private int _walkDistance = 32;
    [SerializeField] private bool _skipExploredTiles;

    private void Start()
    {
        // Vector2[,] path2 = 
    
        HashSet<Vector2Int> path = RandomWalk(new Vector2Int(0, 0), _walkDistance);
        
        
        
        foreach(Vector2Int tile in path)
        {
            Instantiate(pathPrefab, new Vector3(tile.x, tile.y, 0.0f), Quaternion.identity);
        }
    }

    private HashSet<Vector2Int> RandomWalk(Vector2Int startPosition, int walkDistance)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>
        {
            startPosition
        };
        
        Vector2Int previousPosition = startPosition;
        for (int i = 1; i < walkDistance; i++)
        {
            Vector2Int newPosition = previousPosition + GetRandomDirection();
            
            if (!path.Contains(newPosition) || !_skipExploredTiles)
            {
                path.Add(newPosition);
                previousPosition = newPosition;
            }
            else i--;
        }
        
        return path;
    }
    
    private Vector2Int GetRandomDirection() => randomDirectionList[Random.Range(0, randomDirectionList.Count)];
    
    public static List<Vector2Int> randomDirectionList = new List<Vector2Int>()
    {
        new Vector2Int(0, 1),   // Up
        new Vector2Int(0, -1),  // Down
        new Vector2Int(1, 0),   // Right
        new Vector2Int(-1, 0)   // Left
    };
}
