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
        static private readonly byte alphathreshold = 128;
        static void Main(string[] args)
        {
            byte[,][] rawimg;

            using (var img = (Bitmap)Bitmap.FromFile("001.png"))
            {
                rawimg = new byte[img.Width % 2 == 0 ? img.Width : img.Width + 1, img.Height % 2 == 0 ? img.Height : img.Height + 1][];
                for (int i = 0; i < rawimg.GetLength(0); i++) for (int j = 0; j < rawimg.GetLength(1); j++) rawimg[i, j] = new byte[4];
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
                for (int x = 0; x < rawimg.GetLength(1); x++) Console.Write(Magic(rawimg[x, y], rawimg[x, y + 1]));
                Console.WriteLine();
            }
        }

        static byte[] lastForegroundColor = new byte[] { 0, 0, 0 };
        static byte[] lastBackgroundColor = new byte[] { 0, 0, 0 };
        static bool isForegroundDefaultColor = true;
        static bool isBackgroundColorDefault = true;

        static string Magic(byte[] a, byte[] b)
        {
            if (a[3] < alphathreshold && b[3] < alphathreshold) //mind a 2 atlatszo
            {
                if (isBackgroundColorDefault) return " ";
                //else if (isForegroundDefaultColor) return "█";
                else
                {
                    isBackgroundColorDefault = true;
                    return @"\033[49m ";
                }
            }
            if (a[3] < alphathreshold && !(b[3] < alphathreshold)) //felso atlatszo csak
            {
                if (isBackgroundColorDefault)
                {
                    isForegroundDefaultColor = false;
                    //lastForegroundColor = b;
                    SetLastColor(true, b);
                    return GetColorEscape(true, b) + "▄";
                }
                //else if (isForegroundDefaultColor)
                //{
                //    isBackgroundColorDefault = false;
                //    //lastBackgroundColor = b;
                //    SetLastColor(false, b);
                //    return GetColorEscape(false, b) + "▀";
                //}
                //else if (lastForegroundColor == b)
                //{
                //    isBackgroundColorDefault = true;
                //    return @"\033[49m▄";
                //}
                else if (lastBackgroundColor == b)
                {
                    isForegroundDefaultColor = true;
                    return @"\033[39m▀";
                }
                else
                {
                    isBackgroundColorDefault = true;
                    isForegroundDefaultColor = false;
                    //lastForegroundColor = b;
                    SetLastColor(true, b);
                    return @"\033[49m" + GetColorEscape(true, b) + "▄";
                }
            }
            if (!(a[3] < alphathreshold) && b[3] < alphathreshold) //also atlatszo
            {
                if (isBackgroundColorDefault)
                {
                    isForegroundDefaultColor = false;
                    //lastForegroundColor = a;
                    SetLastColor(true, a);
                    return GetColorEscape(true, a) + "▀";
                }
                //else if (isForegroundDefaultColor)
                //{
                //    isBackgroundColorDefault = false;
                //    //lastBackgroundColor = a;
                //    SetLastColor(false, a);
                //    return GetColorEscape(false, a) + "▄";
                //}
                //else if (lastForegroundColor == a)
                //{
                //    isBackgroundColorDefault = true;
                //    return @"\033[49m▀";
                //}
                else if (lastBackgroundColor == a)
                {
                    isForegroundDefaultColor = true;
                    return @"\033[39m▄";
                }
                else
                {
                    isBackgroundColorDefault = true;
                    isForegroundDefaultColor = false;
                    //lastForegroundColor = a;
                    SetLastColor(true, a);
                    return @"\033[49m" + GetColorEscape(true, a) + "▀";
                }
            }
            else //mind a 2 nem atlatszo
            {
                if (a[0] == b[0] && a[1] == b[1] && a[2] == b[2])//mind a 2 egyforma szinu
                {
                    if (!isBackgroundColorDefault && lastBackgroundColor[0] == a[0] && lastBackgroundColor[1] == a[1] && lastBackgroundColor[2] == a[2]) return " ";
                    if (!isForegroundDefaultColor && lastForegroundColor[0] == a[0] && lastForegroundColor[1] == a[1] && lastForegroundColor[2] == a[2]) return "█";
                    else
                    {
                        isBackgroundColorDefault = false;
                        SetLastColor(false, a);
                        return GetColorEscape(false, a) + " ";
                    }
                }
                else //nem egyforma szinuek
                {
                    //mind 2 szin stimmel
                    {
                        if (!isBackgroundColorDefault && !isForegroundDefaultColor &&
                        lastBackgroundColor[0] == a[0] && lastBackgroundColor[1] == a[1] && lastBackgroundColor[2] == a[2] &&
                        lastForegroundColor[0] == b[0] && lastForegroundColor[1] == b[1] && lastForegroundColor[2] == b[2]) return "▄";

                        else if (!isBackgroundColorDefault && !isForegroundDefaultColor &&
                            lastBackgroundColor[0] == b[0] && lastBackgroundColor[1] == b[1] && lastBackgroundColor[2] == b[2] &&
                            lastForegroundColor[0] == a[0] && lastForegroundColor[1] == a[1] && lastForegroundColor[2] == a[2]) return "▀";
                    }

                    //csak 1 szin stimmel
                    {
                        //foreground stimmel
                        {
                            if (!isForegroundDefaultColor)
                            {
                                if (lastForegroundColor[0] == a[0] && lastForegroundColor[1] == a[1] && lastForegroundColor[2] == a[2])
                                {
                                    isBackgroundColorDefault = false;
                                    //lastBackgroundColor = b;
                                    SetLastColor(false, b);
                                    return GetColorEscape(false, b) + "▀";
                                }
                                else if (lastForegroundColor[0] == b[0] && lastForegroundColor[1] == b[1] && lastForegroundColor[2] == b[2])
                                {
                                    isBackgroundColorDefault = false;
                                    //lastBackgroundColor = a;
                                    SetLastColor(false, a);
                                    return GetColorEscape(false, a) + "▄";
                                }
                            }
                        }

                        //background stimmel
                        {
                            if (!isBackgroundColorDefault)
                            {
                                if (lastBackgroundColor[0] == a[0] && lastBackgroundColor[1] == a[1] && lastBackgroundColor[2] == a[2])
                                {
                                    isForegroundDefaultColor = false;
                                    //lastForegroundColor = b;
                                    SetLastColor(true, b);
                                    return GetColorEscape(false, b) + "▄";
                                }
                                else if (lastBackgroundColor[0] == b[0] && lastBackgroundColor[1] == b[1] && lastBackgroundColor[2] == b[2])
                                {
                                    isForegroundDefaultColor = false;
                                    //lastForegroundColor = a;
                                    SetLastColor(true, a);
                                    return GetColorEscape(false, a) + "▀";
                                }
                            }
                        }
                    }

                }
            }

            return " ";
        }

        static string GetColorEscape(bool foreground, byte[] color)
        {
            return (foreground ? @"\033[38;2;" : @"\033[48;2;") + color[0] + ";" + color[1] + ";" + color[2] + "m";
        }
        static void SetLastColor(bool fore, byte[] c)
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
