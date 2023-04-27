using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCaro
{
    public partial class Form1 : Form
    {
        // Vùng thuộc tính
        #region Properties
        ChessBoardManager ChessBoard;
        #endregion
        // Kết thúc vùng thuộc tính

        // Vùng khởi tạo
        #region Initialize             

        public Form1()
        {
            InitializeComponent();

            ChessBoard = new ChessBoardManager(pnlChessBoard, txbPlayerName, pctbMark);
            ChessBoard.EndedGame += ChessBoard_EndedGame;
            ChessBoard.PlayerMarked +=ChessBoard_PlayerMarked;

            prcbCoolDown.Step = Const.COOL_DOWN_STEP;
            prcbCoolDown.Maximum = Const.COOL_DOWN_TIME;
            prcbCoolDown.Value = 0;

            tmCoolDown.Interval = Const.COOL_DOWN_INTERVAL;
            NewGame();
        }

        #endregion
        // Kết thúc vùng thuộc tính

        // Vùng phương thức
        #region Methods
        void EndGame()
        {
            tmCoolDown.Stop();
            pnlChessBoard.Enabled = false;
            undoToolStripMenuItem.Enabled = false;
            if (MessageBox.Show("Bạn có muốn tiếp tục ván mới không ?", "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                NewGame();          
        }

        void NewGame()
        {
            prcbCoolDown.Value = 0;
            tmCoolDown.Stop();
            undoToolStripMenuItem.Enabled = true;
            ChessBoard.DrawChessBoard();      
        }

        void Quit()
        {
            Application.Exit();
        }
        void Undo()
        {
            prcbCoolDown.Value = 0;
            ChessBoard.Undo();
        }
         
        private void ChessBoard_PlayerMarked(object sender, EventArgs e)
        {
            tmCoolDown.Start();
            prcbCoolDown.Value = 0;
        }

        private void ChessBoard_EndedGame(object sender, EventArgs e)
        {
            EndGame();
        }

        private void tmCoolDown_Tick(object sender, EventArgs e) // Event của Timer CoolDown
        {
            prcbCoolDown.PerformStep();

            if (prcbCoolDown.Value >= prcbCoolDown.Maximum)
            {             
                EndGame();              
            }
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Quit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn thoát không ?", "Thông báo", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                e.Cancel = true;
        }
        #endregion
        // Kết thúc vùng phương thức
    }
}
