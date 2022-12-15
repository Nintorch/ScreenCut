using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenCut.Tools
{
    internal class ToolArrow : ITool
    {
        public void MouseDoubleClick(MainForm form)
        {

        }

        public void MouseDown(MainForm form)
        {
            form.isDrawing = true;
            form.mousePositionPrev = Control.MousePosition;
        }

        public void MouseUp(MainForm form)
        {
            form.updateDraw = true;
        }

        public void MouseMove(MainForm form, Point position)
        {

        }

        public void Paint(MainForm form, Graphics gr)
        {
            if (form.updateDraw)
            {
                ScreenPaint(form, gr);
                form.isDrawing = false;
                form.updateDraw = false;
            }
        }

        public void ScreenPaint(MainForm form, Graphics gr)
        {
            if (form.isDrawing)
            {
                gr.DrawLine(form.drawingPen, form.mousePositionPrev, Control.MousePosition);
            }
        }

        public void Selected(MainForm form)
        {
            form.drawingPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
        }

        public void Unselected(MainForm form)
        {
            form.drawingPen.EndCap = System.Drawing.Drawing2D.LineCap.NoAnchor;
        }
    }
}
