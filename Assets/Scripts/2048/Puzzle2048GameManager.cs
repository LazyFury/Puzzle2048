using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine.InputSystem;

namespace Puzzle2048
{

    public class Puzzle2048GameManager : Actor
    {
        #region InSideClass
        [Serializable]
        class SetupBoard
        {
            public int width;
            public int height;

            public SetupBoard(int width, int height)
            {
                this.width = width;
                this.height = height;
            }
        }
        [Serializable]
        class SetupTile
        {
            public float width;
            public float height;

            public SetupTile(float width, float height)
            {
                this.width = width;
                this.height = height;
            }
        }


        #endregion

        #region Setup
        [SerializeField] SetupBoard boardConfig = new SetupBoard(5, 5);
        [SerializeField] SetupTile tileConfig = new SetupTile(0.9f, 0.9f);
        [SerializeField] Transform background;
        [SerializeField] TileFactory tileFactory;

        [SerializeField] UnityEngine.SpriteRenderer ground;
        [SerializeField] float groundPadding = .2f;

        PuzzleInputActions PlayerController;
        [SerializeField]
        CinemachineImpulseSource impulseSource;
        #region Step Score
        public UnityEngine.Events.UnityEvent<string> onStepChange;
        public UnityEngine.Events.UnityEvent<string> onScoreChange;
        public UnityEngine.Events.UnityEvent onStartGame;
        int step;
        int score;
        public int Step { get => step; set { step = value; onStepChange?.Invoke(step.ToString()); } }
        public int Score { get => score; set { score = value; onScoreChange?.Invoke(score.ToString()); } }
        #endregion

        #endregion


        #region RefData
        [SerializeField]
        TileValue[][] TileMatrix;
        #endregion

        private void Awake()
        {
            PlayerController = new PuzzleInputActions();
        }
        private void OnEnable()
        {
            PlayerController.Enable();
        }
        private void OnDisable()
        {
            PlayerController.Disable();
        }

        Vector2 touchStartPosition;
        private void Start()
        {
            StartGame();

            PlayerController.Puzzle2048.Move.performed += ctx =>
            {
                var direction = ctx.ReadValue<Vector2>();
                if (direction.x > 0)
                {
                    Move(Direction.Right);
                }
                else if (direction.x < 0)
                {
                    Move(Direction.Left);
                }
                else if (direction.y > 0)
                {
                    Move(Direction.Up);
                }
                else if (direction.y < 0)
                {
                    Move(Direction.Down);
                }
            };

            PlayerController.Puzzle2048.PrimaryContact.started += ctx =>
            {
                touchStartPosition = PlayerController.Puzzle2048.PrimaryPosition.ReadValue<Vector2>();
                //Debug.Log("touch Start");
            };
            PlayerController.Puzzle2048.PrimaryContact.canceled += ctx =>
            {
                if (touchStartPosition == new Vector2()) { return; }
                var move = PlayerController.Puzzle2048.PrimaryPosition.ReadValue<Vector2>() - touchStartPosition;
                float minDistance = 10;
                if (Mathf.Abs(move.x) - Mathf.Abs(move.y) > 10)
                {
                    if (move.x > minDistance)
                    {
                        Move(Direction.Right);
                    }
                    else if (move.x < -minDistance)
                    {
                        Move(Direction.Left);
                    }
                }
                else if (Mathf.Abs(move.y) - Mathf.Abs(move.x) > 10)
                {
                    if (move.y > minDistance)
                    {
                        Move(Direction.Up);
                    }
                    else if (move.y < -minDistance)
                    {
                        Move(Direction.Down);
                    }
                }
            };
        }

        private void Update()
        {

        }

        public UnityEngine.Events.UnityEvent onGameOver;
        [SerializeField]
        bool gameOver = false;
        bool GameOver
        {
            get => gameOver;
            set
            {
                gameOver = value;
                if (gameOver)
                {
                    onGameOver?.Invoke();
                }
            }
        }
        private void Move(Direction direction)
        {
            //有效的移动，如果有效可以生成新的Tile
            bool validMove = false;

            //处理Tile移动
            validMove = sortTileMatrix(direction);

            //处理可抵消
            direction.TileMatrixLoopWithDiection(TileMatrix, (int x, int y, ref TileValue[][] matrix, Direction dir) =>
            {

                void UpgradeTile(ref TileValue origin, ref TileValue neighbor)
                {
                    if (neighbor.tile != null && origin.tile.Type == neighbor.tile.Type)
                    {
                        if (origin.tile.Type == TileType.OneThousandTwoHundredTwentyFour) { return; }
                        Score += (int)origin.tile.Type;
                        origin.UpgradeTile(tileFactory);
                        neighbor.tile.Recycle();
                        neighbor.tile = null;
                        impulseSource.GenerateImpulseWithForce(.1f);
                    }
                }

                if (matrix[x][y] == null || matrix[x][y].tile == null) { return; }

                if (dir == Direction.Up)
                {
                    if (y > matrix[x].Length - 1 || y <= 0)
                    {
                        return;
                    }
                    UpgradeTile(ref matrix[x][y], ref matrix[x][y - 1]);
                }
                else if (dir == Direction.Down)
                {
                    if (y >= matrix[x].Length - 1 || y < 0)
                    {
                        return;
                    }
                    UpgradeTile(ref matrix[x][y], ref matrix[x][y + 1]);
                }
                else if (dir == Direction.Left)
                {
                    if (x > matrix.Length - 1 || x <= 0)
                    {
                        return;
                    }
                    UpgradeTile(ref matrix[x][y], ref matrix[x - 1][y]);
                }
                else if (dir == Direction.Right)
                {
                    if (x >= matrix.Length - 1 || x < 0)
                    {
                        return;
                    }
                    UpgradeTile(ref matrix[x][y], ref matrix[x + 1][y]);
                }
            });

            //移除可抵消Tile后第二次重排
            sortTileMatrix(direction);
            //本来以为 delegate 回调会有执行顺序的问题，其实是同步的
            //协程保留，后期处理动画
            if (validMove)
            {
                StartCoroutine(nameof(TryGenerateTile));
                Step++;
                impulseSource.GenerateImpulseWithForce(.1f);
                Vibrator.Vibrate(60);
            }

            int canMoveStep = 0;
            /*checkGameOver*/
            direction.TileMatrixLoopWithDiection(TileMatrix, (int x, int y, ref TileValue[][] matrix, Direction dir) =>
            {
                if (matrix[x][y] == null || matrix[x][y].tile == null)
                {
                    canMoveStep++;
                    return;
                }
                if (x >= 0 && y >= 0 && x < matrix.Length-1 && y < matrix[x].Length-1)
                {
                    if (matrix[x + 1][y].tile != null && matrix[x + 1][y].tile.Type == matrix[x][y].tile.Type)
                    {
                        canMoveStep++;
                    }
                    if (matrix[x][y + 1].tile != null && matrix[x][y + 1].tile.Type == matrix[x][y].tile.Type)
                    {
                        canMoveStep++;
                    }
                }
            });


            if (canMoveStep <= 0)
            {
                //show Game Over
                GameOver = true;
            }

            bool sortTileMatrix(Direction direction)
            {
                bool isValidMove = false;
                direction.TileMatrixLoopWithDiection(TileMatrix, (int x, int y, ref TileValue[][] matrix, Direction dir) =>
                {
                    if (matrix[x][y] == null || matrix[x][y].tile == null) { return; }
                    var newX = x;
                    var newY = y;

                    if (dir == Direction.Right)
                    {
                        while (newX < boardConfig.width - 1 && matrix[newX + 1][newY].tile == null)
                        {
                            newX++;
                        }
                    }
                    if (dir == Direction.Left)
                    {
                        while (newX > 0 && matrix[newX - 1][newY].tile == null)
                        {
                            newX--;
                        }
                    }
                    if (dir == Direction.Down)
                    {
                        while (newY > 0 && matrix[newX][newY - 1].tile == null)
                        {
                            newY--;
                        }
                    }
                    if (dir == Direction.Up)
                    {
                        while (newY < boardConfig.height - 1 && matrix[newX][newY + 1].tile == null)
                        {
                            newY++;
                        }
                    }

                    if (newX != x || newY != y)
                    {
                        //Debug.Log($"Change{x},{y} .{newX},{newY},{matrix[x][y].tile == null}");
                        matrix[newX][newY].SetTile(matrix[x][y].tile);
                        matrix[x][y].SetTile(null);
                        isValidMove = true;
                    }
                });
                return isValidMove;
            }



        }

        IEnumerator TryGenerateTile()
        {
            //Debug.Log("Can Move Try Generate Tile");

            yield return new WaitForSeconds(.2f);
            GenerateTile(1);
        }

        public void InitBoard()
        {
            ground.size = new Vector2(
                (boardConfig.width * tileConfig.width) + groundPadding,
                (boardConfig.height * tileConfig.height) + groundPadding
            );

            //clean children
            RemoveAllAttachActors();
            //init array
            ResetTileMatrix();
            //spawn background child
            Vector2 center = new Vector2((float)((boardConfig.width - 1) * tileConfig.width) / 2, (float)((boardConfig.height - 1) * tileConfig.height) / 2);
            Debug.Log(center);
            for (int i = 0, x = 0; x < boardConfig.width; x++)
            {
                for (int y = 0; y < boardConfig.height; y++, i++)
                {
                    Transform tile = Instantiate(background, transform);
                    tile.name = $"TileBackground_{x}_{y}";
                    tile.transform.localPosition = new Vector3(x * tileConfig.width - center.x, y * tileConfig.height - center.y, 0);
                    TileMatrix[x][y].background = tile;
                }
            }

        }

        public void EndGame()
        {
            ResetTileMatrix();
        }

        private void ResetTileMatrix()
        {
            TileMatrix = new TileValue[boardConfig.width][];
            for (int x = 0; x < boardConfig.width; x++)
            {
                TileMatrix[x] = new TileValue[boardConfig.height];
                for (int y = 0; y < boardConfig.height; y++)
                {
                    TileMatrix[x][y] = new TileValue()
                    {
                        background = null,
                        tile = null,
                    };
                }
            }
        }


        public void StartGame()
        {
            onStartGame?.Invoke();
            Score = 0;
            Step = 0;
            GameOver = false;

            InitBoard();
            GenerateTile(8);

        }

        private void GenerateTile(int count)
        {
            var list = ExtractValueList(ExtractValueListResult.Empty);
            count = Mathf.Max(count, 0);//不可小于0
            count = Mathf.Min(count, list.Count);//不可大于list.Count

            Dictionary<int, string> indexList = new Dictionary<int, string>();
            for (; count > 0;)
            {
                int index = UnityEngine.Random.Range(0, list.Count);
                //不在相同位置生成
                if (indexList.ContainsKey(index))
                {
                    continue;
                }

                indexList[index] = "";
                count--;

                var offset = list[index];
                var typeInt = Mathf.Pow(2, UnityEngine.Random.Range(1, 3));

                Tile tile = tileFactory.Get((TileType)typeInt);
                Debug.Assert(tile != null, $"tile is null,type is {(TileType)typeInt}");
                TileMatrix[offset.x][offset.y]?.SetTile(tile);


            }
        }

        enum ExtractValueListResult
        {
            Empty, valuable
        }
        private List<Vector2Int> ExtractValueList(ExtractValueListResult type)
        {
            List<Vector2Int> list = new List<Vector2Int>();
            for (int x = 0; x < TileMatrix.Length; x++)
            {
                for (int y = 0; y < TileMatrix[x].Length; y++)
                {
                    TileValue tileValue = TileMatrix[x][y];
                    if (tileValue == null || tileValue.tile == null)
                    {
                        if (type == ExtractValueListResult.Empty)
                            list.Add(new Vector2Int(x, y));
                    }
                    else
                    {
                        if (type == ExtractValueListResult.valuable)
                            list.Add(new Vector2Int(x, y));
                    }
                }
            }
            return list;
        }


    }
}

