using System.Drawing;

namespace ScreenCut
{
    /*
     * An empty drawing tool when no tools are selected
     */

    internal class ToolEmpty : IDrawTool
    {
        public void MouseDoubleClick(MainForm form) { }

        public void MouseDownDrawing(MainForm form) { }

        public void MouseDownNotDrawing(MainForm form) { }

        public void MouseUpDrawing(MainForm form) { }

        public void Paint(MainForm form, Graphics gr, Point mousePosition) { }

        public void PostPaint(MainForm form, Graphics gr) { }
    }
}
