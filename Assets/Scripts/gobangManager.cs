using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
public enum GameTurn
{
    Person,//人
    AI,
}

public enum GameState
{
    Ready,
    GameOver,
}

public class gobangManager : MonoBehaviour
{
    public gobangBoard board { get; set; }//棋盘
    public GameTurn turn { get; set; }//轮到谁下
    public int pieceNum { get; set; }//棋子数

    public Vector3 zero;
    public float width;
    private int row;
    private int col;
    public int depth;
    private gobangAI AI { set; get; }
    private GameObject chessParent;
    public GameObject blackPiece;
    public GameObject whitePiece;
    public GameState state = GameState.Ready;
    public GameObject win;
    public GameObject defeat;
    public gobangUI UI;
    public AudioSource error;
    public AudioSource piece;
    private List<Tuple<int,GameObject,Tuple<int,int>>> gamelist;
    public void getStart(int w,int l,GameTurn first)//构造函数
    {
        this.pieceNum = 0;
        this.board = new gobangBoard(w, l);//构造棋盘
        this.turn = first;
        this.chessParent = new GameObject();
        this.gamelist = new List<Tuple<int, GameObject, Tuple<int, int>>>();
    }

    public bool playChess()
    {
        if (this.turn == GameTurn.Person)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //转化成世界坐标系
                Vector3 offsetPos = pos - zero;
                //先计算落子位置
                row = Mathf.RoundToInt(offsetPos.y / width);
                col = Mathf.RoundToInt(offsetPos.x / width);
                if (!judgeExist(row, col))
                {
                    this.playErrorAudio();
                    return false;
                }
                //此处生成棋子
                Vector3 piecePos = new Vector3(col * width, row * width, zero.z) + zero;
                //生成棋子
                GameObject newPiece;
                if (blackPiece != null)
                {
                    newPiece = Instantiate(blackPiece, chessParent.transform);
                    newPiece.transform.position = piecePos;
                    this.playPieceAudio();
                    this.gamelist.Add(new Tuple<int, GameObject, Tuple<int, int>>
                        (pieceNum, newPiece, new Tuple<int, int>(row, col)));
                    this.pieceNum++;
                    this.turn = GameTurn.AI;//换边
                    this.board[row, col] = Cell.Black;
                    if (this.board.isGameOver(pieceColor.BLACK, row, col))
                        return true;
                }
            }
        }
        else if (turn == GameTurn.AI)
        {
            AI.alphaBetaSearch(board, turn, this.depth);//先搜两层看看
            this.row = AI.next.Item1;
            this.col = AI.next.Item2;
            Vector3 piecePos = new Vector3(col * width, row * width, zero.z) + zero;
            GameObject newPiece;
            if (whitePiece != null)
            {
                newPiece = Instantiate(whitePiece,chessParent.transform);
                newPiece.transform.position = piecePos;
                this.playPieceAudio();
                this.gamelist.Add(new Tuple<int, GameObject, Tuple<int, int>>
                        (pieceNum, newPiece, new Tuple<int, int>(row, col)));
                this.pieceNum++;
                this.turn = GameTurn.Person;//换边
                this.board[row, col] = Cell.White;
                if (this.board.isGameOver(pieceColor.WHITE, row, col))
                    return true;
            }
        }
        return false;
    }

    public bool judgeExist(int x,int y)
    {
        return board[x, y] == Cell.Empty;
        
    }
    
    public void restart()
    {
        if (chessParent != null)
            Destroy(this.chessParent);
        this.AI.reset();
        this.board.reset();
        this.turn = GameTurn.Person;
        this.pieceNum = 0;
        this.state = GameState.Ready;
        this.win.SetActive(false);
        this.defeat.SetActive(false);
        this.chessParent = new GameObject();
    }

    public bool repentance()
    {
        if (this.gamelist.Count() == 0)
            return false;
        var item1 = this.gamelist.Last();
        this.gamelist.Remove(item1);
        this.board[item1.Item3.Item1, item1.Item3.Item2] = Cell.Empty;//置为空
        if (item1.Item2 != null)
            Destroy(item1.Item2);

        var item2 = this.gamelist.Last();
        this.gamelist.Remove(item2);
        this.board[item2.Item3.Item1, item2.Item3.Item2] = Cell.Empty;//置为空
        if (item2.Item2 != null)
            Destroy(item2.Item2);
        this.pieceNum -= 2;
        return true;
        
    }

    public void playErrorAudio()
    {
        if (this.error == null)
            return;
        this.error.Play();
    }

    public void playPieceAudio()
    {
        if (this.piece == null)
            return;
        this.piece.Play();
    }
    // Start is called before the first frame update
    void Start()
    {
        getStart(15, 15, GameTurn.Person);//此处留下接口
        AI = new gobangAI(board, this.depth);//初始化
    }

    // Update is called once per frame
    void Update()
    {
        if (UI.restartEnabled)
        {
            this.restart();
            this.UI.restartEnabled = false;
            return;
        }
        //允许游戏结束后还做重新开始操作
        if (this.state != GameState.Ready)
            return;
        if (UI.repentanceEnabled)
        {
            if (!this.repentance())
                this.error.Play();
            this.UI.repentanceEnabled = false;
            return;
        }
        if (playChess())
        {
            if (turn == GameTurn.AI)//注意此处完成了换边
                this.win.SetActive(true);
            else if (turn == GameTurn.Person)
                this.defeat.SetActive(true);
            this.state = GameState.GameOver;
            Debug.Log("结束");//暂留
        }
    }
}
