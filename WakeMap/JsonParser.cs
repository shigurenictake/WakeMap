using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeMap
{
    internal class JsonParser
    {
        public void JsonParserTest()
        {
            string strDictAWake = @"
                {
                    aWake1:
                    {
                        info: { row: 1, id: 1 },
                        pos1: { x: 120.007 , y: 35.846 },
                        pos2: { x: 124.496 , y: 33.370 },
                        pos3: { x: 121.259 , y: 31.974 },
                        pos4: { x: 123.925 , y: 30.197 }
                    },
                    aWake2:
                    {
                        info: { row: 2, id: 2 },
                        pos1: { x: 136.238 , y: 38.892 },
                        pos2: { x: 133.572 , y: 39.781 },
                        pos3: { x: 136.238 , y: 40.479 },
                        pos4: { x: 134.080 , y: 41.495 }
                    },
                    aWake3:
                    {
                        info: { row: 3, id: 3 },
                        pos1: { x: 143.855 , y: 34.703 },
                        pos2: { x: 145.505 , y: 33.307 },
                        pos3: { x: 143.030 , y: 32.545 },
                        pos4: { x: 145.378 , y: 31.276 }
                    }
                }";

            Dictionary<string, Dictionary<string, Dictionary<string, double>>> g_dictA = ParseDictJSON(strDictAWake);

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

        static Dictionary<string, Dictionary<string, Dictionary<string, double>>> ParseDictJSON(string jsonString)
        {
            Dictionary<string, Dictionary<string, Dictionary<string, double>>> result = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();

            // 不要な空白や改行を削除
            jsonString = jsonString.Replace(" ", "").Replace("\r", "").Replace("\n", "");

            int pos = 0;
            int length = jsonString.Length;

            int loopcount = 0;

            //debug
            {
                Console.WriteLine("");
                Console.WriteLine("======= ParseDictJSON 開始 =======");
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
                string key = jsonString.Substring(keyStart, keyEnd - (keyStart - 1) ).Trim('\"');

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
                Dictionary<string, Dictionary<string, double>> subObject = ParseSubObject(subObjectString);

                //debug
                {
                    Console.WriteLine($"=======ParseDictJSON ParseSubObject 結果=======");
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
                    Console.WriteLine($"=======ParseDictJSON result.Add結果=======");
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

        static Dictionary<string, Dictionary<string, double>> ParseSubObject(string subObjectString)
        {
            Dictionary<string, Dictionary<string, double>> result = new Dictionary<string, Dictionary<string, double>>();

            // キーと値の組みを検索
            int pos = 0;
            int length = subObjectString.Length;

            int loopcount = 0;

            //debug
            {
                Console.WriteLine("");
                Console.WriteLine( "    ======= ParseSubObject 開始 =======");
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
                string key = subObjectString.Substring(keyStart, keyEnd - (keyStart - 1)).Trim('\"');

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
                Dictionary<string, double> value = ParseValue(valueString);

                // 結果に追加
                result.Add(key, value);

                // 次のキーの位置へ移動
                pos = valueEnd + 1;

                loopcount++;
            }

            return result;
        }

        static Dictionary<string, double> ParseValue(string valueString)
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
                    string key = keyValue[0].Trim('\"');
                    double value;
                    if (double.TryParse(keyValue[1], out value))
                    {
                        result.Add(key, value);
                    }
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
    }
}
