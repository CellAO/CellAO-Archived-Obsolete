using System;
using System.Diagnostics;

namespace AO.Core
{
    /// <summary>
    /// Debughelpers
    /// </summary>
    public class Debughelpers
    {
        /// <summary>
        /// Convert byte[] to readable Hex dump
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static string PacketToHex(byte[] packet)
        {
            string line = "";
            string ascii = "";
            string output = "";

            foreach (byte b in packet)
            {
                line = line + b.ToString("X2") + " ";
                if ((b < 32) || (b == 127))
                {
                    ascii += ".";
                }
                else
                {
                    ascii += (char) b;
                }
                if (line.Length == 16*3)
                {
                    output += line + ascii + "\r\n";
                    line = "";
                    ascii = "";
                }
            }
            if (line != "")
            {
                output += line.PadRight(16*3) + ascii + "\r\n";
            }
            return output;
        }

        #region Show stack trace
        /// <summary>
        /// Print the stack trace
        /// </summary>
        public static void ShowStackTrace()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Stacktrace:");
            StackTrace st = new StackTrace();
            StackFrame[] sf = st.GetFrames();
            foreach (StackFrame ssf in sf)
            {
                if ((ssf.GetMethod().Name != "ShowStackTrace") &&
                    (ssf.GetMethod().DeclaringType.Namespace.Substring(0, 6) != "System"))
                    Console.WriteLine(ssf.GetMethod().DeclaringType.Namespace + "  " + ssf.GetMethod().Name);
            }
            Console.ResetColor();
            Console.WriteLine("Trace END ---------------------------------------------------------------------");
        }
        #endregion
    }
}