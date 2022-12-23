using Renga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Quick
{
    public static class UIExtension
    {
        public static IAction CreateAction(this IUI UI, string displayName)
        {
            IAction action = UI.CreateAction();
            action.DisplayName = displayName;
            return action;
        }
    }
    public static class ISelectionExtension
    {

        public static int? GetFirstSelectedObject(this ISelection selection)
        {
            int[] selectedObjects = (int[])selection.GetSelectedObjects();

            if (selectedObjects.Length == 0)
            {
                return null;
            }

            return selectedObjects[0];
        }

    }
    public static class IParameterExtension
    {
        public static object GetParameterValue(this IParameter parameter)
        {
            switch (parameter.ValueType)
            {
                case ParameterValueType.ParameterValueType_Double:
                    return parameter.GetDoubleValue();
                case ParameterValueType.ParameterValueType_String:
                    return parameter.GetStringValue();
                case ParameterValueType.ParameterValueType_Int:
                    return parameter.GetIntValue();
                case ParameterValueType.ParameterValueType_Bool:
                    return parameter.GetBoolValue();
                default:
                    return null;
            }
        }
    }
    public static class IPropertyExtension
    {
        public static object GetPropertyValue(this IProperty property)
        {
            switch (property.Type)
            {
                case PropertyType.PropertyType_Angle: 
                    return property.GetAngleValue(AngleUnit.AngleUnit_Unknown);
                case PropertyType.PropertyType_Double: 
                    return property.GetDoubleValue();
                case PropertyType.PropertyType_String: 
                    return property.GetStringValue();
                case PropertyType.PropertyType_Area:
                    return property.GetAreaValue(AreaUnit.AreaUnit_Unknown);
                case PropertyType.PropertyType_Boolean: 
                    return property.GetBooleanValue();
                case PropertyType.PropertyType_Enumeration: 
                    return property.GetEnumerationValue();
                case PropertyType.PropertyType_Integer: 
                    return property.GetIntegerValue();
                case PropertyType.PropertyType_Length: 
                    return property.GetLengthValue(LengthUnit.LengthUnit_Unknown);
                case PropertyType.PropertyType_Logical:
                    return property.GetLogicalValue();
                case PropertyType.PropertyType_Mass: 
                    return property.GetMassValue(MassUnit.MassUnit_Unknown);
                case PropertyType.PropertyType_Volume: 
                    return property.GetVolumeValue(VolumeUnit.VolumeUnit_Unknown);
                default:
                    return null;
            }
        }
    }
    public static class IParameterContainerExtension
    {
        public static List<IParameter> ToList(this IParameterContainer parameterContainer)
        {
            IGuidCollection IDs = parameterContainer.GetIds();
            List<IParameter> parameters = new List<IParameter>();
            for (int i = 0; i < IDs.Count; i++)
            {
                parameters.Add(parameterContainer.Get(IDs.Get(i)));

            }
            return parameters;
        }

        public static Dictionary<string, object> ToDictionary(this IParameterContainer parameterContainer)
        {
            IGuidCollection IDs = parameterContainer.GetIds();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            for (int i = 0; i < IDs.Count; i++)
            {
                IParameter parameter = parameterContainer.Get(IDs.Get(i));
                parameters.Add(parameter.Definition.Name, parameter.GetParameterValue());
            }
            return parameters;
        }
    }
    public static class IPropertyContainerExtension
    {
        public static List<IProperty> ToList(this IPropertyContainer propertyContainer)
        {
            IGuidCollection IDs = propertyContainer.GetIds();
            List<IProperty> properties = new List<IProperty>();
            for (int i = 0; i < IDs.Count; i++)
            {
                properties.Add(propertyContainer.Get(IDs.Get(i)));

            }
            return properties;
        }
        public static Dictionary<string, object> ToDictionary(this IPropertyContainer propertyContainer)
        {
            IGuidCollection IDs = propertyContainer.GetIds();
            Dictionary<string, object> properties = new Dictionary<string, object>();
            for (int i = 0; i < IDs.Count; i++)
            {
                IProperty property = propertyContainer.Get(IDs.Get(i));
                properties.Add(property.Name, property.GetPropertyValue());
            }
            return properties;
        }
    }

    public static class IModelObjectCollectionExtension
    {
        public static List<IModelObject> ToList(this IModelObjectCollection modelObjectCollection)
        {
            int[] IDs = (int[])modelObjectCollection.GetIds();
            List<IModelObject> modelObjects = new List<IModelObject>();
            foreach (int ID in IDs)
            {
                modelObjects.Add(modelObjectCollection.GetById(ID));
            }
            return modelObjects;
        }
    }
    public static class DictionaryExtension
    {
        public static bool CompareDictionaryByKeys<TKey,TValue>(this Dictionary<TKey,TValue> firstDictionary, 
            Dictionary<TKey, TValue> secondDictionary, List<TKey> keys)
        {
            foreach (TKey key in keys)
            {
                TValue value;
                TValue value2;
                firstDictionary.TryGetValue(key, out value);
                secondDictionary.TryGetValue(key, out value2);
                if (!object.Equals(value,value2))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
