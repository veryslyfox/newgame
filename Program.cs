using System.Diagnostics;
enum Cell
{
    Empty,
    Player,
    Barrier,
    Exit,
    Projectile,
    OffsetedProjectile,
    Cannon,
    Never,
    Bomb,
}
static class Program
{
    const long ProjectileDelay = 500;
    const long CannonDelay = 2500;
    public static Cell[,] _field = new Cell[10, 16];
    public static int _fx = _field.GetLength(0) - 1;
    public static int _fy = _field.GetLength(1) - 1;
    public static int _playerX, _playerY;
    public static Random _rng = new();
    private static string _error = "";
    private static bool _isWin;
    private static bool _isGameOver;
    private static long _time;
    private static long _moveProjectilesTime;
    private static long _cannonTime;
    private static double _temp = 1;
    private static int _rounds;
    private static Cell _moveCell = Cell.Empty;
    private static bool _active = true;
    static void Main()
    {
    A:
        Array.Clear(_field);
        _isWin = false;
        _isGameOver = false;
        _playerX = 0;
        _playerY = 0;
        Read("Lvl");
        TimeSync();
        _moveProjectilesTime = _time;
        _temp = 0.01;
        _rounds = 0;
        Console.CursorVisible = false;
        DrawField();
        while (!(_isWin || _isGameOver))
        {
            ProcessInput();
            ProcessLogic();
            DrawField();
            _rounds++;
        }
        if (_isWin)
        {
            Console.WriteLine("Game win!");
        }
        else
        {
            Console.WriteLine("Game over");
        }
        Console.ReadKey();
        goto A;
    }
    private static void ProcessLogic()
    {
        if (_rounds == 80)
        {
            _temp = 1;
        }
        OffsetPlayer(0, 0);
        _field[_playerX, _playerY] = Cell.Player;
        if (_active)
        {
            TimeSync();
            var isFire = false;
            var isMove = false;
            for (int column = 0; column <= _fx; column++)
            {
                for (int row = 0; row <= _fy; row++)
                {
                    if (_field[column, row] == Cell.Projectile && _time - _moveProjectilesTime > ProjectileDelay * _temp)
                    {
                        AddProjectile(column, row);
                        isMove = true;
                    }
                    if (_field[column, row] == Cell.OffsetedProjectile)
                    {
                        _field[column, row] = Cell.Projectile;
                    }
                    if (_field[column, row] == Cell.Cannon && (_time - _cannonTime) > CannonDelay * _temp)
                    {
                        if (_field[column, row] == Cell.Player)
                        {
                            _isGameOver = true;
                        }
                        _field[column, row + 1] = Cell.Projectile;
                        isFire = true;
                    }
                }
            }
            if (isFire)
            {
                _cannonTime = _time;
            }
            if (isMove)
            {
                _moveProjectilesTime = _time;
            }
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
            case ConsoleKey.R:
            ExecutionInput();
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
        if (cell is Cell.Barrier or Cell.Cannon)
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
        _field[_playerX, _playerY] = _moveCell;
        _playerX = xNext;
        _playerY = yNext;
    }
    private static void AddProjectile(int x, int y)
    {
        if (y != _field.GetLength(1) - 1)
        {
            if (_field[x, y + 1] is Cell.Barrier or Cell.Exit)
            {
                goto end;
            }
            _field[x, y] = Cell.Empty;
            if (_field[x, y + 1] == Cell.Player)
            {
                _isGameOver = true;
                return;
            }
            _field[x, y + 1] = Cell.OffsetedProjectile;
            return;
        }
        _field[x, y] = Cell.Empty;
    end:;
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
                        symbol = ' ';
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
                        symbol = 'v';
                        break;
                    case Cell.Cannon:
                        symbol = '/';
                        break;
                }
                Console.Write(symbol);
            }
            Console.WriteLine();
        }
        Console.WriteLine(_error);
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
    static void Init(string name)
    {
        var file = File.OpenText(name);
        var column = 0;
        var row = 0;
#pragma warning disable
        for (int i = 0; i <= long.MaxValue; i++)
#pragma warning restore
        {
            foreach (var symbol in file.ReadLine()!)
            {
                if (file.EndOfStream)
                {
                    return;
                }
                column++;
            }
            row = i;
        }
        _field = new Cell[column, row];
    }
    static void Write(string name, int stringLength, int stringCount)
    {
        var file = File.CreateText(name);
        file.WriteLine();
    }
    static void Read(string name)
    {
        var file = File.OpenText(name);
        for (int row = 0; row <= _fx; row++)
        {
            var column = 0;
            foreach (var symbol in file.ReadLine()!)
            {
                if (file.EndOfStream)
                {
                    return;
                }
                Cell cell;
                switch (symbol)
                {
                    case '.':
                        cell = Cell.Empty;
                        break;
                    case '#':
                        cell = Cell.Barrier;
                        break;
                    case '/':
                        cell = Cell.Cannon;
                        break;
                    case '!':
                        cell = Cell.Player;
                        _playerX = column;
                        _playerY = row;
                        break;
                    case '>':
                        cell = Cell.Exit;
                        break;
                    default:
                        cell = Cell.Never;
                        break;
                }
                _field[column, row] = cell;
                column++;
            }
        }
    }
    static void PlayerClear()
    {
        for (int column = 0; column <= _fx; column++)
        {
            for (int i = 0; i <= _fy; i++)
            {

            }
        }
    }
    static void ExecutionInput()
    {
        var command = Console.ReadLine();
        if (command == "deactive")
        {
            _active = false;
        }
        if (command == "active")
        {
            _active = true;
            _moveCell = Cell.Empty;
            
        }
        if (int.TryParse(command, out int i))
        {
            _moveCell = (Cell)i;
        }
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