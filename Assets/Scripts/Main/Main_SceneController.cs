using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Common.DictionaryExtension;

public class Main_SceneController : MonoBehaviour
{
    static Main_SceneController instance;

    public static Main_SceneController Instance
    {
        get { return instance; }
    }

    [SerializeField]
    Main_PlayerCharacter playerCharacter;
    [SerializeField]
    GameObject staticWallPrefab;
    [SerializeField]
    Main_Wall wallPrefab;
    [SerializeField]
    GameObject floorPrefab;
    [SerializeField]
    Main_Bom bomPrefab;
    [SerializeField]
    Main_Fire firePrefab;

    Dictionary<Main_MapGenerator.Coordinate, GameObject> staticWalls = new Dictionary<Main_MapGenerator.Coordinate, GameObject>();
    Dictionary<Main_MapGenerator.Coordinate, Main_Wall> walls = new Dictionary<Main_MapGenerator.Coordinate, Main_Wall>();
    Dictionary<Main_MapGenerator.Coordinate, Main_Bom> boms = new Dictionary<Main_MapGenerator.Coordinate, Main_Bom>();

    void Awake()
    {
        if (null != instance)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
    }

    void Start()
    {
        var playerCharacterSpawnCoordinate = new Main_MapGenerator.Coordinate(1, 1);
        var mapGenerator = new Main_MapGenerator();
        var map = mapGenerator.Generate(13, 13, new Main_MapGenerator.Coordinate[]{ playerCharacterSpawnCoordinate });
        foreach (var cell in map)
        {
            var position = CoordinateToPosition(cell.coordinate);
            Instantiate(floorPrefab, position, Quaternion.identity);
            switch (cell.type)
            {
                case Main_MapGenerator.Cell.Types.StaticWall:
                    var staticWall = Instantiate(staticWallPrefab, position, Quaternion.identity);
                    staticWalls.Add(cell.coordinate, staticWall);
                    break;
                case Main_MapGenerator.Cell.Types.Wall:
                    var wall = Instantiate(wallPrefab, position, Quaternion.identity);
                    walls.Add(cell.coordinate, wall);
                    break;
            }
        }
        playerCharacter.transform.position = CoordinateToPosition(playerCharacterSpawnCoordinate);
    }

    public Main_Bom SpawnBom(Vector3 position, int firePower)
    {
        Main_AudioManager.Instance.put.Play();
        walls.RemoveAll((k, v) => null == v);

        var coordinate = Main_SceneController.Instance.PositionToCoordinate(position);
        var bom = Instantiate(bomPrefab, CoordinateToPosition(coordinate), Quaternion.identity);
        bom.Initialize(coordinate, firePower);
        boms.Add(coordinate, bom);
        return bom;
    }

    public void SpawnFire(Main_MapGenerator.Coordinate coordinate, int firePower)
    {
        var directions = new Main_MapGenerator.Coordinate[]
        {
            new Main_MapGenerator.Coordinate(1, 0),
            new Main_MapGenerator.Coordinate(-1, 0),
            new Main_MapGenerator.Coordinate(0, 1),
            new Main_MapGenerator.Coordinate(0, -1)
        };

        Instantiate(firePrefab, CoordinateToPosition(coordinate), Quaternion.identity);
        foreach (var direction in directions)
        {
            for (var i = 1; i <= firePower; i++)
            {
                var targetCoordinate = coordinate + direction * i;
                if (staticWalls.ContainsKey(targetCoordinate))
                {
                    break;
                }
                var position = CoordinateToPosition(targetCoordinate);
                Instantiate(firePrefab, position, Quaternion.identity);
                if (walls.ContainsKey(targetCoordinate) || boms.ContainsKey(targetCoordinate))
                {
                    break;
                }
            }
        }
    }

    public bool IsBomPuttable(Vector3 position)
    {
        boms.RemoveAll((k, v) => null == v);

        var coordinate = Main_SceneController.Instance.PositionToCoordinate(position);
        return !boms.ContainsKey(coordinate);
    }

    public Main_MapGenerator.Coordinate PositionToCoordinate(Vector3 position)
    {
        var x = Mathf.RoundToInt(position.x);
        var y = Mathf.RoundToInt(position.y);
        return new Main_MapGenerator.Coordinate(x, y);
    }

    public Vector3 CoordinateToPosition(Main_MapGenerator.Coordinate coordinate)
    {
        return new Vector3(coordinate.x, coordinate.y);
    }
}
