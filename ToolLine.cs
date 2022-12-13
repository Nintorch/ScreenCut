using System.Drawing;
using System.Windows.Forms;

namespace ScreenCut
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

    internal class ToolLine : IDrawTool
    {
        public void MouseDoubleClick(MainForm form)
        {
            if (form.isDrawing)
                form.isDrawing = false;
        }

        public void MouseDownDrawing(MainForm form)
        {
            form.updateDraw = true;
        }

        public void MouseDownNotDrawing(MainForm form)
        {
            form.updateDraw = true;
            form.mousePositionPrev = Control.MousePosition;
        }

        public void MouseUpDrawing(MainForm form) { }

        public void Paint(MainForm form, Graphics gr, Point mousePosition)
        {
            if (form.updateDraw)
            {
                gr.DrawLine(form.drawingPen, form.mousePositionPrev, mousePosition);
                form.mousePositionPrev = mousePosition;
                form.updateDraw = false;
            }
        }

        public void PostPaint(MainForm form, Graphics gr)
        {
            if (form.isDrawing)
                gr.DrawLine(form.drawingPen, form.mousePositionPrev, Control.MousePosition);
        }
    }
}
