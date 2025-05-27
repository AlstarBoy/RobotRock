
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles detection and execution of merges.
/// </summary>
public class MergeManager : MonoBehaviour
{
    public static MergeManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void CheckForMerge(CelestialObject placedObj)
    {
        List<CelestialObject> matching = GetMatchingNeighbors(placedObj);

        if (matching.Count >= 3)
        {
            PerformMerge(matching, placedObj.tier + 1);
        }
    }

    public bool CanMerge(CelestialObject obj)
    {
        return GetMatchingNeighbors(obj).Count >= 3;
    }

    private List<CelestialObject> GetMatchingNeighbors(CelestialObject origin)
    {
        List<CelestialObject> matches = new List<CelestialObject> { origin };
        Queue<CelestialObject> queue = new Queue<CelestialObject>();
        HashSet<Tile> visited = new HashSet<Tile>();
        queue.Enqueue(origin);

        while (queue.Count > 0)
        {
            CelestialObject current = queue.Dequeue();
            Tile tile = current.currentTile;

            foreach (Tile neighbor in GridManager.Instance.GetAllNeighbors(tile))
            {
                if (visited.Contains(neighbor) || !neighbor.IsOccupied) continue;

                CelestialObject obj = neighbor.currentObject;
                if (obj.tier == origin.tier && obj.objectName == origin.objectName)
                {
                    matches.Add(obj);
                    queue.Enqueue(obj);
                }

                visited.Add(neighbor);
            }
        }

        return matches;
    }

    private void PerformMerge(List<CelestialObject> group, int newTier)
    {
        Vector2 averagePos = Vector2.zero;
        foreach (var obj in group)
        {
            averagePos += (Vector2)obj.transform.position;
            obj.currentTile.Clear();
            obj.MergeInto();
        }

        Vector2 spawnPos = averagePos / group.Count;
        Vector3 spawnPos3D = new Vector3(spawnPos.x, spawnPos.y, 0);

        // Spawn the new merged object at a 3D position
        GameObject newObj = ObjectPooler.Instance.SpawnFromPool("Tier" + newTier, spawnPos3D, Quaternion.identity);
        CelestialObject newCelestial = newObj.GetComponent<CelestialObject>();
        Tile newTile = GridManager.Instance.GetClosestTile(spawnPos);
        newTile.SetObject(newCelestial);

        // Check for further merges
        CheckForMerge(newCelestial);
    }
}
