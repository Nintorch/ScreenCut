using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ScreenCut
{
    public partial class Form1 : Form
    {
        // TODO: fix multiple screen support
        // TODO: make the code cleaner
        // TODO: allow the selection to be repositionable, size needs to be cropped if went out of screen
        // TODO: First reposition and drawing after selecting drawing
        // TODO: come up with a good name for app
        // TODO: eyedropper for choosing color
        // TODO: size changable from corners and middle of dimensions
        // TODO: make the displayed size not go out of the screen
        // TODO: fix jittering around toolbar when the app is painted

        // TODO: icon for each draw tool
        // TODO: allow selecting draw tool mode like in photoshop with little settings icon

        // TODO: Rectangle tool: stroke mode or fill mode
        // TODO: censor tool: rectangle mode and custom - several levels of blurriness

        // DLL libraries used to manage hotkeys
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        const int HotKeyID = 1;

        private bool init = false;
        private Image screenshot;
        private ImageAttributes ia;

        private bool selecting;
        private Point ul_point;
        private Point dr_point;
        private Rectangle ssRect;
        private Pen pen;
        
        private Point mousePositionPrev;
        private Point mousePosition;

        private enum DrawingType
        {
            FreeDraw,
            Line,
            Text,
        }

        private DrawingType drawing;
        private bool isDrawing = false;
        private Pen drawingPen;
        private float dpWidth;

        // (used for line and text drawing)
        private bool updateDraw;

        private Screen currentScreen = Screen.PrimaryScreen;
        private Point currentScreenOffset;
        private Size currentScreenSize = Screen.PrimaryScreen.Bounds.Size;

        // Initialize form
        public Form1()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            RegisterHotKey(Handle, HotKeyID, 0, (int)Keys.F6);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == HotKeyID && !Visible)
            {
                currentScreenSize = Screen.FromPoint(Point.Empty).Bounds.Size;
                foreach (Screen s in Screen.AllScreens)
                {
                    if (s.Bounds.Contains(MousePosition))
                    {
                        currentScreenOffset = new Point(s.Bounds.Left, s.Bounds.Top);
                        break;
                    }
                }

                currentScreen = Screen.FromPoint(MousePosition);

                var bounds = currentScreen.Bounds;
                Size = bounds.Size;
                Location = currentScreen.Bounds.Location;
                MakeScreenshot();
                Visible = true;
            }
            base.WndProc(ref m);
        }

        // Initialize program..
        private void Form1_Load(object sender, EventArgs e)
        {
            pen = new Pen(Color.White)
            {
                Width = 2
            };

            UpdateDrawingPen(Color.Red, 3);

            mousePosition = mousePositionPrev = new Point();

            foreach (Control c in pDrawSettings.Controls)
            {
                c.KeyDown += Form1_KeyDown;
            }

            Timer timer = new Timer();
            timer.Interval = 10;
            timer.Tick += new EventHandler(MouseMoveEvent);
            timer.Start();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Make screenshot and make
            // the form invisible when opened
            // (because it doesn't work in Form1_Load
            // or form constructor)
            if (!init)
            {
                init = true;
                HideForm();
                MakeScreenshot();

                ColorMatrix m = new ColorMatrix();
                m.Matrix33 = 0.5f;
                ia = new ImageAttributes();
                ia.SetColorMatrix(m, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            }

            // Draw stuff from user
            if (isDrawing)
            {
                Point mouse = MousePosition;
                using (var gr = Graphics.FromImage(screenshot))
                    switch (drawing)
                    {
                        case DrawingType.FreeDraw:
                            gr.DrawLine(drawingPen, mousePositionPrev, mouse);
                            mousePositionPrev = mouse;
                            break;
                        case DrawingType.Line:
                            if (updateDraw)
                            {
                                gr.DrawLine(drawingPen, mousePositionPrev, mouse);
                                mousePositionPrev = mouse;
                                updateDraw = false;
                            }
                            break;
                        case DrawingType.Text:
                            if (updateDraw)
                                gr.DrawString(tbText.Text, tbText.Font, drawingPen.Brush, new PointF(tbText.Left, tbText.Top));
                            isDrawing = false;
                            break;
                        default:
                            break;
                    }
            }

            // Draw screenshot
            var g = e.Graphics;
            g.DrawImage(screenshot, new Rectangle(0, 0, currentScreen.Bounds.Width, currentScreen.Bounds.Height),
                0, 0, screenshot.Width, screenshot.Height, GraphicsUnit.Pixel, ia);

            g.DrawImage(screenshot, ssRect, ssRect, GraphicsUnit.Pixel);

            if (isDrawing && drawing == DrawingType.Line)
                g.DrawLine(drawingPen, mousePositionPrev, mousePosition);

            g.DrawRectangle(pen, ssRect);
        }
        
        private void MouseMoveEvent(object sender, EventArgs e)
        {
            mousePosition = MousePosition;

            if (selecting)
            {
                dr_point = MousePosition;
                UpdateSSRect();
                Refresh();
            }
            if (isDrawing)
                Refresh();

            updateDraw = false;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (ssRect.IntersectsWith(new Rectangle(e.Location,new Size(1,1))))
            {
                if (!isDrawing)
                {
                    // Initialize drawing
                    isDrawing = true;
                    mousePositionPrev = mousePosition;
                    switch (drawing)
                    {
                        case DrawingType.Line:
                            updateDraw = true;
                            break;
                        case DrawingType.Text:
                            tbText.Visible = true;
                            tbText.Left = mousePosition.X;
                            tbText.Top = mousePosition.Y - 4;
                            tbText.Focus();
                            tbText.Height = (int)dpWidth;
                            tbText.Font = new Font(tbText.Font.FontFamily, dpWidth);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    if (drawing == DrawingType.Line)
                    {
                        updateDraw = true;
                        Refresh();
                        mousePositionPrev = mousePosition;
                    }
                }
            }
            else // if (!new Rectangle(e.Location, new Size(1, 1)).IntersectsWith(pDrawSettings.ClientRectangle))
            {
                selecting = true;
                ul_point = e.Location;
                pDrawSettings.Visible = false;
                lSize.Visible = true;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (selecting)
            {
                dr_point = e.Location;
                UpdateSSRect();

                pDrawSettings.Left = ssRect.Right + 4;
                pDrawSettings.Top = ssRect.Top;
                pDrawSettings.Visible = true;

                Refresh();

                selecting = false;
            }
            else
            {
                if (isDrawing)
                {
                    if (drawing == DrawingType.FreeDraw)
                        isDrawing = false;
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // "Close" the app if Escape was pressed
            if (e.KeyCode == Keys.Escape)
                HideForm();
            // Save screenshot if Enter was pressed
            else if (e.KeyCode == Keys.Enter)
                bSave_Click(null, null);
            // Copy screenshot to clipboard if CTRL+C were pressed
            else if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
                bCopy_Click(null, null);
            else if (e.KeyCode == Keys.A && e.Modifiers == Keys.Control)
            {
                ssRect = Screen.PrimaryScreen.Bounds;
                lSize.Top = -lSize.Height - 5;
                Refresh();
            }
        }

        private void tbText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                tbText.Visible = false;
                tbText.Text = "";
            }
            else if (e.KeyCode == Keys.Enter)
            {
                isDrawing = true;
                updateDraw = true;
                Refresh();
                tbText.Visible = false;
                tbText.Text = "";
            }
        }

        private void Form1_DoubleClick(object sender, EventArgs e)
        {
            if (isDrawing && drawing == DrawingType.Line)
                isDrawing = false;
        }

        private void MakeScreenshot()
        {
            screenshot = new Bitmap(currentScreenSize.Width, currentScreenSize.Height);
            using (var sg = Graphics.FromImage(screenshot))
                sg.CopyFromScreen(currentScreenOffset, Point.Empty, currentScreenSize);
        }

        private void UpdateSSRect()
        {
            ssRect = new Rectangle(
                ul_point.X, ul_point.Y,
                dr_point.X - ul_point.X, dr_point.Y - ul_point.Y
            );

            lSize.Text = $"{Math.Abs(ssRect.Width)}x{Math.Abs(ssRect.Height)}";

            if (ssRect.Width < 0)
            {
                ssRect.X = ssRect.X + ssRect.Width;
                ssRect.Width = -ssRect.Width;
                lSize.Left = ssRect.Left + 10;
            }
            else
                lSize.Left = ssRect.Right - lSize.Width;

            if (ssRect.Height < 0)
            {
                ssRect.Y = ssRect.Y + ssRect.Height;
                ssRect.Height = -ssRect.Height;
                lSize.Top = ssRect.Y - lSize.Height - 2;
            }
            else
                lSize.Top = ssRect.Bottom + 2;
        }

        private void UpdateDrawingPen(Color color, float width)
        {
            drawingPen = new Pen(color);
            drawingPen.Width = width;
            dpWidth = width;
        }

        private Image GetClippedScreenshot()
        {
            Image s = new Bitmap(ssRect.Width, ssRect.Height);
            using (var g = Graphics.FromImage(s))
                g.DrawImage(screenshot, new Rectangle(new Point(0, 0), s.Size), ssRect, GraphicsUnit.Pixel);
            return s;
        }

        private void HideForm()
        {
            Visible = false;
            pDrawSettings.Visible = false;
            isDrawing = false;
            ssRect = new Rectangle(0, 0, 0, 0);
            lbDraw.SelectedIndex = -1;
            lSize.Visible = false;
        }

        private void lbDraw_SelectedIndexChanged(object sender, EventArgs e)
        {
            drawing = (DrawingType)lbDraw.SelectedIndex;
            isDrawing = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                UpdateDrawingPen(colorDialog1.Color, dpWidth);
            }
        }

        private void nWidth_ValueChanged(object sender, EventArgs e)
        {
            UpdateDrawingPen(drawingPen.Color, (float)nWidth.Value);
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            if (dSaveScreenshot.ShowDialog() == DialogResult.OK)
            {
                GetClippedScreenshot().Save(dSaveScreenshot.FileName);
                HideForm();
            }
        }

        private void bCopy_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetImage(GetClippedScreenshot());
            }
            catch (ExternalException)
            {
                MessageBox.Show(
                    "Can't copy the screenshot at the moment because clipboard is being used by another process. Try again later."
                    );
            }
            HideForm();
        }
    }
}
