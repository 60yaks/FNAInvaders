namespace System;

internal static class ObjectExtensions
{
    public static T CheckArgumentNullException<T>(this T @object, string paramName) => @object ?? throw new ArgumentNullException(paramName);
}
