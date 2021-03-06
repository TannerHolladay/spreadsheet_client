// Written by Joe Zachary for CS 3500, September 2011.
//updated by Noah Carlson for CS 3505, April 2021.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SS
{
    /// <summary>
    ///     The type of delegate used to register for SelectionChanged events
    /// </summary>
    /// <param name="sender"></param>
    public delegate void SelectionChangedHandler(SpreadsheetPanel sender);


    /// <summary>
    ///     A panel that displays a spreadsheet with 26 columns (labeled A-Z) and 99 rows
    ///     (labeled 1-99).  Each cell on the grid can display a non-editable string.  One
    ///     of the cells is always selected (and highlighted).  When the selection changes, a
    ///     SelectionChanged event is fired.  Clients can register to be notified of
    ///     such events.
    ///     None of the cells are editable.  They are for display purposes only.
    /// </summary>
    public partial class SpreadsheetPanel : UserControl
    {
        // These constants control the layout of the spreadsheet grid.  The height and
        // width measurements are in pixels.
        private const int DATA_COL_WIDTH = 80;
        private const int DATA_ROW_HEIGHT = 20;
        private const int LABEL_COL_WIDTH = 30;
        private const int LABEL_ROW_HEIGHT = 30;
        private const int PADDING = 2;
        private const int SCROLLBAR_WIDTH = 20;
        private const int COL_COUNT = 26;
        private const int ROW_COUNT = 99;

        // The SpreadsheetPanel is composed of a DrawingPanel (where the grid is drawn),
        // a horizontal scroll bar, and a vertical scroll bar.
        private readonly DrawingPanel _drawingPanel;
        private readonly HScrollBar _hScroll;
        private readonly VScrollBar _vScroll;


        /// <summary>
        ///     Creates an empty SpreadsheetPanel
        /// </summary>
        public SpreadsheetPanel()
        {
            InitializeComponent();

            // The DrawingPanel is quite large, since it has 26 columns and 99 rows.  The
            // SpreadsheetPanel itself will usually be smaller, which is why scroll bars
            // are necessary.
            _drawingPanel = new DrawingPanel(this)
            {
                Location = new Point(0, 0),
                AutoScroll = false
            };

            // A custom vertical scroll bar.  It is designed to scroll in multiples of rows.
            _vScroll = new VScrollBar
            {
                SmallChange = 1,
                Maximum = ROW_COUNT
            };

            // A custom horizontal scroll bar.  It is designed to scroll in multiples of columns.
            _hScroll = new HScrollBar
            {
                SmallChange = 1,
                Maximum = COL_COUNT
            };

            // Add the drawing panel and the scroll bars to the SpreadsheetPanel.
            Controls.Add(_drawingPanel);
            Controls.Add(_vScroll);
            Controls.Add(_hScroll);

            // Arrange for the drawing panel to be notified when it needs to scroll itself.
            _hScroll.Scroll += _drawingPanel.HandleHScroll;
            _vScroll.Scroll += _drawingPanel.HandleVScroll;
        }

        /// <summary>
        ///     The event used to send notifications of a selection change
        /// </summary>
        public event SelectionChangedHandler SelectionChanged;


        /// <summary>
        ///     If the zero-based column and row are in range, sets the value of that
        ///     cell and returns true.  Otherwise, returns false.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetValue(int col, int row, string value)
        {
            return _drawingPanel.SetValue(col, row, value);
        }


        /// <summary>
        ///     If the zero-based column and row are in range, assigns the value
        ///     of that cell to the out parameter and returns true.  Otherwise,
        ///     returns false.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool GetValue(int col, int row, out string value)
        {
            return _drawingPanel.GetValue(col, row, out value);
        }


        /// <summary>
        ///     If the zero-based column and row are in range, uses them to set
        ///     the current selection and returns true.  Otherwise, returns false.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public bool SetSelection(int col, int row)
        {
            return _drawingPanel.SetSelection(col, row);
        }


        /// <summary>
        ///     Assigns the column and row of the current selection to the
        ///     out parameters.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        public void GetSelection(out int col, out int row)
        {
            _drawingPanel.GetSelection(out col, out row);
        }

        /// <summary>
        ///     Adds or updates online users' selections
        /// </summary>
        /// <param name="col">column of selection</param>
        /// <param name="row">row of selection</param>
        /// <param name="id">id of user</param>
        /// <param name="name">username of user</param>
        public void UpdateOnlineSelection(int col, int row, int id, string name)
        {
            _drawingPanel.setOnlineSelection(col, row, id, name);
        }

        public void SetID(int id)
        {
            _drawingPanel.SetID(id);
            Invalidate();
        }


        /// <summary>
        ///     When the SpreadsheetPanel is resized, we set the size and locations of the three
        ///     components that make it up.
        /// </summary>
        /// <param name="eventargs"></param>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            if (FindForm() != null && FindForm().WindowState == FormWindowState.Minimized) return;
            _drawingPanel.Size = new Size(Width - SCROLLBAR_WIDTH, Height - SCROLLBAR_WIDTH);
            _vScroll.Location = new Point(Width - SCROLLBAR_WIDTH, 0);
            _vScroll.Size = new Size(SCROLLBAR_WIDTH, Height - SCROLLBAR_WIDTH);
            _vScroll.LargeChange = (Height - SCROLLBAR_WIDTH) / DATA_ROW_HEIGHT;
            _hScroll.Location = new Point(0, Height - SCROLLBAR_WIDTH);
            _hScroll.Size = new Size(Width - SCROLLBAR_WIDTH, SCROLLBAR_WIDTH);
            _hScroll.LargeChange = (Width - SCROLLBAR_WIDTH) / DATA_COL_WIDTH;
        }


        /// <summary>
        ///     Used internally to keep track of cell addresses
        /// </summary>
        private class Address
        {
            public Address(int c, int r)
            {
                Col = c;
                Row = r;
            }

            public int Col { get; }
            public int Row { get; }

            public override int GetHashCode()
            {
                return Col.GetHashCode() ^ Row.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Address address)) return false;
                return Col == address.Col && Row == address.Row;
            }
        }


        /// <summary>
        ///     The panel where the spreadsheet grid is drawn.  It keeps track of the
        ///     current selection as well as what is supposed to be drawn in each cell.
        /// </summary>
        private class DrawingPanel : Panel
        {
            // Coordinate of cell in upper-left corner of display
            private int _firstColumn;
            private int _firstRow;

            private int _id;

            // Columns and rows are numbered beginning with 0.  This is the coordinate
            // of the selected cell.
            private int _selectedCol;
            private int _selectedRow;

            // The containing panel
            private readonly SpreadsheetPanel _ssp;

            // The strings contained by the spreadsheet
            private readonly Dictionary<Address, string> _values;

            private readonly Dictionary<int, Tuple<int, int, string>> _selections =
                new Dictionary<int, Tuple<int, int, string>>();


            public DrawingPanel(SpreadsheetPanel ss)
            {
                DoubleBuffered = true;
                _values = new Dictionary<Address, string>();
                _ssp = ss;
                _id = -1;
            }

            private bool HasID()
            {
                return _id != -1;
            }

            private static bool InvalidAddress(int col, int row)
            {
                return col < 0 || row < 0 || col >= COL_COUNT || row >= ROW_COUNT;
            }


            public void Clear()
            {
                _values.Clear();
                Invalidate();
            }

            public void SetID(int id)
            {
                _id = id;
            }

            public bool SetValue(int col, int row, string c)
            {
                if (InvalidAddress(col, row)) return false;

                var a = new Address(col, row);
                if (string.IsNullOrEmpty(c))
                    _values.Remove(a);
                else
                    _values[a] = c;
                Invalidate();
                return true;
            }

            /// <summary>
            ///     Adds selection to the Dictionary
            /// </summary>
            /// <param name="col"></param>
            /// <param name="row"></param>
            /// <param name="id"></param>
            /// <param name="name"></param>
            public void setOnlineSelection(int col, int row, int id, string name)
            {
                var t = new Tuple<int, int, string>(col, row, name);

                if (!_selections.ContainsKey(id))
                    _selections.Add(id, t);
                else
                    _selections[id] = t;

                Invalidate();
            }

            public bool GetValue(int col, int row, out string c)
            {
                if (InvalidAddress(col, row))
                {
                    c = null;
                    return false;
                }

                if (!_values.TryGetValue(new Address(col, row), out c)) c = "";
                return true;
            }


            public bool SetSelection(int col, int row)
            {
                if (InvalidAddress(col, row)) return false;
                _selectedCol = col;
                _selectedRow = row;
                Invalidate();
                return true;
            }


            public void GetSelection(out int col, out int row)
            {
                col = _selectedCol;
                row = _selectedRow;
            }


            public void HandleHScroll(object sender, ScrollEventArgs args)
            {
                _firstColumn = args.NewValue;
                Invalidate();
            }

            public void HandleVScroll(object sender, ScrollEventArgs args)
            {
                _firstRow = args.NewValue;
                Invalidate();
            }


            protected override void OnPaint(PaintEventArgs e)
            {
                // Clip based on what needs to be refreshed.
                var clip = new Region(e.ClipRectangle);
                e.Graphics.Clip = clip;

                // Color the background of the data area white
                e.Graphics.FillRectangle(
                    new SolidBrush(Color.White),
                    LABEL_COL_WIDTH,
                    LABEL_ROW_HEIGHT,
                    (COL_COUNT - _firstColumn) * DATA_COL_WIDTH,
                    (ROW_COUNT - _firstRow) * DATA_ROW_HEIGHT);

                // Pen, brush, and fonts to use
                Brush brush = new SolidBrush(Color.Black);
                var pen = new Pen(brush);
                var regularFont = Font;
                var boldFont = new Font(regularFont, FontStyle.Bold);

                // Draw the column lines
                int bottom = LABEL_ROW_HEIGHT + (ROW_COUNT - _firstRow) * DATA_ROW_HEIGHT;
                e.Graphics.DrawLine(pen, new Point(0, 0), new Point(0, bottom));
                for (var x = 0; x <= COL_COUNT - _firstColumn; x++)
                    e.Graphics.DrawLine(
                        pen,
                        new Point(LABEL_COL_WIDTH + x * DATA_COL_WIDTH, 0),
                        new Point(LABEL_COL_WIDTH + x * DATA_COL_WIDTH, bottom));

                // Draw the column labels
                for (var x = 0; x < COL_COUNT - _firstColumn; x++)
                {
                    var f = _selectedCol - _firstColumn == x ? boldFont : Font;
                    DrawColumnLabel(e.Graphics, x, f);
                }

                // Draw the row lines
                int right = LABEL_COL_WIDTH + (COL_COUNT - _firstColumn) * DATA_COL_WIDTH;
                e.Graphics.DrawLine(pen, new Point(0, 0), new Point(right, 0));
                for (var y = 0; y <= ROW_COUNT - _firstRow; y++)
                    e.Graphics.DrawLine(
                        pen,
                        new Point(0, LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT),
                        new Point(right, LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT));

                // Draw the row labels
                for (var y = 0; y < ROW_COUNT - _firstRow; y++)
                {
                    var f = _selectedRow - _firstRow == y ? boldFont : Font;
                    DrawRowLabel(e.Graphics, y, f);
                }

                if (HasID())
                    // Highlight the selection, if it is visible
                    if (_selectedCol - _firstColumn >= 0 && _selectedRow - _firstRow >= 0)
                    {
                        var b = NextColorBrush(_id);
                        var p = new Pen(b);
                        e.Graphics.DrawRectangle(
                            p,
                            new Rectangle(LABEL_COL_WIDTH + (_selectedCol - _firstColumn) * DATA_COL_WIDTH + 1,
                                LABEL_ROW_HEIGHT + (_selectedRow - _firstRow) * DATA_ROW_HEIGHT + 1,
                                DATA_COL_WIDTH - 2,
                                DATA_ROW_HEIGHT - 2));
                    }

                //Paints each selection
                foreach (var client in _selections)
                    //checks if it is in view
                    if (client.Value.Item1 - _firstColumn >= 0 && client.Value.Item2 - _firstRow >= 0)
                    {
                        //gets the next color of brush
                        var b = NextColorBrush(client.Key);
                        var p = new Pen(b);
                        e.Graphics.DrawRectangle(
                            p,
                            new Rectangle(LABEL_COL_WIDTH + (client.Value.Item1 - _firstColumn) * DATA_COL_WIDTH + 1,
                                LABEL_ROW_HEIGHT + (client.Value.Item2 - _firstRow) * DATA_ROW_HEIGHT + 1,
                                DATA_COL_WIDTH - 2,
                                DATA_ROW_HEIGHT - 2));
                    }

                // Draw the text
                foreach (var address in _values)
                {
                    string text = address.Value;
                    int x = address.Key.Col - _firstColumn;
                    int y = address.Key.Row - _firstRow;
                    float height = e.Graphics.MeasureString(text, regularFont).Height;
                    float width = e.Graphics.MeasureString(text, regularFont).Width;
                    if (x < 0 || y < 0) continue;
                    var cellClip = new Region(new Rectangle(LABEL_COL_WIDTH + x * DATA_COL_WIDTH + PADDING,
                        LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT,
                        DATA_COL_WIDTH - 2 * PADDING,
                        DATA_ROW_HEIGHT));
                    cellClip.Intersect(clip);
                    e.Graphics.Clip = cellClip;
                    e.Graphics.DrawString(
                        text,
                        regularFont,
                        brush,
                        LABEL_COL_WIDTH + x * DATA_COL_WIDTH + PADDING,
                        LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT + (DATA_ROW_HEIGHT - height) / 2);
                }
            }

            private static Brush NextColorBrush(int id)
            {
                int color = id % 8;
                Brush b = new SolidBrush(Color.Black);
                switch (color)
                {
                    case 0:
                    {
                        b = new SolidBrush(Color.Blue);
                        break;
                    }
                    case 1:
                    {
                        b = new SolidBrush(Color.Red);
                        break;
                    }
                    case 2:
                    {
                        b = new SolidBrush(Color.Green);
                        break;
                    }
                    case 3:
                    {
                        b = new SolidBrush(Color.Gold);
                        break;
                    }
                    case 4:
                    {
                        b = new SolidBrush(Color.HotPink);
                        break;
                    }
                    case 5:
                    {
                        b = new SolidBrush(Color.Aqua);
                        break;
                    }
                    case 6:
                    {
                        b = new SolidBrush(Color.DarkOliveGreen);
                        break;
                    }
                    case 7:
                    {
                        b = new SolidBrush(Color.Chocolate);
                        break;
                    }
                }

                return b;
            }

            /// <summary>
            ///     Draws a column label.  The columns are indexed beginning with zero.
            /// </summary>
            /// <param name="g"></param>
            /// <param name="x"></param>
            /// <param name="f"></param>
            private void DrawColumnLabel(Graphics g, int x, Font f)
            {
                var label = ((char) ('A' + x + _firstColumn)).ToString();
                float height = g.MeasureString(label, f).Height;
                float width = g.MeasureString(label, f).Width;
                g.DrawString(
                    label,
                    f,
                    new SolidBrush(Color.Black),
                    LABEL_COL_WIDTH + x * DATA_COL_WIDTH + (DATA_COL_WIDTH - width) / 2,
                    (LABEL_ROW_HEIGHT - height) / 2);
            }


            /// <summary>
            ///     Draws a row label.  The rows are indexed beginning with zero.
            /// </summary>
            /// <param name="g"></param>
            /// <param name="y"></param>
            /// <param name="f"></param>
            private void DrawRowLabel(Graphics g, int y, Font f)
            {
                var label = (y + 1 + _firstRow).ToString();
                float height = g.MeasureString(label, f).Height;
                float width = g.MeasureString(label, f).Width;
                g.DrawString(
                    label,
                    f,
                    new SolidBrush(Color.Black),
                    LABEL_COL_WIDTH - width - PADDING,
                    LABEL_ROW_HEIGHT + y * DATA_ROW_HEIGHT + (DATA_ROW_HEIGHT - height) / 2);
            }


            /// <summary>
            ///     Determines which cell, if any, was clicked.  Generates a SelectionChanged event.  All of
            ///     the indexes are zero based.
            /// </summary>
            /// <param name="e"></param>
            protected override void OnMouseClick(MouseEventArgs e)
            {
                base.OnClick(e);
                int x = (e.X - LABEL_COL_WIDTH) / DATA_COL_WIDTH;
                int y = (e.Y - LABEL_ROW_HEIGHT) / DATA_ROW_HEIGHT;
                if (e.X > LABEL_COL_WIDTH && e.Y > LABEL_ROW_HEIGHT && x + _firstColumn < COL_COUNT &&
                    y + _firstRow < ROW_COUNT)
                {
                    _selectedCol = x + _firstColumn;
                    _selectedRow = y + _firstRow;
                    _ssp.SelectionChanged?.Invoke(_ssp);
                }

                Invalidate();
            }
        }
    }
}