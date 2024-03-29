﻿using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using ScreenCut.Tools;
using System.Net.Http.Headers;

namespace ScreenCut
{
    public partial class MainForm : Form
    {
        // TODO: fix multiple screen support
        // TODO: selected area size needs to be cropped if went out of screen
        // TODO: First reposition and drawing after selecting drawing
        // TODO: size changable from corners and middle of dimensions
        // TODO: make the displayed size not go out of the screen
        // TODO: fix jittering around toolbar when the app is painted

        // TODO: icon for each draw tool
        // TODO: allow selecting draw tool mode like in photoshop with little settings icon

        // TODO: optimize censor tool to use less RAM

        // DLL libraries used to manage hotkeys
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        const int HotKeyID = 1;

        public bool init = false;
        public Bitmap screenshot;
        public ImageAttributes ia;

        public bool selecting;
        public Rectangle ssRect;
        public Pen selectingPen;
        public Pen selectingPen2;

        public Point mousePositionPrev;

        public bool isDrawing = false;
        public Pen drawingPen;
        public bool updateDraw;

        public Screen currentScreen = Screen.PrimaryScreen;
        public Point currentScreenOffset;
        public Size currentScreenSize = Screen.PrimaryScreen.Bounds.Size;

        public enum ToolType
        {
            DrawFreeDraw = 0,
            DrawLine,
            DrawText,
            DrawRectangle,
            DrawArrow,

            ColorPicker,
            Censor,
        }

        /* The list of all available tools. */
        private readonly Dictionary<ToolType,ITool> drawTools = new Dictionary<ToolType, ITool>
        {
            { ToolType.DrawFreeDraw, new ToolFreeDraw() },
            { ToolType.DrawLine, new ToolLine() },
            { ToolType.DrawText, new ToolText() },
            { ToolType.DrawRectangle, new ToolRectangle() },
            { ToolType.DrawArrow, new ToolArrow() },

            { ToolType.ColorPicker, new ToolColorPicker() },
            { ToolType.Censor, new ToolCensor() },
        };

        private readonly ITool emptyTool = new ToolEmpty();
        private ITool currentTool;

        // Initialize form
        public MainForm()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            // Register the summon hotkey
            RegisterHotKey(Handle, HotKeyID, 0, (int)Keys.F6);
        }

        protected override void WndProc(ref Message m)
        {
            // Summon hotkey was pressed
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == HotKeyID && !Visible)
            {
                currentScreenSize = Screen.FromPoint(Point.Empty).Bounds.Size;

                // Get the correct screen offset by iterating through Screen.AllScreens
                // (Screen.FromPoint doesn't give correct position values somewhy)
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

        private void Form1_Load(object sender, EventArgs e)
        {
            selectingPen = new Pen(Color.White)
            {
                Width = 2,
                DashPattern = new float[]{ 5, 5 }
            };
            selectingPen2 = new Pen(Color.White)
            {
                Width = 2
            };

            UpdateDrawingPen(Color.Red, (int)nWidth.Value);

            mousePositionPrev = Point.Empty;

            foreach (Control c in pDrawSettings.Controls)
            {
                c.KeyDown += Form1_KeyDown;
            }

            currentTool = emptyTool;

            // Setup the custom update interval
            Timer timer = new Timer
            {
                Interval = 10
            };
            timer.Tick += new EventHandler(MouseMoveEvent);
            timer.Start();
        }

        private void InitialScreenshot()
        {
            // Make screenshot and make
            // the form invisible when opened
            // (because it doesn't work in Form1_Load
            // or form constructor)

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

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (!init)
            {
                InitialScreenshot();
                init = true;
            }

            // Draw stuff from user
            if (isDrawing)
            {
                using (var gr = Graphics.FromImage(screenshot))
                    currentTool.Paint(this, gr);
            }

            // Draw screenshot
            var g = e.Graphics;
            g.DrawImage(screenshot, new Rectangle(0, 0, currentScreen.Bounds.Width, currentScreen.Bounds.Height),
                0, 0, screenshot.Width, screenshot.Height, GraphicsUnit.Pixel, ia);

            g.DrawImage(screenshot, ssRect, ssRect, GraphicsUnit.Pixel);

            currentTool.ScreenPaint(this, g);

            DrawSelectedAreaBorder(g);
        }

        private void DrawSelectedAreaBorder(Graphics g)
        {
            // Draw stroke around selected area
            g.DrawRectangle(selectingPen, ssRect);

            // Top-left corner
            g.DrawLine(selectingPen2, ssRect.Left - 5, ssRect.Top - 5, ssRect.Left + 5, ssRect.Top - 5);
            g.DrawLine(selectingPen2, ssRect.Left - 5, ssRect.Top - 5, ssRect.Left - 5, ssRect.Top + 5);

            // Top-right corner
            g.DrawLine(selectingPen2, ssRect.Right + 5, ssRect.Top - 5, ssRect.Right - 5, ssRect.Top - 5);
            g.DrawLine(selectingPen2, ssRect.Right + 5, ssRect.Top - 5, ssRect.Right + 5, ssRect.Top + 5);

            // Down-left corner
            g.DrawLine(selectingPen2, ssRect.Left - 5, ssRect.Bottom + 5, ssRect.Left + 5, ssRect.Bottom + 5);
            g.DrawLine(selectingPen2, ssRect.Left - 5, ssRect.Bottom + 5, ssRect.Left - 5, ssRect.Bottom - 5);

            // Down-right corner
            g.DrawLine(selectingPen2, ssRect.Right + 5, ssRect.Bottom + 5, ssRect.Right - 5, ssRect.Bottom + 5);
            g.DrawLine(selectingPen2, ssRect.Right + 5, ssRect.Bottom + 5, ssRect.Right + 5, ssRect.Bottom - 5);

            // Top middle
            g.DrawLine(selectingPen2, ssRect.Left + ssRect.Width / 2 - 5, ssRect.Top - 5,
                 ssRect.Left + ssRect.Width / 2 + 5, ssRect.Top - 5);

            // Down middle
            g.DrawLine(selectingPen2, ssRect.Left + ssRect.Width / 2 - 5, ssRect.Bottom + 5,
                 ssRect.Left + ssRect.Width / 2 + 5, ssRect.Bottom + 5);

            // Left middle
            g.DrawLine(selectingPen2, ssRect.Left - 5, ssRect.Top + ssRect.Height / 2 - 5,
                ssRect.Left - 5, ssRect.Top + ssRect.Height / 2 + 5);

            // Right middle
            g.DrawLine(selectingPen2, ssRect.Right + 5, ssRect.Top + ssRect.Height / 2 - 5,
                ssRect.Right + 5, ssRect.Top + ssRect.Height / 2 + 5);
        }

        private void MouseMoveEvent(object sender, EventArgs e)
        {
            if (selecting)
            {
                UpdateSSRect();
                Refresh();
            }
            else
            {
                if (isDrawing)
                    Refresh();
                currentTool.MouseMove(this,Control.MousePosition);
            }

            updateDraw = false;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (ssRect.Contains(e.Location))
            {
                currentTool.MouseDown(this);
            }
            else // if (!new Rectangle(e.Location, new Size(1, 1)).IntersectsWith(pDrawSettings.ClientRectangle))
            {
                selecting = true;
                mousePositionPrev = e.Location;
                pDrawSettings.Visible = false;
                lSize.Visible = true;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (selecting)
            {
                UpdateSSRect();

                pDrawSettings.Left = ssRect.Right + 4;
                pDrawSettings.Top = ssRect.Top;
                pDrawSettings.Visible = true;

                selecting = false;
            }
            else
            {
                currentTool.MouseUp(this);
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
                ssRect = currentScreen.Bounds;
                lSize.Top = -lSize.Height - 5;
                Refresh();
            }
        }

        private void TbText_KeyDown(object sender, KeyEventArgs e)
        {
            (drawTools[ToolType.DrawText] as ToolText).TextBox_KeyDown(this, e);
        }

        private void Form1_DoubleClick(object sender, EventArgs e)
        {
            currentTool.MouseDoubleClick(this);
        }

        private void LbDraw_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbDraw.SelectedIndex >= 0 && lbDraw.SelectedIndex < drawTools.Count)
            {
                currentTool.Unselected(this);
                currentTool = drawTools[(ToolType)lbDraw.SelectedIndex];
                currentTool.Selected(this);
            }
            isDrawing = false;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                UpdateDrawingPen(colorDialog1.Color, drawingPen.Width);
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

        private void MakeScreenshot()
        {
            screenshot = new Bitmap(currentScreenSize.Width, currentScreenSize.Height);
            using (var sg = Graphics.FromImage(screenshot))
                sg.CopyFromScreen(currentScreenOffset, Point.Empty, currentScreenSize);
        }

        public void UpdateSSRect()
        {
            var mouse = Control.MousePosition;
            ssRect = new Rectangle(
                mousePositionPrev.X, mousePositionPrev.Y,
                mouse.X - mousePositionPrev.X, mouse.Y - mousePositionPrev.Y
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
            ssRect = Rectangle.Empty;
            lbDraw.SelectedIndex = -1;
            lSize.Visible = false;

            currentTool.Unselected(this);
            currentTool = emptyTool;
        }

        public TextBox GetTextBox()
        {
            return tbText;
        }

        public Panel GetDrawSettings()
        {
            return pDrawSettings;
        }

        public Label GetSizeLabel()
        {
            return lSize;
        }
    }
}
