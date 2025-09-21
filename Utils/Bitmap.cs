using System;
using System.IO;

internal class Bitmap
{
    public readonly int Width, Height;
    public readonly byte[] Data;


    public Bitmap(int width, int height)
    {
        Width = width;
        Height = height;
        Data = new byte[Width * Height * 4];
    }

    public Bitmap(int width, int height, byte[] data) : this(width, height)
    {
        Data = data;
    }

    public byte[] GetBytes()
    {
        int headerSize = 14 + 124;
        byte[] bytes = new byte[headerSize + Data.Length];
        bytes[0] = (byte)'B';
        bytes[1] = (byte)'M';
        bytes[14] = 40;
        Array.Copy(BitConverter.GetBytes(bytes.Length), 0, bytes, 2, 4);
        Array.Copy(BitConverter.GetBytes(0), 0, bytes, 6, 4);
        Array.Copy(BitConverter.GetBytes(headerSize), 0, bytes, 10, 4);

        Array.Copy(BitConverter.GetBytes(124), 0, bytes, 14, 4);
        Array.Copy(BitConverter.GetBytes(Width), 0, bytes, 18, 4);
        Array.Copy(BitConverter.GetBytes(-Height), 0, bytes, 22, 4);
        Array.Copy(BitConverter.GetBytes(1), 0, bytes, 26, 2);
        Array.Copy(BitConverter.GetBytes(32), 0, bytes, 28, 2);
        Array.Copy(BitConverter.GetBytes(3), 0, bytes, 30, 4);

        Array.Copy(BitConverter.GetBytes(Data.Length), 0, bytes, 34, 4);
        Array.Copy(BitConverter.GetBytes(2835), 0, bytes, 38, 4);       
        Array.Copy(BitConverter.GetBytes(2835), 0, bytes, 42, 4);       
        Array.Copy(BitConverter.GetBytes(0), 0, bytes, 46, 4);          
        Array.Copy(BitConverter.GetBytes(0), 0, bytes, 50, 4); 

        Array.Copy(BitConverter.GetBytes(0x00FF0000), 0, bytes, 54, 4);
        Array.Copy(BitConverter.GetBytes(0x0000FF00), 0, bytes, 58, 4);
        Array.Copy(BitConverter.GetBytes(0x000000FF), 0, bytes, 62, 4);
        Array.Copy(BitConverter.GetBytes(unchecked((int)0xFF000000)), 0, bytes, 66, 4);

        Array.Copy(Data, 0, bytes, headerSize, Data.Length);
        return bytes;
    }
}