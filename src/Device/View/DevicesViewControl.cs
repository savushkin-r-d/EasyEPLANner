using BrightIdeasSoftware;
using EasyEPlanner.Devices.ViewModel;
using EasyEPlanner.Devices.ViewModel.ViewInterface;
using EasyEPlanner;
using EditorControls;
using Eplan.EplApi.DataModel;
using EplanDevice;
using IO.View;
using IO.ViewModel;
using StaticHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EasyEPlanner.Devices.View
{
    public partial class DevicesViewControl : Form
    {
        private const string SearchPlaceholder = "Поиск...";
        private bool cancelChanges;
        private bool isCellEditing;
        private string searchText = string.Empty;
        private System.Windows.Forms.Timer textBoxSearchTypingTimer;
        private bool updatingModelFilter;
        private TextBox textBoxCellEditor;
        private ComboBox comboBoxCellEditor;
        private ToolStripMenuItem goToFsaMenuItem;
        private SearchIterator searchIterator;
        private bool runtimeInitialized;

        public static DevicesViewControl Instance { get; private set; }

        public static IDevicesViewModel DataContext { get; private set; }

        public static void Start()
        {
            DataContext = new DevicesViewModel(DeviceManager.GetInstance());
            if (Instance is null || Instance.IsDisposed)
            {
                Instance = new DevicesViewControl();
            }

            Instance.EnsureRuntimeInitialized();
            Instance.InitDataDevicesTree();
            Instance.Show();
            Instance.BringToFront();
            Instance.WindowState = FormWindowState.Normal;
        }

        public void Clear()
        {
            DataContext = new DevicesViewModel(null);
            devicesTree.BeginUpdate();
            devicesTree.ClearObjects();
            devicesTree.EndUpdate();
        }

        public void RebuildTree()
        {
            EnsureRuntimeInitialized();
            DataContext.RebuildTree();
            InitDataDevicesTree();
        }

        public void RefreshTree()
        {
            if (devicesTree.Items.Count == 0)
            {
                devicesTree.BeginUpdate();
                devicesTree.ClearObjects();
                devicesTree.EndUpdate();
            }
            else
            {
                devicesTree.FocusedObject = devicesTree.SelectedObject;
                devicesTree.RefreshObjects(DataContext.Roots.Cast<object>().ToList());
            }
        }

        public DevicesViewControl()
        {
            InitializeComponent();
            if (!IsInDesignEnvironment())
            {
                EnsureRuntimeInitialized();
            }
        }

        private static bool IsInDesignEnvironment()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return true;

            try
            {
                var processName = Process.GetCurrentProcess().ProcessName;
                if (string.Equals(processName, "devenv",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                // WinForms designer can run out-of-process in this host.
                if (processName.IndexOf("DesignToolsServer",
                    StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }

                var domainName = AppDomain.CurrentDomain.FriendlyName ?? string.Empty;
                return domainName.IndexOf("DesignToolsServer",
                    StringComparison.OrdinalIgnoreCase) >= 0;
            }
            catch
            {
                return false;
            }
        }

        private void EnsureRuntimeInitialized()
        {
            if (runtimeInitialized)
                return;

            runtimeInitialized = true;
            DevicesIconFactory.Populate(ViewItemImageList);
            InitDevicesTree();
            InitSearch();
            InitContextMenu();
        }

        private static string GetToolTipForNameColumn(object obj)
        {
            // For device rows we always show the name tooltip,
            // even when text equals visible cell content.
            if (obj is DevicesDeviceNode deviceNode)
                return deviceNode.Name ?? string.Empty;

            if (obj is not IToolTip toolTip || string.IsNullOrWhiteSpace(toolTip.Name))
                return string.Empty;

            var displayName = (obj as IViewItem)?.Name;
            if (displayName is not null &&
                string.Equals(toolTip.Name, displayName, StringComparison.Ordinal))
            {
                return string.Empty;
            }

            return toolTip.Name;
        }

        private static string GetToolTipForValueColumn(object obj)
        {
            if (obj is IToolTip toolTip)
                return toolTip.Description ?? string.Empty;

            return (obj as IViewItem)?.Description ?? string.Empty;
        }

        private void InitDevicesTree()
        {
            devicesTree.CanExpandGetter = obj =>
                obj is IExpandable exp && (exp.Items?.Any() ?? false);
            devicesTree.ChildrenGetter = obj => (obj as IExpandable)?.Items;

            devicesTree.CellToolTipGetter = (col, obj) => col.Index switch
            {
                0 => GetToolTipForNameColumn(obj),
                1 => GetToolTipForValueColumn(obj),
                _ => string.Empty,
            };

            var nameColumn = new OLVColumn("Название", nameof(IViewItem.Name))
            {
                ImageGetter = obj => (int)((obj as IHasDevicesIcon)?.Icon ?? DevicesIcon.None),
                AspectGetter = obj => (obj as IViewItem)?.Name,
                IsEditable = false,
                Sortable = false,
            };

            var valueColumn = new OLVColumn("Значение", nameof(IViewItem.Description))
            {
                ImageGetter = obj => (int)((obj as IHasDevicesDescriptionIcon)?.DescriptionIcon
                    ?? DevicesIcon.None),
                AspectGetter = obj => (obj as IViewItem)?.Description,
                IsEditable = true,
                Sortable = false,
                MinimumWidth = 100,
            };

            devicesTree.Columns.Add(nameColumn);
            devicesTree.Columns.Add(valueColumn);

            devicesTree.UseAlternatingBackColors = true;
            devicesTree.AlternateRowBackColor = Color.FromArgb(250, 250, 250);
            devicesTree.RowHeight = 20;

            devicesTree.ModelFilter = new ModelFilter(obj =>
                obj is not IFilterableViewItem item ||
                item.Filter(searchText, hideEmptyItems: false));
        }

        private void InitSearch()
        {
            searchIterator = new SearchIterator
            {
                Dock = DockStyle.Fill,
            };
            searchIteratorPanel.Controls.Add(searchIterator);

            searchIterator.IndexChanged += SearchIterator_IndexChanged;
            searchIterator.SearchSettingsChanged += UpdateModelFilter;
        }

        private void InitContextMenu()
        {
            var menu = new ContextMenuStrip(components);
            goToFsaMenuItem = new ToolStripMenuItem("Перейти на ФСА")
            {
                Image = Properties.Resources.devicesTreeGoToFsa,
            };
            goToFsaMenuItem.Click += GoToFsaMenuItem_Click;
            menu.Items.Add(goToFsaMenuItem);
            menu.Opening += ContextMenu_Opening;
            devicesTree.ContextMenuStrip = menu;
        }

        private void InitDataDevicesTree()
        {
            devicesTree.BeginUpdate();
            devicesTree.Roots = DataContext.Roots.Cast<object>();
            devicesTree.Columns[0].Width = 220;
            devicesTree.Columns[1].Width = 180;
            devicesTree.Expand(DataContext.Root);
            RestoreExpanded(DataContext.Roots.Cast<IExpandable>());
            devicesTree.SelectObject(DataContext.Root, true);
            devicesTree.EnsureModelVisible(DataContext.Root);
            devicesTree.EndUpdate();
            AutoResizeColumns(devicesTree);
            UpdateGroupingButtonText();
            UpdateModelFilter();
        }

        private static void RestoreExpanded(IEnumerable<IExpandable> items)
        {
            foreach (var item in items)
            {
                if (item.Expanded)
                    Instance?.devicesTree.Expand(item);
                if (item.Items is not null)
                    RestoreExpanded(item.Items.OfType<IExpandable>());
            }
        }

        private void Expand_Click(object sender, EventArgs e)
        {
            int level = Convert.ToInt32((sender as ToolStripMenuItem).Tag);
            devicesTree.BeginUpdate();
            devicesTree.Expanded -= ItemExpanded;
            devicesTree.Collapsed -= ItemCollapsed;
            devicesTree.SelectedIndex = 0;
            ExpandToLevel(level, devicesTree.Objects);
            devicesTree.Expanded += ItemExpanded;
            devicesTree.Collapsed += ItemCollapsed;
            devicesTree.EnsureModelVisible(devicesTree.SelectedObject);
            devicesTree.EndUpdate();
            AutoResizeColumns(devicesTree);
        }

        private void ExpandToLevel(int level, IEnumerable items)
        {
            foreach (var item in items.OfType<IExpandable>())
            {
                if (level > 0 && !devicesTree.IsExpanded(item))
                {
                    devicesTree.Expand(item);
                    item.Expanded = true;
                }

                if (level == 0 && devicesTree.IsExpanded(item))
                {
                    devicesTree.Collapse(item);
                    item.Expanded = false;
                }

                ExpandToLevel(level > 0 ? level - 1 : 0, item.Items ?? Array.Empty<IViewItem>());
            }
        }

        private void SyncButton_Click(object sender, EventArgs e)
        {
            EProjectManager.GetInstance().SyncAndSave(false);
            Editor.Editor.GetInstance().EditorForm.RefreshTree();
            DFrm.GetInstance().RefreshTree();
            IOViewControl.Instance?.RebuildTree();
            RebuildTree();
        }

        private void GroupingToggleButton_Click(object sender, EventArgs e)
        {
            DataContext.GroupingMode = groupingToggleButton.Checked
                ? DevicesGroupingMode.ObjectThenType
                : DevicesGroupingMode.TypeThenObject;
            RebuildTree();
        }

        private void UpdateGroupingButtonText()
        {
            groupingToggleButton.Text = string.Empty;
            groupingToggleButton.ToolTipText = DataContext.GroupingMode is DevicesGroupingMode.ObjectThenType
                ? "Тип → Объект"
                : "Объект → Тип";
            groupingToggleButton.Image = DataContext.GroupingMode is DevicesGroupingMode.ObjectThenType
                ? Properties.Resources.devicesGroupingObjectType
                : Properties.Resources.devicesGroupingTypeObject;
            groupingToggleButton.Checked =
                DataContext.GroupingMode is DevicesGroupingMode.ObjectThenType;
        }

        private void ItemExpanded(object sender, TreeBranchExpandedEventArgs e)
        {
            if (e.Model is IExpandable expandable)
                expandable.Expanded = true;
            AutoResizeColumns(devicesTree);
        }

        private void ItemCollapsed(object sender, TreeBranchCollapsedEventArgs e)
        {
            if (e.Model is IExpandable expandable)
                expandable.Expanded = false;
            AutoResizeColumns(devicesTree);
        }

        private static void AutoResizeColumns(TreeListView tree)
        {
            if (tree is null)
                return;

            foreach (ColumnHeader column in tree.Columns)
                column.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void CellEditStarting(object sender, CellEditEventArgs e)
        {
            if (e.Column.Index != 1)
            {
                e.Cancel = true;
                return;
            }

            if (devicesTree.SelectedObject is IComboBoxEditable comboItem)
            {
                isCellEditing = true;
                InitComboBoxCellEditor(comboItem.ComboBoxItems, comboItem.Value, e.CellBounds);
                e.Control = comboBoxCellEditor;
                comboBoxCellEditor.Focus();
                devicesTree.Freeze();
                return;
            }

            if (devicesTree.SelectedObject is not IEditable editable)
            {
                e.Cancel = true;
                return;
            }

            isCellEditing = true;
            InitTextBoxCellEditor(editable.Value, e.CellBounds);
            e.Control = textBoxCellEditor;
            textBoxCellEditor.Focus();
            devicesTree.Freeze();
        }

        private void CellEditFinishing(object sender, CellEditEventArgs e)
        {
            isCellEditing = false;

            if (cancelChanges ||
                devicesTree.SelectedObject is not IEditable editable)
            {
                e.Cancel = true;
                cancelChanges = false;
                devicesTree.Unfreeze();
                return;
            }

            if (comboBoxCellEditor is not null)
                devicesTree.Controls.Remove(comboBoxCellEditor);
            if (textBoxCellEditor is not null)
                devicesTree.Controls.Remove(textBoxCellEditor);

            var modified = editable.SetValue(e.NewValue?.ToString() ?? string.Empty);
            if (modified)
                RefreshTree();

            e.Cancel = true;
            devicesTree.Unfreeze();
        }

        private static Rectangle GetCellEditorBounds(TreeListView tree, Rectangle bounds)
        {
            int height = tree.RowHeight > 0 ? tree.RowHeight : 20;
            return new Rectangle(bounds.X, bounds.Y, bounds.Width, height);
        }

        private void InitTextBoxCellEditor(string text, Rectangle bounds)
        {
            textBoxCellEditor = new TextBox
            {
                Text = text,
                Bounds = GetCellEditorBounds(devicesTree, bounds),
                BorderStyle = BorderStyle.FixedSingle,
                Font = devicesTree.Font,
            };
            textBoxCellEditor.LostFocus += CellEditor_LostFocus;
            textBoxCellEditor.KeyDown += CellEditor_KeyDown;
            devicesTree.Controls.Add(textBoxCellEditor);
        }

        private void InitComboBoxCellEditor(
            IEnumerable<string> items,
            string text,
            Rectangle bounds)
        {
            comboBoxCellEditor = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Bounds = GetCellEditorBounds(devicesTree, bounds),
                Font = devicesTree.Font,
                IntegralHeight = false,
                FlatStyle = FlatStyle.Flat,
            };
            comboBoxCellEditor.Items.AddRange(items.Cast<object>().ToArray());
            comboBoxCellEditor.Text = text;
            comboBoxCellEditor.LostFocus += CellEditor_LostFocus;
            comboBoxCellEditor.KeyDown += CellEditor_KeyDown;
            devicesTree.Controls.Add(comboBoxCellEditor);
        }

        private void CellEditor_LostFocus(object sender, EventArgs e)
        {
            if (isCellEditing)
                devicesTree.FinishCellEdit();
        }

        private void CellEditor_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    devicesTree.FinishCellEdit();
                    break;
                case Keys.Escape:
                    cancelChanges = true;
                    devicesTree.FinishCellEdit();
                    break;
            }
        }

        private void DevicesTree_FormatCell(object sender, FormatCellEventArgs e)
        {
            if (e.Model is IBoldName)
                e.Item.Font = new Font(devicesTree.Font, FontStyle.Bold);
        }

        private void DevicesTree_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && isCellEditing)
            {
                cancelChanges = true;
                devicesTree.FinishCellEdit();
            }
        }

        private void DevicesTree_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle ||
                (e.Button == MouseButtons.Left && ModifierKeys.HasFlag(Keys.Control)))
            {
                GoToFsaAt(e.Location);
                return;
            }

            if (e.Button != MouseButtons.Right)
                return;

            var item = devicesTree.GetItemAt(e.X, e.Y) as OLVListItem;
            if (item != null && !item.Selected)
                devicesTree.SelectedObject = item.RowObject;
        }

        private void ContextMenu_Opening(object sender, CancelEventArgs e)
        {
            goToFsaMenuItem.Enabled = TryGetSelectedEplanFunction(out _);
        }

        private void GoToFsaMenuItem_Click(object sender, EventArgs e)
        {
            if (!TryGetSelectedEplanFunction(out var function))
                return;

            OpenFunctionPageWithError(function);
        }

        private void GoToFsaAt(Point location)
        {
            var rowObject = (devicesTree.GetItemAt(location.X, location.Y) as OLVListItem)
                ?.RowObject;
            if (!TryGetEplanFunction(rowObject, out var function))
                return;

            OpenFunctionPageWithError(function);
        }

        private static void OpenFunctionPageWithError(Function function)
        {
            try
            {
                EplanNavigateHelper.OpenFunctionPage(function);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Переход на ФСА",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool TryGetSelectedEplanFunction(out Function function)
        {
            function = null;
            var selected = devicesTree.SelectedObjects?.Cast<object>().ToList() ?? new List<object>();
            return selected.Count == 1 && TryGetEplanFunction(selected[0], out function);
        }

        private static bool TryGetEplanFunction(object viewObject, out Function function)
        {
            function = null;
            if (viewObject is not IGoToFsa goToFsa)
                return false;

            return EplanNavigateHelper.TryGetFunction(goToFsa.EplanFunction, out function);
        }

        private void SearchTSButton_Click(object sender, EventArgs e)
        {
            searchButtonToolStrip.Visible = false;
            searchBoxTLP.Visible = true;

            textBox_search.Focus();
        }

        private void TextBox_search_TextChanged(object sender, EventArgs e)
        {
            if (textBox_search.Text is SearchPlaceholder or "")
            {
                if (searchIterator is not null)
                    searchIterator.Maximum = 0;
                searchText = string.Empty;
                UpdateModelFilter();
                return;
            }

            textBoxSearchTypingTimer ??= new System.Windows.Forms.Timer { Interval = 500 };
            textBoxSearchTypingTimer.Tick -= TextBoxSearchTypingTimer_Tick;
            textBoxSearchTypingTimer.Tick += TextBoxSearchTypingTimer_Tick;
            textBoxSearchTypingTimer.Stop();
            textBoxSearchTypingTimer.Tag = textBox_search.Text;
            textBoxSearchTypingTimer.Start();
        }

        private void TextBoxSearchTypingTimer_Tick(object sender, EventArgs e)
        {
            textBoxSearchTypingTimer?.Stop();
            searchText = textBoxSearchTypingTimer?.Tag?.ToString() ?? string.Empty;
            UpdateModelFilter();
        }

        private void TextBox_search_Enter(object sender, EventArgs e)
        {
            if (textBox_search.Text == SearchPlaceholder)
            {
                textBox_search.ForeColor = Color.Black;
                textBox_search.Text = string.Empty;
            }
        }

        private void TextBox_search_Leave(object sender, EventArgs e)
        {
            if (searchIterator?.SettingsButtonsFocused == true)
            {
                textBox_search.Focus();
                return;
            }

            if (textBox_search.Text == string.Empty && !updatingModelFilter)
            {
                textBox_search.ForeColor = Color.Gray;
                textBox_search.Text = SearchPlaceholder;
                searchBoxTLP.Visible = false;
                searchButtonToolStrip.Visible = true;
            }
        }

        private void UpdateModelFilter()
        {
            updatingModelFilter = true;
            var searchBoxWasFocused = textBox_search.Focused;

            devicesTree.UseFiltering = false;
            DataContext.SearchContext.FoundItems.Clear();
            ResetFilter(DataContext.Roots.Cast<IFilterableViewItem>());

            TextMatchFilter highlightingFilter = null;
            if (!string.IsNullOrEmpty(searchText))
            {
                devicesTree.UseFiltering = true;
                searchIterator.Maximum = DataContext.SearchContext.FoundItems.Count;
                highlightingFilter = TextMatchFilter.Contains(devicesTree, searchText);
            }
            else
            {
                searchIterator.Maximum = 0;
            }

            devicesTree.DefaultRenderer = highlightingFilter is null
                ? null
                : new HighlightTextRenderer(highlightingFilter)
                {
                    FillBrush = new SolidBrush(Color.LightGreen),
                    FramePen = new Pen(Color.DarkGreen),
                };

            if (searchBoxWasFocused)
                textBox_search.Focus();

            updatingModelFilter = false;
        }

        private static void ResetFilter(IEnumerable<IFilterableViewItem> items)
        {
            foreach (var item in items)
            {
                item.ResetFilter();
                if (item is IExpandable expandable && expandable.Items is not null)
                {
                    ResetFilter(expandable.Items.OfType<IFilterableViewItem>());
                }
            }
        }

        private void SearchIterator_IndexChanged(object sender, int index)
        {
            var item = DataContext.SearchContext.FoundItems.ElementAtOrDefault(index - 1);
            if (item is null)
                return;

            RecursiveExpandParent(item);
            if (item is IExpandable expandable && devicesTree.CanExpand(expandable))
                devicesTree.Expand(expandable);

            devicesTree.SelectObject(item, true);
            devicesTree.EnsureModelVisible(item);
        }

        private void RecursiveExpandParent(IFilterableViewItem item)
        {
            if (item is not FilterableViewItemBase node || node.ParentItem is not IExpandable parent)
                return;

            if (node.ParentItem is IFilterableViewItem parentFilterable)
                RecursiveExpandParent(parentFilterable);

            if (devicesTree.CanExpand(parent))
                devicesTree.Expand(parent);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveCfg();
            base.OnFormClosing(e);
        }
    }
}
