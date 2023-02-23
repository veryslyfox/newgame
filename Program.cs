enum Cell
{
    Empty,
    Player,
    Barrier,
    Exit,
}
static class Program
{
    public static Cell[,] _field = new Cell[30, 30];
    public static int _playerX, _playerY;
    public static Random _rng = new();
    private static string _error;
    private static bool _isWin;
    private static bool _isGameOver;
    static void Main()
    {
        Console.CursorVisible = false;
        _field[29, 29] = Cell.Exit;
        for (int i = 0; i < 200; i++)
        {
            _field[_rng.Next(30), _rng.Next(30)] = Cell.Barrier;
        }
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
    }
    private static void ProcessInput()
    {
        var key = Console.ReadKey(true);
        switch (key.Key)
        {
            case ConsoleKey.W:
            case ConsoleKey.UpArrow:
                OffsetHandle(0, -1);
                break;
            case ConsoleKey.S:
            case ConsoleKey.DownArrow:
                OffsetHandle(0, 1);
                break;
            case ConsoleKey.A:
            case ConsoleKey.LeftArrow:
                OffsetHandle(-1, 0);
                break;
            case ConsoleKey.D:
            case ConsoleKey.RightArrow:
                OffsetHandle(1, 0);
                break;
        }
    }
    private static void OffsetHandle(int x, int y)
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
        _field[_playerX, _playerY] = Cell.Empty;
        _playerX = xNext;
        _playerY = yNext;
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
                }
                Console.Write(symbol);
            }
            Console.WriteLine();
        }
        Console.WriteLine(_error);
    }
}