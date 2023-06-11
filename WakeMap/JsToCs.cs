using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeMap
{
    /// <summary>WebView2に読み込ませるためのJsで実行する関数を保持させたクラス</summary>
    //[System.Runtime.InteropServices.ClassInterface(System.Runtime.InteropServices.ClassInterfaceType.AutoDual)]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class JsToCs
    {
        //WakeControllerクラス参照用
        public WakeController refWakeController = null;

        //C#_WebViewチェック
        public string CheckCsharpWebView()
        {
            return "CsharpWebView_OK";
        }

        //htmlを新しいウィンドウで開く
        public void WindowOpen(string url)
        {
            string fimeneme = System.IO.Path.GetFileName(url);
        
            switch (fimeneme)
            {
                case "subPatternA.html":
                    (new SubFormGenerator()).CreateSubForm(fimeneme, url);
                    break;
                case "subPatternB.html":
                    (new SubFormGenerator()).CreateSubForm(fimeneme, url);
                    break;
                default:
                    break;
            }
        }
    }
}
