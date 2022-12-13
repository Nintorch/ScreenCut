using System.Drawing;

namespace ScreenCut
{
    internal interface IDrawTool
    {
        // Paint event
        void Paint(MainForm form, Graphics gr, Point mousePosition);

        // Post paint event (drawing on the screen, not the screenshot)
        void PostPaint(MainForm form, Graphics gr);

        // Left mouse button press when isDrawing is false
        // (the drawing is not in process)
        void MouseDownNotDrawing(MainForm form);

        // Left mouse button press when isDrawing is true
        // (drawing is in process)
        void MouseDownDrawing(MainForm form);

        // Left mouse button release while drawing is in process
        void MouseUpDrawing(MainForm form);

        // Left mouse button double click
        void MouseDoubleClick(MainForm form);
    }
}
