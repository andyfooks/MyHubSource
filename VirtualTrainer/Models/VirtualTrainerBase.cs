using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrainer
{
    public abstract class VirtualTrainerBase
    {
        public virtual List<RuleConfigurationObject> GetAllConfigProps()
        {
            List<RuleConfigurationObject> returnList = new List<RuleConfigurationObject>();

            var configProperties = this.GetType().GetProperties().Where
                (
                    a => a.CustomAttributes.Where
                    (
                        b => b.AttributeType == typeof(IsRuleConfigurationParticipant)
                    ).Any()
                );

            foreach (var configProperty in configProperties)
            {
                if (configProperty.PropertyType.UnderlyingSystemType.Name.Contains("ICollection"))
                {
                    // Is an ICollection type object, Get the collection object type.
                    if (configProperty.PropertyType.UnderlyingSystemType.GenericTypeArguments[0].UnderlyingSystemType.Module.Name == this.GetType().Module.Name &&
                        configProperty.PropertyType.UnderlyingSystemType.GenericTypeArguments[0].UnderlyingSystemType.BaseType == this.GetType().BaseType)
                    {
                        // Get the available properties for this type arguments type.
                        var instance = (VirtualTrainerBase)Activator.CreateInstance(configProperty.PropertyType.UnderlyingSystemType.GenericTypeArguments[0].UnderlyingSystemType);
                        returnList.AddRange(instance.GetAllConfigProps());

                        // Now loop through the individual collection items.
                        IEnumerable instanceCollectionItems = (IEnumerable)this.GetType().GetProperty(configProperty.Name).GetValue(this);
                        if (instanceCollectionItems != null)
                        {
                            //List<RuleConfigurationObject> ICollectionRuleConfig = new List<RuleConfigurationObject>();
                            RuleConfigurationObject ICollectionRuleConfig = new RuleConfigurationObject() {
                                ClassName = this.GetType().Name,
                                ClassPropertyName = configProperty.Name,
                                PropertyType = configProperty.PropertyType
                            };
                            foreach (VirtualTrainerBase collectionItem in instanceCollectionItems)
                            {
                                //ICollectionRuleConfig.CollectionInstanceRuleConfigurations.(collectionItem.GetAllConfigProps());
                            }
                        }
                    }
                }
                else if (configProperty.PropertyType.Module.Name == this.GetType().Module.Name)
                {
                    // Is a custom Virtual Trainer Object that implements Virtual Trainer Base, call objects GetAllConfigurationProperties property
                    if (configProperty.PropertyType.BaseType == this.GetType().BaseType)
                    {
                        var vtObject = (VirtualTrainerBase)this.GetType().GetProperty(configProperty.Name).GetValue(this);
                        if (vtObject == null)
                        {
                            vtObject = (VirtualTrainerBase)Activator.CreateInstance(configProperty.PropertyType);
                        }
                        returnList.AddRange(vtObject.GetAllConfigProps());
                    }
                }
                else
                {
                    // Is a System Type e.g. string, int etc....this is the proerty we have been searching for.
                    object propertyValue = this.GetType().GetProperty(configProperty.Name).GetValue(this, null);
                    returnList.Add(new RuleConfigurationObject()
                    {
                        ClassName = this.GetType().Name,
                        ClassPropertyName = configProperty.Name,
                        ContextValue = (propertyValue == null ? "" : propertyValue.ToString()),
                        PropertyType = configProperty.PropertyType
                    });
                }
            }

            return returnList;
        }
    }
}
