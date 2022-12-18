using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ScreenCut.Tools
{
    internal class ToolCensor : ITool
    {
        Bitmap BlurredScreenshot;
        TextureBrush Brush;
        Pen Pen;

        public enum Mode
        {
            FreeDraw, Rectangle
        }
        public Mode CurrentMode = Mode.FreeDraw;

        public int BlurRadius = 5;

        public void MouseDoubleClick(MainForm form)
        {

        }

        public void MouseDown(MainForm form)
        {
            if (!form.isDrawing)
            {
                form.mousePositionPrev = Control.MousePosition;
                form.isDrawing = true;
                if (CurrentMode == Mode.FreeDraw)
                    form.updateDraw = true;
            }
        }

        public void MouseMove(MainForm form, Point position)
        {

        }

        public void MouseUp(MainForm form)
        {
            if (CurrentMode == Mode.FreeDraw && form.isDrawing)
                form.isDrawing = false;
            else if (CurrentMode == Mode.Rectangle)
                form.updateDraw = true;
        }

        public void Paint(MainForm form, Graphics gr)
        {
            var mouse = Control.MousePosition;
            if (CurrentMode == Mode.FreeDraw)
            {
                var width = form.drawingPen.Width;
                Pen.Width = width;
                gr.DrawLine(Pen, form.mousePositionPrev, mouse);
                gr.FillEllipse(Brush, mouse.X - width / 2, mouse.Y - width / 2, width, width);
                gr.FillEllipse(Brush,
                    form.mousePositionPrev.X - width / 2,
                    form.mousePositionPrev.Y - width / 2, width, width);
                form.mousePositionPrev = mouse;
            }
            else if (form.updateDraw)
            {
                ScreenPaint(form, gr);
                form.updateDraw = false;
                form.isDrawing = false;
            }
        }

        public void ScreenPaint(MainForm form, Graphics gr)
        {
            if (CurrentMode == Mode.Rectangle && form.isDrawing)
            {
                var mouse = Control.MousePosition;
                var rect = Rectangle.FromLTRB(
                    Math.Min(form.mousePositionPrev.X, mouse.X),
                    Math.Min(form.mousePositionPrev.Y, mouse.Y),
                    Math.Max(form.mousePositionPrev.X, mouse.X),
                    Math.Max(form.mousePositionPrev.Y, mouse.Y));
                    gr.FillRectangle(Brush, rect);
            }
        }

        public void Selected(MainForm form)
        {
            BlurredScreenshot = new Bitmap(form.screenshot,
                form.currentScreenSize.Width / BlurRadius, form.currentScreenSize.Height / BlurRadius);
            var BlurredScreenshot2 = new Bitmap(form.currentScreenSize.Width, form.currentScreenSize.Height);
            using (Graphics g = Graphics.FromImage(BlurredScreenshot2))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(BlurredScreenshot, new Rectangle(0, 0, form.currentScreenSize.Width, form.currentScreenSize.Height));
            }
            BlurredScreenshot = BlurredScreenshot2;

            Brush = new TextureBrush(BlurredScreenshot);
            Brush.Transform = new Matrix(1, 0, 0, 1, 0, 0);

            Pen = new Pen(Brush);
        }

        public void Unselected(MainForm form)
        {
            
        }
    }
}
