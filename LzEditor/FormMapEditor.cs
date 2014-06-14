using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using LzEngine.World;

namespace LzEditor
{
    public partial class FormMapEditor : Form
    {
        #region Editor Context

        private BrushMode _brush = BrushMode.Select;
        private int _canvasMouseX;
        private int _canvasMouseY;
        private bool _isDirty;
        private int _palletEndIndex;
        private bool _palletGrapped;
        private int _palletStartIndex;
        private bool _showGrid = true;

        private float _magnificant = 1;
        private bool _brushPressed;

        private MapObject _objectBrush;

        #endregion

        #region Object Context

        private WorldMap _map;

        #endregion

        public FormMapEditor()
        {
            InitializeComponent();
        }

        private void panelCanvas_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.ScaleTransform(1.0f / _magnificant, 1.0f / _magnificant);

            if (_map != null)
                _map.Paint(e.Graphics, Point.Empty);

            // mouse hover
            var canvasCursor = GetCurrentCanvasPixelPosition();
            if (_brush == BrushMode.Draw)
            {
                // Create a new color matrix and set the alpha value to 0.5
                var cm = new ColorMatrix();
                cm.Matrix00 = cm.Matrix11 = cm.Matrix22 = cm.Matrix44 = 1;
                cm.Matrix33 = 0.5f;

                // Create a new image attribute object and set the color matrix to
                // the one just created
                var imageAttribute = new ImageAttributes();
                imageAttribute.SetColorMatrix(cm);

                if (canvasCursor.X >= 0 && canvasCursor.Y >= 0)
                {
                    var srcRect = GetPalletSelectedPixelRectangle();
                    var destRect = new Rectangle(canvasCursor.X, canvasCursor.Y, srcRect.Width, srcRect.Height);
                    e.Graphics.DrawImage(picturePallet.Image, destRect, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, imageAttribute);
                }
            }

            // grid
            if (_showGrid)
                DrawGrid(e.Graphics);

            // cursor
            if (_brush != BrushMode.Select)
            {
                e.Graphics.DrawRectangle(Pens.Red, canvasCursor.X, canvasCursor.Y, WorldConstants.TileLength - 1, WorldConstants.TileLength - 1);
            }
        }

        private void DrawGrid(Graphics g)
        {
            for (var x = 0; x <= _map.X; ++x)
                g.DrawLine(Pens.LightBlue, x * WorldConstants.TileLength, 0, x * WorldConstants.TileLength, _map.Height);
            g.DrawLine(Pens.LightBlue, _map.Width - 1, 0, _map.Width - 1, _map.Height);

            for (var y = 0; y <= _map.Y; ++y)
                g.DrawLine(Pens.LightBlue, 0, y * WorldConstants.TileLength, _map.Width, y * WorldConstants.TileLength);
            g.DrawLine(Pens.LightBlue, 0, _map.Height - 1, _map.Width, _map.Height - 1);
        }

        private void DrawGrid(Control control, Graphics g)
        {
            var controlSize = control.ClientSize;
            for (var x = 0; x <= controlSize.Width / WorldConstants.TileLength; ++x)
                g.DrawLine(Pens.LightBlue, x * WorldConstants.TileLength, 0, x * WorldConstants.TileLength,
                           controlSize.Height);
            g.DrawLine(Pens.LightBlue, controlSize.Width - 1, 0, controlSize.Width - 1, controlSize.Height);

            for (var y = 0; y <= controlSize.Height / WorldConstants.TileLength; ++y)
                g.DrawLine(Pens.LightBlue, 0, y * WorldConstants.TileLength, controlSize.Width,
                           y * WorldConstants.TileLength);
            g.DrawLine(Pens.LightBlue, 0, controlSize.Height - 1, controlSize.Width, controlSize.Height - 1);
        }

        private void picturePallet_Paint(object sender, PaintEventArgs e)
        {
            DrawGrid(picturePallet, e.Graphics);

            var selectRectangle = GetPalletSelectedPixelRectangle();
            e.Graphics.DrawRectangle(Pens.Red, selectRectangle);
        }

        private Rectangle GetPalletSelectedPixelRectangle()
        {
            var indexRect = GetPalletSelectedIndexRectangle();
            var pixelRect = new Rectangle(indexRect.X * WorldConstants.TileLength, indexRect.Y * WorldConstants.TileLength,
                                          indexRect.Width * WorldConstants.TileLength,
                                          indexRect.Height * WorldConstants.TileLength);
            pixelRect.Width += -1 + WorldConstants.TileLength;
            pixelRect.Height += -1 + WorldConstants.TileLength;
            return pixelRect;
        }

        private Rectangle GetPalletSelectedIndexRectangle()
        {
            var startXIndex = _palletStartIndex % _map.Pallet.XCount;
            var startYIndex = _palletStartIndex / _map.Pallet.XCount;
            var endXIndex = _palletEndIndex % _map.Pallet.XCount;
            var endYIndex = _palletEndIndex / _map.Pallet.XCount;
            var selectRectangle = Rectangle.FromLTRB(Math.Min(startXIndex, endXIndex), Math.Min(startYIndex, endYIndex),
                                                     Math.Max(startXIndex, endXIndex), Math.Max(startYIndex, endYIndex));
            return selectRectangle;
        }

        private void picturePallet_MouseDown(object sender, MouseEventArgs e)
        {
            var palletSize = picturePallet.Image.Size;
            if (e.X < 0 || e.X >= palletSize.Width || e.Y < 0 || e.Y >= palletSize.Height)
                return;

            _palletGrapped = true;

            UpdatePalletStartIndex(e.X, e.Y);
            UpdatePalletEndIndex(e.X, e.Y);
            picturePallet.Invalidate();
        }

        private void picturePallet_MouseUp(object sender, MouseEventArgs e)
        {
            _palletGrapped = false;

            UpdatePalletEndIndex(e.X, e.Y);
            picturePallet.Invalidate();
        }

        private void picturePallet_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_palletGrapped)
                return;

            UpdatePalletEndIndex(e.X, e.Y);
            picturePallet.Invalidate();
        }

        private void UpdatePalletStartIndex(int mouseX, int mouseY)
        {
            _palletStartIndex = (mouseY / WorldConstants.TileLength) * _map.Pallet.XCount + mouseX / WorldConstants.TileLength;
        }

        private void UpdatePalletEndIndex(int mouseX, int mouseY)
        {
            var x = mouseX / WorldConstants.TileLength;
            var y = mouseY / WorldConstants.TileLength;
            if (x < 0) x = 0;
            else if (x >= _map.Pallet.XCount) x = _map.Pallet.XCount - 1;
            if (y < 0) y = 0;
            else if (y >= _map.Pallet.YCount) y = _map.Pallet.YCount - 1;

            _palletEndIndex = y * _map.Pallet.XCount + x;
        }

        private void splitBrowser_Panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_map == null)
                return;

            var resizeForm = new FormResize {X = _map.X, Y = _map.Y};
            if (resizeForm.ShowDialog(this) == DialogResult.OK)
            {
                _map.Resize(resizeForm.X, resizeForm.Y);

                ResizeCanvas();
            }
        }

        private void FormMapEditor_Load(object sender, EventArgs e)
        {
            typeof (Panel).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                          .SetValue(panelCanvas, true);

            LoadObjectFiles();
            LoadTilesetFiles();

            UpdateEditorTitle();
            ResetCanvas(180, 180);
        }

        private void ResetCanvas(int canvasXCount, int canvasYCount)
        {
            _map.Resize(canvasXCount, canvasYCount);
            _map.Clear();

            _isDirty = false;

            ResizeCanvas();
        }

        private void ResizeCanvas()
        {
            if (_map == null)
                return;

            panelCanvas.Size = new Size((int)Math.Floor(_map.X * WorldConstants.TileLength / _magnificant),
                                        (int)Math.Floor(_map.Y * WorldConstants.TileLength / _magnificant));
            panelCanvas.Invalidate();
        }

        private void LoadTilesetFiles()
        {
            comboBoxTile.Items.Clear();
            comboBoxTile.Items.AddRange(
                Directory.GetFiles(ResourcePath.TilesetRoot, "*.png")
                         .Select(Path.GetFileNameWithoutExtension).Cast<object>().ToArray());

            comboBoxTile.SelectedIndex = 0;
            LoadPallet((string)comboBoxTile.Items[0]);
        }

        private void LoadObjectFiles()
        {
            if (!Directory.Exists(ResourcePath.ObjectRoot))
                Directory.CreateDirectory(ResourcePath.ObjectRoot);

            var objectRoot = new TreeNode("Objects", 1, 1,
                                          Directory.GetFiles(ResourcePath.ObjectRoot, "*.xml")
                                                   .Select(
                                                       e =>
                                                       new TreeNode(Path.GetFileNameWithoutExtension(e), 3, 3) {Tag = e})
                                                   .ToArray());
            treeObjects.Nodes.Clear();
            treeObjects.Nodes.Add(objectRoot);

            objectRoot.ExpandAll();
        }

        private void LoadPallet(string tilesetName)
        {
            if (!tilesetName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                tilesetName += ".png";

            if (_map != null)
                _map.Pallet = Pallet.Get(tilesetName);
            else _map = new WorldMap(tilesetName);

            picturePallet.Image = _map.Pallet.Image;
        }

        private void panelCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var mouseX = (int)(e.X * _magnificant / WorldConstants.TileLength);
            var mouseY = (int)(e.Y * _magnificant / WorldConstants.TileLength);

            if (_canvasMouseX == mouseX && _canvasMouseY == mouseY)
                return;

            _canvasMouseX = mouseX;
            _canvasMouseY = mouseY;

            if (_brushPressed)
                ProcessBrushEvent(e.Button);

            panelCanvas.Invalidate();
        }

        private Point GetCurrentCanvasPixelPosition()
        {
            var indexPos = GetCurrentCanvasIndexPosition();
            if (indexPos.X < 0 && indexPos.Y < 0)
                return indexPos;

            return new Point(indexPos.X * WorldConstants.TileLength, indexPos.Y * WorldConstants.TileLength);
        }

        private Point GetCurrentCanvasIndexPosition()
        {
            if (_map == null || _canvasMouseX < 0 || _canvasMouseY < 0 || _canvasMouseX >= _map.X ||
                _canvasMouseY >= _map.Y)
                return new Point(-1, -1);

            return new Point(_canvasMouseX, _canvasMouseY);
        }

        private void panelCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            var button = e.Button;
            ProcessBrushEvent(button);

            _brushPressed = true;
        }

        private void ProcessBrushEvent(MouseButtons button)
        {
            if (_brush == BrushMode.Draw)
            {
                if (tabControl.SelectedTab == tabPageTiles)
                {
                    var selectedIndexRect = GetPalletSelectedIndexRectangle();
                    foreach (var dy in Enumerable.Range(0, selectedIndexRect.Height + 1))
                    {
                        var y = _canvasMouseY + dy;
                        if (y < 0 || y >= _map.Y)
                            continue;

                        foreach (var dx in Enumerable.Range(0, selectedIndexRect.Width + 1))
                        {
                            var x = _canvasMouseX + dx;
                            if (x < 0 || x >= _map.X)
                                continue;

                            if (button == MouseButtons.Left)
                                _map.Tiles[y, x] = (selectedIndexRect.Top + dy) * _map.Pallet.XCount +
                                                   (selectedIndexRect.Left + dx);
                            else if (button == MouseButtons.Right)
                                _map.Tiles[y, x] = -1;
                        }
                    }
                }
                else if (tabControl.SelectedTab == tabPageObjects)
                {
                    var currentIndexPos = GetCurrentCanvasIndexPosition();
                    if (button == MouseButtons.Left)
                    {
                        if (_objectBrush != null)
                            _map.AddObject(currentIndexPos, _objectBrush);
                    }
                    else if (button == MouseButtons.Right)
                        _map.RemoveObject(currentIndexPos);
                }
            }

            var currentIndex = GetCurrentCanvasIndexPosition();
            if (currentIndex.X >= 0 && currentIndex.Y >= 0)
            {
                if (_brush == BrushMode.Obstacle)
                {
                    if (button == MouseButtons.Left)
                    {
                        if (!_map.Obstacles.Contains(currentIndex))
                            _map.Obstacles.Add(currentIndex);
                    }
                    else
                    {
                        _map.Obstacles.Remove(currentIndex);
                    }
                }
            }

            _isDirty = true;
            UpdateEditorTitle();
            panelCanvas.Invalidate();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var newForm = new FormRename {Text = "New"};
            if (newForm.ShowDialog(this) == DialogResult.OK)
            {
                _map.Name = newForm.NewName;
                UpdateEditorTitle();

                ResetCanvas(180, 180);
            }
        }

        private void UpdateEditorTitle()
        {
            var objectName = _map != null && !string.IsNullOrWhiteSpace(_map.Name) ? _map.Name : "Nonamed";
            var title = string.Format("{0} [{1}]", "Map Editor", objectName);
            if (_isDirty)
                title += " *";
            Text = title;
        }

        private void brushItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var clickedItem = (ToolStripMenuItem) sender;
            foreach (var item in brushToolStripMenuItem.DropDownItems.OfType<ToolStripMenuItem>())
                item.Checked = false;

            clickedItem.Checked = true;

            if (clickedItem == selectToolStripMenuItem) _brush = BrushMode.Select;
            else if (clickedItem == drawToolStripMenuItem) _brush = BrushMode.Draw;
            else if (clickedItem == obstacleToolStripMenuItem) _brush = BrushMode.Obstacle;
        }

        private enum BrushMode
        {
            Select,
            Draw,
            Obstacle
        }

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _showGrid = gridToolStripMenuItem.Checked;
            panelCanvas.Invalidate();
        }

        private void panelCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            _brushPressed = false;
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_magnificant <= 1)
                return;

            _magnificant--;
            ResizeCanvas();
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_magnificant >= 6)
                return;

            _magnificant++;
            ResizeCanvas();
        }

        private void treeObjects_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            _objectBrush = null;

            var objectXmlPath = (string) e.Node.Tag;
            if (!File.Exists(objectXmlPath))
                return;

            _objectBrush = MapObject.Load<MapObject>(objectXmlPath);
        }

        private void comboBoxTile_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPallet((string)comboBoxTile.SelectedItem);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_isDirty)
                AskToSaveObjectName(true);

            var openDialog = new OpenFileDialog
                {
                    Filter = "Map File (*.xml)|*.xml",
                    Title = "Open Map xml",
                    RestoreDirectory = true,
                    InitialDirectory = ResourcePath.MapRoot
                };
            if (openDialog.ShowDialog(this) == DialogResult.OK)
            {
                _map = MapObject.Load<WorldMap>(openDialog.FileName);

                ResizeCanvas();

                LoadPallet(_map.Pallet.FileName);
                UpdateEditorTitle();
                panelCanvas.Invalidate();
            }
        }


        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AskToSaveObjectName(false);
        }

        private void AskToSaveObjectName(bool needToAsk)
        {
            if (_map == null)
                return;

            if (needToAsk || string.IsNullOrWhiteSpace(_map.Name))
            {
                var newForm = new FormRename { Text = "Save", NewName = _map.Name };
                if (newForm.ShowDialog(this) != DialogResult.OK)
                    return;

                _map.Name = newForm.NewName;
                UpdateEditorTitle();
            }

            SaveCanvas();
        }

        private void SaveCanvas()
        {
            if (_map == null)
                return;

            _map.Save(ResourcePath.MapRoot);

            _isDirty = false;
            UpdateEditorTitle();
        }
    }
}