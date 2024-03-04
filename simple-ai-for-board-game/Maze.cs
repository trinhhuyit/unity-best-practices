using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Models
{
    public class Maze
    {
        private readonly int[,] _data;
        private readonly int _width;
        private readonly int _height;

        private static readonly int[] DirX = {0, 1, 0, -1, -1, 1, 1, -1};
        private static readonly int[] DirY = {1, 0, -1, 0, 1, 1, -1, -1};

        public Maze(int width, int height, int[,] data)
        {
            this._data = data;
            this._width = width;
            this._height = height;
        }


        public bool IsInMaze(MovePoint point)
        {
            return IsInMaze(point.X, point.Y);
        }

        private bool IsInMaze(int x, int y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _height;
        }

        public bool IsOnRoad(int x, int y)
        {
            return _data[x, y] == 1;
        }

        public MovePoint GetNearestPoint(MovePoint end, MovePoint currentPosition)
        {
            if (_data[end.X, end.Y] == 1) return end;
            else
            {
                List<MovePoint> allPoints = new List<MovePoint>();
                MovePoint mvUp;
                for (int i = end.Y; i >= 0; i--)
                {
                    if (_data[end.X, i] == 1)
                    {
                        //Up
                        allPoints.Add(MovePoint.Create(end.X, i));
                        break;
                    }
                }

                MovePoint mvDown;
                for (int i = end.Y; i < _height; i++)
                {
                    if (_data[end.X, i] == 1)
                    {
                        //Down
                        allPoints.Add(MovePoint.Create(end.X, i));
                        break;
                    }
                }

                MovePoint mvLeft;
                for (int i = end.X; i >= 0; i--)
                {
                    if (_data[i, end.Y] == 1)
                    {
                        //Left
                        allPoints.Add(MovePoint.Create(i, end.Y));
                        break;
                    }
                }

                MovePoint mvRight;
                for (int i = end.X; i < _width; i++)
                {
                    if (_data[i, end.Y] == 1)
                    {
                        //Right
                        allPoints.Add(MovePoint.Create(i, end.Y));
                        break;
                    }
                }

                return allPoints.OrderBy(x => MovePoint.Distance(x, currentPosition)).First();
            }
        }

        public List<MovePoint> FindRoad(MovePoint start, MovePoint end)
        {
            Queue<MovePoint> queuePoint = new Queue<MovePoint>();
            List<MovePoint> checkedPoint = new List<MovePoint>();

            List<MovePoint> road = new List<MovePoint>();

            //Enqueue start point to queue
            queuePoint.Enqueue(start);

            //Add current check point to checkedPoint list
            checkedPoint.Add(start);

            while (queuePoint.Count != 0)
            {
                //Get current point from queue.
                MovePoint currentPoint = queuePoint.Dequeue();
                for (int i = 0; i < 4; i++)
                {
                    //Calculate next point
                    int nextX = currentPoint.X + DirX[i];
                    int nextY = currentPoint.Y + DirY[i];
                    if (IsInMaze(nextX, nextY) &&
                        _data[nextX, nextY] == 1 &&
                        !checkedPoint.Any(x => x.X == nextX && x.Y == nextY))
                    {
                        if (nextX == end.X && nextY == end.Y)
                        {
                            road.Add(MovePoint.Create(nextX, nextY));
                            road.Add(MovePoint.Create(currentPoint.X, currentPoint.Y));

                            while (currentPoint.LastPoint != null)
                            {
                                currentPoint = currentPoint.LastPoint;
                                road.Add(MovePoint.Create(currentPoint.X, currentPoint.Y));
                            }

                            return road;
                        }
                        else
                        {
                            MovePoint newPoint = MovePoint.Create(nextX, nextY, currentPoint);
                            queuePoint.Enqueue(newPoint);
                            checkedPoint.Add(newPoint);
                        }
                    }
                }
            }

            return road;
        }
    }
}