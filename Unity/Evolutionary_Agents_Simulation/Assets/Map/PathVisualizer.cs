using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathVisualizer : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed at which the object will move along the path
    public Transform visualizerObject; // Object with Trail Renderer attached
    public GameObject pathVisualizer; // Assign this in the Unity Editor

    // Call this method to start visualizing a path
    public void VisualizePath(List<Vector2Int> path, float tileSize, Vector3 gridOrigin)
    {
        StartCoroutine(FollowPath(path, tileSize, gridOrigin));
    }

    // Coroutine to move the visualizer object along the path
    IEnumerator FollowPath(List<Vector2Int> path, float tileSize, Vector3 gridOrigin)
    {
        foreach (Vector2Int cell in path)
        {
            // Convert grid coordinates to world position
            Vector3 worldPosition = gridOrigin + new Vector3(cell.x * tileSize + tileSize / 2, 0, cell.y * tileSize + tileSize / 2);

            // Move the visualizer object to each position along the path
            while (visualizerObject.position != worldPosition)
            {
                visualizerObject.position = Vector3.MoveTowards(visualizerObject.position, worldPosition, moveSpeed * Time.deltaTime);
                yield return null; // Wait a frame before continuing
            }
        }
    }

    
    
    // Coroutine to move the pathVisualizer along the path
    IEnumerator FollowPath(List<Vector2Int> path, Vector3 gridOrigin, float tileSize)
    {
        foreach (Vector2Int cell in path)
        {
            Vector3 worldPosition = gridOrigin + new Vector3(cell.x * tileSize, 1, cell.y * tileSize) + new Vector3(tileSize / 2.0f, 0, tileSize / 2.0f);
            pathVisualizer.transform.position = worldPosition;
            yield return new WaitForSeconds(0.01f); // Adjust for visual effect
        }
    }

    // Method to start path visualization
    public void StartVisualizingPath(List<Vector2Int> path, Vector3 gridOrigin, float tileSize)
    {
        StartCoroutine(FollowPath(path, gridOrigin, tileSize));
    }

    int[,] visualizePath(int[,] map, List<Vector2Int> path)
    {
        Debug.Log("Visualizing path with path length: " + path.Count);

        //StartVisualizingPath(path, transform.position, MapModel.TileSize);

        return map;
    }

}
