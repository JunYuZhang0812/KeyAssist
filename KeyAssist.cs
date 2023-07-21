using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Win32.Hooks;

namespace KeyAssist
{
    public partial class KeyAssist : Form
    {
        public class KeyData
        {
            public int m_keyHead;
            public List<int> m_keyList;

            public KeyData()
            {
                m_keyList = new List<int>();
            }
            public string KeyHeadStr
            {
                get
                {
                    return KeyCodeStr.Get(m_keyHead) ?? "";
                }
            }
            public string KeyListStr
            {
                get
                {
                    string str = "";
                    for (int i = 0; i < m_keyList.Count; i++)
                    {
                        var key = m_keyList[i];
                        str += KeyCodeStr.Get(key);
                        if (i < m_keyList.Count - 1)
                        {
                            str += "  ";
                        }
                    }
                    return str;
                }
            }
            public void Save()
            {
                if (m_keyHead == 0) return;
                var name = GetXmlName();
                var ele = XmlOp.Instance.GetXmlElement(name);
                if( ele == null )
                {
                    ele = XmlOp.Instance.AddXmlElement(name, "", null , false);
                }
                XmlOp.Instance.SetOrAddXmlAttributValue("KeyHead", m_keyHead.ToString(), ele, false);
                XmlOp.Instance.SetOrAddXmlAttributValue("KeyList", GetValueXmlStr(), ele, false);
                XmlOp.Instance.Save();
            }
            public string GetXmlName()
            {
                return "KEY_" + m_keyHead.ToString();
            }
            private string GetValueXmlStr()
            {
                string value = "";
                for (int i = 0; i < m_keyList.Count; i++)
                {
                    value += m_keyList[i];
                    if( i < m_keyList.Count - 1)
                    {
                        value += "|";
                    }
                }
                return value;
            }
        }
        private bool m_isRuning = false;
        private bool m_isEditoring = false;
        private List<KeyData> m_keyDataList = new List<KeyData>();
        private int m_currKey;
        private bool m_isSelfSend = false;
        private List<int> m_keyCache = new List<int>();

        private MouseKeyHook m_mouseKeyboardHook = null;
        private SupperKeyHook m_keyboardHook = null;

        private int m_inputKey;
        public KeyAssist()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            m_isEditoring = false;
            m_isRuning = false;
            m_isSelfSend = false;
            InitTimer();
            InitKeyData();
            InitKeyboardHook();
            InitScreenData();
            RefreshGrid();
        }
        private void InitTimer()
        {
            int time = 100;
            var timeStr = XmlOp.Instance.GetXmlElementValue("TimeInterval");
            if(!string.IsNullOrEmpty(timeStr))
            {
                time = int.Parse(timeStr);
            }
            m_inputTimeInterval.Text = time.ToString();
            m_timer.Interval = time;
            m_timer.Start();
        }
        private void InitKeyData()
        {
            m_keyDataList.Clear();
            var keyEleList = XmlOp.Instance.GetXmlAllElement();
            if(keyEleList != null)
            {
                for (int i = 0; i < keyEleList.Count; i++)
                {
                    var ele = keyEleList[i];
                    var name = ele.Name.ToString();
                    if(name.Contains("KEY_"))
                    {
                        var keyHead = int.Parse(XmlOp.Instance.GetXmlAttributValue("KeyHead", ele));
                        var value = XmlOp.Instance.GetXmlAttributValue("KeyList", ele);
                        var list = value.Split('|');
                        var keyList = new List<int>();
                        for (int j = 0; j < list.Length; j++)
                        {
                            keyList.Add(int.Parse(list[j].ToString()));
                        }
                        m_keyDataList.Add(new KeyData()
                        {
                            m_keyHead = keyHead,
                            m_keyList = keyList,
                        });
                    }
                }
            }
        }
        int wcol1 = 60;
        int wcol2 = 30;
        private void RefreshGrid()
        {
            m_grid.Rows.Clear();
            m_grid.Columns.Clear();
            DataGridViewTextBoxColumn text = new DataGridViewTextBoxColumn();
            text.Name = "键名";
            text.Width = wcol1;
            text.ReadOnly = true;
            text.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_grid.Columns.Add(text);
            DataGridViewTextBoxColumn text2 = new DataGridViewTextBoxColumn();
            text2.Name = "";
            text2.Width = wcol2;
            text2.ReadOnly = true;
            text2.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_grid.Columns.Add(text2);
            DataGridViewTextBoxColumn text3 = new DataGridViewTextBoxColumn();
            text3.Name = "替换按键";
            text3.Width = m_grid.Width - wcol1 - wcol2 - 43;
            text3.ReadOnly = true;
            text3.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_grid.Columns.Add(text3);
            for (int i = 0; i < m_keyDataList.Count; i++)
            {
                AddRow(m_keyDataList[i]);
            }
        }
        private void AddRow( KeyData keyData)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell c1 = new DataGridViewTextBoxCell();
            c1.Value = keyData.KeyHeadStr;
            c1.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(c1);

            DataGridViewTextBoxCell c2 = new DataGridViewTextBoxCell();
            c2.Value = "=>";
            c2.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(c2);

            DataGridViewTextBoxCell c3 = new DataGridViewTextBoxCell();
            c3.Value = keyData.KeyListStr;
            c3.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            row.Cells.Add(c3);

            m_grid.Rows.Add(row);
        }
        private void m_grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void InitKeyboardHook()
        {
            m_mouseKeyboardHook = new MouseKeyHook(false, true);//鼠标，键盘  
            //m_keyboardHook.KeyDown += new KeyEventHandler(OnKeyDown);
            //m_keyboardHook.KeyUp += new KeyEventHandler(OnKeyUp);
            //m_keyboardHook.KeyPress += new KeyPressEventHandler(OnKeyPress);
            m_keyboardHook = new SupperKeyHook(Handle);
            m_keyboardHook.KeyDown = OnKeyDown;
            m_keyboardHook.KeyUp = OnKeyUp;
            m_async.RunWorkerAsync();
        }
        private bool m_isKeyDown = false;
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if(m_isSelfSend)
            {
                //继续让系统处理按键消息
                return;
            }
            while (true)
            {
                try
                {
                    if (m_keyCache.Count > 0)
                    {
                        //只剩最后一个键就长按
                        if (m_keyCache.Count == 1)
                        {
                            SendKey(m_keyCache[m_keyCache.Count - 1]);
                        }
                        break;
                    }
                    if (m_isKeyDown ) break;
                    m_isKeyDown = true;
                    m_textMessage.Text = e.KeyCode.ToString();
                    if (m_isEditoring)
                    {
                        EditorKeyList(e.KeyValue);
                    }
                    if (m_isRuning)
                    {
                        OnInputKey(e.KeyValue);
                    }
                }
                finally
                {
                }
                break;
            }
            //不让系统处理按键消息
            e.Handled = true;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if( m_isSelfSend )
            {
                m_isSelfSend = false;
                return;
            }
            m_isKeyDown = false;
            m_currKey = 0;
            e.Handled = true;
        }
        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void OnKeyDown(int keyCode )
        {
            while (true)
            {
                try
                {
                    if (m_keyCache.Count > 0)
                    {
                        if (m_keyCache.Count == 1)
                        {
                            SendKey(m_keyCache[m_keyCache.Count - 1]);
                        }
                        break;
                    }
                    if (m_isKeyDown) break;
                    m_isKeyDown = true;
                    if (m_isEditoring)
                    {
                        EditorKeyList(keyCode);
                    }
                    if (m_isRuning)
                    {
                        OnInputKey(keyCode);
                    }
                }
                finally
                {
                }
                break;
            }
        }

        private void OnKeyUp(int keyCode)
        {
            m_isKeyDown = false;
            m_currKey = 0;
        }


        private void KeyAssist_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_isEditoring)
            {
                if (MessageBox.Show("之前的编辑还未保存，是否保存", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    StopEditor(true);
                }
                else
                {
                    StopEditor(false);
                }
            }
            //if( MessageBox.Show("是否退出","提示",MessageBoxButtons.YesNo) == DialogResult.Yes)
            //{
            m_timer.Stop();
            if (m_keyboardHook != null)
            {
                m_keyboardHook.Stop();
            }
            if(m_mouseKeyboardHook != null )
            {
                m_mouseKeyboardHook.Stop();
            }
            //    e.Cancel = false;
            //}
            //else
            //{
            //    e.Cancel = true;
            //}
        }

        private void m_timer_Tick(object sender, EventArgs e)
        {
            if(m_inputKey != 0)
            {
                m_textMessage.Text = KeyCodeStr.Get(m_inputKey) ?? "";
                m_inputKey = 0;
            }
            AutoSideHideOrShow();
            if (m_keyCache.Count > 0)
            {
                //长按就一直按最后一个键
                if(!m_isKeyDown || m_keyCache.Count > 1)
                {
                    var keyCode = m_keyCache[0];
                    m_keyCache.RemoveAt(0);
                    SendKey(keyCode);
                    m_textMessage.Text = m_textMessage.Text + " " + (KeyCodeStr.Get(keyCode) ?? "");
                }
            }
        }
        private void CopyList<T>(List<T> newList , List<T> sourceList )
        {
            newList.Clear();
            for (int i = 0; i < sourceList.Count; i++)
            {
                newList.Add(sourceList[i]);
            }
        }

        private void OnInputKey(int keyCode )
        {
            KeyData data = null;
            for (int i = 0; i < m_keyDataList.Count; i++)
            {
                if(m_keyDataList[i].m_keyHead == keyCode)
                {
                    data = m_keyDataList[i];
                    break;
                }
            }
            if (data == null || data.m_keyList.Count <= 0)
            {
                SendKey(keyCode);
                m_keyCache.Clear();
            }
            else
            {
                if(m_currKey == keyCode)
                {
                    m_keyCache.Add(data.m_keyList[data.m_keyList.Count - 1]);
                }
                else
                {
                    m_inputKey = keyCode;
                    CopyList(m_keyCache, data.m_keyList);
                }
            }
            m_currKey = keyCode;
        }

        private void m_btnStart_Click(object sender, EventArgs e)
        {
            if (m_isEditoring)
            {
                if (MessageBox.Show("之前的编辑还未保存，是否保存", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    StopEditor(true);
                }
                else
                {
                    StopEditor(false);
                }
            }
            if (!m_isRuning)
            {
                m_isRuning = true;
                m_btnStart.Text = "停止";
                m_keyboardHook.Start();
                m_mouseKeyboardHook.Start();
            }
            else
            {
                m_isRuning = false;
                m_btnStart.Text = "启动";
                m_keyboardHook.Stop();
                m_mouseKeyboardHook.Stop();
            }
        }

        private void m_btnRemove_Click(object sender, EventArgs e)
        {
            if (m_isEditoring)
            {
                if( MessageBox.Show("之前的编辑还未保存，是否保存","提示" , MessageBoxButtons.YesNo) == DialogResult.Yes )
                {
                    StopEditor(true);
                }
                else
                {
                    StopEditor(false);
                }
            }
            var row = m_grid.CurrentRow;
            if (row == null)
            {
                MessageBox.Show("请先点击选中目标行");
                return;
            }
            var keyData = m_keyDataList[row.Index];
            m_keyDataList.RemoveAt(row.Index);
            m_grid.Rows.Remove(row);
            XmlOp.Instance.DeleteXmlElement(keyData.GetXmlName());
        }

        private void m_btnAdd_Click(object sender, EventArgs e)
        {
            if (m_isEditoring)
            {
                if (MessageBox.Show("之前的编辑还未保存，是否保存", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    StopEditor(true);
                }
                else
                {
                    StopEditor(false);
                }
            }
            var keyData = new KeyData();
            m_keyDataList.Add(keyData);
            AddRow(keyData);
            keyData.Save();
            m_grid.CurrentCell = m_grid.Rows[m_grid.Rows.Count - 1].Cells[0];
        }

        private void m_btnEdit_Click(object sender, EventArgs e)
        {
            var row = m_grid.CurrentRow;
            if (row == null)
            {
                MessageBox.Show("请先点击选中目标单元格");
                return;
            }
            if (m_isEditoring)
            {
                StopEditor(true);
            }
            else
            {
                StartEditor();
            }
        }
        private void StartEditor()
        {
            if (m_isRuning)
            {
                MessageBox.Show("请先停止启动");
                return;
            }
            m_isEditoring = true;
            m_keyboardHook.Start();
            m_mouseKeyboardHook.Start();
            m_btnEdit.Text = "保存";
            m_textMessage.Text = "已进入按键录入模式，直接点击键盘完成录入";
        }
        private void StopEditor(bool save )
        {
            var row = m_grid.CurrentRow;
            if (row == null)
            {
                MessageBox.Show("请先点击选中目标单元格");
                return;
            }
            m_isEditoring = false;
            m_btnEdit.Text = "编辑";
            m_keyboardHook.Stop();
            m_mouseKeyboardHook.Stop();
            if (save)
            {
                var keyData = m_keyDataList[row.Index];
                keyData.Save();
                m_textMessage.Text = "已退出编辑模式，按键已保存";
            }
        }
        private void EditorKeyList( int keyCode )
        {
            var row = m_grid.CurrentRow;
            if (row == null)
            {
                MessageBox.Show("请先点击选中目标行");
                return;
            }
            var data = m_keyDataList[row.Index];
            if ( keyCode == KeyCode.KEY_BACK )
            {
                if (data.m_keyList.Count > 0)
                {
                    data.m_keyList.RemoveAt(data.m_keyList.Count - 1);
                    row.Cells[2].Value = data.KeyListStr;
                }
                else
                {
                    data.m_keyHead = 0;
                    row.Cells[0].Value = "";
                }
                return;
            }
            var codeStr = KeyCodeStr.Get(keyCode);
            if(codeStr == null)
            {
                return;
            }
            if(data.m_keyHead == 0)
            {
                data.m_keyHead = keyCode;
                row.Cells[0].Value = data.KeyHeadStr;
            }
            else
            {
                data.m_keyList.Add(keyCode);
                row.Cells[2].Value = data.KeyListStr;
            }
        }
        private void SendKey(int keyCode)
        {
            m_isSelfSend = true;
            m_mouseKeyboardHook.SendKey(keyCode);
        }
        private void m_async_DoWork(object sender, DoWorkEventArgs e)
        {
            if (m_keyboardHook != null)
            {
                while (true)
                {
                    m_keyboardHook.Update();
                }
            }
        }


        private void m_grid_SizeChanged(object sender, EventArgs e)
        {
            if (m_grid.Rows.Count <= 0) return;
            var col = m_grid.Columns[2];
            col.Width = m_grid.Width - wcol1 - wcol2 - 43;
        }
        private void m_inputTimeInterval_TextChanged(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(m_inputTimeInterval.Text))
            {
                m_inputTimeInterval.Text = "100";
            }
            var time = int.Parse(m_inputTimeInterval.Text);
            XmlOp.Instance.SetOrAddElement("TimeInterval", time.ToString());
            m_timer.Interval = time;
        }

        #region 边缘停靠
        int ScreenLeft = 0;
        int ScreenRight = 0;
        void InitScreenData()
        {
            ScreenLeft = 0;
            ScreenRight = 0;
            var scenes = Screen.AllScreens;
            for (int i = 0; i < scenes.Length; i++)
            {
                var area = scenes[i].WorkingArea;
                if (area.Left < ScreenLeft)
                    ScreenLeft = area.Left;
                if (area.Right > ScreenRight)
                    ScreenRight = area.Right;
            }
        }
        void AutoSideHideOrShow()
        {
            //如果窗体最小化或最大化了则什么也不做
            if (this.WindowState == FormWindowState.Minimized || this.WindowState == FormWindowState.Maximized)
            {
                return;
            }
            int sideThickness = 5;//边缘的厚度，窗体停靠在边缘隐藏后留出来的可见部分的厚度
            int corr = 5;
            int topCorr = 3;
            var ScreenTop = 0;
            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                var area = Screen.AllScreens[i].WorkingArea;
                if (Cursor.Position.X >= area.Left && Cursor.Position.X < area.Right)
                {
                    ScreenTop = area.Top;
                    break;
                }
            }
            //如果鼠标在窗体内
            if (Cursor.Position.X >= this.Left - corr && Cursor.Position.X < this.Right + corr && Cursor.Position.Y >= this.Top - topCorr && Cursor.Position.Y < this.Bottom)
            {
                //如果窗体离屏幕边缘很近，则自动停靠在该边缘
                if (this.Top <= ScreenTop + sideThickness)
                {
                    this.Top = ScreenTop;
                }
                if (this.Left <= ScreenLeft + sideThickness)
                {
                    this.Left = ScreenLeft;
                }
                if (this.Left >= ScreenRight - this.Width - sideThickness)
                {
                    this.Left = ScreenRight - this.Width;
                }
            }
            //当鼠标离开窗体以后
            else
            {
                //隐藏到屏幕左边缘
                if (this.Left == ScreenLeft)
                {
                    this.Left = ScreenLeft + sideThickness - this.Width - corr;
                }
                //隐藏到屏幕右边缘
                else if (this.Left == ScreenRight - this.Width)
                {
                    this.Left = ScreenRight - sideThickness + corr;
                }
                //隐藏到屏幕上边缘
                else if (this.Top == ScreenTop && this.Left > ScreenLeft && this.Left < ScreenRight - this.Width)
                {
                    this.Top = ScreenTop + sideThickness - this.Height - topCorr;
                }
            }
        }
        #endregion

        
    }
}
