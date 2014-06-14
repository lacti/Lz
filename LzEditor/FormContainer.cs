using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LzEditor
{
    public partial class FormContainer : Form
    {
        public FormContainer()
        {
            InitializeComponent();
        }

        private void objectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowEditor<FormObjectEditor>();
        }

        private void ShowEditor<TForm>() where TForm : Form, new()
        {
            var editor = new TForm {MdiParent = this};
            editor.Show();
        }
    }
}
