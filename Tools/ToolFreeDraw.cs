using System.Drawing;
using System.Windows.Forms;

namespace ScreenCut.Tools
{
    /*
     * Free draw tool
     * 
     * While the left mouse button is held and
     * the mouse cursor is moved, a curve will be
     * drawn from the cursor movements
     * Release the left mouse button to cancel
     */

    internal class ToolFreeDraw : ITool
    {
        public void MouseDoubleClick(MainForm form) { }

        public void MouseDown(MainForm form)
        {
            if (!form.isDrawing)
            {
                form.updateDraw = true;
                form.mousePositionPrev = Control.MousePosition;
                form.isDrawing = true;
            }
        }

        public void MouseMove(MainForm form, Point position) { }

        public void MouseUp(MainForm form)
        {
            if (form.isDrawing)
                form.isDrawing = false;
        }

        public void Paint(MainForm form, Graphics gr)
        {
            var mouse = Control.MousePosition;
            var width = form.drawingPen.Width;
            gr.DrawLine(form.drawingPen, form.mousePositionPrev, mouse);
            gr.FillEllipse(form.drawingPen.Brush, mouse.X - width / 2, mouse.Y - width / 2, width, width);
            gr.FillEllipse(form.drawingPen.Brush,
                form.mousePositionPrev.X - width / 2,
                form.mousePositionPrev.Y - width / 2, width, width);
            form.mousePositionPrev = mouse;
        }

        public void ScreenPaint(MainForm form, Graphics gr) { }

        public void Selected(MainForm form) { }

        public void Unselected(MainForm form) { }
    }
}
