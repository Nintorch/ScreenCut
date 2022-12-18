using System.Drawing;
using System.Windows.Forms;

namespace ScreenCut.Tools
{
    /*
     * Line Tool
     * 
     * A tool to draw lines
     * Left click on a selected area to start drawing the line
     * After another left click the line will be drawn on the screenshot
     * and another line will start drawing from that spot
     * Double click to cancel
     */

    internal class ToolLine : ITool
    {
        public void MouseDoubleClick(MainForm form)
        {
            if (form.isDrawing)
                form.isDrawing = false;
        }

        public void MouseDown(MainForm form)
        {
            form.updateDraw = true;
            if (!form.isDrawing)
            {
                form.mousePositionPrev = Control.MousePosition;
                form.isDrawing = true;
            }
        }

        public void MouseMove(MainForm form, Point position)
        {

        }

        public void MouseUp(MainForm form) { }

        public void Paint(MainForm form, Graphics gr)
        {
            if (form.updateDraw)
            {
                var mouse = Control.MousePosition;
                var width = form.drawingPen.Width;
                gr.DrawLine(form.drawingPen, form.mousePositionPrev, mouse);
                gr.FillEllipse(form.drawingPen.Brush, mouse.X - width / 2,
                    mouse.Y - width / 2, width, width);
                gr.FillEllipse(form.drawingPen.Brush,
                    form.mousePositionPrev.X - width / 2,
                    form.mousePositionPrev.Y - width / 2, width, width);
                form.mousePositionPrev = mouse;
                form.updateDraw = false;
            }
        }

        public void ScreenPaint(MainForm form, Graphics gr)
        {
            if (form.isDrawing)
            {
                var mouse = Control.MousePosition;
                var width = form.drawingPen.Width;
                gr.DrawLine(form.drawingPen, form.mousePositionPrev, mouse);
                gr.FillEllipse(form.drawingPen.Brush, mouse.X - width / 2,
                    mouse.Y - width / 2, width, width);
            }
        }

        public void Selected(MainForm form) { }

        public void Unselected(MainForm form) { }
    }
}
