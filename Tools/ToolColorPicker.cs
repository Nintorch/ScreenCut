using System.Drawing;
using System.Windows.Forms;

namespace ScreenCut.Tools
{
    internal class ToolColorPicker : ITool
    {
        /*
         * Color picker tool
         * 
         * A tool to select a color from the screenshot
         * Move the mouse cursor in the selected area and
         * the cursor will be changed to color picker cursor
         * Move the cursor out of the selected area and
         * the cursor will be changed to default image
         * 
         * Press the left mouse button to select the color
         */

        readonly Cursor ColorPickerCursor;

        public ToolColorPicker()
        {
            ColorPickerCursor = new Cursor(Properties.Resources.colorpicker.GetHicon());
        }

        public void MouseDoubleClick(MainForm form) { }

        public void MouseDown(MainForm form)
        {
            var mouse = Control.MousePosition;
            form.drawingPen.Color = form.screenshot.GetPixel(mouse.X, mouse.Y);
        }

        public void MouseMove(MainForm form, Point position)
        {
            if (form.ssRect.Contains(position))
            {
                if (!form.isDrawing)
                {
                    form.isDrawing = true;
                    form.Cursor = ColorPickerCursor;
                }
            }
            else
            {
                if (form.isDrawing)
                {
                    form.isDrawing = false;
                    form.Cursor = Cursors.Default;
                }
            }
        }

        public void MouseUp(MainForm form) { }

        public void Paint(MainForm form, Graphics gr) { }

        public void ScreenPaint(MainForm form, Graphics gr) { }

        public void Selected(MainForm form) { }

        public void Unselected(MainForm form)
        {
            form.Cursor = Cursors.Default;
        }
    }
}
