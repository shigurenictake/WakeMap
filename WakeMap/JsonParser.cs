using System;
using System.Collections.Generic;

namespace WakeMap
{
    internal class JsonParser
    {
        public void JsonParserTest()
        {
            string strDictAWake = @"
                {
                    'aWake1':
                    {
                        info: { row: '1', id: '11' },
                        pos1: { x: 120.007 , y: 35.846 , 'date': 'yyyymmdd' , 'time': 'hhmmss' },
                        pos2: { x: 124.496 , y: 33.370 , 'date': 'yyyymmdd' , 'time': 'hhmmss' },
                        pos3: { x: 121.259 , y: 31.974 , 'date': 'yyyymmdd' , 'time': 'hhmmss' },
                        pos4: { x: 123.925 , y: 30.197 , 'date': 'yyyymmdd' , 'time': 'hhmmss' }
                    },
                    aWake2:
                    {
                        info: { row: 2, id: 12 },
                        pos1: { x: 136.238 , y: 38.892 , 'date': '20221101' , 'time': '112131' },
                        pos2: { x: 133.572 , y: 39.781 , 'date': '20221102' , 'time': '112132' },
                        pos3: { x: 136.238 , y: 40.479 , 'date': '20221103' , 'time': '112133' },
                        pos4: { x: 134.080 , y: 41.495 , 'date': '20221104' , 'time': '112134' }
                    },
                    aWake3:
                    {
                        info: { row: 3, id: 13 },
                        pos1: { x: 143.855 , y: 34.703 , 'date': '20221101' , 'time': '112141' },
                        pos2: { x: 145.505 , y: 33.307 , 'date': '20221102' , 'time': '112142' },
                        pos3: { x: 143.030 , y: 32.545 , 'date': '20221103' , 'time': '112143' },
                        pos4: { x: 145.378 , y: 31.276 , 'date': '20221104' , 'time': '112144' }
                    }
                }";

            Dictionary<string, Dictionary<string, Dictionary<string, string>>> g_dictA = ParseDictSDictSDictSS(strDictAWake);

            // 結果を出力して確認する
            foreach (var pair1 in g_dictA)
            {
                Console.WriteLine("Key1: " + pair1.Key);
                foreach (var pair2 in pair1.Value)
                {
                    Console.WriteLine("    Key2: " + pair2.Key);
                    foreach (var pair3 in pair2.Value)
                    {
                        Console.WriteLine("        Key3: " + pair3.Key + ", Value: " + pair3.Value);
                    }
                }
            }
        }

        static Dictionary<string, Dictionary<string, Dictionary<string, string>>> ParseDictSDictSDictSS(string jsonString)
        {
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> result = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

            // 不要な空白や改行を削除
            jsonString = jsonString.Replace(" ", "").Replace("\r", "").Replace("\n", "");

            int pos = 0;
            int length = jsonString.Length;

            int loopcount = 0;

            //debug
            {
                Console.WriteLine("");
                Console.WriteLine("======= ParseDictSDictSDictSS 開始 =======");
                Console.WriteLine($"jsonString =");
                Console.WriteLine($"00        10        20        30        40        50        60        70        80        90        100       110       120       130       140       150       160       170       190       200       210       220       230       240       ");
                Console.WriteLine($"012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789");
                Console.WriteLine($"{jsonString}");
                Console.WriteLine($"  length = {length}");
                Console.WriteLine($"");
            }

            // JSON文字列の解析
            while (pos < length-1)
            {
                //debug
                {
                    Console.WriteLine("======= ループカウント =======");
                    Console.WriteLine($"  loopcount = {loopcount}, pos = {pos}");
                    Console.WriteLine($"");
                }

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

                //debug
                {
                    Console.WriteLine("======= 各種取得処理 結果 =======");
                    Console.WriteLine($"  keyStart = {keyStart}");
                    Console.WriteLine($"  keyEnd = {keyEnd}");
                    Console.WriteLine($"  key = {key}");
                    Console.WriteLine($"  subObjectStart = {subObjectStart}");
                    Console.WriteLine("");
                }

                // サブオブジェクトの終了位置を検索
                int subObjectEnd = FindMatchingClosingBracket(jsonString, subObjectStart);
                if (subObjectEnd == -1)
                    break;

                // サブオブジェクトを取得
                string subObjectString = jsonString.Substring(subObjectStart, subObjectEnd - subObjectStart + 1);

                //debug
                {
                    Console.WriteLine("======= サブオブジェクトを取得処理 結果 =======");
                    Console.WriteLine($"  subObjectEnd = {subObjectEnd}");
                    Console.WriteLine($"  subObjectString = {subObjectString}");
                    Console.WriteLine("");
                }

                // サブオブジェクトの解析
                Dictionary<string, Dictionary<string, string>> subObject = ParseDictSDictSS(subObjectString);

                //debug
                {
                    Console.WriteLine($"=======ParseDictSDictSDictSS ParseDictSDictSS 結果=======");
                    foreach (var kvpA in subObject)
                    {
                        Console.WriteLine($"  kvpA.Key = {kvpA.Key}");
                        foreach (var kvpB in kvpA.Value)
                        {
                            Console.WriteLine($"    kvpB.Key = {kvpB.Key}, kvpB.Value = {kvpB.Value}");
                        }
                    }
                    Console.WriteLine($"");
                }

                // 結果に追加
                result.Add(key, subObject);

                //debug
                {
                    Console.WriteLine($"=======ParseDictSDictSDictSS result.Add結果=======");
                    foreach (var kvpA in result)
                    {
                        Console.WriteLine($"  kvpA.Key = {kvpA.Key}");
                        foreach (var kvpB in kvpA.Value)
                        {
                            Console.WriteLine($"    kvpB.Key = {kvpB.Key}");
                            foreach (var kvpC in kvpB.Value)
                            {
                                Console.WriteLine($"    kvpC.Key = {kvpB.Key}, kvpC.Value = {kvpB.Value}");
                            }
                        }
                    }
                    Console.WriteLine($"");
                }

                // 次のキーの位置へ移動
                pos = subObjectEnd + 1;

                loopcount++;
            }

            return result;
        }

        static Dictionary<string, Dictionary<string, string>> ParseDictSDictSS(string subObjectString)
        {
            Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();

            // キーと値の組みを検索
            int pos = 0;
            int length = subObjectString.Length;

            int loopcount = 0;

            //debug
            {
                Console.WriteLine("");
                Console.WriteLine( "    ======= ParseDictSDictSS 開始 =======");
                Console.WriteLine($"    subObjectString =");
                Console.WriteLine($"    00        10        20        30        40        50        60        70        80        90        100       110       120       130       140       150       160       170       190       200       210       220       230       240       ");
                Console.WriteLine($"    012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789");
                Console.WriteLine($"    {subObjectString}");
                Console.WriteLine($"    length = {length}");
                Console.WriteLine($"");
            }

            while (pos < length-1)
            {
                //debug
                {
                    Console.WriteLine( "      ======= ループカウント =======");
                    Console.WriteLine($"        loopcount = {loopcount}, pos = {pos}");
                    Console.WriteLine($"");
                }

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

                //debug
                {
                    Console.WriteLine( "      ======= 各種取得処理 結果 =======");
                    Console.WriteLine($"        keyStart = {keyStart}");
                    Console.WriteLine($"        keyEnd = {keyEnd}");
                    Console.WriteLine($"        key = {key}");
                    Console.WriteLine($"        subObjectString = {subObjectString}");
                    Console.WriteLine("");
                }

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

                //debug
                {
                    Console.WriteLine( "      ======= 値を取得処理 結果 =======");
                    Console.WriteLine($"        valueStart = {valueStart}");
                    Console.WriteLine($"        valueEnd = {valueEnd}");
                    Console.WriteLine($"        valueString = {valueString}");
                    Console.WriteLine("");
                }

                // 値の解析
                Dictionary<string, string> value = ParseDictSS(valueString);

                // 結果に追加
                result.Add(key, value);

                // 次のキーの位置へ移動
                pos = valueEnd + 1;

                loopcount++;
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

        // 文字列の前後にある " または ' を取り除く
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
