//#define DISABLE_UNITY_DEBUG_OUTPUT

using UnityEngine;

public static class DebugSettings {
	public static bool IsDebugEnabled {
		get {
#if DISABLE_UNITY_DEBUG_OUTPUT
			return false;
#else
			return true;
#endif
		}
	}

	public static bool ShowOnlyAnalytics {
		get {
			return false;
		}
	}
}

#if DISABLE_UNITY_DEBUG_OUTPUT
public class Debug
{
	public static bool isDebugBuild
	{
		get
		{
			return false;
        }
	}

	public static void Log(object message)
	{
	}

	public static void Log(object message, Object context)
	{
	}

	public static void LogError(object message)
	{
	}

	public static void LogError(object message, Object context)
	{
	}

	public static void LogWarning(object message)
	{
	}

	public static void LogWarning(object message, Object context)
	{
	}

	public static void LogException(System.Exception e)
	{
	}

	public static void LogException(System.Exception e, Object context)
	{
	}

	public static void Break()
	{
	}
}
#endif