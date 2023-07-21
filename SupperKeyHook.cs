using SuperKeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;
using System.Threading;
//直接操作驱动的超级键盘钩子
public class SupperKeyHook
{
    WinIoSys m_ioSys = new WinIoSys();
    DirectInput m_direcInput;
    private Keyboard m_curKeyboard;
    private Dictionary<Key, bool> m_keysStateDic = new Dictionary<Key, bool>();
    private List<Key> m_tempCache = new List<Key>();
    IntPtr hwnd;

    public Keyboard Keyboard
    {
        get
        {
            return m_curKeyboard;
        }
    }
    public Action<int> KeyDown;
    public Action<int> KeyUp;

    public SupperKeyHook(IntPtr hwnd)
    {
        this.hwnd = hwnd;
        InitKeyBoard();
    }
    public void Start()
    {
        m_ioSys.InitSuperKeys();
    }

    public void Stop()
    {
        m_ioSys.CloseSuperKeys();
    }

    public void SendKey(int keyCode )
    {
        var key = (WinIoSys.Key)keyCode;
        if ( keyCode >= KeyCode.KEY_LEFT_ARROW && keyCode <= KeyCode.KEY_DOWN_ARROW)
        {
            m_ioSys.KeyDownEx(key);
            m_ioSys.KeyUpEx(key);
        }
        else
        {
            m_ioSys.KeyDown(key);
            m_ioSys.KeyUp(key);
        }
    }

    public void Update()
    {
        if (m_curKeyboard == null) return;
        KeyboardState state = m_curKeyboard.GetCurrentState();
        var keys = state.PressedKeys;
        for (int i = 0; i < keys.Count; i++)
        {
            var key = keys[i];
            if(!m_keysStateDic.ContainsKey(key))
            {
                m_keysStateDic.Add(key, true);
                if (KeyDown != null)
                {
                    var keyCode = TransKeyCode(key);
                    if(keyCode != 0 )
                    {
                        KeyDown(keyCode);
                    }
                }
            }
            else
            {
                m_keysStateDic[key] = true;
                if (KeyDown != null)
                {
                    var keyCode = TransKeyCode(key);
                    if (keyCode != 0)
                    {
                        KeyDown(keyCode);
                    }
                }
            }
        }
        m_tempCache.Clear();
        var itor = m_keysStateDic.GetEnumerator();
        while (itor.MoveNext())
        {
            var item = itor.Current;
            var key = item.Key;
            if(item.Value)
            {
                if (!keys.Contains(key))
                {
                    m_tempCache.Add(key);
                    if (KeyUp != null)
                    {
                        var keyCode = TransKeyCode(key);
                        if (keyCode != 0)
                        {
                            KeyUp(keyCode);
                        }
                    }
                }
            }
        }
        itor.Dispose();
        for (int i = 0; i < m_tempCache.Count; i++)
        {
            m_keysStateDic[m_tempCache[i]] = false;
        }
    }
    
    private void InitKeyBoard()
    {
        m_keysStateDic.Clear();
        m_direcInput = new DirectInput();
        m_curKeyboard = new Keyboard(m_direcInput);
        m_curKeyboard.SetCooperativeLevel(hwnd, CooperativeLevel.Background | CooperativeLevel.NonExclusive);
        m_curKeyboard.Acquire();//连接键盘
    }

    Dictionary<Key, int> m_keyCodeTrans = new Dictionary<Key, int>()
    {
        [Key.A] = KeyCode.KEY_A,
        [Key.B] = KeyCode.KEY_B,
        [Key.C] = KeyCode.KEY_C,
        [Key.D] = KeyCode.KEY_D,
        [Key.E] = KeyCode.KEY_E,
        [Key.F] = KeyCode.KEY_F,
        [Key.G] = KeyCode.KEY_G,
        [Key.H] = KeyCode.KEY_H,
        [Key.I] = KeyCode.KEY_I,
        [Key.J] = KeyCode.KEY_J,
        [Key.K] = KeyCode.KEY_K,
        [Key.L] = KeyCode.KEY_L,
        [Key.M] = KeyCode.KEY_M,
        [Key.N] = KeyCode.KEY_N,
        [Key.O] = KeyCode.KEY_O,
        [Key.P] = KeyCode.KEY_P,
        [Key.Q] = KeyCode.KEY_Q,
        [Key.R] = KeyCode.KEY_R,
        [Key.S] = KeyCode.KEY_S,
        [Key.T] = KeyCode.KEY_T,
        [Key.U] = KeyCode.KEY_U,
        [Key.V] = KeyCode.KEY_V,
        [Key.W] = KeyCode.KEY_W,
        [Key.X] = KeyCode.KEY_X,
        [Key.Y] = KeyCode.KEY_Y,
        [Key.Z] = KeyCode.KEY_Z,
        [Key.Space] = KeyCode.KEY_SPACE,
        [Key.Up] = KeyCode.KEY_UP_ARROW,
        [Key.Down] = KeyCode.KEY_DOWN_ARROW,
        [Key.Left] = KeyCode.KEY_LEFT_ARROW,
        [Key.Right] = KeyCode.KEY_RIGHT_ARROW,
    };

    private int TransKeyCode(Key key)
    {
        if( m_keyCodeTrans.ContainsKey(key))
        {
            return m_keyCodeTrans[key];
        }
        return 0;
    }
}
