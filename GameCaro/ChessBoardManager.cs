using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCaro
{
    public class ChessBoardManager
    {
        // Vùng thuộc tính
        #region Properties  
        private Panel chessBoard;

        public Panel ChessBoard
        {
            get { return chessBoard; }
            set { chessBoard = value; }
        }

        private List<Player> player;

        public List<Player> Player
        {
            get { return player; }
            set { player = value; }
        }

        private int currentPlayer;  

        public int CurrentPlayer
        {
            get { return currentPlayer; }
            set { currentPlayer = value; }
        }

        private TextBox playerName;

        public TextBox PlayerName
        {
            get { return playerName; }
            set { playerName = value; }
        }

        private PictureBox playerMark;

        public PictureBox PlayerMark
        {
            get { return playerMark; }
            set { playerMark = value; }
        }

        private List<List<Button>> matrix;

        public List<List<Button>> Matrix
        {
            get { return matrix; }
            set { matrix = value; }
        }

        private event EventHandler playerMarked;
        public event EventHandler PlayerMarked
        {
            add
            {
                playerMarked += value ;
            }
            remove 
            {
                playerMarked -= value;
            }   
        }

        private event EventHandler endedGame;
        public event EventHandler EndedGame
        {
            add
            {
                endedGame += value;
            }
            remove
            {
                endedGame -= value;
            }
        }

        private Stack<PlayInfo> playTimeLine;

        public Stack<PlayInfo> PlayTimeLine
        {
            get { return playTimeLine; }
            set { playTimeLine = value; }
        }
        #endregion
        // Kết thúc vùng thuộc tính

        // Vùng khởi tạo
        #region Initialize 
        public ChessBoardManager(Panel chessBoard, TextBox playerName, PictureBox mark)
        {
            this.ChessBoard = chessBoard;
            this.PlayerName = playerName;
            this.PlayerMark = mark;
            this.Player = new List<Player>() 
            { 
                new Player ("X", Image.FromFile(Application.StartupPath + "\\Resources\\x.png")),
                new Player ("O", Image.FromFile(Application.StartupPath + "\\Resources\\o.png")),
            };
            
            
        }
        #endregion
        // Kết thúc vùng khởi tạo

        // Vùng phương thức
        #region Methods 

        public void DrawChessBoard() // Hàm vẽ bàn cờ
        {
            ChessBoard.Enabled = true;
            ChessBoard.Controls.Clear();
            PlayTimeLine = new Stack<PlayInfo>();
            CurrentPlayer = 0;
            ChangePlayer();
            Matrix = new List<List<Button>>();
            Button oldButton = new Button() { Width = 0, Location = new Point(0, 0) };
            for (int i = 0; i < Const.CHESS_BOARD_HEIGHT; i++)
            {
                Matrix.Add(new List<Button>());
                for (int j = 0; j < Const.CHESS_BOARD_WIDTH+1; j++)
                {
                    Button btn = new Button()
                    {
                        Width = Const.CHESS_WIDTH,
                        Height = Const.CHESS_HEIGHT,
                        Location = new Point(oldButton.Location.X + oldButton.Width, oldButton.Location.Y),
                        BackgroundImageLayout = ImageLayout.Stretch,
                        Tag = i.ToString()
                    };
                    btn.Click += btn_Click;
                    ChessBoard.Controls.Add(btn);
                    Matrix[i].Add(btn);
                    oldButton = btn;
                }
                oldButton.Location = new Point(0, oldButton.Location.Y + Const.CHESS_HEIGHT);
                oldButton.Width = 0;
                oldButton.Height = 0;
            }
        }

        void btn_Click(object sender, EventArgs e) // Hàm xử lý khi click
        {
            Button btn = sender as Button;
            if (btn.BackgroundImage != null)
                return;
            Mark(btn);
            PlayTimeLine.Push(new PlayInfo(GetChessPoint(btn), CurrentPlayer));
            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1; 
            ChangePlayer();
               
            if (playerMarked != null)
                playerMarked(this, new EventArgs());
            if (isEndGame(btn))
            {
                CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
                MessageBox.Show(Player[CurrentPlayer].Name + " đã chiến thắng !");
                EndGame();
            }
        }
        public void EndGame() // Hàm xử lý kết thúc game
        {
            if (endedGame != null)
                endedGame(this,new EventArgs());
        }

        public bool Undo()
        {
            if (PlayTimeLine.Count <= 0)
                return false;

            PlayInfo oldPoint = PlayTimeLine.Pop();
            Button btn = Matrix[oldPoint.Point.Y][oldPoint.Point.X];

            btn.BackgroundImage = null;
            if(PlayTimeLine.Count <=0)
            {
                CurrentPlayer = 0;
            }
            else
            {
                oldPoint = PlayTimeLine.Peek();
                CurrentPlayer = oldPoint.CurrentPlayer == 1 ? 0 : 1;
            }
            
            ChangePlayer();
            return true;
        }

        // Hàm kiểm tra xem kết thúc game
        private bool isEndGame (Button btn)
        {
            return isEndHorizontal(btn)|| isEndVertical(btn) || isEndPrimary(btn) ||  isEndSub(btn);
        }
        private Point GetChessPoint(Button btn)
        {
            int vertical = Convert.ToInt32(btn.Tag);
            int horizontal = Matrix[vertical].IndexOf(btn);

            Point point = new Point(horizontal, vertical);

            return point;
        }
        private bool isEndHorizontal (Button btn)  // Kết thúc hàng ngang
        {
            Point point = GetChessPoint(btn);

            int countLeft = 0;
            for (int i = point.X; i >= 0; i--)
            {
                if (Matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    countLeft++;
                }
                else
                    break;
            }
            int countRight = 0;
            for (int i = point.X+1; i < Const.CHESS_BOARD_WIDTH; i++)
            {
                if (Matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    countRight++;
                }
                else
                    break;
            }

            return countLeft + countRight == 5; // Nếu như đủ 5 con trên 1 hàng thì trả về true => kết thúc theo hàng ngang
        }
        private bool isEndVertical(Button btn) // Kết thúc hàng dọc
        {
            Point point = GetChessPoint(btn);

            int countTop = 0;
            for (int i = point.Y; i >= 0; i--)
            {
                if (Matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                    break;
            }
            int countBottom = 0;
            for (int i = point.Y + 1; i < Const.CHESS_BOARD_HEIGHT; i++)
            {
                if (Matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else
                    break;
            }

            return countTop + countBottom == 5; // Nếu như đủ 5 con trên 1 cột thì trả về true => kết thúc theo hàng dọc
        }
        private bool isEndPrimary(Button btn) // Kết thúc đường chéo chính
        {
            Point point = GetChessPoint(btn);

            int countTop = 0;
            for (int i = 0; i <= point.X; i++)
            {
                if (point.X - i < 0 || point.Y - i < 0)
                    break;

                if (Matrix[point.Y - i][point.X -i].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                    break;
            }
            int countBottom = 0;
            for (int i = 1; i <= Const.CHESS_BOARD_WIDTH - point.X; i++)
            {
                if (point.Y + i >= Const.CHESS_BOARD_HEIGHT || point.X + i >= Const.CHESS_BOARD_WIDTH)
                    break;
                if (Matrix[point.Y + i][point.X + i].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else
                    break;
            }

            return countTop + countBottom == 5;
        }
        private bool isEndSub(Button btn) // Kết thúc đường chéo phụ
        {
            Point point = GetChessPoint(btn);

            int countTop = 0;
            for (int i = 0; i <= point.X; i++)
            {
                if (point.X + i > Const.CHESS_BOARD_WIDTH || point.Y - i < 0)
                    break;

                if (Matrix[point.Y - i][point.X + i].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                    break;
            }
            int countBottom = 0;
            for (int i = 1; i <= Const.CHESS_BOARD_WIDTH - point.X; i++)
            {
                if (point.Y + i >= Const.CHESS_BOARD_HEIGHT || point.X - i < 0)
                    break;
                if (Matrix[point.Y + i][point.X - i].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else
                    break;
            }

            return countTop + countBottom == 5;
        }


        private void Mark(Button btn) 
        {
            btn.BackgroundImage = Player[CurrentPlayer].Mark; 
        }
        private void ChangePlayer() // Hàm thay đổi người chơi
        {
            PlayerName.Text = Player[CurrentPlayer].Name;
            PlayerMark.Image = Player[CurrentPlayer].Mark;
        }
        #endregion
        // Kết thúc vùng phương thức
    }      
}
