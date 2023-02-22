enum Cell
{
    Empty,
    Player
}
static class Program
{
    public static Cell[,] _field = new Cell[30, 30];
    public static int _playerX, _playerY;
    static void Main()
    {
        while (true)
        {
            ProcessInput();
            DrawField();
        }
    }

    private static void ProcessInput()
    {
        var key = Console.ReadKey();
        switch (key.Key)
        {
            case ConsoleKey.W:
            case ConsoleKey.UpArrow:
                if (_playerY != 0)
                    _playerY--;
                break;
            case ConsoleKey.S:
            case ConsoleKey.DownArrow:
                if (_playerY != _field.GetLength(1) - 1)
                    _playerY++;
                break;
            case ConsoleKey.A:
            case ConsoleKey.LeftArrow:
                if (_playerX != 0)
                    _playerX--;
                break;
            case ConsoleKey.D:
            case ConsoleKey.RightArrow:
                if (_playerX != _field.GetLength(0) - 1)
                    _playerX++;
                break;
        }
    }
    private static void DrawField()
    {
        Console.Clear();
        Console.SetCursorPosition(_playerX, _playerY);
        for (int y = 0; y < _field.GetLength(1); y++)
        {
            for (int x = 0; x < _field.GetLength(0); x++)
            {
                if (_field[x, y] == Cell.Player)
                {
                    Console.Write('!');
                }

            }
            Console.WriteLine();
        }
    }
}