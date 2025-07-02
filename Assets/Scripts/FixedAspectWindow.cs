using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class FixedAspectWindow : MonoBehaviour
{
#if UNITY_STANDALONE_WIN
    const int GWL_WNDPROC = -4;
    const int WM_SIZING = 0x0214;

    const int WMSZ_LEFT = 1;
    const int WMSZ_RIGHT = 2;
    const int WMSZ_TOP = 3;
    const int WMSZ_TOPLEFT = 4;
    const int WMSZ_TOPRIGHT = 5;
    const int WMSZ_BOTTOM = 6;
    const int WMSZ_BOTTOMLEFT = 7;
    const int WMSZ_BOTTOMRIGHT = 8;

    private delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    private static WndProcDelegate newWndProc = CustomWndProc;
    private static IntPtr oldWndProc = IntPtr.Zero;
    private static IntPtr hWnd = IntPtr.Zero;

    [DllImport("user32.dll")]
    static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, WndProcDelegate newProc);

    [DllImport("user32.dll")]
    static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    void Start()
    {
        hWnd = GetActiveWindow();
        oldWndProc = SetWindowLongPtr(hWnd, GWL_WNDPROC, newWndProc);
    }

    static IntPtr CustomWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        if (msg == WM_SIZING)
        {
            Rect rect = Marshal.PtrToStructure<Rect>(lParam);
            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            float targetAspect = 16f / 9f;

            switch (wParam.ToInt32())
            {
                case WMSZ_LEFT:
                case WMSZ_RIGHT:
                case WMSZ_TOP:
                case WMSZ_BOTTOM:
                case WMSZ_TOPLEFT:
                case WMSZ_TOPRIGHT:
                case WMSZ_BOTTOMLEFT:
                case WMSZ_BOTTOMRIGHT:
                    height = Mathf.RoundToInt(width / targetAspect);
                    rect.bottom = rect.top + height;
                    Marshal.StructureToPtr(rect, lParam, true);
                    break;
            }
        }

        return CallWindowProc(oldWndProc, hWnd, msg, wParam, lParam);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }
#endif
}

