using UnityEngine;
using System;
public enum Cell
{
    Empty = 2,
    Void = 3,
    Black = 0,
    White = 1,
}

public enum pieceColor
{
    BLACK,
    WHITE,
}
public enum Direction
{
    VERTICAL = 0,
    TRANSVERSE = 1,
    LEFT = 2,
    RIGHT = 3,
}
public class gobangBoard : MonoBehaviour
{
    //预设的长宽是15，这里留一个接口
    //留set,get默认方法
    public int width { get; set; }
    public int length { get; set; }
    private Cell[,] board;
    public gobangBoard() {
        this.width = 15;
        this.length = 15;
    }
    public gobangBoard(int w,int l) 
    {
        width = w;
        length = l;
        board = new Cell[w, l];//棋盘数组
        for (int i = 0; i < w; i++)
            for (int j = 0; j < l; j++)
                board[i, j] = Cell.Empty;
    }

    public Cell this[int x,int y]
    {
        get
        {
            //这样处理就没有越界问题
            if (x < 0 || x >= width || y < 0 || y >= length)
                return Cell.Void;
            return board[x, y];
        }
        set
        {
            if (x < 0 || x >= width || y < 0 || y >= length)
                board[x, y] = Cell.Void;
            board[x, y] = value;
        }
    }
    

    /// <summary>
    /// 判断游戏是否结束
    /// </summary>
    /// <returns></returns>
    public bool isGameOver(pieceColor color,int i,int j)
    {
        Cell temp = Cell.Void;
        if (color == pieceColor.BLACK)
            temp = Cell.Black;
        else if (color == pieceColor.WHITE)
            temp = Cell.White;

        foreach (var item in Enum.GetValues(typeof(Direction))){
            int res = 1;//初始的时候原位置就有一个棋子
            switch (item)
            {
                case Direction.VERTICAL:
                    for(int k = 1; ;k++)
                    {
                        if (this[i + k, j] != temp)
                            break;
                        res++;
                    }
                    for (int k = 1; ; k++)
                    {
                        if (this[i - k, j] != temp)
                            break;
                        res++;
                    }
                    if (res >= 5)
                        return true;
                    break;
                case Direction.TRANSVERSE:
                    for (int k = 1; ; k++)
                    {
                        if (this[i, j + k] != temp)
                            break;
                        res++;
                    }
                    for (int k = 1; ; k++)
                    {
                        if (this[i, j - k] != temp)
                            break;
                        res++;
                    }
                    if (res >= 5)
                        return true;
                    break;
                case Direction.LEFT:
                    for (int k = 1; ; k++)
                    {
                        if (this[i + k, j + k] != temp)
                            break;
                        res++;
                    }
                    for (int k = 1; ; k++)
                    {
                        if (this[i - k, j - k] != temp)
                            break;
                        res++;
                    }
                    if (res >= 5)
                        return true;
                    break;
                case Direction.RIGHT:
                    for (int k = 1; ; k++)
                    {
                        if (this[i - k, j + k] != temp)
                            break;
                        res++;
                    }
                    for (int k = 1; ; k++)
                    {
                        if (this[i + k, j - k] != temp)
                            break;
                        res++;
                    }
                    if (res >= 5)
                        return true;
                    break;
            }

        } 
        
        return false;
    }

    public void reset()
    {
        for (int i = 0; i < this.width; i++)
            for (int j = 0; j < this.length; j++)
                board[i, j] = Cell.Empty;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
