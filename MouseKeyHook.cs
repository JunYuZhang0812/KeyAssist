using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Win32;
using System.Drawing;

namespace KeyAssist
{
    class MouseKeyHook
    {
        [StructLayout(LayoutKind.Sequential)]
        private class POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private class MouseHookStruct
        {
            public POINT pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private class MouseLLHookStruct
        {
            public POINT pt;
            public int mouseData;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private class KeyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        #region 设置键盘钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int UnhookWindowsHookEx(int idHook);
        /// <summary>
        /// 传递钩子
        /// </summary>
        /// <param name="idHook">是您自己的钩子函数的句柄，用该句柄可以遍历钩子链</param>
        /// <param name="nCode">把传入的参数简单传给CallNextHookEx即可</param>
        /// <param name="wParam">把传入的参数简单传给CallNextHookEx即可</param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        private delegate int HookProc(int nCode, int wParam, IntPtr lParam);
        /// <summary>
        /// 键盘事件
        /// </summary>
        /// <param name="bVk">虚拟键值</param>
        /// <param name="bScan">一般为0</param>
        /// <param name="dwFlags">这里是整数类型 0 为按下，2为释放</param>
        /// <param name="dwExtraInfo">这里是整数类型 一般情况下设成为0 </param>

        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        public static extern void keybd_event(byte bVk,byte bScan,int dwFlags,int dwExtraInfo );
        #endregion
        #region 窗口相关
        //向窗口发送消息
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        /// <summary>
        /// 寻找窗口
        /// </summary>
        /// <param name="lpClassName">null</param>
        /// <param name="lpWindowName">窗口名</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        //获取当前活动窗口句柄
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();
        /// <summary>
        /// 显示窗口
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="nCmdShow">0 关闭窗口 1 正常大小显示窗口 2 最小化窗口 3 最大化窗口</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        /// <summary>
        /// 获取窗口大小及位置
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpRect"></param>
        /// <returns></returns>
        [DllImport("user32.dll")][return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        /*示例
         * InPtr awin = GetForegroundWindow();    //获取当前窗口句柄
         * RECT rect = new RECT();
         * GetWindowRect(awin, ref rect);
         * int width = rc.Right - rc.Left;  //窗口的宽度
         * int height = rc.Bottom - rc.Top; //窗口的高度
         * int x = rc.Left;       
         * int y = rc.Top;
         */
        /// <summary>
        /// 遍历窗口
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public delegate bool CallBack(int hwnd, int lParam);
        [DllImport("user32")]
        public static extern int EnumWindows(CallBack x, int y);
        /*示例
         * CallBack myCallBack = new CallBack(Recall);
         * EnumWindows(myCallBack, 0);
         * 
         * public bool Recall(int hwnd, int lParam)
         * {
         * 
         * }
         */

        /// <summary>
        /// 获得窗口名
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpString"></param>
        /// <param name="nMaxCount"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern int GetWindowTextW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpString, int nMaxCount);
        /// <summary>
        /// 获得类名
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpString"></param>
        /// <param name="nMaxCount"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern int GetClassNameW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpString, int nMaxCount);

        #endregion

        [DllImport("user32")]
        private static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);

        [DllImport("user32")]
        private static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern short GetKeyState(int vKey);

        [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleA", SetLastError = true, CharSet = CharSet.Ansi, ThrowOnUnmappableChar = true)]
        public static extern IntPtr GetModuleHandleA(String lpModuleName);

        [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = true, CharSet = CharSet.Unicode, ThrowOnUnmappableChar = true)]
        public static extern IntPtr GetModuleHandleW(String lpModuleName);



        //消息参数的值
        private const int WH_MOUSE_LL = 14;
        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE = 7;
        private const int WH_KEYBOARD = 2;
        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_MBUTTONUP = 0x208;
        private const int WM_LBUTTONDBLCLK = 0x203;
        private const int WM_RBUTTONDBLCLK = 0x206;
        private const int WM_MBUTTONDBLCLK = 0x209;
        private const int WM_MOUSEWHEEL = 0x020A;

        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_SYSKEYUP = 0x105;

        private const byte VK_SHIFT = 0x10;
        private const byte VK_CAPITAL = 0x14;
        private const byte VK_NUMLOCK = 0x90;

        public MouseKeyHook()
        {
            //Start();
        }
        public MouseKeyHook(bool InstallMouseHook, bool InstallKeyboardHook)
        {
            //Start(InstallMouseHook, InstallKeyboardHook);
        }

        ~MouseKeyHook()
        {
            Stop(true, true, false);
        }

        public event MouseEventHandler OnMouseActivity; //MouseEventHandler是委托，表示处理窗体、控件或其他组件的 MouseDown、MouseUp 或 MouseMove 事件的方法。
        public event KeyEventHandler KeyDown;
        public event KeyPressEventHandler KeyPress;
        public event KeyEventHandler KeyUp;

        private int hMouseHook = 0; //标记mouse hook是否安装
        private int hKeyboardHook = 0;

        private static HookProc MouseHookProcedure;
        private static HookProc KeyboardHookProcedure;

        //---------------------------------------------------------------------------
        public void Start()
        {
            this.Start(true, true);
        }

        public void Start(bool InstallMouseHook, bool InstallKeyboardHook)
        {
            IntPtr HM = Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]);

            if (hMouseHook == 0 && InstallMouseHook)
            {
                MouseHookProcedure = new HookProc(MouseHookProc);//钩子的处理函数
                hMouseHook = SetWindowsHookEx(
                        WH_MOUSE_LL,
                        MouseHookProcedure,
                        GetModuleHandleW(Process.GetCurrentProcess().MainModule.ModuleName),//本进程模块句柄
                        0);
                if (hMouseHook == 0)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    Stop(true, false, false);
                    throw new Win32Exception(errorCode);
                }
            }

            if (hKeyboardHook == 0 && InstallKeyboardHook)
            {
                KeyboardHookProcedure = new HookProc(KeyboardHookProc);
                hKeyboardHook = SetWindowsHookEx(
                        WH_KEYBOARD_LL,
                        KeyboardHookProcedure,
                    //Marshal.GetHINSTANCE( Assembly.GetExecutingAssembly().GetModules()[0]),
                        GetModuleHandleW(Process.GetCurrentProcess().MainModule.ModuleName),
                        0);
                if (hKeyboardHook == 0)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    Stop(false, true, false);
                    throw new Win32Exception(errorCode);
                }
            }
        }
        //-------------------------------------------------
        public void Stop()
        {
            this.Stop(true, true, true);
        }

        public void Stop(bool UninstallMouseHook, bool UninstallKeyboardHook, bool ThrowExceptions)
        {
            if (hMouseHook != 0 && UninstallMouseHook)
            {
                int retMouse = UnhookWindowsHookEx(hMouseHook);
                hMouseHook = 0;
                if (retMouse == 0 && ThrowExceptions)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode);
                }
            }

            if (hKeyboardHook != 0 && UninstallKeyboardHook)
            {
                int retKeyboard = UnhookWindowsHookEx(hKeyboardHook);
                hKeyboardHook = 0;
                if (retKeyboard == 0 && ThrowExceptions)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode);
                }
            }
        }
        //获取当前活动窗口
        public IntPtr CurrentWindow()
        {
            return GetForegroundWindow();
        }
        //模拟按键
        public void SendKey( int keyCode)
        {
            //StringBuilder s = new StringBuilder(256);
            //GetWindowTextW(window, s,s.Capacity);
            //MessageBox.Show(s.ToString());
            //SendMessage(window, WindowsMessages.WM_SYSKEYDOWN, keyCode, 0);
            //SendMessage(window, WindowsMessages.WM_SYSKEYUP, keyCode, 0);
            keybd_event((byte)keyCode, 0, KeyEvent.KEY_DOWN, 0);
            keybd_event((byte)keyCode, 0, KeyEvent.KEY_UP, 0);
        }
        //-------------------------------------------------------------------------------

        private int MouseHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if ((nCode >= 0) && (OnMouseActivity != null))
            {
                MouseLLHookStruct mouseHookStruct = (MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseLLHookStruct));

                MouseButtons button = MouseButtons.None;
                short mouseDelta = 0;
                int clickCount = 0;
                switch (wParam)
                {
                    case WM_LBUTTONDOWN://513出现了
                        button = MouseButtons.Left;
                        clickCount = 1;
                        break;
                    case WM_RBUTTONDOWN://516出现了
                        button = MouseButtons.Right;
                        clickCount = 1;
                        break;
                    case WM_LBUTTONDBLCLK://515  doubleclick没有出现过
                        button = MouseButtons.XButton1;
                        clickCount = 2;
                        break;
                    case WM_RBUTTONDBLCLK://518
                        button = MouseButtons.XButton1;
                        clickCount = 2;
                        break;
                    case WM_MOUSEMOVE://512 出现了
                        button = MouseButtons.XButton2;
                        clickCount = 0;
                        break;
                    case WM_MOUSEWHEEL://522 没试
                        mouseDelta = (short)((mouseHookStruct.mouseData >> 16) & 0xffff);
                        clickCount = 0;
                        break;
                }

                MouseEventArgs e = new MouseEventArgs(
                                                   button,
                                                   clickCount,
                                                   mouseHookStruct.pt.x,
                                                   mouseHookStruct.pt.y,
                                                   mouseDelta);
                OnMouseActivity(this, e);//转给委托函数
            }
            return CallNextHookEx(hMouseHook, nCode, wParam, lParam);
        }
        //------------------------------------------------------------------------------------
        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            bool handled = false;
            if ((nCode >= 0) && (KeyDown != null || KeyUp != null || KeyPress != null))
            {
                KeyboardHookStruct MyKeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                if (KeyDown != null && (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                    KeyEventArgs e = new KeyEventArgs(keyData);
                    KeyDown(this, e);               //转给委托函数
                    handled = handled || e.Handled;
                }

                if (KeyPress != null && wParam == WM_KEYDOWN)
                {
                    bool isDownShift = ((GetKeyState(VK_SHIFT) & 0x80) == 0x80 ? true : false);
                    bool isDownCapslock = (GetKeyState(VK_CAPITAL) != 0 ? true : false);

                    byte[] keyState = new byte[256];
                    GetKeyboardState(keyState);
                    byte[] inBuffer = new byte[2];
                    if (ToAscii(MyKeyboardHookStruct.vkCode,
                              MyKeyboardHookStruct.scanCode,
                              keyState,
                              inBuffer,
                              MyKeyboardHookStruct.flags) == 1)
                    {
                        char key = (char)inBuffer[0];
                        if ((isDownCapslock ^ isDownShift) && Char.IsLetter(key)) key = Char.ToUpper(key);
                        KeyPressEventArgs e = new KeyPressEventArgs(key);
                        KeyPress(this, e);
                        handled = handled || e.Handled;
                    }
                }

                if (KeyUp != null && (wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                    KeyEventArgs e = new KeyEventArgs(keyData);
                    KeyUp(this, e);
                    handled = handled || e.Handled;
                }

            }

            if (handled)
                return 1;
            else
                return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
        }
    }
}
public class Tool
{
    /// <summary>
    /// 抓取屏幕
    /// </summary>
    public void ScreenShot()
    {
        //获得当前屏幕的分辨率
        Screen scr = Screen.PrimaryScreen;
        Rectangle rc = scr.Bounds;
        int iWidth = rc.Width;
        int iHeight = rc.Height;
        //创建一个和屏幕一样大的Bitmap
        Image myImage = new Bitmap(iWidth, iHeight);
        //从一个继承自Image类的对象中创建Graphics对象
        Graphics g = Graphics.FromImage(myImage);
        //抓屏并拷贝到myimage里
        g.CopyFromScreen(new Point(0, 0), new Point(0, 0), new Size(iWidth, iHeight));
        //保存为文件
        myImage.Save(@"c:/1.jpeg");

    }
}
