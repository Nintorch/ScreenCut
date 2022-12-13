using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace ScreenCut
{
    public partial class MainForm : Form
    {
        // TODO: fix multiple screen support
        // TODO: make the code cleaner
        // TODO: allow the selection to be repositionable, size needs to be cropped if went out of screen
        // TODO: First reposition and drawing after selecting drawing
        // TODO: eyedropper for choosing color
        // TODO: size changable from corners and middle of dimensions
        // TODO: make the displayed size not go out of the screen
        // TODO: fix jittering around toolbar when the app is painted

        // TODO: icon for each draw tool
        // TODO: allow selecting draw tool mode like in photoshop with little settings icon

        // TODO: Rectangle tool: stroke mode or fill mode
        // TODO: censor tool: rectangle mode and custom - several levels of blurriness
        // TODO: arrow tool

        // DLL libraries used to manage hotkeys
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        const int HotKeyID = 1;

        public bool init = false;
        public Image screenshot;
        public ImageAttributes ia;

        public bool selecting;
        public Point ul_point;
        public Point dr_point;
        public Rectangle ssRect;
        public Pen pen;

        public Point mousePositionPrev;

        public bool isDrawing = false;
        public Pen drawingPen;
        public float dpWidth;

        // (used for line and text drawing)
        public bool updateDraw;

        private Screen currentScreen = Screen.PrimaryScreen;
        private Point currentScreenOffset;
        private Size currentScreenSize = Screen.PrimaryScreen.Bounds.Size;

        /* The list of all available drawing tools. */
        private readonly List<IDrawTool> drawTools = new List<IDrawTool>
        {
            new ToolFreeDraw(),
            new ToolLine(),
            new ToolText(),
        };

        private readonly IDrawTool emptyTool = new ToolEmpty();
        private IDrawTool currentDrawTool;

        // Initialize form
        public MainForm()
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

            mousePositionPrev = new Point();

            foreach (Control c in pDrawSettings.Controls)
            {
                c.KeyDown += Form1_KeyDown;
            }

            currentDrawTool = emptyTool;

            Timer timer = new Timer
            {
                Interval = 10
            };
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

                ColorMatrix m = new ColorMatrix
                {
                    Matrix33 = 0.5f
                };
                ia = new ImageAttributes();
                ia.SetColorMatrix(m, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                return;
            }

            // Draw stuff from user
            if (isDrawing)
            {
                Point mouse = MousePosition;
                using (var gr = Graphics.FromImage(screenshot))
                    currentDrawTool.Paint(this, gr, mouse);
            }

            // Draw screenshot
            var g = e.Graphics;
            g.DrawImage(screenshot, new Rectangle(0, 0, currentScreen.Bounds.Width, currentScreen.Bounds.Height),
                0, 0, screenshot.Width, screenshot.Height, GraphicsUnit.Pixel, ia);

            g.DrawImage(screenshot, ssRect, ssRect, GraphicsUnit.Pixel);

            currentDrawTool.PostPaint(this, g);

            g.DrawRectangle(pen, ssRect);
        }

        public TextBox GetTextBox()
        {
            return tbText;
        }
        
        private void MouseMoveEvent(object sender, EventArgs e)
        {
            if (selecting)
            {
                dr_point = MousePosition;
                UpdateSSRect();
                Refresh();
            }
            else if (isDrawing)
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
                    currentDrawTool.MouseDownNotDrawing(this);
                }
                else
                    currentDrawTool.MouseDownDrawing(this);
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

                selecting = false;
            }
            else
            {
                if (isDrawing)
                    currentDrawTool.MouseUpDrawing(this);
            }
            Refresh();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // "Close" the app if Escape was pressed
            if (e.KeyCode == Keys.Escape)
                HideForm();
            // Save screenshot if Enter was pressed
            else if (e.KeyCode == Keys.Enter)
                BSave_Click(null, null);
            // Copy screenshot to clipboard if CTRL+C were pressed
            else if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
                BCopy_Click(null, null);
            else if (e.KeyCode == Keys.A && e.Modifiers == Keys.Control)
            {
                ssRect = Screen.PrimaryScreen.Bounds;
                lSize.Top = -lSize.Height - 5;
                Refresh();
            }
        }

        private void TbText_KeyDown(object sender, KeyEventArgs e)
        {
            // TODO: fix it?
            (drawTools[2] as ToolText).TextBox_KeyDown(this, e);
        }

        private void Form1_DoubleClick(object sender, EventArgs e)
        {
            currentDrawTool.MouseDoubleClick(this);
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
                ssRect.X += ssRect.Width;
                ssRect.Width = -ssRect.Width;
                lSize.Left = ssRect.Left + 10;
            }
            else
                lSize.Left = ssRect.Right - lSize.Width;

            if (ssRect.Height < 0)
            {
                ssRect.Y += ssRect.Height;
                ssRect.Height = -ssRect.Height;
                lSize.Top = ssRect.Y - lSize.Height - 2;
            }
            else
                lSize.Top = ssRect.Bottom + 2;
        }

        private void UpdateDrawingPen(Color color, float width)
        {
            drawingPen = new Pen(color)
            {
                Width = width
            };
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

        private void LbDraw_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbDraw.SelectedIndex >= 0 && lbDraw.SelectedIndex < drawTools.Count)
                currentDrawTool = drawTools[lbDraw.SelectedIndex];
            isDrawing = false;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                UpdateDrawingPen(colorDialog1.Color, dpWidth);
            }
        }

        private void NWidth_ValueChanged(object sender, EventArgs e)
        {
            UpdateDrawingPen(drawingPen.Color, (float)nWidth.Value);
        }

        private void BSave_Click(object sender, EventArgs e)
        {
            if (dSaveScreenshot.ShowDialog() == DialogResult.OK)
            {
                GetClippedScreenshot().Save(dSaveScreenshot.FileName);
                HideForm();
            }
        }

        private void BCopy_Click(object sender, EventArgs e)
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
