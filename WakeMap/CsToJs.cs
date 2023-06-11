using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WakeMap
{
    //C#からJavaScriptにアクセス
    public class CsToJs
    {
        //参照元クラスのWebView2取得用(初期化は参照元クラスで行う)
        public Microsoft.Web.WebView2.WinForms.WebView2 webView;

        //参照フォーム(初期化は参照元クラスで行う)
        public Form refForm;

        //C#_WebView判定フラグにtrueをセット
        public async void SetIsCsharpWebView()
        {
            await webView.ExecuteScriptAsync("isCsharpWebView =  Boolean(1);");//true
        }

        //JavaScript実行要求
        public async void RemoveCloseBtn()
        {
            System.Text.StringBuilder js = new System.Text.StringBuilder();
            //js.AppendLine("console.log('RemoveCloseBtn start');");
            js.AppendLine("var element = document.getElementById('btn-close');");
            js.AppendLine("element.parentNode.remove();");//親要素と一緒に削除
            //js.AppendLine("console.log('RemoveCloseBtn end');");
            await webView.ExecuteScriptAsync(js.ToString());
        }

        //タイトルを設定する
        public async void SetTitle()
        {
            //htmlからタイトルを取得する
            string strTitle = await webView.ExecuteScriptAsync("document.title");
            //タイトルを設定
            refForm.Text = strTitle.Trim('"');
        }

        //PatternA固有操作
        public async void OperatePatternA()
        {
            System.Text.StringBuilder js = new System.Text.StringBuilder();
            js.AppendLine("var hw1Element = document.getElementById('hw1');");
            js.AppendLine("hw1Element.style.width = '80%'; ");// widthの値を変更
            await webView.ExecuteScriptAsync(js.ToString());
        }

        //PatternB固有操作
        public async void OperatePatternB()
        {
            System.Text.StringBuilder js = new System.Text.StringBuilder();
            //詳細へボタンの非表示
            js.AppendLine("document.getElementById('btn-godetail').style.display='none';");
            await webView.ExecuteScriptAsync(js.ToString());

            await webView.ExecuteScriptAsync("InitWake();");

        }

        //詳細へボタンをクリックする
        public async void buttonClickGoDetail()
        {
            System.Text.StringBuilder js = new System.Text.StringBuilder();
            js.AppendLine("var element = document.getElementById('btn-godetail').click();");
            //js.AppendLine("element.click();");
            await webView.ExecuteScriptAsync(js.ToString());
        }

    }
}
