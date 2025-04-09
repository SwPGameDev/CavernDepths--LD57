using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    Tilemap tilemap;
    Grid grid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        grid = GetComponent<Grid>();

        tilemap.CompressBounds();

        Debug.Log("SIZE: " + tilemap.size);
        Debug.Log("Bounds: " + tilemap.cellBounds);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
