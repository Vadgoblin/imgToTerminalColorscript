using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace imgToTerminalColorscript
{
    internal class Program
    {
        static private readonly byte alphathreshold = 2;
        static void Main(string[] args)
        {
            int[,][] rawimg;

            using (var img = (Bitmap)Bitmap.FromFile(args[0]))
            {
                //img.Save("test", System.Drawing.Imaging.ImageFormat.Tiff);

                rawimg = new int[img.Width+1, img.Height % 2 == 0 ? img.Height : img.Height + 1][];
                for (int i = 0; i < rawimg.GetLength(0); i++) for (int j = 0; j < rawimg.GetLength(1); j++) rawimg[i, j] = new int[4];
                for (int x = 0; x < img.Width; x++)
                {
                    for (int y = 0; y < img.Height; y++)
                    {
                        rawimg[x, y][0] = img.GetPixel(x, y).R;
                        rawimg[x, y][1] = img.GetPixel(x, y).G;
                        rawimg[x, y][2] = img.GetPixel(x, y).B;
                        rawimg[x, y][3] = img.GetPixel(x, y).A;
                    }
                }
            }

            for(int y = 0;y<rawimg.GetLength(1);y+=2)
            {
                var s = "";
                for (int x = 0; x < rawimg.GetLength(0); x++) 
                    s+=Magic(rawimg[x, y], rawimg[x, y + 1]);

                int lastnonspaceidex = s.Length-1;
                for (int i = s.Length - 1; i >= 0; i--)
                {
                    if (s[i] == ' ') lastnonspaceidex = i;
                    else break;
                }
                Console.WriteLine(s.Substring(0,lastnonspaceidex)+ (y== rawimg.GetLength(1)-2?resetall:""));
            }
        }

        static int[] lastForegroundColor = new int[] { -1, -1, -1 };
        static int[] lastBackgroundColor = new int[] { 0, 0, 0 };
        //static bool isForegroundDefaultColor = true;
        static bool isBackgroundColorDefault = true;

        static string Magic(int[] a, int[] b)
        {
            if (a[3] < alphathreshold && b[3] < alphathreshold) //mind a 2 atlatszo
            {
                if (isBackgroundColorDefault) return " ";
                else
                {
                    return ResetBackgroundColor();
                }
            }
            if (a[3] < alphathreshold && !(b[3] < alphathreshold)) //felso atlatszo csak
            {
                if (isBackgroundColorDefault)
                {
                    if (lastColorCheck(lastForegroundColor, b)) return "▄";
                    else return SetForeORBackground(true, b)+ "▄";
                }
                return ResetBackgroundColorAndSetForeground(b) + "▄";
            }
            if (!(a[3] < alphathreshold) && b[3] < alphathreshold) //also atlatszo
            {
                if (isBackgroundColorDefault)
                {
                    if (lastColorCheck(lastForegroundColor, b)) return "▀";
                    else return SetForeORBackground(true, b) + "▀";
                }
                return ResetBackgroundColorAndSetForeground(b) + "▀";
            }

            else //egyik sem atlatszo
            {
                //megegyezik a ket szin
                if (lastColorCheck(a, b))
                {
                    if (!isBackgroundColorDefault && lastColorCheck(lastBackgroundColor, a)) return " ";
                    else if (lastColorCheck(lastForegroundColor, a)) return "█";
                    else return SetForeORBackground(false, a) + " ";
                }
                else//a ket szin kulonbozo
                {
                    //legutobbi szinek stimmelnek
                    if (!isBackgroundColorDefault && lastColorCheck(lastForegroundColor, a) && lastColorCheck(lastBackgroundColor, b)) return "▀";
                    else if (!isBackgroundColorDefault && lastColorCheck(lastForegroundColor, b) && lastColorCheck(lastBackgroundColor, a)) return "▄";

                    //csak az egyik legutobbi stimmel
                    //if (lastColorCheck(lastBackgroundColor, a)) return SetForeORBackground(true, b) + "▄";
                    //else if (lastColorCheck(lastBackgroundColor, b)) return SetForeORBackground(true, a) + "▀";
                    //else if (lastColorCheck(lastForegroundColor,a)) return SetForeORBackground(false,b) + "▀";
                    //else if(lastColorCheck(lastForegroundColor,b)) return SetForeORBackground(false, a) + "▄";

                    //egyik sem stimmel
                    else return SetForegroundAndBackground(a, b) + "▀";
                }
            }
        }

        static string SetForeORBackground(bool foreground, int[] color)
        {
            if (foreground)
            {
                lastForegroundColor = color;
            }
            else
            {
                lastBackgroundColor = color;
                isBackgroundColorDefault = false;
            }

            return (foreground ? @"\033[38;2;" : @"\033[48;2;") + color[0] + ";" + color[1] + ";" + color[2] + "m";
        }

        static string SetForegroundAndBackground(int[] foreground, int[] background)
        {
            isBackgroundColorDefault = false;
            lastBackgroundColor = background;
            lastForegroundColor = foreground;
            //return SetForeORBackground(false, background) + SetForeORBackground(true, foreground);
            return @"\033[38;2;" + foreground[0] + ";" + foreground[1] + ";" + foreground[2] + ";48;2;" + background[0] + ";" + background[1] + ";" + background[2] +"m";
        }

        static string ResetBackgroundColor()
        {
            isBackgroundColorDefault = true;
            return @"\033[49m";
        }
        
        static string ResetBackgroundColorAndSetForeground(int[] foregroundcolor)
        {
            //isBackgroundColorDefault = true;
            //lastForegroundColor = foregroundcolor;

            //return @"\033[49;38;2;" + foregroundcolor[0] + ";" + foregroundcolor[1] + ";" + foregroundcolor[2] + ";m";
            return ResetBackgroundColor()+SetForeORBackground(true,foregroundcolor);
        }

        static bool lastColorCheck(int[] a, int[] b)
        {
            return a[0] == b[0] && a[1] == b[1] && a[2] == b[2];
        }

        static readonly string resetall = @"\033[49m\033[39m";

        static void aSetLastColor(bool fore, byte[] c)
        {
            if(fore)
            {
                lastForegroundColor[0] = c[0];
                lastForegroundColor[1] = c[1];
                lastForegroundColor[2] = c[2];
            }
            else
            {
                lastBackgroundColor[0] = c[0];
                lastBackgroundColor[1] = c[1];
                lastBackgroundColor[2] = c[2];
            }
        }
    }
}
