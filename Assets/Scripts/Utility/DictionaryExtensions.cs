using System;
using System.Collections.Generic;

// http://baba-s.hatenablog.com/entry/2014/11/20/222405
public static class DictionaryExtensions
{
    /// <summary>
    /// 指定したキーを持つ値を削除します。
    /// 削除前に指定された関数を呼び出します
    /// </summary>
    public static void Remove<TKey, TValue>(
        this Dictionary<TKey, TValue> self,
        TKey key,
        Action<TValue> act
    )
    {
        if (!self.ContainsKey(key))
        {
            return;
        }
        act(self[key]);
        self.Remove(key);
    }
}
