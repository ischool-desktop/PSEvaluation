using System;
using System.Collections.Generic;
using System.Text;
using DevComponents.DotNetBar;
using System.Windows.Forms.Layout;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace HsinChu.TransferReport
{
    /// <summary>
    /// 以卡片形式Layout子控制項的Form
    /// </summary>
    public class CardPanelEx : PanelEx
    {
        private CardLayoutEngine _layoutEngine = new CardLayoutEngine();
        /// <summary>
        /// 建構子
        /// </summary>
        public CardPanelEx()
        {
            //this.DoubleBuffered = true;
            this.Click += new EventHandler(CardPanelEx_Click);
        }

        private void CardPanelEx_Click(object sender, EventArgs e)
        {
            this.Focus();
        }
        /// <summary>
        /// 每個子控制項的寬度
        /// </summary>
        public int CardWidth
        {
            get { return _layoutEngine.CardWidth; }
            set { _layoutEngine.CardWidth = value; }
        }
        /// <summary>
        /// Layout的最小寬度
        /// </summary>
        public int MinWidth
        {
            get { return _layoutEngine.MinWidth; }
            set { _layoutEngine.MinWidth = value; }
        }
        /// <summary>
        /// LayoutEngine
        /// </summary>
        public override LayoutEngine LayoutEngine
        {
            get
            {
                return _layoutEngine;
            }
        }

        private class CardLayoutEngine : LayoutEngine
        {
            private int _CardWidth;

            private int _MinWidth = 2;

            public int CardWidth
            {
                get { return _CardWidth; }
                set { _CardWidth = value; }
            }

            public int MinWidth
            {
                get { return _MinWidth; }
                set { _MinWidth = value; }
            }

            public CardLayoutEngine()
            {
            }

            private List<Control> _SuspendLayoutList = new List<Control>();

            public override bool Layout(
                object container,
                LayoutEventArgs layoutEventArgs)
            {

                if (!((CardPanelEx)container).Visible) return false;

                CardPanelEx parent = container as CardPanelEx;
                // Use DisplayRectangle so that parent.Padding is honored.
                Rectangle parentDisplayRectangle = parent.DisplayRectangle;
                Point nextControlLocation = parentDisplayRectangle.Location;
                Dictionary<Control, Point> controlLocation = new Dictionary<Control, Point>();
                int Columns, ColumnWidth, i;
                //計算可容量的行數
                Columns = _CardWidth == 0 ? 1 : (parentDisplayRectangle.Width - _MinWidth) / (_CardWidth + _MinWidth);
                if (Columns == 0)
                {
                    Columns = 1;
                    ColumnWidth = 0;
                }
                else
                {
                    //計算每行行距
                    ColumnWidth = (parentDisplayRectangle.Width - Columns * _CardWidth - 5) / (Columns + 1);
                }
                //每行各自獨立Layout
                int[] nextColumnY = new int[Columns];
                for (i = 0; i < Columns; i++)
                {
                    nextColumnY[i] = parentDisplayRectangle.Y + _MinWidth;
                }
                //移至第一行
                nextControlLocation.Offset(ColumnWidth, _MinWidth);

                i = 0;
                //開始排列每一個包含的控制項
                foreach (Control c in parent.Controls)
                {
                    //控制項不顯示當沒看到
                    if (!c.Visible)
                    {
                        continue;
                    }
                    //控制項位置需要變更
                    if (c.Location.X != nextControlLocation.X || c.Location.Y != nextControlLocation.Y)
                    {
                        //如果顯示區域步購大則加大顯示區域
                        if (parentDisplayRectangle.Height < nextControlLocation.Y + c.Height + _MinWidth)
                        {
                            parentDisplayRectangle.Inflate(0, c.Height + _MinWidth);
                        }
                        //設定控制項位置
                        controlLocation.Add(c, nextControlLocation);
                        nextControlLocation = new Point(nextControlLocation.X, nextControlLocation.Y);
                    }
                    //將第i行的高度往下增加並將i推進至下一行
                    nextColumnY[i++] += c.Height + _MinWidth;

                    if (i == Columns)
                    {
                        i = 0;
                        nextControlLocation.X = parentDisplayRectangle.Location.X + ColumnWidth;
                    }
                    else
                    {
                        nextControlLocation.X += (ColumnWidth + _CardWidth);
                    }
                    nextControlLocation.Y = nextColumnY[i];
                }
                //真的把位置設定上去
                foreach (Control c in controlLocation.Keys)
                {
                    c.Location = controlLocation[c];
                }
                return false;
            }
        }

    }
}