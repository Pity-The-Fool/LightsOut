using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LightsOut
{
    public partial class MainForm : Form
    {
        private const int GridOffset = 25; // Distance from upper-left side of window
        private static int GridLength = 225; // Size in pixels of grid
        private static int NumCells = 3; // Number of cells in grid
        private int CellLength = GridLength / NumCells;

        private bool[,] grid; // Stores on/off state of cells in grid
        private Random rand; // Used to generate random numbers

        public MainForm()
        {
            InitializeComponent();
            x3ToolStripMenuItem.Checked = true;

            rand = new Random(); // Initializes random number generator
            grid = new bool[NumCells, NumCells];

            // Turn entire grid on
            for (int r = 0; r < NumCells; r++)
            {
                for (int c = 0; c < NumCells; c++)
                {
                    grid[r, c] = true;
                }
            }
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            for (int r = 0; r < NumCells; r++)
                for (int c = 0; c < NumCells; c++)
                {
                    // Get proper pen and brush for on/off
                    // grid section
                    Brush brush;
                    Pen pen;

                    if (grid[r, c])
                    {
                        pen = Pens.Black;
                        brush = Brushes.White; // On
                    }
                    else
                    {
                        pen = Pens.White;
                        brush = Brushes.Black; // Off
                    }

                    CellLength = GridLength / NumCells;

                    // Determine (x,y) coord of row and col to draw rectangle
                    int x = c * CellLength + GridOffset;
                    int y = r * CellLength + GridOffset;

                    // Draw outline and inner rectangle
                    g.DrawRectangle(pen, x, y, CellLength, CellLength);
                    g.FillRectangle(brush, x + 1, y + 1, CellLength - 1, CellLength - 1);
                }
        }

        private void InitializeBoard()
        {
            grid = new bool[NumCells, NumCells];

            // redraw board
            this.Invalidate();

            // reset checked menu option
            if (NumCells == 3)
            {
                x3ToolStripMenuItem.Checked = true;
                x4ToolStripMenuItem.Checked = false;
                x5ToolStripMenuItem.Checked = false;
            }
            else if (NumCells == 4)
            {
                x3ToolStripMenuItem.Checked = false;
                x4ToolStripMenuItem.Checked = true;
                x5ToolStripMenuItem.Checked = false;
            }
            else
            {
                x3ToolStripMenuItem.Checked = false;
                x4ToolStripMenuItem.Checked = false;
                x5ToolStripMenuItem.Checked = true;
            }

            // Turn entire grid on
            for (int r = 0; r < NumCells; r++)
            {
                for (int c = 0; c < NumCells; c++)
                {
                    grid[r, c] = true;
                }
            }
        }

        private void x3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NumCells = 3;
            InitializeBoard();
        }

        private void x4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NumCells = 4;
            InitializeBoard();
        }

        private void x5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NumCells = 5;
            InitializeBoard();
        }

        private bool PlayerWon()
        {
            bool result = false;

            for (int r = 0; r < NumCells; r++)
                for (int c = 0; c < NumCells; c++)
                {
                    if (grid[r, c] == false)
                        result = true;
                    else
                        return false;
                }
            return result;
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            // Make sure click was inside the grid
            if (e.X < GridOffset || e.X > CellLength * NumCells + GridOffset ||
            e.Y < GridOffset || e.Y > CellLength * NumCells + GridOffset)
                return;

            // Find row, col of mouse press
            int r = (e.Y - GridOffset) / CellLength;
            int c = (e.X - GridOffset) / CellLength;

            // Invert selected box and all surrounding boxes
            for (int i = r - 1; i <= r + 1; i++)
                for (int j = c - 1; j <= c + 1; j++)
                    if (i >= 0 && i < NumCells && j >= 0 && j < NumCells)
                        grid[i, j] = !grid[i, j];

            // Redraw grid
            this.Invalidate();

            // Check to see if puzzle has been solved
            if (PlayerWon())
            {
                // Display winner dialog box
                MessageBox.Show(this, "Congratulations! You've won!", "Lights Out!",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void newGameButton_Click(object sender, EventArgs e)
        {
            // Fill grid with either white or black
            for (int r = 0; r < NumCells; r++)
                for (int c = 0; c < NumCells; c++)
                    grid[r, c] = rand.Next(2) == 1;

            // Redraw grid
            this.Invalidate();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newGameButton_Click(sender, e);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutBox = new AboutForm();
            aboutBox.ShowDialog(this);
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            Control control = (Control)sender;

            /***********************************************************************************************
             * Code example from:
             * https://msdn.microsoft.com/en-us/library/system.windows.forms.control.resize(v=vs.110).aspx
            ***********************************************************************************************/
            // Make Form remain a square
            if (control.Size.Height != control.Size.Width)
            {
                control.Size = new Size(control.Size.Width, control.Size.Width);
            }

            GridLength = control.Size.Width - 125;
            this.Invalidate();
        }
    }
}
