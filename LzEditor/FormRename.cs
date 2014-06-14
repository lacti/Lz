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
    public partial class FormRename : Form
    {
        public FormRename()
        {
            InitializeComponent();
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            string name;
            if (!CheckAndParseValue(textName, out name))
                return;

            NewName = name;

            DialogResult = DialogResult.OK;
            Close();
        }

        public string NewName { get; set; }

        private void text_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            var textBox = sender as TextBox;
            if (textBox == null)
                return;

            string value;
            if (!CheckAndParseValue(textBox, out value))
                return;

            buttonConfirm_Click(sender, e);
        }

        private bool CheckAndParseValue(TextBox textBox, out string value)
        {
            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                value = textBox.Text.Trim();
                return true;
            }

            MessageBox.Show(this, "Invalid name!", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            textBox.SelectAll();
            textBox.Focus();

            value = null;
            return false;
        }

        private void FormRename_Load(object sender, EventArgs e)
        {
            textName.Text = NewName;
        }
    }
}
