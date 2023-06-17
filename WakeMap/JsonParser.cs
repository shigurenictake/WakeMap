using System;
using System.Collections.Generic;

namespace WakeMap
{
    public class JsonParser
    {
        //3次元のJSON文字列をディクショナリーに変換
        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> ParseDictSDictSDictSS(string jsonString)
        {
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> result = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

            // 不要な空白や改行を削除
            jsonString = jsonString.Replace(" ", "").Replace("\r", "").Replace("\n", "");

            int pos = 0;
            int length = jsonString.Length;

            // JSON文字列の解析
            while (pos < length-1)
            {
                // キーの開始位置を検索
                int keyStart = Math.Min((jsonString.IndexOf("{", pos) + 1), (jsonString.IndexOf(",", pos) + 1));
                if (keyStart == -1)
                    break;

                // キーの終了位置を検索
                int keyEnd = jsonString.IndexOf(":", keyStart + 1) - 1;
                if (keyEnd == -1)
                    break;

                // キーを取得
                string key = jsonString.Substring(keyStart, keyEnd - (keyStart - 1) );
                key = TrimQuotes(key);

                // サブオブジェクトの開始位置を検索
                int subObjectStart = jsonString.IndexOf("{", keyEnd);
                if (subObjectStart == -1)
                    break;

                // サブオブジェクトの終了位置を検索
                int subObjectEnd = FindMatchingClosingBracket(jsonString, subObjectStart);
                if (subObjectEnd == -1)
                    break;

                // サブオブジェクトを取得
                string subObjectString = jsonString.Substring(subObjectStart, subObjectEnd - subObjectStart + 1);

                // サブオブジェクトの解析
                Dictionary<string, Dictionary<string, string>> subObject = ParseDictSDictSS(subObjectString);

                // 結果に追加
                result.Add(key, subObject);

                // 次のキーの位置へ移動
                pos = subObjectEnd + 1;
            }

            return result;
        }

        static Dictionary<string, Dictionary<string, string>> ParseDictSDictSS(string subObjectString)
        {
            Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();

            // キーと値の組みを検索
            int pos = 0;
            int length = subObjectString.Length;

            while (pos < length-1)
            {
                // キーの開始位置を検索
                int keyStart = Math.Min( (subObjectString.IndexOf("{", pos) + 1), (subObjectString.IndexOf(",", pos) + 1) );
                if (keyStart == -1)
                    break;

                // キーの終了位置を検索
                int keyEnd = subObjectString.IndexOf(":", keyStart + 1) - 1;
                if (keyEnd == -1)
                    break;

                // キーを取得
                string key = subObjectString.Substring(keyStart, keyEnd - (keyStart - 1));
                key = TrimQuotes(key);

                // 値の開始位置を検索
                int valueStart = subObjectString.IndexOf("{", keyEnd);
                if (valueStart == -1)
                    break;

                // 値の終了位置を検索
                int valueEnd = FindMatchingClosingBracket(subObjectString, valueStart);
                if (valueEnd == -1)
                    break;

                // 値を取得
                string valueString = subObjectString.Substring(valueStart, valueEnd - valueStart + 1);

                // 値の解析
                Dictionary<string, string> value = ParseDictSS(valueString);

                // 結果に追加
                result.Add(key, value);

                // 次のキーの位置へ移動
                pos = valueEnd + 1;
            }

            return result;
        }

        //<string, double>
        static Dictionary<string, double> ParseDictSD(string valueString)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();

            // 不要な文字を削除
            valueString = valueString.Replace("{", "").Replace("}", "");

            // キーと値の組みを検索
            string[] pairs = valueString.Split(',');
            foreach (string pair in pairs)
            {
                string[] keyValue = pair.Split(':');
                if (keyValue.Length == 2)
                {
                    string key = TrimQuotes(keyValue[0]);
                    double value;
                    if (double.TryParse(keyValue[1], out value))
                    {
                        result.Add(key, value);
                    }
                }
            }

            return result;
        }

        //<string, string>
        static Dictionary<string, string> ParseDictSS(string jsonString)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // 不要な文字を削除
            jsonString = jsonString.Replace("{", "").Replace("}", "");

            // キーと値の組みを検索
            string[] pairs = jsonString.Split(',');
            foreach (string pair in pairs)
            {
                string[] keyValue = pair.Split(':');
                if (keyValue.Length == 2)
                {
                    string key = TrimQuotes(keyValue[0]);
                    string value = TrimQuotes(keyValue[1]);
                    result.Add(key, value);
                }
            }
            return result;
        }

        static int FindMatchingClosingBracket(string input, int openingBracketIndex)
        {
            int length = input.Length;
            int bracketCount = 1;

            for (int i = openingBracketIndex + 1; i < length; i++)
            {
                char c = input[i];
                if (c == '{')
                    bracketCount++;
                else if (c == '}')
                    bracketCount--;

                if (bracketCount == 0)
                    return i;
            }

            return -1;
        }

        // 前後の " または ' を取り除く
        static string TrimQuotes(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            char firstChar = input[0];
            char lastChar = input[input.Length - 1];

            if ((firstChar == '"' && lastChar == '"') || (firstChar == '\'' && lastChar == '\''))
            {
                return input.Substring(1, input.Length - 2);
            }

            return input;
        }
    }
}
