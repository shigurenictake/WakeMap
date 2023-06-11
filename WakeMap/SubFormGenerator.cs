using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WakeMap
{
    internal class SubFormGenerator
    {
        //SubForm生成
        public void CreateSubForm(string formname, string url)
        {
            //指定した名前のフォームがあれば取得する
            Form subform = this.GetFormByName(formname);

            //二重起動防止
            if (subform == null || subform.IsDisposed)
            {
                //ヌル、または破棄されていたら、新しいウィンドウで起動する
                subform = new MainForm(url);
                subform.Show();
                //フォーム名を設定
                subform.Name = formname;
            }
            //フォームにフォーカスを当てる。
            if (!subform.Visible)
            {
                subform.Show();
            }
            else
            {
                subform.Activate();
            }
        }

        //指定した名前のFormオブジェクトがあれば取得する
        public Form GetFormByName(string formname)
        {
            Form retform = null;
            foreach (Form form in Application.OpenForms)
            {
                if (form.Name == formname)
                {
                    retform = (Form)form;
                    break;
                }
            }
            return retform;
        }

    }
}
