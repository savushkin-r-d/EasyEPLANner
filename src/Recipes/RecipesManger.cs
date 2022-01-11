using EasyEPlanner;
using Editor;
using System.Collections.Generic;
using System.IO;
using LuaInterface;

namespace Recipe
{
    public class RecipesManger : TreeViewItem, IRecipesManager
    {
        private RecipesManger()
        {
            InitLua();
            recipes = new List<Recipe>();
        }

        public static RecipesManger GetInstance()
        {
            if (instance == null)
            {
                instance = new RecipesManger();
            }

            return instance;
        }

        public Recipe AddRecipe( string name )
        {
            Recipe recipe = new Recipe( name );
            recipe.AddParent(this);
            recipes.Add(recipe);
            return recipe;

        }

        public void ClearRecipesByObj(int ObjId)
        {
            foreach (Recipe recipe in recipes)
            {
                recipe.DeleteParamByObj(ObjId);
            }
        }

        public void ClearRecipesParams(int ObjId, int ObjParam)
        {
            foreach (Recipe recipe in recipes)
            {
                recipe.DeleteParam(ObjId, ObjParam);
            }
        }

        #region Реализация работы с Lua
        /// <summary>
        /// Инициализировать Lua-скрипт для чтения описания объектов
        /// </summary>
        private void InitLua()
        {
            lua = new Lua();
            InitReadRecipesLuaScript();

        }

        /// <summary>
        /// Загрузка скрипта для чтения ограничений объектов.
        /// </summary>
        private void InitReadRecipesLuaScript()
        {
            lua.RegisterFunction("AddRecipe", this,
                GetType().GetMethod("AddRecipe"));
            string recipesFileName = "sys_recipes.lua";
            string pathToRecipesInitializer = Path
                .Combine(ProjectManager.GetInstance().SystemFilesPath,
                recipesFileName);
            lua.DoFile(pathToRecipesInitializer);
        }

        /// <summary>
        /// Загрузка ограничений объектов
        /// </summary>
        /// <param name="LuaStr">Описание ограничений объектов</param>
        public void LoadRecipes(string LuaStr)
        {
            //Очистка ранее считанного списка рецептов
            recipes.Clear();

            //Выполнения Lua скрипта с описанием объектов.
            lua.DoString(LuaStr);
            lua.DoString("init_recipes()");
        }

        /// <summary>
        /// Сохранение в виде таблицы Lua.
        /// </summary>
        /// <param name="prefix">Префикс (для выравнивания).</param>
        /// <returns>Описание в виде таблицы Lua.</returns>
        public string SaveAsLuaTable(string prefix)
        {
            string res = "";
            foreach (Recipe obj in recipes)
            {
                int num = recipes.IndexOf(obj) + 1;
                res += obj.SaveAsLuaTable(prefix + "\t\t", num);
            }
            res = res.Replace("\t", "    ");
            return res;
        }
        #endregion

        #region Реализация ITreeViewItem
        public override string[] DisplayText
        {
            get
            {
                string res = "\"" + "Рецепты" + "\"";
                if (recipes.Count > 0)
                {
                    res += " (" + recipes.Count + ")";
                }

                return new string[] { res, "" };
            }
        }

        public override ITreeViewItem[] Items
        {
            get
            {
                return recipes.ToArray();
            }
        }

        public override bool Delete(object child)
        {
            Recipe recipe = child as Recipe;

            if (recipe != null)
            {
                recipes.Remove(recipe);
                return true;
            }
            return false;
        }

        override public bool IsInsertableCopy
        {
            get
            {
                return true;
            }
        }

        public override ITreeViewItem InsertCopy(object obj)
        {
            var recipe = obj as Recipe;
            if (recipe != null)
            {
                Recipe newRecipe = recipe.Clone();
                newRecipe.AddParent(this);
                recipes.Add( newRecipe );
                return newRecipe;
            }

            return null;
        }

        public override bool IsInsertable
        {
            get
            {
                return true;
            }
        }

        public override ITreeViewItem Insert()
        {
            Recipe newRecipe = 
                new Recipe("Рецепт № " + (recipes.Count + 1).ToString());
            newRecipe.AddParent(this);
            recipes.Add(newRecipe);
            return newRecipe;
        }

        override public ITreeViewItem Replace(object child, object copyObject)
        {
            var replacedRecipe = child as Recipe;
            var copyRecipe = copyObject as  Recipe;
            bool objectsNotNull = replacedRecipe != null && copyRecipe != null;
            if (objectsNotNull)
            {
                Recipe newRecipe = copyRecipe.Clone();

                int replacedIdx = recipes.IndexOf(replacedRecipe);
                recipes.Remove(replacedRecipe);
                recipes.Insert(replacedIdx, newRecipe);

                newRecipe.AddParent(this);
                return newRecipe;
            }

            return null;
        }

        override public ITreeViewItem MoveUp(object child)
        {
            var movedRecipe = child as Recipe;
            if (movedRecipe != null)
            {
                int index = recipes.IndexOf(movedRecipe);
                if (index > 0)
                {
                    recipes.Remove(movedRecipe);
                    recipes.Insert(index - 1, movedRecipe);
                    recipes[index].AddParent(this);
                    return recipes[index];
                }
            }
            return null;
        }

        ///Последняя позиция в списке, с которой мы можем переместиться на
        ///одну позицию вниз
        private const int lastPosInTheList = 2;

        override public ITreeViewItem MoveDown(object child)
        {
            var movedRecipe = child as Recipe;
            if (movedRecipe != null)
            {
                int index = recipes.IndexOf(movedRecipe);
                if (index <= recipes.Count - lastPosInTheList)
                {
                    recipes.Remove(movedRecipe);
                    recipes.Insert(index + 1, movedRecipe);
                    recipes[index].AddParent(this);
                    return recipes[index];
                }
            }

            return null;
        }
        #endregion

        /// <summary>
        /// Единственный объект менеджера объектов.
        /// </summary>
        private static RecipesManger instance;

        /// <summary>
        /// Список всех технологических объектов в дереве.
        /// </summary>
        private List<Recipe> recipes;

        /// <summary>
        /// Экземпляр LUA.
        /// </summary>
        private Lua lua;
    }

}
