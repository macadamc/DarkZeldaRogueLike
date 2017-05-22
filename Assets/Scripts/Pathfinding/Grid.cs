using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ShadyPixel.Astar
{
    public class Grid : MonoBehaviour
    {
        public bool displayGridGizmos;
        [Range(0.0f, 1.0f)]
        public float gizmosAlpha;
        public LayerMask unwalkableMask;
        public Vector2 gridWorldSize;
        public float nodeRadius;
        public TerrainType[] walkableRegions;
        public float obstacleShell = 0.25f;
        public int obstacleProximityPenalty = 10;
        public int blurCostAmount = 3;

        Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();
        LayerMask walkableMask;

        Node[,] grid;

        float nodeDiameter;
        int gridSizeX, gridSizeY;

        int penaltyMin = int.MaxValue;
        int penaltyMax = int.MinValue;

        public void Start()
        {
            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

            foreach (TerrainType region in walkableRegions)
            {
                walkableMask.value |= region.terrainMask.value;
                walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2), region.terrainPenalty);
            }

            Invoke("CreateGrid", 0.1f);
            //CreateGrid();
        }

        public int MaxSize
        {
            get
            {
                return gridSizeX * gridSizeY;
            }
        }

        void CreateGrid()
        {
            grid = new Node[gridSizeX, gridSizeY];
            Vector2 worldBottomLeft = (Vector2)transform.position - Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                    bool walkable = !(Physics2D.OverlapCircle(worldPoint, (nodeRadius) + obstacleShell, unwalkableMask));

                    int movementPenalty = 0;

                    Collider2D col = Physics2D.OverlapCircle(worldPoint, nodeRadius);

                    if (col != null)
                        walkableRegionsDictionary.TryGetValue(col.gameObject.layer, out movementPenalty);

                    if (!walkable)
                    {
                        movementPenalty += obstacleProximityPenalty;
                    }


                    grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
                }
            }

            BlurPenaltyMap(blurCostAmount);

        }

        void BlurPenaltyMap(int blurSize)
        {
            int kernelSize = blurSize * 2 + 1;
            int kernelExtents = (kernelSize - 1) / 2;

            int[,] penaltiesHorizontalPass = new int[gridSizeX, gridSizeY];
            int[,] penaltiesVerticalPass = new int[gridSizeX, gridSizeY];

            for (int y = 0; y < gridSizeY; y++)
            {
                for (int x = -kernelExtents; x <= kernelExtents; x++)
                {
                    int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                    penaltiesHorizontalPass[0, y] += grid[sampleX, y].movementPenalty;
                }

                for (int x = 1; x < gridSizeX; x++)
                {
                    int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSizeX);
                    int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridSizeX - 1);

                    penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] - grid[removeIndex, y].movementPenalty + grid[addIndex, y].movementPenalty;
                }
            }

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = -kernelExtents; y <= kernelExtents; y++)
                {
                    int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                    penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];
                }

                int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
                grid[x, 0].movementPenalty = blurredPenalty;

                for (int y = 1; y < gridSizeY; y++)
                {
                    int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSizeY);
                    int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridSizeY - 1);

                    penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];
                    blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                    grid[x, y].movementPenalty = blurredPenalty;

                    if (blurredPenalty > penaltyMax)
                    {
                        penaltyMax = blurredPenalty;
                    }
                    if (blurredPenalty < penaltyMin)
                    {
                        penaltyMin = blurredPenalty;
                    }
                }
            }

        }

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }

            return neighbours;
        }

        public Node NodeFromWorldPoint(Vector2 worldPosition)
        {
            float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
            float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
            return grid[x, y];
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y));
            if (grid != null && displayGridGizmos)
            {
                foreach (Node n in grid)
                {

                    Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, n.movementPenalty));
                    Gizmos.color = (n.walkable) ? Gizmos.color : Color.red;
                    Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, gizmosAlpha);
                    Gizmos.DrawCube(n.worldPosition, Vector2.one * nodeDiameter);
                }
            }
        }

        [System.Serializable]
        public class TerrainType
        {
            public LayerMask terrainMask;
            public int terrainPenalty;
        }


    }
}
