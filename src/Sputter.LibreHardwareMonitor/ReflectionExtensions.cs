﻿using System.Reflection;

namespace Sputter.LibreHardwareMonitor;

public static class ReflectionExtensions {
	public static T? GetFieldValue<T>(this object obj, string name) {
		// Set the flags so that private and public fields from instances will be found
		var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		var field = obj.GetType().GetField(name, bindingFlags);
		return (T?)field?.GetValue(obj);
	}

	public static T? GetPropertyValue<T>(this object obj, string name) {
		// Set the flags so that private and public fields from instances will be found
		var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		var field = obj.GetType().GetProperty(name, bindingFlags);
		return (T?)field?.GetValue(obj);
	}
}
