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
    public partial class FormObjectEditor : Form
    {
        #region Editor Context

        private BrushMode _brush = BrushMode.None;
        private int _canvasMouseX;
        private int _canvasMouseY;
        private bool _isDirty;
        private int _palletEndIndex;
        private bool _palletGrapped;
        private int _palletStartIndex;
        private Pallet _pallet;

        #endregion

        #region Object Context

        private MapObject _object;

        #endregion

        public FormObjectEditor()
        {
            InitializeComponent();
        }

        private void FormObjectEditor_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                return;

            MoveCanvasToCenter();
        }

        private void MoveCanvasToCenter()
        {
            var parentSize = panelCanvas.Parent.ClientSize;
            var canvasSize = panelCanvas.ClientSize;
            panelCanvas.Location = new Point(
                (parentSize.Width - canvasSize.Width) / 2,
                (parentSize.Height - canvasSize.Height) / 2
                );
        }

        private void panelCanvas_Paint(object sender, PaintEventArgs e)
        {
            if (_object != null)
                _object.Paint(e.Graphics, Point.Empty);

            // mouse hover
            var canvasCursor = GetCurrentCanvasPixelPosition();
            if (_brush == BrushMode.Draw)
            {
                // Create a new color matrix and set the alpha value to 0.5
                ColorMatrix cm = new ColorMatrix();
                cm.Matrix00 = cm.Matrix11 = cm.Matrix22 = cm.Matrix44 = 1;
                cm.Matrix33 = 0.5f;

                // Create a new image attribute object and set the color matrix to
                // the one just created
                ImageAttributes imageAttribute = new ImageAttributes();
                imageAttribute.SetColorMatrix(cm);

                if (canvasCursor.X >= 0 && canvasCursor.Y >= 0)
                {
                    var srcRect = GetPalletSelectedPixelRectangle();
                    var destRect = new Rectangle(canvasCursor.X, canvasCursor.Y, srcRect.Width, srcRect.Height);
                    e.Graphics.DrawImage(picturePallet.Image, destRect, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, imageAttribute);
                }
            }

            // grid
            DrawGrid(panelCanvas, e.Graphics);

            // cursor
            if (_brush != BrushMode.None)
            {
                e.Graphics.DrawRectangle(Pens.Red, canvasCursor.X, canvasCursor.Y, WorldConstants.TileLength - 1, WorldConstants.TileLength - 1);
            }
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
            var startXIndex = _palletStartIndex % _pallet.XCount;
            var startYIndex = _palletStartIndex / _pallet.XCount;
            var endXIndex = _palletEndIndex % _pallet.XCount;
            var endYIndex = _palletEndIndex / _pallet.XCount;
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
            _palletStartIndex = (mouseY / WorldConstants.TileLength) * _pallet.XCount + mouseX / WorldConstants.TileLength;
        }

        private void UpdatePalletEndIndex(int mouseX, int mouseY)
        {
            var x = mouseX / WorldConstants.TileLength;
            var y = mouseY / WorldConstants.TileLength;
            if (x < 0) x = 0;
            else if (x >= _pallet.XCount) x = _pallet.XCount - 1;
            if (y < 0) y = 0;
            else if (y >= _pallet.YCount) y = _pallet.YCount - 1;

            _palletEndIndex = y * _pallet.XCount + x;
        }

        private void splitBrowser_Panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_object == null)
                return;

            var resizeForm = new FormResize {X = _object.X, Y = _object.Y};
            if (resizeForm.ShowDialog(this) == DialogResult.OK)
            {
                _object.Resize(resizeForm.X, resizeForm.Y);

                ResizeCanvas();
            }
        }

        private void FormObjectEditor_Load(object sender, EventArgs e)
        {
            LoadTilesetFiles();
            LoadObjectFiles();

            typeof (Panel).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                          .SetValue(panelCanvas, true);

            UpdateEditorTitle();
            ResetCanvas(10, 10);
        }

        private void ResetCanvas(int canvasXCount, int canvasYCount)
        {
            if (_object == null)
                _object = new MapObject(_pallet.FileName);

            _object.Resize(canvasXCount, canvasYCount);
            _object.Clear();

            _isDirty = false;

            ResizeCanvas();
        }

        private void ResizeCanvas()
        {
            if (_object == null)
                return;

            panelCanvas.Size = new Size(_object.X * WorldConstants.TileLength,
                                        _object.Y * WorldConstants.TileLength);
            panelCanvas.Invalidate();
            MoveCanvasToCenter();
        }

        private void LoadTilesetFiles()
        {
            var tilesetRoot = new TreeNode("Tilesets", 1, 1,
                                           Directory.GetFiles(ResourcePath.TilesetRoot, "*.png")
                                                    .Select(
                                                        e =>
                                                        new TreeNode(Path.GetFileNameWithoutExtension(e), 3, 3)
                                                            {
                                                                Tag = e
                                                            })
                                                    .ToArray());
            treeResources.Nodes.Clear();
            treeResources.Nodes.Add(tilesetRoot);

            LoadPallet(tilesetRoot.FirstNode);
            tilesetRoot.ExpandAll();
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

        private void LoadPallet(TreeNode resourceNode)
        {
            LoadPallet(resourceNode.Text + ".png");
        }

        private void LoadPallet(string tilesetName)
        {
            _pallet = Pallet.Get(tilesetName);
            picturePallet.Image = _pallet.Image;
        }

        private void treeResources_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            LoadPallet(e.Node);

            panelCanvas.Invalidate();
        }

        private void panelCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var mouseX = e.X / WorldConstants.TileLength;
            var mouseY = e.Y / WorldConstants.TileLength;

            if (_canvasMouseX == mouseX && _canvasMouseY == mouseY)
                return;

            _canvasMouseX = mouseX;
            _canvasMouseY = mouseY;

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
            if (_object == null || _canvasMouseX < 0 || _canvasMouseY < 0 || _canvasMouseX >= _object.X ||
                _canvasMouseY >= _object.Y)
                return new Point(-1, -1);

            return new Point(_canvasMouseX, _canvasMouseY);
        }

        private void panelCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (_brush == BrushMode.Draw)
            {
                var selectedIndexRect = GetPalletSelectedIndexRectangle();
                foreach (var dy in Enumerable.Range(0, selectedIndexRect.Height + 1))
                {
                    var y = _canvasMouseY + dy;
                    if (y < 0 || y >= _object.Y)
                        continue;

                    foreach (var dx in Enumerable.Range(0, selectedIndexRect.Width + 1))
                    {
                        var x = _canvasMouseX + dx;
                        if (x < 0 || x >= _object.X)
                            continue;

                        if (e.Button == MouseButtons.Left)
                            _object.Tiles[y, x] = (selectedIndexRect.Top + dy) * _pallet.XCount +
                                                 (selectedIndexRect.Left + dx);
                        else if (e.Button == MouseButtons.Right)
                            _object.Tiles[y, x] = -1;
                    }
                }
            }

            var currentIndex = GetCurrentCanvasIndexPosition();
            if (currentIndex.X >= 0 && currentIndex.Y >= 0)
            {
                if (_brush == BrushMode.Remove)
                {
                    _object.Tiles[currentIndex.Y, currentIndex.X] = -1;
                }
                else if (_brush == BrushMode.Origin)
                {
                    _object.Origin = currentIndex;
                }
                else if (_brush == BrushMode.Obstacle)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        if (!_object.Obstacles.Contains(currentIndex))
                            _object.Obstacles.Add(currentIndex);
                    }
                    else
                    {
                        _object.Obstacles.Remove(currentIndex);
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
                _object.Name = newForm.NewName;
                UpdateEditorTitle();

                ResetCanvas(10, 10);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AskToSaveObjectName();
        }

        private bool AskToSaveObjectName()
        {
            if (_object == null)
                return false;

            if (string.IsNullOrWhiteSpace(_object.Name))
            {
                var newForm = new FormRename {Text = "Save"};
                if (newForm.ShowDialog(this) != DialogResult.OK)
                    return false;

                _object.Name = newForm.NewName;
                UpdateEditorTitle();
            }

            SaveCanvas();
            return true;
        }

        private void SaveCanvas()
        {
            if (_object == null)
                return;

            _object.Save(ResourcePath.ObjectRoot);

            _isDirty = false;

            LoadObjectFiles();
            UpdateEditorTitle();
        }

        private void UpdateEditorTitle()
        {
            var objectName = _object != null && !string.IsNullOrWhiteSpace(_object.Name) ? _object.Name : "Nonamed";
            var title = string.Format("{0} [{1}]", "Object Editor", objectName);
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

            if (clickedItem == noneToolStripMenuItem) _brush = BrushMode.None;
            else if (clickedItem == drawToolStripMenuItem) _brush = BrushMode.Draw;
            else if (clickedItem == removeToolStripMenuItem) _brush = BrushMode.Remove;
            else if (clickedItem == originToolStripMenuItem) _brush = BrushMode.Origin;
            else if (clickedItem == obstacleToolStripMenuItem) _brush = BrushMode.Obstacle;
        }

        private enum BrushMode
        {
            None,
            Draw,
            Remove,
            Origin,
            Obstacle
        }

        private void treeObjects_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == treeObjects.Nodes[0])
                return;

            if (_isDirty && !AskToSaveObjectName())
                return;

            var objectXmlPath = Path.Combine(ResourcePath.ObjectRoot, e.Node.Text + ".xml");
            if (!File.Exists(objectXmlPath))
                return;

            _object = MapObject.Load<MapObject>(objectXmlPath);
            ResizeCanvas();

            LoadPallet(_object.Pallet.FileName);
            UpdateEditorTitle();
            panelCanvas.Invalidate();
        }
    }
}