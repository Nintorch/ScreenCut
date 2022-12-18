using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenCut.Tools
{
    /*
     * Text drawing tool
     * 
     * Left click on a selected area, a text box will appear
     * After the user puts text in it and presses Enter,
     * the text will be rendered on the screenshot
     * Press Escape to cancel
     */

    internal class ToolText : ITool
    {
        public void MouseDoubleClick(MainForm form) { }

        public void MouseDown(MainForm form)
        {
            if (!form.isDrawing)
            {
                form.isDrawing = true;

                var tbText = form.GetTextBox();
                tbText.Visible = true;
                tbText.Left = Control.MousePosition.X;
                tbText.Top = Control.MousePosition.Y - 4;
                tbText.Focus();
                tbText.Height = (int)form.drawingPen.Width+5;
                tbText.Font = new Font(tbText.Font.FontFamily, form.drawingPen.Width+5);
            }
        }

        public void MouseUp(MainForm form) { }

        public void MouseMove(MainForm form, Point position) { }

        public void Paint(MainForm form, Graphics gr)
        {
            if (form.updateDraw)
            {
                var tbText = form.GetTextBox();
                gr.DrawString(tbText.Text, tbText.Font, form.drawingPen.Brush, new PointF(tbText.Left, tbText.Top));
            }
            form.isDrawing = false;
        }

        public void ScreenPaint(MainForm form, Graphics gr) { }

        public void TextBox_KeyDown(MainForm form, KeyEventArgs e)
        {
            var tbText = form.GetTextBox();
            if (e.KeyCode == Keys.Escape)
            {
                tbText.Visible = false;
                tbText.Text = "";
            }
            else if (e.KeyCode == Keys.Enter)
            {
                form.isDrawing = true;
                form.updateDraw = true;
                form.Refresh();
                tbText.Visible = false;
                tbText.Text = "";
            }
        }

        public void Selected(MainForm form) { }

        public void Unselected(MainForm form) { }
    }
}
