using Renga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Quick
{
    [AttributeUsage(AttributeTargets.Method)]
    public class dropDownButtonFunctionAttribute : System.Attribute
    {
        public string Name { get; }

        public dropDownButtonFunctionAttribute(string name)
        {
            Name = name;
        }
    }

    public class QuickPlugin : IPlugin
    {
        private IApplication _renga;
        private IUI _UI;
        private List<ActionEventSource> _actionEventSources = new List<ActionEventSource>();
        private List<IAction> _actions = new List<IAction>();
        private SelectParamsWindow _paramsWindow;
        public bool Initialize(string pluginFolder)
        {
            _renga = new Renga.Application();
            _UI = _renga.UI;

            InitializeActions();
            InitializePanelExtension();

            return true;
        }

        private void InitializeActions()
        {
            foreach (MethodInfo methodInfo in this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.IsDefined(typeof(dropDownButtonFunctionAttribute))))
            {
                EventHandler eventHandler = (EventHandler)Delegate.CreateDelegate(
                    typeof(EventHandler), this, methodInfo);

                var attribute = (dropDownButtonFunctionAttribute)methodInfo.GetCustomAttribute(typeof(dropDownButtonFunctionAttribute));
                CreateActionWithTriggerEvent(attribute.Name, eventHandler);
            }
        }

        private void CreateActionWithTriggerEvent(string actionDisplayName, EventHandler eventHandler)
        {
            _actions.Add(_UI.CreateAction(actionDisplayName));
            _actionEventSources.Add(new ActionEventSource(_actions.Last()));
            _actionEventSources.Last().Triggered += eventHandler;
        }

        private void InitializePanelExtension()
        {
            IUIPanelExtension panel = _UI.CreateUIPanelExtension();
            IDropDownButton dropDownButton = _UI.CreateDropDownButton();
            dropDownButton.ToolTip = "Quick";

            foreach (IAction action in _actions)
            {
                dropDownButton.AddAction(action);
            }

            panel.AddDropDownButton(dropDownButton);
            _UI.AddExtensionToPrimaryPanel(panel);
        }

        [dropDownButtonFunction("Выделить объекты такого же типа")]
        private void SelectSameTypeObjects(object sender, EventArgs args)
        {
            ISelection selection = _renga.Selection;
            int? objectID = selection.GetFirstSelectedObject();

            if (objectID == null)
            {
                return;
            }

            selection.SetSelectedObjects(new int[] { objectID.Value });

            IProject project = _renga.Project;
            IModelObjectCollection modelObjects = project.Model.GetObjects();
            IModelObject modelObject = modelObjects.GetById(objectID.Value);
            Guid ObjectTypeToSelect = modelObject.ObjectType;

            List<int> IDsToSelect = modelObjects.ToList().Where(mo => mo.ObjectType == ObjectTypeToSelect)
                .Select(mo => mo.Id).ToList();
            
            selection.SetSelectedObjects(IDsToSelect.ToArray());
        }
        [dropDownButtonFunction("Выделить объекты такого же типа+")]
        private void OpenSelectParameters(object sender, EventArgs args) 
        {
            ISelection selection = _renga.Selection;
            int? objectID = selection.GetFirstSelectedObject();
            if (objectID == null)
            {
                return;
            }
            selection.SetSelectedObjects(new int[] { objectID.Value });

            MethodBase method = MethodBase.GetCurrentMethod();
            var attribute = (dropDownButtonFunctionAttribute)method.GetCustomAttribute(typeof(dropDownButtonFunctionAttribute));
            IAction action = _actions.Where(x => x.DisplayName == attribute.Name).First();
            action.Enabled = false;

            IProject project = _renga.Project;
            IModel model = project.Model;
            IModelObjectCollection modelObjects = model.GetObjects();
            Guid ObjectTypeToSelect = modelObjects.GetById(objectID.Value).ObjectType;
            IModelObject modelObject = modelObjects.GetById(objectID.Value);

            _paramsWindow = new SelectParamsWindow(action, selection, modelObject, model);
            _paramsWindow.Show();
            
        }

        public void Stop()
        {
            foreach(ActionEventSource actionEventSource in _actionEventSources)
            {
                actionEventSource.Dispose();
            }

            _renga.Quit();
        }
    }
}


