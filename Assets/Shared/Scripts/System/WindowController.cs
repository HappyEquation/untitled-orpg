using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;

/// <summary>
/// Allow a standalone windows application to run without border at a specific screen position or in fullscreen.
/// You can also use the command argument -popupwindow in order to do the same thing with less control.
/// This allows you to run other program over your unity application (as by example a virtual keyboard).
/// </summary>
[ExecuteInEditMode]
public class WindowController : MonoBehaviour
{
    public Rect ScreenPosition;
    public bool IsFullscreen = false;

    Rect GetFullscreenResolution()
    {
        Resolution resolution = Screen.currentResolution;
        return new Rect(0f, 0f, (float)resolution.width, (float)resolution.height);
    }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
    private const int SW_SHOWNORMAL = 1;
    private const int SW_SHOWMINIMIZED = 2;
    private const int SW_SHOWMAXIMIZED = 3;

    const uint SWP_SHOWWINDOW = 0x0040;
    const int GWL_STYLE = -16;
    const int WS_BORDER = 1;

    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    public static extern IntPtr FindWindow(System.String className, System.String windowName);
    [DllImport("user32.dll", EntryPoint = "ShowWindowAsync")]
    private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    public void SetPosition(int x, int y, int resX = 0, int resY = 0)
    {
        IntPtr wnd = FindWindow(null, "PrestigeOnline");
        if (IsFullscreen)
            ScreenPosition = GetFullscreenResolution();

        SetWindowLong(GetForegroundWindow(), GWL_STYLE, WS_BORDER);
        bool result = SetWindowPos(GetForegroundWindow(), 0, (int)ScreenPosition.x, (int)ScreenPosition.y, (int)ScreenPosition.width, (int)ScreenPosition.height, SWP_SHOWWINDOW);

        SetWindowPos(wnd, 0, x, y, resX, resY, resX * resY == 0 ? 1 : 0);
        ShowWindowAsync(wnd, SW_SHOWMAXIMIZED);
    }

#endif

#if UNITY_EDITOR
    
    void Update()
    {
        if (IsFullscreen)
            ScreenPosition = GetFullscreenResolution();
    }
#else
    void Update()
    {
        if(Input.GetKeyDown("escape")) 
        {
            //When a key is pressed down it see if it was the escape key if it was it will execute the code
            Application.Quit(); // Quits the game
        }

    }

    void Start()
    {
        if (IsFullscreen) ScreenPosition = GetFullscreenResolution();

    }

    // Use this for initialization
    void Awake()
    {
        SetPosition(0, 0);
    }
#endif


}