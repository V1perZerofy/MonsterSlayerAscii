using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Runtime.InteropServices;

namespace MonsterSlayerAscii
{
    internal static class ConsoleMouseSupport
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadConsoleInput(IntPtr hConsoleInput, [Out] INPUT_RECORD[] lpBuffer, uint nLength, out uint lpNumberOfEventsRead);

        private const int STD_INPUT_HANDLE = -10;
        private const uint ENABLE_MOUSE_INPUT = 0x0010;
        private const uint ENABLE_EXTENDED_FLAGS = 0x0080;
        private const uint ENABLE_PROCESSED_INPUT = 0x0001;
        private const uint ENABLE_QUICK_EDIT_MODE = 0x0040;

        private const ushort MOUSE_EVENT = 0x0002;

        [StructLayout(LayoutKind.Explicit)]
        private struct INPUT_RECORD
        {
            [FieldOffset(0)] public ushort EventType;
            [FieldOffset(4)] public MOUSE_EVENT_RECORD MouseEvent;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSE_EVENT_RECORD
        {
            public COORD dwMousePosition;
            public uint dwButtonState;
            public uint dwControlKeyState;
            public uint dwEventFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct COORD
        {
            public short X;
            public short Y;
        }

        public static void EnableMouseSupport()
        {
            IntPtr handleIn = GetStdHandle(STD_INPUT_HANDLE);

            GetConsoleMode(handleIn, out uint mode);
            mode &= ~ENABLE_QUICK_EDIT_MODE;
            mode |= ENABLE_MOUSE_INPUT | ENABLE_EXTENDED_FLAGS | ENABLE_PROCESSED_INPUT;
            SetConsoleMode(handleIn, mode);
        }

        public static (int x, int y, bool clicked) WaitForMouseClick()
        {
            IntPtr handleIn = GetStdHandle(STD_INPUT_HANDLE);
            INPUT_RECORD[] record = new INPUT_RECORD[1];

            while (true)
            {
                ReadConsoleInput(handleIn, record, 1, out _);

                if (record[0].EventType == MOUSE_EVENT)
                {
                    if (record[0].MouseEvent.dwButtonState == 0x0001) // Left click
                    {
                        var pos = record[0].MouseEvent.dwMousePosition;
                        return (pos.X, pos.Y, true);
                    }
                }
            }
        }
    }
}