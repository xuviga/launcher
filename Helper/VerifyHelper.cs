using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace iMine.Launcher.Helper
{
    public static class VerifyHelper
    {
        public static Predicate<int> Positive = i => i > 0;
        public static Predicate<int> NotNegative = i => i >= 0;
        public static Predicate<long> LPositive = l => l > 0;
        public static Predicate<long> LNotNegative = l => l >= 0;
        public static Predicate<string> NotEmpty = s => s.Length > 0;
        public static Regex UsernamePattern = new Regex("[a-zA-Zа-яА-Я0-9_.\\-]{1,16}");

        public static TV GetMapValue<TK, TV>(Dictionary<TK, TV> map, TK key, string error)
        {
            if (!map.ContainsKey(key))
                throw new ArgumentException(error);
            return Verify(map[key], it => it != null, error);
        }

        public static bool IsValidIdName(string name)
        {
            return name.Length > 0 && name.Length <= 255 &&
                   name.ToCharArray().ToList().TrueForAll(it => IsValidIdNameChar(it));
        }

        public static bool IsValidIdNameChar(int ch)
        {
            return ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch >= '0' && ch <= '9' || ch == '-' || ch == '_';
        }

        public static bool IsValidUsername(string username)
        {
            return UsernamePattern.IsMatch(username);
        }

        public static void PutIfAbsent<TK, TV>(Dictionary<TK, TV> map, TK key, TV value, string error)
        {
            if (map.ContainsKey(key) || value == null)
                throw new ArgumentException(error);
            map[key] = value;
        }

        public static T Verify<T>(T obj, Predicate<T> predicate, string error)
        {
            if (predicate.Invoke(obj))
                return obj;
            throw new ArgumentException(error);
        }

        public static double VerifyDouble(double d, Predicate<double> predicate, string error)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));
            if (predicate.Invoke(d))
                return d;
            throw new ArgumentException(error);
        }

        public static string VerifyIdName(string name)
        {
            return Verify(name, IsValidIdName, "Invalid name: '" + name + "'");

        }

        public static int VerifyInt(int i, Predicate<int> predicate, string error)
        {
            if (predicate.Invoke(i))
                return i;
            throw new ArgumentException(error);
        }

        public static long VerifyLong(long l, Predicate<long> predicate, string error)
        {
            if (predicate.Invoke(l))
                return l;
            throw new ArgumentException(error);
        }

        public static string VerifyUsername(string username)
        {
            return Verify(username, IsValidUsername, "Invalid username: '" + username + "'");
        }
    }
}