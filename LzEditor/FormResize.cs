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
    public partial class FormResize : Form
    {
        public FormResize()
        {
            InitializeComponent();
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            int x, y;
            if (!CheckAndParseValue(textX, out x) || !CheckAndParseValue(textY, out y))
                return;

            X = x;
            Y = y;

            DialogResult = DialogResult.OK;
            Close();
        }

        public int X { get; set; }
        public int Y { get; set; }

        private void text_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            var textBox = sender as TextBox;
            if (textBox == null)
                return;

            int value;
            if (!CheckAndParseValue(textBox, out value))
                return;

            if (textBox == textX) textY.Focus();
            else buttonConfirm_Click(sender, e);
        }

        private bool CheckAndParseValue(TextBox textBox, out int value)
        {
            if (int.TryParse(textBox.Text, out value))
                return true;

            MessageBox.Show(this, "Invalid numeric format!", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            textBox.SelectAll();
            textBox.Focus();
            return false;
        }

        private void FormResize_Load(object sender, EventArgs e)
        {
            textX.Text = X.ToString();
            textY.Text = Y.ToString();
        }
    }
}
