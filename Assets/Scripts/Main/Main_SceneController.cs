using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Common.DictionaryExtension;

/// <summary>
/// シーンコントローラ
/// </summary>
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
        // プレイヤーキャラ配置
        var playerCharacterSpawnCoordinate = new Main_MapGenerator.Coordinate(1, 1);
        playerCharacter.transform.position = CoordinateToPosition(playerCharacterSpawnCoordinate);

        // マップ情報生成
        var mapGenerator = new Main_MapGenerator();
        var map = mapGenerator.Generate(13, 13, new Main_MapGenerator.Coordinate[]{ playerCharacterSpawnCoordinate });
        // マップ情報を元にマップ組み立て
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

    }

    /// <summary>
    /// 爆弾を出現させます
    /// </summary>
    /// <returns>The bom.</returns>
    /// <param name="position">Position.</param>
    /// <param name="firePower">Fire power.</param>
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

    /// <summary>
    /// 炎を出現させます
    /// </summary>
    /// <param name="coordinate">Coordinate.</param>
    /// <param name="firePower">Fire power.</param>
    public void SpawnFire(Main_MapGenerator.Coordinate coordinate, int firePower)
    {
        // 炎の伸びる方向を定義
        var directions = new Main_MapGenerator.Coordinate[]
        {
            new Main_MapGenerator.Coordinate(1, 0),
            new Main_MapGenerator.Coordinate(-1, 0),
            new Main_MapGenerator.Coordinate(0, 1),
            new Main_MapGenerator.Coordinate(0, -1)
        };

        // まずは爆弾と同じマスに炎を出現
        Instantiate(firePrefab, CoordinateToPosition(coordinate), Quaternion.identity);
        // 各方向に対して順次炎を伸ばしていく
        foreach (var direction in directions)
        {
            for (var i = 1; i <= firePower; i++)
            {
                var targetCoordinate = coordinate + direction * i;
                if (staticWalls.ContainsKey(targetCoordinate))
                {
                    // 壊せない壁があった場合は炎を置く前に終了（該当方向にはこれ以上伸ばさない）
                    break;
                }
                var position = CoordinateToPosition(targetCoordinate);
                Instantiate(firePrefab, position, Quaternion.identity);
                if (walls.ContainsKey(targetCoordinate) || boms.ContainsKey(targetCoordinate))
                {
                    // 壊せる壁や他の爆弾があった場合は炎をそのマスに置いてから終了（該当方向にはこれ以上伸ばさない）
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 任意の位置に爆弾が配置可能かどうかを返します
    /// </summary>
    /// <returns><c>true</c> if this instance is bom puttable the specified position; otherwise, <c>false</c>.</returns>
    /// <param name="position">Position.</param>
    public bool IsBomPuttable(Vector3 position)
    {
        boms.RemoveAll((k, v) => null == v);

        var coordinate = Main_SceneController.Instance.PositionToCoordinate(position);
        return !boms.ContainsKey(coordinate) && !walls.ContainsKey(coordinate);
    }

    /// <summary>
    /// ワールド座標をマス目座標に変換します
    /// </summary>
    /// <returns>The to coordinate.</returns>
    /// <param name="position">Position.</param>
    public Main_MapGenerator.Coordinate PositionToCoordinate(Vector3 position)
    {
        var x = Mathf.RoundToInt(position.x);
        var y = Mathf.RoundToInt(position.y);
        return new Main_MapGenerator.Coordinate(x, y);
    }

    /// <summary>
    /// マス目座標をワールド座標に変換します
    /// </summary>
    /// <returns>The to position.</returns>
    /// <param name="coordinate">Coordinate.</param>
    public Vector3 CoordinateToPosition(Main_MapGenerator.Coordinate coordinate)
    {
        return new Vector3(coordinate.x, coordinate.y);
    }
}
