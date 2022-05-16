using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
public static class Vibrator
{
#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    public static AndroidJavaObject CurrentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    public static AndroidJavaObject VibratorObject = CurrentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#endif

    public static void Vibrate(long milliseconds)
    {


#if UNITY_ANDROID && !UNITY_EDITOR
 string vibrateRequest = "android.permission.VIBRATE";
        if (Permission.HasUserAuthorizedPermission(vibrateRequest))
        {
        VibratorObject.Call("vibrate", milliseconds);
        }
        Permission.RequestUserPermission(vibrateRequest);
#elif UNITY_IOS
        //Handheld.Vibrate();
#endif

    }

    public static void Cancel()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        VibratorObject.Call("cancel");
#endif
    }
}
