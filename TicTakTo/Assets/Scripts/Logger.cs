using UnityEngine;
using System;
using System.Diagnostics;

using Debug = UnityEngine.Debug;

public static class Logger
{
    [Conditional("DEV_VER")]
    //입력 : 메세지
    public static void Info(string message)
    {
        Debug.LogFormat("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), message);
    }

    [Conditional("DEV_VER")]
    public static void Warning(string message) 
    {
        Debug.LogWarningFormat("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), message);
    }

    // 유저에게 배포할때는 남겨두어야한다.
    public static void Error(string message)
    {
        Debug.LogErrorFormat("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), message);
    }

}
