using System.Drawing;

namespace ScreenCut.Tools
{
    internal interface ITool
    {
        // Paint event
        void Paint(MainForm form, Graphics gr);

        // Post paint event (drawing on the screen, not the screenshot)
        void ScreenPaint(MainForm form, Graphics gr);

        // Left mouse button press when isDrawing is true
        // (drawing is in process)
        void MouseDown(MainForm form);

        // Left mouse button release while drawing is in process
        void MouseUp(MainForm form);

        // Left mouse button double click
        void MouseDoubleClick(MainForm form);

        // Mouse move event
        void MouseMove(MainForm form, Point position);

        // An event ran when the tool is selected from toolbar
        void Selected(MainForm form);

        // An event ran when a different tool is selected from the toolbar
        void Unselected(MainForm form);
    }
}
