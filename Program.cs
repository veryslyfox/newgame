using System.Diagnostics;
enum Cell
{
    Empty,
    Player,
    Barrier,
    Exit,
    Projectile,
}
static class Program
{
    public static Cell[,] _field = new Cell[30, 30];
    public static int _fx = 29;
    public static int _fy = 29;
    public static int _playerX, _playerY;
    public static Random _rng = new();
    private static string _error = "";
    private static bool _isWin;
    private static bool _isGameOver;
    private static long _time;
    private static long _moveProjectilesTime;
    static void Main()
    {
        TimeSync();
        _moveProjectilesTime = _time;
        Console.CursorVisible = false;
        // for (int i = 0; i < 100; i++)
        // {
        //     _field[_rng.Next(_field.GetLength(0)), _rng.Next(_field.GetLength(1))] = Cell.Barrier;
        // }
        for (int i = 0; i < 100; i++)
        {
            _field[_rng.Next(_field.GetLength(0)), _rng.Next(_field.GetLength(1))] = Cell.Projectile;
        }
        _field[_field.GetLength(0) - 1, _field.GetLength(1) - 1] = Cell.Exit;
        DrawField();
        while (!(_isWin || _isGameOver))
        {
            ProcessInput();
            ProcessLogic();
            DrawField();
        }
        if (_isWin)
        {
            Console.WriteLine("Game win!");
        }
        else
        {
            Console.WriteLine("Game over");
        }
        Console.ReadLine();
    }
    private static void ProcessLogic()
    {
        _field[_playerX, _playerY] = Cell.Player;
        TimeSync();
        if (_time - _moveProjectilesTime > 500)
        {
            for (int column = 0; column < _fx; column++)
            {
                for (int row = 0; row < _fy; row++)
                {
                    if (_field[column, row] == Cell.Projectile)
                    {
                        AddProjectile(column, row);
                    }
                }
            }
            _moveProjectilesTime = _time;
        }
    }
    private static void ProcessInput()
    {
        if (!Console.KeyAvailable)
        {
            return;
        }
        var key = Console.ReadKey(true);
        switch (key.Key)
        {
            case ConsoleKey.W:
            case ConsoleKey.UpArrow:
                OffsetPlayer(0, -1);
                break;
            case ConsoleKey.S:
            case ConsoleKey.DownArrow:
                OffsetPlayer(0, 1);
                break;
            case ConsoleKey.A:
            case ConsoleKey.LeftArrow:
                OffsetPlayer(-1, 0);
                break;
            case ConsoleKey.D:
            case ConsoleKey.RightArrow:
                OffsetPlayer(1, 0);
                break;
        }
    }
    private static void OffsetPlayer(int x, int y)
    {
        var xNext = _playerX + x;
        var yNext = _playerY + y;
        if (xNext < 0 || yNext < 0 || xNext > _field.GetLength(0) - 1 || yNext > _field.GetLength(1) - 1)
        {
            return;
        }
        var cell = _field[xNext, yNext];
        if (cell == Cell.Barrier)
        {
            return;
        }
        if (cell == Cell.Exit)
        {
            _isWin = true;
        }
        if (cell == Cell.Projectile)
        {
            _isGameOver = true;
        }
        _field[_playerX, _playerY] = Cell.Empty;
        _playerX = xNext;
        _playerY = yNext;
    }
    private static void AddProjectile(int x, int y)
    {
        if (y != _field.GetLength(1) - 1)
        {
            _field[x, y] = Cell.Empty;
            _field[x, y + 1] = Cell.Projectile;
        }
    }
    private static void DrawField()
    {
        Console.SetCursorPosition(0, 0);
        for (int y = 0; y < _field.GetLength(1); y++)
        {
            for (int x = 0; x < _field.GetLength(0); x++)
            {
                char symbol = '?';
                switch (_field[x, y])
                {
                    case Cell.Empty:
                        symbol = '.';
                        break;
                    case Cell.Player:
                        symbol = '!';
                        break;
                    case Cell.Barrier:
                        symbol = '#';
                        break;
                    case Cell.Exit:
                        symbol = '>';
                        break;
                    case Cell.Projectile:
                        symbol = '+';
                        break;
                }
                Console.Write(symbol);
            }
            Console.WriteLine();
        }
        Console.WriteLine(_error);
        TimeSync();
        Console.WriteLine(_time);
    }
    // static Direct GetDirect(this Cell cell)
    // {
    //     return cell switch
    //     {
    //         Cell.ProjectileD => Direct.Down,
    //         Cell.ProjectileL => Direct.Left,
    //         Cell.ProjectileR => Direct.Right,
    //         Cell.ProjectileU => Direct.Up,
    //         _ => Direct.Never,
    //     };
    // }
    static void TimeSync()
    {
        _time = (long)(Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency * 1000);
    }
}
enum Direct
{
    Left,
    Right,
    Up,
    Down,
    Never,
}