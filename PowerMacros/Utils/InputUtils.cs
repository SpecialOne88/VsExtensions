using PowerMacros.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PowerMacros.Utils
{
    public class MacroKeyboardHook
    {
        private IntPtr _hookId = IntPtr.Zero;

        public event EventHandler<MacroKeyEventArgs> KeyDown;
        public event EventHandler<MacroKeyEventArgs> KeyUp;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelKeyboardProc _proc;

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;

        public void Install()
        {
            _proc = HookCallback;
            _hookId = SetHook(_proc);
        }

        public void Uninstall()
        {
            if (_hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookId);
                _hookId = IntPtr.Zero;
            }
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(null), 0);
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys keyCode = (Keys)vkCode;
                Keys modifiers = GetCurrentModifiers();

                if (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
                {
                    KeyDown?.Invoke(this, new MacroKeyEventArgs(keyCode, modifiers));
                }
                else if (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)
                {
                    KeyUp?.Invoke(this, new MacroKeyEventArgs(keyCode, modifiers));
                }
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private Keys GetCurrentModifiers()
        {
            Keys modifiers = Keys.None;

            if ((GetKeyState((int)Keys.ShiftKey) & 0x8000) != 0)
            {
                modifiers |= Keys.Shift;
            }

            if ((GetKeyState((int)Keys.ControlKey) & 0x8000) != 0)
            {
                modifiers |= Keys.Control;
            }

            if ((GetKeyState((int)Keys.Menu) & 0x8000) != 0)
            {
                modifiers |= Keys.Alt;
            }

            return modifiers;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern short GetKeyState(int keyCode);
    }

    public class InputSimulator
    {
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;

        public void SimulateKeyDown(Keys key, Keys modifiers)
        {
            // Press modifier keys first
            if ((modifiers & Keys.Control) == Keys.Control)
            {
                keybd_event((byte)Keys.ControlKey, 0, 0, UIntPtr.Zero);
            }

            if ((modifiers & Keys.Shift) == Keys.Shift)
            {
                keybd_event((byte)Keys.ShiftKey, 0, 0, UIntPtr.Zero);
            }

            if ((modifiers & Keys.Alt) == Keys.Alt)
            {
                keybd_event((byte)Keys.Menu, 0, 0, UIntPtr.Zero);
            }

            // Press the actual key
            byte virtualKeyCode = (byte)key;
            keybd_event(virtualKeyCode, 0, IsExtendedKey(key) ? KEYEVENTF_EXTENDEDKEY : 0, UIntPtr.Zero);
        }

        public void SimulateKeyUp(Keys key, Keys modifiers)
        {
            // Release the actual key
            byte virtualKeyCode = (byte)key;
            keybd_event(virtualKeyCode, 0, KEYEVENTF_KEYUP | (IsExtendedKey(key) ? KEYEVENTF_EXTENDEDKEY : 0), UIntPtr.Zero);

            // Release modifier keys last (reverse order)
            if ((modifiers & Keys.Alt) == Keys.Alt)
            {
                keybd_event((byte)Keys.Menu, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            }

            if ((modifiers & Keys.Shift) == Keys.Shift)
            {
                keybd_event((byte)Keys.ShiftKey, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            }

            if ((modifiers & Keys.Control) == Keys.Control)
            {
                keybd_event((byte)Keys.ControlKey, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            }
        }

        private bool IsExtendedKey(Keys key)
        {
            // Extended keys are keys that require the extended key flag in keybd_event
            return key == Keys.Insert || key == Keys.Delete || key == Keys.Home || key == Keys.End ||
                   key == Keys.PageUp || key == Keys.PageDown || key == Keys.Up || key == Keys.Down ||
                   key == Keys.Left || key == Keys.Right || key == Keys.NumLock;
        }

        public static async Task PlayRecordedMacro(List<MacroAction> actions)
        {
            var inputSimulator = new InputSimulator();

            foreach (var action in actions)
            {
                await Task.Delay((int)action.Delay);
                if (action.Type == MacroActionType.KeyDown)
                {
                    inputSimulator.SimulateKeyDown(action.KeyCode, action.Modifiers);
                }
                else if (action.Type == MacroActionType.KeyUp)
                {
                    inputSimulator.SimulateKeyUp(action.KeyCode, action.Modifiers);
                }
            }
        }
    }

    public class MacroKeyEventArgs : EventArgs
    {
        public Keys KeyCode { get; }
        public Keys Modifiers { get; }

        public MacroKeyEventArgs(Keys keyCode, Keys modifiers)
        {
            KeyCode = keyCode;
            Modifiers = modifiers;
        }
    }
}
