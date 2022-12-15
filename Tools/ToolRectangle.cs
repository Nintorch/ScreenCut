using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenCut.Tools
{
    internal class ToolRectangle : ITool
    {
        /*
         * Rectangle tool
         * 
         * As the name implies, a tool to draw rectangles
         * Press the left mouse button to start drawing
         * Release the left mouse button to stop drawing
         */

        public enum Mode
        {
            Stroke, Fill
        }

        public Mode CurrentMode = Mode.Fill;

        public void MouseDoubleClick(MainForm form)
        {

        }

        public void MouseDown(MainForm form)
        {
            if (!form.isDrawing)
            {
                form.isDrawing = true;
                form.mousePositionPrev = Control.MousePosition;
            }
        }

        public void MouseUp(MainForm form)
        {
            form.updateDraw = true;
        }

        public void MouseMove(MainForm form, Point position) { }

        public void Paint(MainForm form, Graphics gr)
        {
            if (form.updateDraw)
            {
                ScreenPaint(form, gr);
                form.updateDraw = false;
                form.isDrawing = false;
            }
        }

        public void ScreenPaint(MainForm form, Graphics gr)
        {
            if (form.isDrawing)
            {
                var mouse = Control.MousePosition;
                var rect = Rectangle.FromLTRB(
                    Math.Min(form.mousePositionPrev.X, mouse.X),
                    Math.Min(form.mousePositionPrev.Y, mouse.Y),
                    Math.Max(form.mousePositionPrev.X, mouse.X),
                    Math.Max(form.mousePositionPrev.Y, mouse.Y));
                if (CurrentMode == Mode.Stroke)
                    gr.DrawRectangle(form.drawingPen, rect);
                else
                    gr.FillRectangle(form.drawingPen.Brush, rect);
            }
        }

        public void Selected(MainForm form) { }

        public void Unselected(MainForm form) { }
    }
}
