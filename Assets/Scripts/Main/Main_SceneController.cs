using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Common.DictionaryExtension;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// シーンコントローラ
/// </summary>
public class Main_SceneController : MonoBehaviour
{
    static int gameLevel = 1;
    static Main_SceneController instance;

    public static Main_SceneController Instance
    {
        get { return instance; }
    }

    public Main_PlayerCharacter playerCharacter;
    [SerializeField]
    GameObject staticWallPrefab;
    [SerializeField]
    Main_Wall wallPrefab;
    [SerializeField]
    GameObject floorPrefab;
    [SerializeField]
    Main_Bom bomPrefab;
    [SerializeField]
    Main_Pitfall pitfallPrefab;
    [SerializeField]
    Main_Fire firePrefab;
    [SerializeField]
    Text bomAmount;
    [SerializeField]
    Text pitfallAmount;
    [SerializeField]
    Text instantWallAmount;
    [SerializeField]
    Text gameLevelText;
    [SerializeField]
    Text messageText;
    [SerializeField]
    Main_Stairs stairs;
    [SerializeField]
    Main_EnemySimple enemySimplePrefab;
    [SerializeField]
    Main_EnemyHoming enemyHomingPrefab;
    [SerializeField]
    Main_EnemyIgnoreWall enemyIgnoreWallPrefab;

    Dictionary<Main_MapGenerator.Coordinate, GameObject> staticWalls = new Dictionary<Main_MapGenerator.Coordinate, GameObject>();
    Dictionary<Main_MapGenerator.Coordinate, Main_Wall> walls = new Dictionary<Main_MapGenerator.Coordinate, Main_Wall>();
    Dictionary<Main_MapGenerator.Coordinate, Main_Bom> boms = new Dictionary<Main_MapGenerator.Coordinate, Main_Bom>();
    Dictionary<Main_MapGenerator.Coordinate, Main_Pitfall> pitfalls = new Dictionary<Main_MapGenerator.Coordinate, Main_Pitfall>();

    public static void StartGame()
    {
        gameLevel = 1;
        SceneManager.LoadScene("Main");
    }

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
        // マップ情報生成
        var mapGenerator = new Main_MapGenerator();
        var roomSettings = new Main_MapGenerator.RoomSettings();
        roomSettings.minWidth = 2;
        roomSettings.minHeight = 2;
        roomSettings.bigRoomRate = 1;
        roomSettings.maxWallThicknessInArea = 2;
        var width = Random.Range(20, 20 + gameLevel);
        var height = Random.Range(20, 20 + gameLevel);
        var map = mapGenerator.Generate(width, height, roomSettings);

        // プレイヤーキャラ配置
        var emptyFloors = map.Where(x => x.type == Main_MapGenerator.Cell.Types.Floor).ToList();
        var playerCell = emptyFloors[UnityEngine.Random.Range(0, emptyFloors.Count)];
        playerCharacter.transform.position = CoordinateToPosition(playerCell.coordinate);
        emptyFloors.Remove(playerCell);
        // プレイヤー周囲のマスは何も置かない
        emptyFloors.RemoveAll(x => Mathf.Sqrt(Mathf.Pow(x.coordinate.x - playerCell.coordinate.x, 2f) + Mathf.Pow(x.coordinate.y - playerCell.coordinate.y, 2f)) < 5f);

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

        // 階段を配置
        var stairsCell = emptyFloors[UnityEngine.Random.Range(0, emptyFloors.Count)];
        stairs.transform.position = CoordinateToPosition(stairsCell.coordinate);
        emptyFloors.Remove(stairsCell);

        // 敵を配置
        for (var i = 0; i < 5 + gameLevel / 2; i++)
        {
            var enemyCell = emptyFloors[UnityEngine.Random.Range(0, emptyFloors.Count)];
            var enemy = Instantiate(enemySimplePrefab, CoordinateToPosition(enemyCell.coordinate), Quaternion.identity);
            enemy.Initialize(Random.Range(0.3f, 1f + (float)gameLevel / 10f));
            emptyFloors.Remove(enemyCell);
        }
        for (var i = 0; i < gameLevel / 3; i++)
        {
            var enemyCell = emptyFloors[UnityEngine.Random.Range(0, emptyFloors.Count)];
            var enemy = Instantiate(enemyHomingPrefab, CoordinateToPosition(enemyCell.coordinate), Quaternion.identity);
            enemy.Initialize(Random.Range(0.5f, 1f + (float)gameLevel / 10f));
            emptyFloors.Remove(enemyCell);
        }
        for (var i = 0; i < gameLevel / 6; i++)
        {
            var enemyCell = emptyFloors[UnityEngine.Random.Range(0, emptyFloors.Count)];
            var enemy = Instantiate(enemyIgnoreWallPrefab, CoordinateToPosition(enemyCell.coordinate), Quaternion.identity);
            enemy.Initialize(Random.Range(0.2f, 0.5f + (float)gameLevel / 30f));
            emptyFloors.Remove(enemyCell);
        }

        gameLevelText.text = "Lv." + gameLevel;

        StartCoroutine(ReadyCoroutine());
    }

    void Update()
    {
        bomAmount.text = "x" + Main_PlayerCharacter.bomAmount;
        pitfallAmount.text = "x" + Main_PlayerCharacter.pitfallAmount;
        instantWallAmount.text = "x" + Main_PlayerCharacter.instantWallAmount;
    }

    IEnumerator ReadyCoroutine()
    {
        Time.timeScale = 0f;
        messageText.text = "Ready...";
        yield return new WaitForSecondsRealtime(2f);
        messageText.text = "GO!!";
        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(1f);
        messageText.text = "";
    }

    /// <summary>
    /// 爆弾を出現させます
    /// </summary>
    /// <returns>The bom.</returns>
    /// <param name="position">Position.</param>
    /// <param name="firePower">Fire power.</param>
    public void SpawnBom(Vector3 position, int firePower)
    {
        Main_AudioManager.Instance.put.Play();

        var coordinate = Main_SceneController.Instance.PositionToCoordinate(position);
        var bom = Instantiate(bomPrefab, CoordinateToPosition(coordinate), Quaternion.identity);
        bom.Initialize(coordinate, firePower);
        boms.Add(coordinate, bom);
    }

    /// <summary>
    /// 落とし穴を出現させます
    /// </summary>
    /// <param name="position">Position.</param>
    public void SpawnPitfall(Vector3 position)
    {
        Main_AudioManager.Instance.put.Play();

        var coordinate = Main_SceneController.Instance.PositionToCoordinate(position);
        var pitfall = Instantiate(pitfallPrefab, CoordinateToPosition(coordinate), Quaternion.identity);
        pitfalls.Add(coordinate, pitfall);
    }

    /// <summary>
    /// 壁を出現させます
    /// </summary>
    /// <param name="position">Position.</param>
    public void SpawnWall(Vector3 position)
    {
        Main_AudioManager.Instance.put.Play();

        var coordinate = Main_SceneController.Instance.PositionToCoordinate(position);
        var wall = Instantiate(wallPrefab, CoordinateToPosition(coordinate), Quaternion.identity);
        walls.Add(coordinate, wall);
    }

    /// <summary>
    /// 炎を出現させます
    /// </summary>
    /// <param name="coordinate">Coordinate.</param>
    /// <param name="firePower">Fire power.</param>
    public void SpawnFire(Main_MapGenerator.Coordinate coordinate, int firePower)
    {
        walls.RemoveAll((k, v) => null == v);

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
    /// 空のマスどうかを返します
    /// </summary>
    /// <returns><c>true</c> if this instance is empty cell the specified position; otherwise, <c>false</c>.</returns>
    /// <param name="position">Position.</param>
    public bool IsEmptyCell(Vector3 position)
    {
        var coordinate = Main_SceneController.Instance.PositionToCoordinate(position);
        return IsEmptyCell(coordinate);
    }

    /// <summary>
    /// 空のマスどうかを返します
    /// </summary>
    /// <returns><c>true</c> if this instance is empty cell the specified coordinate; otherwise, <c>false</c>.</returns>
    /// <param name="coordinate">Coordinate.</param>
    public bool IsEmptyCell(Main_MapGenerator.Coordinate coordinate, bool ignorePitfalls = false, bool ignoreWalls = false)
    {
        boms.RemoveAll((k, v) => null == v);
        pitfalls.RemoveAll((k, v) => null == v);
        walls.RemoveAll((k, v) => null == v);

        return !boms.ContainsKey(coordinate) &&
        (ignorePitfalls || !pitfalls.ContainsKey(coordinate)) &&
        (ignoreWalls || !walls.ContainsKey(coordinate))
        && !staticWalls.ContainsKey(coordinate);
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

    /// <summary>
    /// ゲームオーバー処理
    /// </summary>
    public void GameOver()
    {
        StartCoroutine(GameOverCoroutine());
    }

    /// <summary>
    /// ゲームオーバー処理コルーチン
    /// </summary>
    /// <returns>The over coroutine.</returns>
    IEnumerator GameOverCoroutine()
    {
        messageText.text = "Game Over";
        yield return new WaitForSecondsRealtime(5f);
        gameLevel = 1;
        Main_PlayerCharacter.bomAmount = 3;
        Main_PlayerCharacter.pitfallAmount = 3;
        Main_PlayerCharacter.instantWallAmount = 3;
        SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// 次のレベルに進む処理
    /// </summary>
    public void NextLevel()
    {
        StartCoroutine(NextLevelCoroutine());
    }

    /// <summary>
    /// 次のレベルに進むコルーチン
    /// </summary>
    /// <returns>The level coroutine.</returns>
    IEnumerator NextLevelCoroutine()
    {
        Time.timeScale = 0f;
        messageText.text = string.Format("Lv.{0} Clear!", gameLevel);
        yield return new WaitForSecondsRealtime(2f);
        gameLevel++;
        // 罠を１個ずつ補充
        Main_PlayerCharacter.bomAmount += 1;
        Main_PlayerCharacter.pitfallAmount += 1;
        Main_PlayerCharacter.instantWallAmount += 1;
        SceneManager.LoadScene("Main");
    }
}
