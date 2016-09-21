
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Resources;
using System.IO;

public class Sound
{
    [DllImport("winmm.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
    public static extern int PlaySound(string name, int hmod, int flags);
    [DllImport("winmm.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
    public static extern int PlaySound(byte[] name, int hmod, int flags);

    // play synchronously 
    public const int SND_SYNC = 0x0;
    // play asynchronously 
    public const int SND_ASYNC = 0x1;
    //Play wav in memory
    public const int SND_MEMORY = 0x4;
    //Play system alias wav 
    public const int SND_ALIAS = 0x10000;
    public const int SND_NODEFAULT = 0x2;
    // name is file name 
    public const int SND_FILENAME = 0x20000;
    // name is resource name or atom 
    public const int SND_RESOURCE = 0x40004;

    public static void PlayWaveFile(string fileWaveFullPath)
    {
        try
        {
            PlaySound(fileWaveFullPath, 0, SND_FILENAME);
        }
        catch
        {
        }
    }

    public static void PlayWaveResource(string WaveResourceName)
    {
        // get the namespace 
        string strNameSpace = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.ToString();

     //   strNameSpace = "PathStation";

        // get the resource into a stream
        Stream resourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(strNameSpace + ".Resources." + WaveResourceName);
        if (resourceStream == null)
            return;

        // bring stream into a byte array
        byte[] wavData = null;
        wavData = new byte[Convert.ToInt32(resourceStream.Length) + 1];
        resourceStream.Read(wavData, 0, Convert.ToInt32(resourceStream.Length));
        resourceStream.Close();

        // play the resource
        PlaySound(wavData, 0, SND_ASYNC | SND_MEMORY);
    }




}

