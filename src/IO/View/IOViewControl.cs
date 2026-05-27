using BrightIdeasSoftware;
using EasyEPlanner;
using Editor;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.Scripting;
using IO.ViewModel;
using PInvoke;
using StaticHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IO.View
{
    public partial class IOViewControl : Form
    {
        private bool cancelChanges = false;

        private bool isCellEditing = false;

        private ToolStripMenuItem shiftModulesToolStripMenuItem;

        private ToolStripMenuItem deleteUndefinedModuleToolStripMenuItem;

        private ToolStripMenuItem goToFsaToolStripMenuItem;

        private class DraggedModule
        {
            public DeletedModule DeletedModule { get; set; }
        }

        public static IOViewControl Instance { get; private set; }

        /// <summary>
        /// Модель представления
        /// </summary>
        public static IIOViewModel DataContext { get; private set; }

        /// <summary>
        /// Запуск окна из меню
        /// </summary>
        public static void Start()
        {
            DataContext ??= new IOViewModel(IOManager.GetInstance());
            Instance ??= new IOViewControl(DataContext);

            Instance.InitDataStructPLC();
            Instance.ShowDlg();
        }

        public void Clear()
        {
            DataContext = new IOViewModel(null);
            InitDataStructPLC();
        }

        private IOViewControl(IIOViewModel viewModel)
        {
            InitializeComponent();
            InitStructPLC();
            InitContextMenu();
        }

        private void InitStructPLC()
        {
            StructPLC.CanExpandGetter = obj => obj is IExpandable exp && (exp.Items?.Any() ?? false);
            StructPLC.ChildrenGetter = obj => (obj as IExpandable)?.Items;
            StructPLC.CellToolTipGetter = (col, obj) => (col.Index) switch
            {
                0 => (obj as IToolTip)?.Name ?? (obj as IViewItem).Name,
                1 => (obj as IToolTip)?.Description ?? (obj as IViewItem).Description,
                _ => ""
            };


            var firstColumn = new OLVColumn("Название", nameof(IViewItem.Name))
            {
                ImageGetter = obj => (obj is IHasIcon item)? (int)item.Icon : -1,
                AspectGetter = obj => (obj as IViewItem).Name,
                IsEditable = true,
                Sortable = false,
            };

            var secondColumn = new OLVColumn("Описание", nameof(IViewItem.Description))
            {
                ImageGetter = obj => (obj is IHasDescriptionIcon item) ? (int)item.Icon : -1,
                AspectGetter = obj => (obj as IViewItem).Description,
                IsEditable = true,
                Sortable = false,
            };


            StructPLC.Columns.Add(firstColumn);
            StructPLC.Columns.Add(secondColumn);
        }

        private void InitContextMenu()
        {
            var contextMenuStrip = new ContextMenuStrip(components);

            shiftModulesToolStripMenuItem =
                new ToolStripMenuItem("Сдвинуть модули")
                {
                    Image = global::EasyEPlanner.Properties.Resources.movedown
                };
            shiftModulesToolStripMenuItem.Click += ShiftModules_Click;

            goToFsaToolStripMenuItem =
                new ToolStripMenuItem("Перейти на ФСА")
                {
                    Image = global::EasyEPlanner.Properties.Resources.highlight
                };
            goToFsaToolStripMenuItem.Click += GoToFsa_Click;

            deleteUndefinedModuleToolStripMenuItem =
                new ToolStripMenuItem("Удалить")
                {
                    Image = global::EasyEPlanner.Properties.Resources.delete,
                    ShortcutKeys = Keys.Delete
                };
            deleteUndefinedModuleToolStripMenuItem.Click +=
                DeleteUndefinedModule_Click;

            contextMenuStrip.Items.Add(shiftModulesToolStripMenuItem);
            contextMenuStrip.Items.Add(goToFsaToolStripMenuItem);
            contextMenuStrip.Items.Add(deleteUndefinedModuleToolStripMenuItem);
            contextMenuStrip.Opening += StructPLCContextMenu_Opening;

            StructPLC.ContextMenuStrip = contextMenuStrip;
            StructPLC.MouseDown += StructPLC_MouseDown;
            StructPLC.AllowDrop = true;
            StructPLC.ItemDrag += StructPLC_ItemDrag;
            StructPLC.DragOver += StructPLC_DragOver;
            StructPLC.DragDrop += StructPLC_DragDrop;
        }

        private void StructPLC_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if ((e.Item as OLVListItem)?.RowObject is DeletedModule
                deletedModule &&
                IsOnlySelectedObject(deletedModule) &&
                deletedModule.IOModule.Function?.IsValid == true)
            {
                DoDragDrop(new DataObject(typeof(DraggedModule).FullName,
                    new DraggedModule { DeletedModule = deletedModule }),
                    DragDropEffects.Move);
            }
        }

        private void StructPLC_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = CanDropModule(e) ?
                DragDropEffects.Move : DragDropEffects.None;
        }

        private void StructPLC_DragDrop(object sender, DragEventArgs e)
        {
            if (!TryGetDragDropModules(e, out var draggedModule,
                out var targetModule))
            {
                return;
            }

            try
            {
                InsertDeletedModule(draggedModule.DeletedModule.IOModule,
                    targetModule);

                EProjectManager.GetInstance().SyncAndSave(false);

                Editor.Editor.GetInstance().EditorForm.RefreshTree();
                DFrm.GetInstance().RefreshTree();

                RebuildTree();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Перемещение модуля",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool CanDropModule(DragEventArgs e)
        {
            return TryGetDragDropModules(e, out var draggedModule,
                out var targetModule) &&
                CanDropModule(draggedModule, targetModule);
        }

        private bool TryGetDragDropModules(DragEventArgs e,
            out DraggedModule draggedModule, out IModule targetModule)
        {
            draggedModule = null;
            targetModule = null;

            if (!e.Data.GetDataPresent(typeof(DraggedModule).FullName))
            {
                return false;
            }

            draggedModule = e.Data.GetData(typeof(DraggedModule).FullName)
                as DraggedModule;
            var point = StructPLC.PointToClient(new Point(e.X, e.Y));
            targetModule = (StructPLC.GetItemAt(point.X, point.Y) as
                OLVListItem)?.RowObject as IModule;

            return draggedModule != null && targetModule != null;
        }

        private static bool CanDropModule(DraggedModule draggedModule,
            IModule targetModule)
        {
            return draggedModule.DeletedModule?.IOModule.Function?.IsValid ==
                true &&
                targetModule.IOModule.Function?.IsValid != true;
        }

        private bool IsOnlySelectedObject(object rowObject)
        {
            var selectedObjects = StructPLC.SelectedObjects?.Cast<object>()
                .ToList() ?? new List<object>();

            return selectedObjects.Count == 1 &&
                ReferenceEquals(selectedObjects[0], rowObject);
        }

        private static bool CanMoveModule(IModule sourceModule,
            IModule targetModule)
        {
            return sourceModule != targetModule &&
                sourceModule.IONode == targetModule.IONode &&
                sourceModule.IOModule.Function?.IsValid == true;
        }

        private void StructPLC_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            var item = StructPLC.GetItemAt(e.X, e.Y) as OLVListItem;
            if (item != null && !item.Selected)
            {
                StructPLC.SelectedObject = item.RowObject;
            }
        }

        private void StructPLCContextMenu_Opening(object sender, CancelEventArgs e)
        {
            var selectedModules = GetSelectedModules().ToList();

            shiftModulesToolStripMenuItem.Enabled =
                selectedModules.Count == 1 &&
                selectedModules[0].IOModule.Function?.IsValid == true;
            goToFsaToolStripMenuItem.Enabled =
                TryGetSelectedEplanFunction(out _);
            deleteUndefinedModuleToolStripMenuItem.Enabled =
                selectedModules.Any();
        }

        private void GoToFsa_Click(object sender, EventArgs e)
        {
            if (!TryGetSelectedEplanFunction(out var function))
            {
                return;
            }

            try
            {
                OpenFunctionPage(function);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Переход на ФСА",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void OpenFunctionPage(Function function)
        {
            var action = new Eplan.EplApi.ApplicationFramework
                .ActionManager().FindAction("edit");
            if (action == null)
            {
                throw new InvalidOperationException(
                    "Не найдена команда EPLAN для открытия страницы.");
            }

            var context = new Eplan.EplApi.ApplicationFramework
                .ActionCallingContext();
            context.AddParameter("PAGENAME", function.Page.Name);

            if (function.Category != Function.Enums.Category.PLCTerminal)
            {
                context.AddParameter("DEVICENAME", function.VisibleName);
            }

            action.Execute(context);
        }

        private void ShiftModules_Click(object sender, EventArgs e)
        {
            if (StructPLC.SelectedObject is not IModule module ||
                !TryGetShiftValue(out int shiftValue))
            {
                return;
            }

            try
            {
                ShiftModulesFrom(module, shiftValue);

                EProjectManager.GetInstance().SyncAndSave(false);

                Editor.Editor.GetInstance().EditorForm.RefreshTree();
                DFrm.GetInstance().RefreshTree();

                RebuildTree();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Сдвиг модулей",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteUndefinedModule_Click(object sender, EventArgs e)
        {
            DeleteSelectedUndefinedModule();
        }

        private void DeleteSelectedUndefinedModule()
        {
            var modules = GetSelectedModules().ToList();
            if (!modules.Any())
            {
                return;
            }

            try
            {
                DeleteModules(modules);

                EProjectManager.GetInstance().SyncAndSave(false);

                Editor.Editor.GetInstance().EditorForm.RefreshTree();
                DFrm.GetInstance().RefreshTree();

                RebuildTree();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Удаление модуля",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private IEnumerable<IModule> GetSelectedModules()
        {
            return StructPLC.SelectedObjects?.OfType<IModule>() ??
                Enumerable.Empty<IModule>();
        }

        private bool TryGetSelectedEplanFunction(out Function function)
        {
            function = null;

            var selectedObjects = StructPLC.SelectedObjects?.Cast<object>()
                .ToList() ?? new List<object>();
            if (selectedObjects.Count != 1)
            {
                return false;
            }

            IEplanFunction eplanFunction = selectedObjects[0] switch
            {
                INode node => node.IONode.Function,
                IModule module => module.IOModule.Function,
                IClamp clamp => clamp.ClampFunction,
                DeletedModule deletedModule => deletedModule.IOModule.Function,
                _ => null
            };

            if (eplanFunction is not StaticHelper.EplanFunction functionWrapper ||
                functionWrapper.Function?.IsValid != true ||
                functionWrapper.Function.Page is null)
            {
                return false;
            }

            function = functionWrapper.Function;
            return true;
        }

        private static bool TryGetShiftValue(out int shiftValue)
        {
            const int DefaultShiftValue = 1;
            const int MaxShiftValue = 99;

            using var form = new Form
            {
                Text = "Сдвинуть модули",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false,
                ShowInTaskbar = false,
                ClientSize = new Size(260, 100)
            };

            var label = new System.Windows.Forms.Label
            {
                Text = "Сдвинуть на:",
                AutoSize = true,
                Location = new Point(12, 15)
            };

            var countInput = new NumericUpDown
            {
                Minimum = 1,
                Maximum = MaxShiftValue,
                Value = DefaultShiftValue,
                Location = new Point(145, 12),
                Width = 95
            };

            var okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(84, 60),
                Width = 75
            };

            var cancelButton = new Button
            {
                Text = "Отмена",
                DialogResult = DialogResult.Cancel,
                Location = new Point(165, 60),
                Width = 75
            };

            form.Controls.AddRange(new Control[]
            {
                label,
                countInput,
                okButton,
                cancelButton
            });

            form.AcceptButton = okButton;
            form.CancelButton = cancelButton;

            bool accepted = form.ShowDialog() == DialogResult.OK;
            shiftValue = accepted ? (int)countInput.Value : 0;

            return accepted;
        }

        private static void ShiftModulesFrom(IModule selectedModule,
            int shiftValue)
        {
            var modules = selectedModule.IONode.IOModules;
            int selectedIndex = modules.IndexOf(selectedModule.IOModule);
            if (selectedIndex < 0)
            {
                throw new InvalidOperationException(
                    "Выбранный модуль не найден в узле.");
            }

            var modulesToShift = modules
                .Skip(selectedIndex)
                .Where(module => module?.Function?.IsValid == true)
                .OrderByDescending(module => module.PhysicalNumber)
                .ToList();

            if (!modulesToShift.Any())
            {
                throw new InvalidOperationException(
                    "Нет модулей для сдвига.");
            }

            foreach (var module in modulesToShift)
            {
                int newPhysicalNumber = module.PhysicalNumber + shiftValue;
                ValidateModuleNumber(module.PhysicalNumber, newPhysicalNumber);
            }

            foreach (var module in modulesToShift)
            {
                int newPhysicalNumber = module.PhysicalNumber + shiftValue;
                RenameModuleWithClamps(module,
                    $"-A{module.PhysicalNumber}",
                    $"-A{newPhysicalNumber}");
            }
        }

        private static void MoveModule(IModule sourceModule,
            IModule targetModule)
        {
            if (!CanMoveModule(sourceModule, targetModule))
            {
                throw new InvalidOperationException(
                    "Модуль можно перемещать только внутри одного узла PLC.");
            }

            var modules = sourceModule.IONode.IOModules;
            int sourceIndex = modules.IndexOf(sourceModule.IOModule);
            int targetIndex = modules.IndexOf(targetModule.IOModule);
            if (sourceIndex < 0 || targetIndex < 0)
            {
                throw new InvalidOperationException(
                    "Выбранный модуль не найден в узле.");
            }

            if (sourceIndex == targetIndex)
            {
                return;
            }

            int sourcePhysicalNumber = sourceModule.IOModule.PhysicalNumber;
            int targetPhysicalNumber = GetPhysicalNumberByIndex(
                sourceModule.IONode, targetIndex);
            string tempName = GetTemporaryModuleName(sourceModule.IOModule);

            RenameModuleWithClamps(sourceModule.IOModule,
                $"-A{sourcePhysicalNumber}", tempName);

            var modulesToShift = GetModulesToShiftForMove(modules,
                sourceIndex, targetIndex);
            ValidateMoveModules(modulesToShift, sourceIndex, targetIndex);
            RenameMoveModules(modulesToShift, sourceIndex, targetIndex);

            RenameModuleWithClamps(sourceModule.IOModule, tempName,
                $"-A{targetPhysicalNumber}");
        }

        private static void InsertDeletedModule(IIOModule deletedModule,
            IModule targetModule)
        {
            if (targetModule == null ||
                deletedModule?.Function?.IsValid != true)
            {
                throw new InvalidOperationException(
                    "Исключенный модуль можно переместить только в узел PLC.");
            }

            var modules = targetModule.IONode.IOModules;
            int targetIndex = modules.IndexOf(targetModule.IOModule);
            if (targetIndex < 0)
            {
                throw new InvalidOperationException(
                    "Целевой модуль не найден в узле.");
            }

            int targetPhysicalNumber = GetPhysicalNumberByIndex(
                targetModule.IONode, targetIndex);

            if (targetModule.IOModule.Function?.IsValid == true)
            {
                throw new InvalidOperationException(
                    "Исключенный модуль можно переместить только в " +
                    "неопределенный модуль.");
            }

            RenameDeletedModule(deletedModule,
                GetDeletedModuleName(deletedModule),
                $"-A{targetPhysicalNumber}");
        }

        private static void RenameDeletedModule(IIOModule deletedModule,
            string oldName, string newName)
        {
            if (deletedModule.Function is not StaticHelper.EplanFunction
                eplanFunction)
            {
                throw new InvalidOperationException(
                    $"Не найдена функция для переименования {oldName}.");
            }

            RenameDeletedFunctionNamePart(eplanFunction.Function, newName);
        }

        private static void RenameDeletedFunctionNamePart(Function function,
            string newName)
        {
            var renamedFunctionName = Regex.Replace(function.Name,
                @"-(?:DEL|D)\d+(?=$|\D)", newName);
            if (renamedFunctionName == function.Name)
            {
                throw new InvalidOperationException(
                    $"Не удалось заменить имя исключенного модуля " +
                    $"\"{function.VisibleName}\".");
            }

            function.Name = renamedFunctionName;
        }

        private static int GetPhysicalNumberByIndex(IIONode node,
            int moduleIndex)
        {
            return node.NodeNumber + moduleIndex + 1;
        }

        private static List<IIOModule> GetModulesToShiftForMove(
            List<IIOModule> modules, int sourceIndex, int targetIndex)
        {
            int startIndex = Math.Min(sourceIndex, targetIndex);
            int endIndex = Math.Max(sourceIndex, targetIndex);

            var range = Enumerable.Range(startIndex, endIndex - startIndex + 1)
                .Where(index => index != sourceIndex)
                .Select(index => modules[index])
                .Where(module => module?.Function?.IsValid == true)
                .ToList();

            return sourceIndex > targetIndex ?
                range.OrderByDescending(module => module.PhysicalNumber)
                    .ToList() :
                range.OrderBy(module => module.PhysicalNumber).ToList();
        }

        private static void ValidateMoveModules(IEnumerable<IIOModule> modules,
            int sourceIndex, int targetIndex)
        {
            int shiftValue = sourceIndex > targetIndex ? 1 : -1;
            foreach (var module in modules)
            {
                int newPhysicalNumber = module.PhysicalNumber + shiftValue;
                ValidateModuleNumber(module.PhysicalNumber, newPhysicalNumber);
            }
        }

        private static void RenameMoveModules(IEnumerable<IIOModule> modules,
            int sourceIndex, int targetIndex)
        {
            int shiftValue = sourceIndex > targetIndex ? 1 : -1;
            foreach (var module in modules)
            {
                int newPhysicalNumber = module.PhysicalNumber + shiftValue;
                RenameModuleWithClamps(module,
                    $"-A{module.PhysicalNumber}",
                    $"-A{newPhysicalNumber}");
            }
        }

        private static void DeleteModules(IEnumerable<IModule> selectedModules)
        {
            var modulesByNode = selectedModules
                .GroupBy(module => module.IONode);

            foreach (var modulesGroup in modulesByNode)
            {
                DeleteModulesFromNode(modulesGroup.Key,
                    modulesGroup.ToList());
            }
        }

        private static void DeleteModulesFromNode(IIONode node,
            List<IModule> selectedModules)
        {
            var modules = node.IOModules;
            var selectedIndexes = selectedModules
                .Select(module => modules.IndexOf(module.IOModule))
                .OrderBy(index => index)
                .ToList();

            if (selectedIndexes.Any(index => index < 0))
            {
                throw new InvalidOperationException(
                    "Один из выбранных модулей не найден в узле.");
            }

            var selectedDefinedModules = selectedModules
                .Select(module => module.IOModule)
                .Where(module => module?.Function?.IsValid == true)
                .OrderByDescending(module => module.PhysicalNumber)
                .ToList();

            var selectedUndefinedIndexes = selectedModules
                .Where(module => IsUndefinedModule(module.IOModule))
                .Select(module => modules.IndexOf(module.IOModule))
                .OrderBy(index => index)
                .ToList();

            if (!selectedDefinedModules.Any() &&
                !selectedUndefinedIndexes.Any())
            {
                throw new InvalidOperationException(
                    "Нет модулей для удаления.");
            }

            foreach (var module in selectedDefinedModules)
            {
                RenameModuleWithClamps(module,
                    $"-A{module.PhysicalNumber}",
                    GetDeletedModuleName(module));
            }

            if (!selectedUndefinedIndexes.Any())
            {
                return;
            }

            int firstDeletedIndex = selectedUndefinedIndexes.First();
            var modulesToShift = modules
                .Skip(firstDeletedIndex + 1)
                .Where(module => module?.Function?.IsValid == true)
                .Where(module => !selectedDefinedModules.Contains(module))
                .OrderBy(module => module.PhysicalNumber)
                .ToList();

            if (!modulesToShift.Any())
            {
                throw new InvalidOperationException(
                    "После выбранного модуля нет модулей для сдвига.");
            }

            foreach (var module in modulesToShift)
            {
                int moduleIndex = modules.IndexOf(module);
                int shiftValue = selectedUndefinedIndexes.Count(index =>
                    index < moduleIndex);
                int newPhysicalNumber = module.PhysicalNumber - shiftValue;
                ValidateModuleNumber(module.PhysicalNumber, newPhysicalNumber);
            }

            foreach (var module in modulesToShift)
            {
                int moduleIndex = modules.IndexOf(module);
                int shiftValue = selectedUndefinedIndexes.Count(index =>
                    index < moduleIndex);
                int newPhysicalNumber = module.PhysicalNumber - shiftValue;
                RenameModuleWithClamps(module,
                    $"-A{module.PhysicalNumber}",
                    $"-A{newPhysicalNumber}");
            }
        }

        private static bool IsUndefinedModule(IIOModule module)
        {
            return module?.Info?.Name == IOModuleInfo.Stub.Name &&
                module.Function is null;
        }

        private static string GetDeletedModuleName(IIOModule module)
        {
            return $"-DEL{module.PhysicalNumber}";
        }

        private static string GetTemporaryModuleName(IIOModule module)
        {
            return $"-DEL{module.PhysicalNumber}TMP";
        }

        private static void RenameModuleWithClamps(IIOModule module,
            string oldName, string newName)
        {
            var functionsToRename = GetModuleRenameFunctions(module, oldName);
            foreach (var function in functionsToRename)
            {
                RenameFunctionNamePart(function, oldName, newName);
            }
        }

        private static void RenameFunctionNamePart(Function function,
            string oldName, string newName)
        {
            var renamedFunctionName = Regex.Replace(function.Name,
                $@"{Regex.Escape(oldName)}(?=$|\D)", newName);
            if (renamedFunctionName != function.Name)
            {
                function.Name = renamedFunctionName;
            }
        }

        private static List<Function> GetModuleRenameFunctions(
            IIOModule module, string oldModuleName)
        {
            var functions = new List<Function>();

            AddRenameCandidate(functions, module.Function, oldModuleName);

            foreach (var clampFunction in module.ClampFunctions.Values
                .Where(function => function?.IsValid == true)
                .Distinct())
            {
                AddRenameCandidate(functions, clampFunction, oldModuleName);
            }

            AddProjectRenameCandidates(functions, module, oldModuleName);

            foreach (var function in functions.ToArray())
            {
                AddRenameCandidate(functions, function.ParentFunction,
                    oldModuleName);

                var pageFunctions = function.Page?.Functions ?? [];
                foreach (var pageFunction in pageFunctions)
                {
                    AddRenameCandidate(functions, pageFunction, oldModuleName);
                }
            }

            if (!functions.Any())
            {
                throw new InvalidOperationException(
                    $"Не найдены функции для переименования {oldModuleName}.");
            }

            return functions;
        }

        private static void AddProjectRenameCandidates(List<Function> functions,
            IIOModule module, string oldModuleName)
        {
            if (module.Function is not StaticHelper.EplanFunction moduleFunction)
            {
                return;
            }

            var project = moduleFunction.Function.Page?.Project;
            if (project is null)
            {
                return;
            }

            var objectFinder = new DMObjectsFinder(project);
            AddProjectRenameCandidates(functions, objectFinder, oldModuleName,
                Function.Enums.Category.PLCBox);
            AddProjectRenameCandidates(functions, objectFinder, oldModuleName,
                Function.Enums.Category.PLCTerminal);
        }

        private static void AddProjectRenameCandidates(List<Function> functions,
            DMObjectsFinder objectFinder,
            string oldModuleName, Function.Enums.Category category)
        {
            var functionsFilter = new FunctionsFilter
            {
                Category = category
            };

            foreach (var function in objectFinder.GetFunctions(functionsFilter))
            {
                AddRenameCandidate(functions, function, oldModuleName);
            }
        }

        private static void AddRenameCandidate(List<Function> functions,
            StaticHelper.IEplanFunction function, string oldModuleName)
        {
            if (function is StaticHelper.EplanFunction eplanFunction)
            {
                AddRenameCandidate(functions, eplanFunction.Function,
                    oldModuleName);
            }
        }

        private static void AddRenameCandidate(List<Function> functions,
            Function function, string oldModuleName)
        {
            if (function is null ||
                !function.IsValid ||
                functions.Contains(function) ||
                !CanRenameModuleFunction(function) ||
                !ContainsModuleName(function.Name, oldModuleName))
            {
                return;
            }

            functions.Add(function);
        }

        private static bool CanRenameModuleFunction(Function function)
        {
            return function.Category is Function.Enums.Category.PLCBox or
                Function.Enums.Category.PLCTerminal;
        }

        private static bool ContainsModuleName(string functionName,
            string moduleName)
        {
            return Regex.IsMatch(functionName,
                $@"{Regex.Escape(moduleName)}(?=$|\D)");
        }

        private static void ValidateModuleNumber(int currentPhysicalNumber,
            int newPhysicalNumber)
        {
            bool movedToAnotherNode =
                currentPhysicalNumber / 100 != newPhysicalNumber / 100;
            bool becameNodeNumber = newPhysicalNumber % 100 == 0;
            if (movedToAnotherNode || becameNodeNumber)
            {
                throw new InvalidOperationException(
                    $"Невозможно переименовать модуль -A{currentPhysicalNumber} " +
                    $"в -A{newPhysicalNumber}: номер выходит за пределы узла.");
            }
        }

        private void InitDataStructPLC()
        {
            StructPLC.BeginUpdate();

            StructPLC.Roots = DataContext.Roots;

            StructPLC.Columns[0].Width = 100;
            StructPLC.Columns[1].Width = 200;

            StructPLC.Expand(DataContext.Root);

            /// Востановление развертки дерева
            RestoreExpanded(DataContext.Roots);

            StructPLC.SelectedIndex = 0;
            StructPLC.SelectedItem.EnsureVisible();

            StructPLC.EndUpdate();

            AutoResizeColumns(StructPLC);
        }

        private void RestoreExpanded(IEnumerable items)
        {
            foreach (var item in items.OfType<IExpandable>())
            {
                if (item.Expanded)
                {
                    StructPLC.Expand(item);
                }

                RestoreExpanded(item.Items);
            }
        }

        private void Expand_Click(object sender, EventArgs e)
        {
            int level = Convert.ToInt32((sender as ToolStripMenuItem).Tag);

            StructPLC.BeginUpdate();

            // Убираем события для оптимизации множественного вызова
            StructPLC.Expanded -= ItemExpanded;
            StructPLC.Collapsed -= ItemCollapsed;

            StructPLC.SelectedIndex = 0;

            ExpandToLevel(level, StructPLC.Objects);

            StructPLC.Expanded += ItemExpanded;
            StructPLC.Collapsed += ItemCollapsed;
            
            StructPLC.EnsureModelVisible(StructPLC.SelectedObject);
            StructPLC.EndUpdate();

            AutoResizeColumns(StructPLC);
        }

        private void ExpandToLevel(int level, IEnumerable items)
        {
            foreach (var item in items.OfType<IExpandable>())
            {
                if (level > 0 && !StructPLC.IsExpanded(item))
                {
                    StructPLC.Expand(item);
                    item?.Expanded = true;
                }
                
                if (level == 0 && StructPLC.IsExpanded(item))
                {
                    StructPLC.Collapse(item);
                    item?.Expanded = false;
                }

                if (item is not null)
                {
                    ExpandToLevel(level - 1, item.Items);
                }
            }
        }

        private TextBox textBoxCellEditor;

        private void CellEditStarting(object sender, CellEditEventArgs e)
        {
            if (StructPLC.SelectedObject is not IEditable item || e.Column.Index != 1)
            {
                e.Cancel = true;
                return;
            }

            isCellEditing = true;

            InitTextBoxCellEditor(item.Value, e.CellBounds);

            e.Control = textBoxCellEditor;
            textBoxCellEditor.Focus();

            StructPLC.Freeze();
        }

        private void CellEditFinishing(object sender, CellEditEventArgs e)
        {
            isCellEditing = false;

            if (cancelChanges || StructPLC.SelectedObject is not IEditable item)
            {
                e.Cancel = true;
                cancelChanges = false;

                StructPLC.Unfreeze();

                return;
            }

            StructPLC.Controls.Remove(textBoxCellEditor);

            var modified = item.SetValue(e.NewValue.ToString());

            if (modified)
            {
                RefreshTree();
                DFrm.GetInstance().RefreshTreeAfterBinding();
            }

            e.Cancel = true;
            StructPLC.Unfreeze();
        }

        private void InitTextBoxCellEditor(string text, Rectangle bounds)
        {
            textBoxCellEditor = new TextBox
            {
                Enabled = true,
                Visible = true,
                Text = text,
                Bounds = bounds,
            };


            textBoxCellEditor.LostFocus += CellEditor_LostFocus;
            textBoxCellEditor.KeyDown += CellEditor_KeyDown;
            
            StructPLC.Controls.Add(textBoxCellEditor);
        }


        private void CellEditor_LostFocus(object sender, EventArgs e)
        {
            if (isCellEditing)
                StructPLC.FinishCellEdit();
        }

       
        private void CellEditor_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    StructPLC.FinishCellEdit();
                    break;

                case Keys.Escape:
                    cancelChanges = true;
                    StructPLC.FinishCellEdit();
                    break;

                default:
                    return; // exit without e.Handled
            }

            e.Handled = true;
        }


        public void RefreshTree()
        {
            if (StructPLC.Items.Count == 0)
            {
                StructPLC.BeginUpdate();
                StructPLC.ClearObjects();
                StructPLC.EndUpdate();
            }
            else
            {
                StructPLC.FocusedObject = StructPLC.SelectedObject;
                StructPLC.RefreshObjects(DataContext.Roots.ToList());
            }
        }

        public void RefreshTreeAfterBinding()
        {
            RefreshTree();
        }

        public void RebuildTree()
        {
            DataContext.RebuildTree();
            InitDataStructPLC();
        }

        private void ItemExpanded(object sender, TreeBranchExpandedEventArgs e)
        {
            (e.Model as IExpandable).Expanded = true;
            AutoResizeColumns(sender as TreeListView);
        }


        private void ItemCollapsed(object sender, TreeBranchCollapsedEventArgs e)
        {
            (e.Model as IExpandable).Expanded = false;
            AutoResizeColumns(sender as TreeListView);
        }


        private static void AutoResizeColumns(TreeListView treeListView)
        {
            if (treeListView is null)
                return;

            foreach (ColumnHeader column in treeListView.Columns)
            {
                column.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
        }

        private void SelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if ((sender as TreeListView)?.SelectedObject is IClamp clamp)
            {
                DataContext.SelectedClampFunction = clamp.ClampFunction;
                DataContext.SelectedClamp = clamp;
            }
            else
            {
                DataContext.SelectedClampFunction = null;
                DataContext.SelectedClamp = null;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            EProjectManager.GetInstance().SyncAndSave(false);

            
            Editor.Editor.GetInstance().EditorForm.RefreshTree();
            DFrm.GetInstance().RefreshTree();

            RebuildTree();
        }

        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            var tlv = sender as TreeListView;
           
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    var selectedModules = GetSelectedModules().ToList();
                    if (selectedModules.Any())
                    {
                        DeleteSelectedUndefinedModule();
                        e.Handled = true;
                        return;
                    }

                    (tlv.SelectedObject as IDeletable)?.Delete();
                    break;

                default:
                    return;
            }

            e.Handled = true;
            RefreshTree();
            DFrm.GetInstance().RefreshTreeAfterBinding();
        }

        private void StructPLC_FormatCell(object sender, FormatCellEventArgs e)
        {
            if (e.Model is IClamp clamp && !clamp.Bound)
            {
                e.Item.SubItems[1].ForeColor = Color.LightSlateGray;
            }
        }
    }
}
