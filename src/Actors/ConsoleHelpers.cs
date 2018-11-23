using System;

namespace Actors
{
    public static class ConsoleHelpers
    {
        public static void PrintAtLocation(int left, int top, string text)
        {
            int currentLeft = Console.CursorLeft;
            int currentTop = Console.CursorTop;

            Console.SetCursorPosition(left, top);
            Console.Write(text);
            Console.SetCursorPosition(currentLeft, currentTop);
        } 
    }
}