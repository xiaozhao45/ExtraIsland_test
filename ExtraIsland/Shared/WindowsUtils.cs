using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace ExtraIsland.Shared;

[SuppressMessage("Interoperability","SYSLIB1054:使用 “LibraryImportAttribute” 而不是 “DllImportAttribute” 在编译时生成 P/Invoke 封送代码")]
public static class WindowsUtils {
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll",CharSet = CharSet.Unicode)]
    static extern int GetWindowText(IntPtr hWnd,StringBuilder text,int count);

    [DllImport("user32.dll",SetLastError = true)]
    // ReSharper disable once IdentifierTypo
    static extern uint GetWindowThreadProcessId(IntPtr hWnd,out int lpdwProcessId);

    // 检测当前系统前台窗口是否属于本进程
    public static bool IsOurWindowInForeground() {
        IntPtr foregroundWindow = GetForegroundWindow();
        if (foregroundWindow == IntPtr.Zero) return false;
        GetWindowThreadProcessId(foregroundWindow,out int processId);
        return processId == Environment.ProcessId;
    }

    // 获取当前前台窗口标题
    public static string? GetActiveWindowTitle() {
        const int nChars = 256;
        StringBuilder buffer = new StringBuilder(nChars);
        IntPtr handle = GetForegroundWindow();
        return GetWindowText(handle,buffer,nChars) > 0 ? buffer.ToString() : null;
    }

    // ReSharper disable IdentifierTypo
    // ReSharper disable StringLiteralTypo
    public static class AppBarHelper {
        public static class AppBarCreator {
            public static readonly DependencyProperty AppBarProperty =
                DependencyProperty.RegisterAttached(
                    "AppBar",
                    typeof(AppBar),
                    typeof(AppBarCreator),
                    new PropertyMetadata(null,OnAppBarChanged));
            static void OnAppBarChanged(DependencyObject d,DependencyPropertyChangedEventArgs e) {
                if (d is Window window && e.NewValue is AppBar appBar) {
                    appBar.AttachedWindow = window;
                }
            }
            public static void SetAppBar(Window element,AppBar? value) {
                if (value == null) return;
                element.SetValue(AppBarProperty,value);
            }

            public static AppBar GetAppBar(Window element) {
                return (AppBar)element.GetValue(AppBarProperty);
            }
        }

        public class AppBar : DependencyObject {
            /// <summary>
            /// 附加到的窗口
            /// </summary>
            public Window? AttachedWindow {
                get => _window;
                set {
                    if (value == null) return;
                    _window = value;
                    _window.Closing += _window_Closing;
                    _window.LocationChanged += _window_LocationChanged;
                    //获取窗口句柄hWnd
                    IntPtr handle = new WindowInteropHelper(value).Handle;
                    if (handle == IntPtr.Zero) {
                        //Win32窗口未创建
                        _window.SourceInitialized += _window_SourceInitialized;
                    } else {
                        _hWnd = handle;
                        CheckPending();
                    }
                }
            }

            void _window_LocationChanged(object? sender,EventArgs e) {
                if (_window != null)
                    Debug.WriteLine(_window.Title + " LocationChanged: Top: " + _window.Top + "  Left: " + _window.Left);
            }

            void _window_Closing(object? sender,CancelEventArgs e) {
                if (_window != null) _window.Closing -= _window_Closing;
                if (Location != AppBarLocation.None)
                    DisableAppBar();
            }

            /// <summary>
            /// 检查是否需要应用之前的Location更改
            /// </summary>
            void CheckPending() {
                //创建AppBar时提前触发的LocationChanged
                if (_locationChangePending) {
                    _locationChangePending = false;
                    LoadAppBar(Location);
                }
            }
            /// <summary>
            /// 载入AppBar
            /// </summary>
            /// <param name="e"></param>
            /// <param name="previous"></param>
            void LoadAppBar(AppBarLocation e,AppBarLocation? previous = null) {

                if (e != AppBarLocation.None) {
                    if (e == AppBarLocation.RegisterOnly) {
                        //仅注册AppBarMsg
                        //如果之前注册过有效的AppBar则先注销，以还原位置
                        if (previous.HasValue && previous.Value != AppBarLocation.RegisterOnly) {
                            if (previous.Value != AppBarLocation.None) {
                                //由生效的AppBar转为RegisterOnly，还原为普通窗口再注册空AppBar
                                DisableAppBar();
                            }
                            RegisterAppBarMsg();
                        } else {
                            //之前未注册过AppBar，直接注册
                            RegisterAppBarMsg();
                        }
                    } else {
                        if (previous.HasValue && previous.Value != AppBarLocation.None) {
                            //之前为RegisterOnly才备份窗口信息
                            if (previous.Value == AppBarLocation.RegisterOnly) {
                                BackupWindowInfo();
                            }
                            SetAppBarPosition(_originalSize);
                            ForceWindowStyles();
                        } else
                            EnableAppBar();
                    }
                } else {
                    DisableAppBar();
                }
            }
            void _window_SourceInitialized(object? sender,EventArgs e) {
                if (_window != null) {
                    _window.SourceInitialized -= _window_SourceInitialized;
                    _hWnd = new WindowInteropHelper(_window).Handle;
                }
                CheckPending();
            }

            /// <summary>
            /// 当有窗口进入或退出全屏时触发 bool参数为true时表示全屏状态
            /// </summary>
            public event EventHandler<bool>? OnFullScreenStateChanged;
            /// <summary>
            /// 期望将AppBar停靠到的位置
            /// </summary>
            public AppBarLocation Location {
                get => (AppBarLocation)GetValue(LocationProperty);
                set => SetValue(LocationProperty,value);
            }

            public static readonly DependencyProperty LocationProperty =
                DependencyProperty.Register(
                    "Location",
                    typeof(AppBarLocation),typeof(AppBar),
                    new PropertyMetadata(AppBarLocation.None,OnLocationChanged));

            bool _locationChangePending;
            static void OnLocationChanged(DependencyObject d,DependencyPropertyChangedEventArgs e) {
                if (DesignerProperties.GetIsInDesignMode(d))
                    return;
                if (d is not AppBar appBar) return;
                if (appBar.AttachedWindow == null) {
                    appBar._locationChangePending = true;
                    return;
                }
                appBar.LoadAppBar((AppBarLocation)e.NewValue,(AppBarLocation)e.OldValue);
            }

            int _callbackId;
            bool _isRegistered;
            Window? _window;
            IntPtr _hWnd;
            WindowStyle _originalStyle;
            Point _originalPosition;
            Size _originalSize = Size.Empty;
            ResizeMode _originalResizeMode;
            bool _originalTopmost;
            IntPtr WndProc(IntPtr hwnd,int msg,IntPtr wParam,
                IntPtr lParam,ref bool handled) {
                if (msg != _callbackId) return IntPtr.Zero;
                Debug.WriteLine(_window?.Title + " AppBarMsg(" + _callbackId + "): " + wParam.ToInt32() + " LParam: "
                                + lParam.ToInt32());
                switch (wParam.ToInt32()) {
                    case (int)Interop.AppBarNotify.AbnPoschanged:
                        Debug.WriteLine("AppBarNotify.ABN_POSCHANGED ! " + _window?.Title);
                        if (Location != AppBarLocation.RegisterOnly)
                            SetAppBarPosition(Size.Empty);
                        handled = true;
                        break;
                    case (int)Interop.AppBarNotify.AbnFullscreenapp:
                        OnFullScreenStateChanged?.Invoke(this,lParam.ToInt32() == 1);
                        handled = true;
                        break;
                }
                return IntPtr.Zero;
            }

            public void BackupWindowInfo() {
                _callbackId = 0;
                if (_window == null) return;
                _originalStyle = _window.WindowStyle;
                _originalSize = new Size(_window.ActualWidth,_window.ActualHeight);
                _originalPosition = new Point(_window.Left,_window.Top);
                _originalResizeMode = _window.ResizeMode;
                _originalTopmost = _window.Topmost;
            }
            public void RestoreWindowInfo() {
                if (_originalSize == Size.Empty) return;
                if (_window == null) return;
                _window.WindowStyle = _originalStyle;
                _window.ResizeMode = _originalResizeMode;
                _window.Topmost = _originalTopmost;
                _window.Left = _originalPosition.X;
                _window.Top = _originalPosition.Y;
                _window.Width = _originalSize.Width;
                _window.Height = _originalSize.Height;
            }
            public void ForceWindowStyles() {
                if (_window == null) return;
                _window.WindowStyle = WindowStyle.None;
                _window.ResizeMode = ResizeMode.NoResize;
                _window.Topmost = true;
            }

            public void RegisterAppBarMsg() {
                Interop.Appbardata data = new Interop.Appbardata();
                data.cbSize = Marshal.SizeOf(data);
                data.hWnd = _hWnd;

                _isRegistered = true;
                _callbackId = Interop.RegisterWindowMessage(Guid.NewGuid().ToString());
                data.uCallbackMessage = _callbackId;
                #pragma warning disable CA1806
                Interop.SHAppBarMessage((int)Interop.AppBarMsg.AbmNew,ref data);
                #pragma warning restore CA1806
                HwndSource? source = HwndSource.FromHwnd(_hWnd);
                Debug.WriteLineIf(source == null,"HwndSource is null!");
                source?.AddHook(WndProc);
                Debug.WriteLine(_window?.Title + " RegisterAppBarMsg: " + _callbackId);
            }
            public void EnableAppBar() {
                if (!_isRegistered) {
                    //备份窗口信息并设置窗口样式
                    BackupWindowInfo();
                    //注册成为AppBar窗口
                    RegisterAppBarMsg();
                    ForceWindowStyles();
                }
                //成为AppBar窗口之后(或已经是)只需要注册并移动窗口位置即可
                SetAppBarPosition(_originalSize);
            }
            public void SetAppBarPosition(Size windowSize) {
                Interop.Appbardata data = new Interop.Appbardata();
                data.cbSize = Marshal.SizeOf(data);
                data.hWnd = _hWnd;
                data.uEdge = (int)Location;
                data.uCallbackMessage = _callbackId;
                Debug.WriteLine("\r\nWindow: " + _window?.Title);

                //获取WPF单位与像素的转换矩阵
                if (_window != null) {
                    CompositionTarget? compositionTarget = PresentationSource.FromVisual(_window)?.CompositionTarget;
                    if (compositionTarget == null)
                        throw new Exception("居然获取不到CompositionTarget?!");
                    Matrix toPixel = compositionTarget.TransformToDevice;
                    Matrix toWpfUnit = compositionTarget.TransformFromDevice;

                    //窗口在屏幕的实际大小
                    if (windowSize == Size.Empty)
                        windowSize = new Size(_window.ActualWidth,_window.ActualHeight);
                    Vector actualSize = toPixel.Transform(new Vector(windowSize.Width,windowSize.Height));
                    //屏幕的真实像素
                    Vector workArea = toPixel.Transform(new Vector(SystemParameters.PrimaryScreenWidth,SystemParameters.PrimaryScreenHeight));
                    Debug.WriteLine("WorkArea Width: {0}, Height: {1}",workArea.X,workArea.Y);

                    if (Location is AppBarLocation.Left or AppBarLocation.Right) {
                        data.rc.top = 0;
                        data.rc.bottom = (int)workArea.Y;
                        if (Location == AppBarLocation.Left) {
                            data.rc.left = 0;
                            data.rc.right = (int)Math.Round(actualSize.X);
                        } else {
                            data.rc.right = (int)workArea.X;
                            data.rc.left = (int)workArea.X - (int)Math.Round(actualSize.X);
                        }
                    } else {
                        data.rc.left = 0;
                        data.rc.right = (int)workArea.X;
                        if (Location == AppBarLocation.Top) {
                            data.rc.top = 0;
                            data.rc.bottom = (int)Math.Round(actualSize.Y);
                        } else {
                            data.rc.bottom = (int)workArea.Y;
                            data.rc.top = (int)workArea.Y - (int)Math.Round(actualSize.Y);
                        }
                    }
                    //以上生成的是四周都没有其他AppBar时的理想位置
                    //系统将自动调整位置以适应其他AppBar
                    Debug.WriteLine("Before QueryPos: Left: {0}, Top: {1}, Right: {2}, Bottom: {3}",data.rc.left,data.rc.top,data.rc.right,
                        data.rc.bottom);
                    Interop.SHAppBarMessage((int)Interop.AppBarMsg.AbmQuerypos,ref data);
                    Debug.WriteLine("After QueryPos: Left: {0}, Top: {1}, Right: {2}, Bottom: {3}",data.rc.left,data.rc.top,data.rc.right,
                        data.rc.bottom);
                    //自定义对齐方式，确保Height和Width不会小于0
                    if (data.rc.bottom - data.rc.top < 0) {
                        if (Location == AppBarLocation.Top)
                            data.rc.bottom = data.rc.top + (int)Math.Round(actualSize.Y); //上对齐
                        else if (Location == AppBarLocation.Bottom)
                            data.rc.top = data.rc.bottom - (int)Math.Round(actualSize.Y); //下对齐
                    }
                    if (data.rc.right - data.rc.left < 0) {
                        if (Location == AppBarLocation.Left)
                            data.rc.right = data.rc.left + (int)Math.Round(actualSize.X); //左对齐
                        else if (Location == AppBarLocation.Right)
                            data.rc.left = data.rc.right - (int)Math.Round(actualSize.X); //右对齐
                    }
                    //调整完毕，设置为最终位置
                    Interop.SHAppBarMessage((int)Interop.AppBarMsg.AbmSetpos,ref data);
                    //应用到窗口
                    Point location = toWpfUnit.Transform(new Point(data.rc.left,data.rc.top));
                    Vector dimension = toWpfUnit.Transform(new Vector(data.rc.right - data.rc.left,
                        data.rc.bottom - data.rc.top));
                    Rect rect = new Rect(location,new Size(dimension.X,dimension.Y));

                    _window.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle,() => {
                        _window.Left = rect.Left;
                        _window.Top = rect.Top;
                        _window.Width = rect.Width;
                        _window.Height = rect.Height;
                    });
                }

                if (_window != null) {
                    Debug.WriteLine("Set {0} Left: {1} ,Top: {2}, Width: {3}, Height: {4}",_window.Title,_window.Left,_window.Top,
                        _window.Width,_window.Height);
                }
            }
            public void DisableAppBar() {
                if (!_isRegistered) return;
                _isRegistered = false;
                Interop.Appbardata data = new Interop.Appbardata();
                data.cbSize = Marshal.SizeOf(data);
                data.hWnd = _hWnd;
                data.uCallbackMessage = _callbackId;
                Interop.SHAppBarMessage((int)Interop.AppBarMsg.AbmRemove,ref data);
                _isRegistered = false;
                RestoreWindowInfo();
                Debug.WriteLine(_window?.Title + " DisableAppBar");
            }
        }

        public enum AppBarLocation {
            Left = 0,
            Top,
            Right,
            Bottom,
            None,
            RegisterOnly = 99
        }

        internal static class Interop {
            [StructLayout(LayoutKind.Sequential)]
            internal struct Rect {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [StructLayout(LayoutKind.Sequential)]
            internal struct Appbardata {
                public int cbSize;
                public IntPtr hWnd;
                public int uCallbackMessage;
                public int uEdge;
                public Rect rc;
                public IntPtr lParam;
            }
            internal enum AppBarMsg {
                AbmNew = 0,
                AbmRemove,
                AbmQuerypos,
                AbmSetpos,
                AbmGetstate,
                AbmGettaskbarpos,
                AbmActivate,
                AbmGetautohidebar,
                AbmSetautohidebar,
                AbmWindowposchanged,
                AbmSetstate
            }
            internal enum AppBarNotify {
                AbnStatechange = 0,
                AbnPoschanged,
                AbnFullscreenapp,
                AbnWindowarrange
            }
            [DllImport("SHELL32",CallingConvention = CallingConvention.StdCall)]
            internal static extern uint SHAppBarMessage(int dwMessage,ref Appbardata pData);
            [DllImport("User32.dll", CharSet = CharSet.Unicode)]
            internal static extern int RegisterWindowMessage(string msg);
        }
    }
    // ReSharper restore IdentifierTypo
    // ReSharper restore StringLiteralTypo
}