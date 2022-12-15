using System.Drawing;
using System.Windows.Forms;

namespace ScreenCut.Tools
{
    /*
     * An empty drawing tool when no tools are selected
     * Allows to move the selected area
     */

    internal class ToolEmpty : ITool
    {
        bool IsMoving = false;

        public void MouseDoubleClick(MainForm form) { }

        public void MouseDown(MainForm form)
        {
            IsMoving = true;
            form.mousePositionPrev = Control.MousePosition;
        }

        public void MouseUp(MainForm form)
        {
            IsMoving = false;
        }
        public void MouseMove(MainForm form, Point position)
        {
            if (form.ssRect.Contains(position))
            {
                if (form.Cursor != Cursors.SizeAll)
                    form.Cursor = Cursors.SizeAll;
            }
            else
            {
                if (form.Cursor != Cursors.Default)
                    form.Cursor = Cursors.Default;
            }

            if (IsMoving)
            {
                int dx = position.X - form.mousePositionPrev.X;
                int dy = position.Y - form.mousePositionPrev.Y;
                form.ssRect.Offset(dx, dy);

                var ds = form.GetDrawSettings();
                ds.Location = Point.Add(ds.Location, new Size(dx,dy));

                var size = form.GetSizeLabel();
                size.Location = Point.Add(size.Location, new Size(dx, dy));

                form.mousePositionPrev = position;
                form.Refresh();
            }
        }

        public void Paint(MainForm form, Graphics gr) { }

        public void ScreenPaint(MainForm form, Graphics gr) { }

        public void Selected(MainForm form) { }

        public void Unselected(MainForm form)
        {
            IsMoving = false;
            form.Cursor = Cursors.Default;
        }
    }
}
