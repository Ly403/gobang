using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum CHESSTYPE
{
    NONE = 0,
    SLEEP_TWO = 1,
    LIVE_TWO = 2,
    SLEEP_THREE = 3,
    LIVE_THREE = 4,
    CHONG_FOUR = 5,
    LIVE_FOUR = 6,
    LIVE_FIVE = 7,
    OPPO_CHONGFOUR_AND_LIVETHREE = 8,
    MY_LIVETHREE_AND_OPPO_NOFOUR = 9,
    OPPO_LIVETHREE_AND_MY_NOTHREE = 10,
    THREE_GR_1 = 11,
    HAVE = 12,
};



public class gobangAI : MonoBehaviour
{
    public int[,,] record;//��¼�ĸ�������û�б�������
    public int[,] count;//��¼�������͵���Ŀ
    public int[,] pos_score;//����
    public int len;
    public int allDepth;
    public Tuple<int, int> next;
    //����Ҫ�õĳ���
    public readonly int NUM = 8;
    //���ֱ�
    public readonly Dictionary<CHESSTYPE, Tuple<int, int>> dicScore
        = new Dictionary<CHESSTYPE, Tuple<int, int>>
    {
            //��һ��Ϊ�������֣��ڶ���Ϊ�Է�����
        { CHESSTYPE.LIVE_FIVE,new Tuple<int, int>(10000,10000)},
        { CHESSTYPE.LIVE_FOUR,new Tuple<int, int>(9050,9030) },
        { CHESSTYPE.CHONG_FOUR,new Tuple<int, int>(9040,400)},
        { CHESSTYPE.LIVE_THREE,new Tuple<int, int>(100,400) },
        { CHESSTYPE.SLEEP_THREE,new Tuple<int, int>(10,10)},
        { CHESSTYPE.LIVE_TWO,new Tuple<int, int>(6,6) },
        { CHESSTYPE.SLEEP_TWO,new Tuple<int, int>(2,2) },
        { CHESSTYPE.OPPO_CHONGFOUR_AND_LIVETHREE,new Tuple<int, int>(0,9020) },
        { CHESSTYPE.MY_LIVETHREE_AND_OPPO_NOFOUR,new Tuple<int, int>(9010,0) },
        { CHESSTYPE.OPPO_LIVETHREE_AND_MY_NOTHREE,new Tuple<int, int>(0,9000) },
        { CHESSTYPE.THREE_GR_1,new Tuple<int, int>(500,2000) },
    };
    //public readonly int DEPTH = 4;
    public gobangAI(gobangBoard board,int allDepth)
    {
        this.allDepth = allDepth;
        this.len = board.length;
        next = null;
        record = new int[len, len, 4];
        count = new int[2, NUM];
        pos_score = new int[len, len];
        for (int i = 0; i < len; i++)
        {
            for (int j = 0; j < len; j++)
            {
                this.record[i, j, 0] = 0;
                this.record[i, j, 1] = 0;
                this.record[i, j, 2] = 0;
                this.record[i, j, 3] = 0;
                //��ֻ��һ����ʼ״̬
                this.pos_score[i, j] = 7 - Math.Max(Math.Abs(i - 7), Math.Abs(j - 7));
                //��ʼ��״̬��Ҫ��Խ�����ķ�Խ��
            }
        }
        //��ʼ��
        for (int i = 0; i < NUM; ++i)
        {
            this.count[0, i] = 0;
            this.count[1, i] = 0;
        }
    }
    public void reset()
    {
        for (int i = 0; i < len; i++)
        {
            for (int j = 0; j < len; j++)
            {
                //ȫ����ʼ��
                this.record[i, j, 0] = 0;
                this.record[i, j, 1] = 0;
                this.record[i, j, 2] = 0;
                this.record[i, j, 3] = 0;
            }
        }

        for (int i = 0; i < NUM; ++i)
        {
            this.count[0, i] = 0;
            this.count[1, i] = 0;
        }
    }


    public bool hasNear(gobangBoard board, int i, int j, int radius)
    {
        int m ;
        int n ;
        for (m = i - radius; m <= i + radius; m++)
        {
            for (n = j - radius; n <= j + radius; n++)
            {
                if (board[m, n] == Cell.Void)
                    continue;
                if (board[m, n] != Cell.Empty)
                    return true;
            }
        }
        return false;
    }
    /// <summary>
    /// �ҿյ�ĺ���
    /// </summary>
    /// <param name="board"></param>
    /// <param name="turn"></param>
    /// <returns></returns>
    public List<Tuple<int, int, int>> genmove(gobangBoard board)
    {
        List<Tuple<int, int, int>> l = new List<Tuple<int, int, int>>();
        for (int i = 0; i < len; i++)
        {
            for (int j = 0; j < len; j++)
            {
                if (board[i, j] == Cell.Empty && this.hasNear(board, i, j, 1))
                {
                    int score = this.pos_score[i, j];
                    Tuple<int, int, int> temp = new Tuple<int, int, int>(score, i, j);
                    l.Add(temp);
                }
            }
        }

        l.Sort((Tuple<int, int, int> a, Tuple<int, int, int> b) =>
        {
            if (a.Item1 > b.Item1)
                return 1;
            else if (a.Item1 == b.Item1)
                return 0;
            else
                return -1;
        });//����������
        return l;
    }

    public List<Tuple<int, int, int>> search(gobangBoard board, GameTurn turn)
    {
        List<Tuple<int, int, int>> l = new List<Tuple<int, int, int>>();
        List<Tuple<int, int, int>> temp = this.genmove(board);
        int max = int.MinValue;
        foreach (var item in temp)
        {
            //�涨����AIִ����,����������ȥ
            if (turn == GameTurn.AI)
                board[item.Item2, item.Item3] = Cell.White;
            else
                board[item.Item2, item.Item3] = Cell.Black;
            int score = this.evaluate(board, turn);
            board[item.Item2, item.Item3] = Cell.Empty;
            if (score > max)
            {
                max = score;
                l.Add(new Tuple<int, int, int>(max, item.Item2, item.Item3));
            }
        }
        return l;
    }

    public int alphaBetaSearch(gobangBoard board, GameTurn turn, int depth = 4, int alpha = int.MinValue + 1, int beta = int.MaxValue)
    {
        int score = evaluate(board, turn);
        if (depth <= 0 || score >= this.dicScore[CHESSTYPE.LIVE_FIVE].Item1)
            return score;

        List<Tuple<int, int, int>> space = genmove(board);
        if (!space.Any())
            return score;
        Tuple<int, int> move = null;

        foreach (var item in space)
        {
            GameTurn temp;
            //����������ȥ
            if (turn == GameTurn.AI)
            {
                board[item.Item2, item.Item3] = Cell.White;
                temp = GameTurn.Person;
            }
            else
            {
                board[item.Item2, item.Item3] = Cell.Black;
                temp = GameTurn.AI;//����
            }
            score = -alphaBetaSearch(board, temp, depth - 1, -beta, -alpha);
            board[item.Item2, item.Item3] = Cell.Empty;

            //alpha-beta��֦
            if (score < beta)
            {
                beta = score;
                move = new Tuple<int, int>(item.Item2, item.Item3);
                if (alpha >= beta)
                    break;
            }
            if (depth == this.allDepth && move != null)
                this.next = move;
        }
        return beta;
    }

    /// <summary>
    /// ��ΪҪalpha-beta��֦��������������ִβ��̶�ΪAI��
    /// �ִ�Ҫ�ò����޶��� GameTurn turn
    /// </summary>
    /// <param name="board"></param>
    /// <param name="turn"></param>
    /// <returns></returns>
    int evaluate(gobangBoard board, GameTurn turn)
    {
        this.reset();
        Cell myValue, oppoValue;
        if (turn == GameTurn.AI)
        {
            myValue = Cell.White;
            oppoValue = Cell.Black;
        }
        else
        {
            myValue = Cell.Black;
            oppoValue = Cell.White;
        }

        ///�����Ǳ����������̣������ֵ
        for (int i = 0; i < this.len; i++)
        {
            for (int j = 0; j < this.len; j++)
            {
                if (board[i, j] == myValue)// || board[i, j] == oppoValue)
                    evaluatePoint(board, i, j, myValue, oppoValue);
                //�˴�myValue������myValue��ɫ������Ϊ����
                else if (board[i, j] == oppoValue)
                    evaluatePoint(board, i, j, oppoValue, myValue);
            }
        }
        //oppoValue����һ��Ҫ�µ�����
        Tuple<int, int> s = this.getScore(myValue, oppoValue);
        //�˴�myValue����myValue�����Դ���Ϊ��һ����myValue
        return s.Item2 - s.Item1;//����÷�-����÷�
    }

    /// <summary>
    /// �ҳ�һ���������ľŸ��㣬Ϊʲô��9����
    /// ���ǵ����ġ��������������߶������
    /// ����������ͱ�������Ҫһ�����ϾŸ�������ж�׼ȷ
    /// </summary>
    /// <param name="board"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="d"></param>
    /// <param name="delta_this"></param>
    /// <param name="delat_that"></param>
    /// <returns></returns>
    public Cell[] getLine(gobangBoard board, int i, int j, int xx, int yy,Cell oppoValue)
    {

        Cell[] res = new Cell[9];
        int tx = i - 5 * xx;
        int ty = j - 5 * yy;
        for (int k = 0; k < 9; k++)
        {
            tx += xx;
            ty += yy;
            res[k] = board[tx, ty];
            res[k] = res[k] == Cell.Void ? oppoValue : res[k];
        }
        return res;
    }

    public CHESSTYPE calculateLine(gobangBoard board, int i, int j, Direction d, Cell myValue, Cell oppoValue)
    {
        int choice = ((int)myValue);

        int xx = 0, yy = 0;
        switch (d)
        {
            case Direction.VERTICAL:
                xx = 1;
                yy = 0;
                break;
            case Direction.TRANSVERSE:
                xx = 0;
                yy = 1;
                break;
            case Direction.LEFT:
                xx = 1;
                yy = 1;
                break;
            case Direction.RIGHT:
                xx = 1;
                yy = -1;
                break;
        }
        Cell[] line = getLine(board, i, j, xx, yy, oppoValue);
        int _left, _right;
        //4���λ����ɫ��ȷ���ģ����ÿ���
        for (_left = 4; _left > 0;_left--)
            if (line[_left - 1] != myValue)
                break;
        
        for (_right = 4; _right < 8;_right++)
            if (line[_right + 1] != myValue)
                break;
        

        int __left, __right;
        for (__left = _left; __left > 0; __left--)
            if (line[__left - 1] == oppoValue)
                break;

        for (__right = _right; __right < 8; __right++)
            if (line[__right + 1] == oppoValue)
                break;

        if (__right - __left + 1 < 5)
        {
            this.Record(board, i, j, xx, yy, __left, __right, d);
            return CHESSTYPE.NONE;
        }

        this.Record(board, i, j, xx, yy, _left, _right, d);

        int skip = _right - _left + 1;
        //����������
        if (skip == 5)
            count[choice, (int)(CHESSTYPE.LIVE_FIVE)] += 1;


        ////����,����
        if (skip == 4)
        {
            bool Eleft = false, Eright = false;
            if (line[_left - 1] == Cell.Empty)
                Eleft = true;
            if (line[_right + 1] == Cell.Empty)
                Eright = true;
            if (Eleft && Eright)//����
                count[choice, (int)CHESSTYPE.LIVE_FOUR]++;
            if (Eleft || Eright)
                count[choice, (int)CHESSTYPE.CHONG_FOUR]++;
        }

        //����������������
        //�ڿպںں�  �������ģ���Գ���Ҳ�������ģ�ע����������3�����жϳ�����
        if (skip == 3)
        {
            bool Eleft = false, Eright = false;
            bool four = true;
            if (line[_left - 1] == Cell.Empty)
            {
                Eleft = true;
                if (line[_left - 2] == myValue)//�˴�Ϊ������
                {
                    four = true;
                    count[choice, (int)CHESSTYPE.CHONG_FOUR]++;
                    this.Record(board, i, j, xx, yy, _left - 2, _left - 1, d);
                }
            }
            if (line[_right + 1] == Cell.Empty)
            {
                if (line[_right + 2] == myValue)//�˴�Ϊ������
                {
                    four = true;
                    count[choice, (int)CHESSTYPE.CHONG_FOUR]++;
                    this.Record(board, i, j, xx, yy, __right + 1, _right + 2, d);
                }
                Eright = true;
            }
            if (!four)
            {
                if (Eleft && Eright)
                {
                    if (__right - __left + 1 > 5)
                        count[choice, ((int)CHESSTYPE.LIVE_THREE)]++;
                    else
                        count[choice, ((int)CHESSTYPE.SLEEP_THREE)]++;
                }
                else if (Eleft || Eright)
                    count[choice, ((int)CHESSTYPE.SLEEP_THREE)]++;
            }
        }


        //���ģ�������������߶�������
        if (skip == 2)
        {
            bool Eleft = false, Eright = false;
            bool three = true;
            if (line[_left - 1] == Cell.Empty)
            {
                Eleft = true;
                if (line[_left - 2] == myValue)
                {
                    this.Record(board, i, j, xx, yy, _left - 2, _left - 1, d);
                    if (line[_left - 3] == Cell.Empty)
                    {
                        three = true;
                        if (line[_right + 1] == Cell.Empty)
                            count[choice, ((int)CHESSTYPE.LIVE_THREE)]++;
                        else
                            count[choice, ((int)CHESSTYPE.SLEEP_THREE)]++;
                    }
                    else if (line[_left - 3] == oppoValue)
                    {
                        if (line[_right + 1] == Cell.Empty)
                        {
                            count[choice, ((int)CHESSTYPE.SLEEP_THREE)]++;
                            three = true;
                        }
                    }
                }
            }
            if (line[_right + 1] == Cell.Empty)
            {
                Eright = true;
                if (line[_right + 2] == myValue)
                {
                    if (line[_right + 3] == myValue)
                    {
                        three = true;
                        this.Record(board, i, j, xx, yy, __right + 1, _right + 2, d);
                        count[choice, ((int)CHESSTYPE.CHONG_FOUR)]++;
                    }
                    else if (line[_right + 3] == Cell.Empty)
                    {
                        three = true;
                        if (line[_left - 1] == Cell.Empty)
                            count[choice, ((int)CHESSTYPE.LIVE_THREE)]++;
                        else
                            count[choice, ((int)CHESSTYPE.SLEEP_THREE)]++;
                    }
                    else if (line[_left - 1] == Cell.Empty)
                    {
                        count[choice, ((int)CHESSTYPE.SLEEP_THREE)]++;
                        three = true;
                    }
                }
                if (!three)
                {
                    if (Eleft && Eright)
                        count[choice, ((int)CHESSTYPE.LIVE_TWO)]++;
                    else if (Eleft || Eright)
                        count[choice, ((int)CHESSTYPE.SLEEP_TWO)]++;
                }
            }
        }

        //������߶�
        if (skip == 1)
        {
            if (line[_left - 1] == Cell.Empty)
            {
                if (line[_left - 2] == myValue)
                    if (line[_left - 3] == Cell.Empty)
                        if (line[_right + 1] == oppoValue)
                            count[choice, ((int)CHESSTYPE.SLEEP_TWO)]++;

            }
            if (line[_right + 1] == Cell.Empty)
            {
                if (line[_right + 2] == myValue)
                {
                    if (line[_right + 3] == Cell.Empty)
                    {
                        if (line[_left - 1] == Cell.Empty)
                            count[choice, ((int)CHESSTYPE.LIVE_TWO)]++;
                        else
                            count[choice, ((int)CHESSTYPE.SLEEP_TWO)]++;
                    }
                }
                else if (line[_right + 2] == Cell.Empty)
                    if (line[_right + 3] == myValue && line[_right + 3] == Cell.Empty)
                        count[choice, ((int)CHESSTYPE.LIVE_TWO)]++;
            }
        }
        return CHESSTYPE.HAVE;
    }

    public void Record(gobangBoard board, int i, int j, int xx, int yy, int left, int right, Direction d)
    {
        int x = i + ((-5 + left) * xx);
        int y = j + ((-5 + right) * yy);
        for (int k = left; k <= right; k++)
        {
            x += xx;
            y += yy;
            if (x >= 0 && x < this.len && y >= 0 && y < this.len)
                this.record[x, y, (int)d] = 1;
        }
    }

    void evaluatePoint(gobangBoard board, int i, int j, Cell myValue, Cell oppoValue)
    {
        Direction[] d = new Direction[4]
        { Direction.VERTICAL, Direction.TRANSVERSE, Direction.LEFT, Direction.RIGHT };
        for (int k = 0; k < 4; k++)
        {
            if (this.record[i, j, k] == 0)
                this.calculateLine(board, i, j, d[k], myValue, oppoValue);
        }
    }

    //�˴���ΪmyValue����һ��Ҫ�µ��壬����myValue��Ӧֵ�ᱻ���ָ�һ��
    public Tuple<int, int> getScore(Cell myValue, Cell oppoValue)
    {
        int myScore = 0, oppoScore = 0;

        //������д��ɱ�·�
        //������
        if (this.count[((int)myValue), ((int)CHESSTYPE.LIVE_FIVE)] > 0)
            return new Tuple<int, int>(this.dicScore[CHESSTYPE.LIVE_FIVE].Item1, 0);
        if (this.count[((int)oppoValue), ((int)CHESSTYPE.LIVE_FIVE)] > 0)
            return new Tuple<int, int>(0, this.dicScore[CHESSTYPE.LIVE_FIVE].Item2);

        //�������ġ����ģ���һ�ּ�����
        if (this.count[((int)myValue), ((int)CHESSTYPE.CHONG_FOUR)] >= 2)
            this.count[((int)myValue), ((int)CHESSTYPE.LIVE_FOUR)]++;
        if (this.count[((int)oppoValue), ((int)CHESSTYPE.CHONG_FOUR)] >= 2)
            this.count[((int)oppoValue), ((int)CHESSTYPE.LIVE_FOUR)]++;

        if (this.count[((int)myValue), ((int)CHESSTYPE.LIVE_FOUR)] > 0)
            return new Tuple<int, int>(this.dicScore[CHESSTYPE.LIVE_FOUR].Item1, 0);
        if (this.count[((int)myValue), ((int)CHESSTYPE.CHONG_FOUR)] > 0)
            return new Tuple<int, int>(this.dicScore[CHESSTYPE.CHONG_FOUR].Item1, 0);

        //�Է��л��ģ����г������л���������
        if (this.count[((int)oppoValue), ((int)CHESSTYPE.LIVE_FOUR)] > 0)
            return new Tuple<int, int>(0, this.dicScore[CHESSTYPE.LIVE_FOUR].Item2);
        if (this.count[((int)oppoValue), ((int)CHESSTYPE.CHONG_FOUR)] > 0 &&
            this.count[((int)oppoValue), ((int)CHESSTYPE.LIVE_THREE)] > 0)
            return new Tuple<int, int>(0, this.dicScore[CHESSTYPE.OPPO_CHONGFOUR_AND_LIVETHREE].Item2);


        //�����������Է��޳���
        if (this.count[((int)myValue), ((int)CHESSTYPE.LIVE_THREE)] > 0 &&
           this.count[((int)oppoValue), ((int)CHESSTYPE.CHONG_FOUR)] == 0)
            return new Tuple<int, int>(this.dicScore[CHESSTYPE.MY_LIVETHREE_AND_OPPO_NOFOUR].Item1, 0);

        //�Է�������ֹһ������������������
        if (this.count[((int)oppoValue), ((int)CHESSTYPE.LIVE_THREE)] > 1 &&
            this.count[((int)myValue), ((int)CHESSTYPE.LIVE_THREE)] == 0 &&
            this.count[((int)myValue), ((int)CHESSTYPE.SLEEP_THREE)] == 0)
            return new Tuple<int, int>(0, this.dicScore[CHESSTYPE.OPPO_LIVETHREE_AND_MY_NOTHREE].Item2);

        ///�����ǷǱ�ɱ��
        //�Է�����
        if (this.count[((int)oppoValue), ((int)CHESSTYPE.LIVE_FOUR)] > 0)
            oppoScore += this.dicScore[CHESSTYPE.LIVE_FOUR].Item2;

        //��������
        if (this.count[((int)myValue), ((int)CHESSTYPE.LIVE_THREE)] > 1)
            myScore += this.dicScore[CHESSTYPE.THREE_GR_1].Item1;
        else if (this.count[((int)myValue), ((int)CHESSTYPE.LIVE_THREE)] > 0)
            myScore += this.dicScore[CHESSTYPE.LIVE_THREE].Item1;

        //�Է�����
        if (this.count[((int)oppoValue), ((int)CHESSTYPE.LIVE_THREE)] > 1)
            oppoScore += this.dicScore[CHESSTYPE.THREE_GR_1].Item2;
        else if (this.count[((int)oppoValue), ((int)CHESSTYPE.LIVE_THREE)] > 0)
            oppoScore += this.dicScore[CHESSTYPE.LIVE_THREE].Item2;


        //���
        if (this.count[((int)myValue), ((int)CHESSTYPE.LIVE_TWO)] > 0)
            myScore += this.dicScore[CHESSTYPE.LIVE_TWO].Item1 * this.count[((int)myValue), ((int)CHESSTYPE.LIVE_TWO)];
        if (this.count[((int)oppoValue), ((int)CHESSTYPE.LIVE_TWO)] > 0)
            oppoScore += this.dicScore[CHESSTYPE.LIVE_TWO].Item2 * this.count[((int)oppoValue), ((int)CHESSTYPE.LIVE_TWO)];

        //�߶�
        if (this.count[((int)myValue), ((int)CHESSTYPE.SLEEP_TWO)] > 0)
            myScore += this.dicScore[CHESSTYPE.SLEEP_TWO].Item1 * this.count[((int)myValue), ((int)CHESSTYPE.SLEEP_TWO)];
        if (this.count[((int)oppoValue), ((int)CHESSTYPE.SLEEP_TWO)] > 0)
            oppoScore += this.dicScore[CHESSTYPE.SLEEP_TWO].Item2 * this.count[((int)oppoValue), ((int)CHESSTYPE.SLEEP_TWO)];
        return new Tuple<int, int>(myScore, oppoScore);
    }

}
