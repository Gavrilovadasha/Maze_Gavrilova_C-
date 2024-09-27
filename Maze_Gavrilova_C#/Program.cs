using System;
using System.IO;
using System.Reflection.Emit;
using System.Threading;

namespace Maze_Gavrilova_C
{
    class Maze
    {
        static void Main()
        {
            /// <summary>
            /// Основная точка входа в программу. Инициализирует игровые параметры,
            /// запускает основной цикл игры и обработку окончания игры.
            /// </summary>
            Console.WindowWidth = 80;
            Console.WindowHeight = 45;

            string mapFileName = "map.txt";
            char playerSymbol = 'm';
            char endPoint = 'C';
            char[,] map;
            bool isEndGame = false;
            int playerStepValue = 1;

            Console.CursorVisible = false;

            map = ReadMap(mapFileName);

            GetPositionEndPoint(map, endPoint, out int endLevelX, out int endLevelY);

            /// <summary>
            /// Основной цикл игры. Продолжается до тех пор,
            /// пока игрок не достиг конца уровня.
            /// </summary>
            while (!isEndGame)
            {
                ShowMap(map);

                GetPlayerPosition(map, playerSymbol, out int playerX, out int playerY);

                GetCommpandFromPlayer(playerStepValue, out int playerDeltaX, out int playerDeltaY);

                map = TryChangePlayerPosition(map, playerSymbol, playerX, playerY, playerDeltaX, playerDeltaY,
                                              out int playerXNext,
                                              out int playerYNext);

                isEndGame = CheckEndLevel(playerXNext, playerYNext, endLevelX, endLevelY);
            }
            Console.Clear();
            Console.WriteLine("Игра окончена, Вы выиграли!");
            Console.ReadKey();
        }

        static char[,] ReadMap(string mapFileName)
        {
            /// <summary>
            /// Читает карту из файла и возвращает ее как двумерный массив символов.
            /// </summary>
            string[] tempMap = File.ReadAllLines(mapFileName);

            var maxIMap = tempMap.Length;
            var maxJmap = tempMap[0].ToCharArray().Length;

            char[,] map = new char[maxIMap, maxJmap];

            for (var i = 0; i < maxIMap; i++)
            {
                for (var j = 0; j < maxJmap; j++)
                {
                    map[i, j] = tempMap[i][j];
                }
            }
            return map;
        }

        static void GetPositionEndPoint(char[,] map, char endLevelSymbol, out int endLevelX, out int endLevelY)
        {
            /// <summary>
            /// Находит позицию конца уровня на карте и сохраняет ее координаты.
            /// </summary>
            endLevelX = 0;
            endLevelY = 0;

            for (var i = 0; i < map.GetLength(0); i++)
            {
                for (var j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == endLevelSymbol)
                    {
                        endLevelX = j;
                        endLevelY = i;
                    }
                }
            }
        }

        static void ShowMap(char[,] map)
        {
            /// <summary>
            /// Отображает карту на консоли, используя Console.SetCursorPosition().
            /// </summary>
            var startX = 0;
            var startY = 0;

            for (var i = 0; i < map.GetLength(0); i++)
            {
                for (var j = 0; j < map.GetLength(1); j++)
                {
                    Console.SetCursorPosition(startX + j, startY + i);
                    Console.Write(map[i, j]);
                }
            }
        }

        static void GetPlayerPosition(char[,] map, char player, out int playerX, out int playerY)
        {
            /// <summary>
            /// На текущую позицию игрока на карте по символу игрока.
            /// </summary>
            playerY = 0;
            playerX = 0;

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == player)
                    {
                        playerX = j;
                        playerY = i;
                    }
                }
            }
        }

        static void GetCommpandFromPlayer(int playerStep, out int playerDeltaX, out int playerDeltaY)
        {
            /// <summary>
            /// Получает команду от пользователя (стрелки) и преобразует ее в изменения координат.
            /// </summary>
            playerDeltaY = 0;
            playerDeltaX = 0;

            ConsoleKey command = Console.ReadKey().Key;

            switch (command)
            {
                case ConsoleKey.UpArrow:
                    playerDeltaX = 0;
                    playerDeltaY = -playerStep;
                    break;
                case ConsoleKey.DownArrow:
                    playerDeltaX = 0;
                    playerDeltaY = playerStep;
                    break;
                case ConsoleKey.LeftArrow:
                    playerDeltaX = -playerStep;
                    playerDeltaY = 0;
                    break;
                case ConsoleKey.RightArrow:
                    playerDeltaX = playerStep;
                    playerDeltaY = 0;
                    break;
                default:
                    return;
            }
        }

        static char[,] TryChangePlayerPosition(char[,] map, char playerSymbol, int playerX, int playerY,
                                               int playerDeltaX, int playerDeltaY,
                                               out int playerXNext,
                                               out int playerYNext)
        {
            /// <summary>
            /// Пытается изменить позицию игрока на карте, проверяет возможность движения и обновляет массив.
            /// </summary>
            playerXNext = playerX;
            playerYNext = playerY;

            if (playerX + playerDeltaX >= 0 && playerX + playerDeltaX < map.GetLength(1) &&
                playerY + playerDeltaY >= 0 && playerY + playerDeltaY < map.GetLength(0))
            {
                char newCellContent = map[playerY + playerDeltaY, playerX + playerDeltaX];

                if (newCellContent != '|' && newCellContent != '_')
                {
                    map[playerY + playerDeltaY, playerX + playerDeltaX] = playerSymbol;

                    if (map[playerY, playerX] != 'a')
                    {
                        map[playerY, playerX] = ' ';
                    }

                    playerXNext = playerX + playerDeltaX;
                    playerYNext = playerY + playerDeltaY;
                }
                else
                {
                    Console.WriteLine("Вы не можете пройти в эту сторону");
                }
            }
            else
            {
                Console.WriteLine("Вы вышли за границу лабиринта");
            }

            return map;
        }

        private static bool CheckEndLevel(int playerX, int playerY, int endLevelX, int endLevelY)
        {
            /// <summary>
            /// Проверяет, достиг ли игрок конца уровня.
            /// </summary>
            bool isEnd = false;

            if (playerX == endLevelX && playerY == endLevelY)
            {
                isEnd = true;
            }
            return isEnd;
        }
    }
}
