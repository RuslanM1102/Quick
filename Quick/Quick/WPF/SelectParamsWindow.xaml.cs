using Renga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Quick
{ 
    public partial class SelectParamsWindow: Window
    {
        private IParameterContainer _parameterContainer;
        private IPropertyContainer _propertyContainer;
        private IAction _action;
        private ISelection _selection;
        private IModelObject _modelObject;
        private IModel _model;
        public SelectParamsWindow(IAction action, ISelection selection, IModelObject modelObject, IModel model)
        {
            InitializeComponent();
            _parameterContainer = modelObject.GetParameters();
            _propertyContainer = modelObject.GetProperties();
            _action = action;
            _selection = selection;
            _modelObject = modelObject;
            _model = model;
            CreateMenu();
        }

        private void CreateMenu()
        {
            List<string> parameterNames = _parameterContainer.ToList().Select(x => x.Definition.Name).ToList();
            parameterNames.Sort();
            foreach(string parameterName in parameterNames)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Content = parameterName;
                CheckBoxList.Items.Add(checkBox);
            }

            CheckBoxList.Items.Add(new Separator());

            List<string> propertyNames = _propertyContainer.ToList().Select(x => x.Name).ToList();
            propertyNames.Sort();

            foreach (string propertyName in propertyNames)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Content = propertyName;
                CheckBoxList.Items.Add(checkBox);
            }
        }
        private List<string>[] GetSearchSettings()
        {
            List<string> parameters = new List<string>();
            List<string> properties = new List<string>();
            List<string>[] settings = new List<string>[2] {parameters, properties};

            var items = CheckBoxList.Items;
            bool isSeparatorPassed = false;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] is CheckBox item)
                {
                    if(item.IsChecked == true)
                    {
                        if (isSeparatorPassed)
                        {
                            properties.Add(item.Content.ToString());
                        }
                        else 
                        {
                            parameters.Add(item.Content.ToString());
                        }
                    }
                }
                else
                {
                    isSeparatorPassed = true;
                }
            }
            return settings;
        }

        private void SelectSameTypeExtended(object sender, RoutedEventArgs e)
        {
            
            Guid ObjectTypeToSelect = _modelObject.ObjectType;
            List<IModelObject> modelObjects = _model.GetObjects().ToList()
                .Where(mo => mo.ObjectType == ObjectTypeToSelect).ToList();
            List<IModelObject> validModelObjects = new List<IModelObject>();
            List<string>[] searchSettings = GetSearchSettings();
            List<string> parametersToSearch = searchSettings[0];
            List<string> propertiesToSearch = searchSettings[1];

            Dictionary<string, object> originalParameters = _modelObject.GetParameters().ToDictionary();
            Dictionary<string, object> originalProperties = _modelObject.GetProperties().ToDictionary();

            foreach(IModelObject modelObject in modelObjects)
            {
                Dictionary<string, object> objectParameters = modelObject.GetParameters().ToDictionary();
                Dictionary<string, object> objectProperties = modelObject.GetProperties().ToDictionary();
                bool isValid = objectParameters.CompareDictionaryByKeysAndValues(originalParameters, parametersToSearch);
                if (isValid)
                {
                    isValid = objectProperties.CompareDictionaryByKeysAndValues(originalProperties, propertiesToSearch);
                }
                if (isValid)
                {
                    validModelObjects.Add(modelObject);
                }
            }

            _selection.SetSelectedObjects(validModelObjects.Select(mo => mo.Id).ToArray());
            this.Close();
        }

        private void Window_Closed(object sender, System.EventArgs e) => _action.Enabled = true;
    }
}
