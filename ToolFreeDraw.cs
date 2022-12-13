using System.Drawing;
using System.Windows.Forms;

namespace ScreenCut
{
    /*
     * Free draw tool
     * 
     * While the left mouse button is held and
     * the mouse cursor is moved, a curve will be
     * drawn from the cursor movements
     * Release the left mouse button to cancel
     */

    internal class ToolFreeDraw : IDrawTool
    {
        public void MouseDoubleClick(MainForm form) { }

        public void MouseDownDrawing(MainForm form) { }

        public void MouseDownNotDrawing(MainForm form)
        {
            form.updateDraw = true;
            form.mousePositionPrev = Control.MousePosition;
        }

        public void MouseUpDrawing(MainForm form)
        {
            form.isDrawing = false;
        }

        public void Paint(MainForm form, Graphics gr, Point mousePosition)
        {
            gr.DrawLine(form.drawingPen, form.mousePositionPrev, mousePosition);
            form.mousePositionPrev = mousePosition;
        }

        public void PostPaint(MainForm form, Graphics gr) { }
    }
}
