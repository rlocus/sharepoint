﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Linq;
using SPCore.Helper;

namespace SPCore.Linq
{
    /// <summary>
    /// Мета данные списка
    /// </summary>
    public sealed class EntityListMetaData
    {
        public string ServerRelativeUrl { get; set; }
        //public SPListTemplateType BaseType { get; set; }
        public List<EntityListContentTypeInfo> ContentTypes { get; set; }
        public string Description { get; set; }
        public bool EnableAttachments { get; set; }
        public List<EntityListFieldInfo> Fields { get; set; }
        public bool Hidden { get; set; }
        public Guid Id { get; set; }
        public bool IsUserInfoList { get; set; }
        public bool IsVirtual { get; set; }
        public SPList List { get; set; }
        public string Name { get; set; }

        private EntityListMetaData()
        { }

        /// <summary>
        /// Получение мета данных списка
        /// </summary>
        /// <param name="entityList">Список</param>
        public static EntityListMetaData GetMetaData<T>(EntityList<T> entityList) where T : EntityItem
        {
            EntityListMetaData res = new EntityListMetaData();

            IEnumerable<string> propNames = typeof(EntityListMetaData).GetProperties().Select(p => p.Name);
            Type entityType = typeof(EntityList<T>);

            FieldInfo listField = entityType.GetField("list",
                BindingFlags.NonPublic | BindingFlags.Instance);

            var listValue = listField.GetValue(entityList);
            Type listType = listValue.GetType();
            PropertyInfo[] listProperties = listType.GetProperties();
            
            foreach (PropertyInfo listProperty in listProperties)
            {
                if (!propNames.Contains(listProperty.Name)) continue;
               
                var listPropertyValue = listProperty.GetValue(listValue, null);

                if (listProperty.Name == "ContentTypes")
                {
                    res.ContentTypes = new List<EntityListContentTypeInfo>();
                    IEnumerable ctypes = listPropertyValue as IEnumerable;
                  
                    if (ctypes != null)
                    {
                        foreach (var ctype in ctypes)
                        {
                            EntityListContentTypeInfo ct = new EntityListContentTypeInfo
                            {
                                Id = ctype.GetPropertyValue<string>("Id"),
                                Name = ctype.GetPropertyValue<string>("Name"),
                                Description = ctype.GetPropertyValue<string>("Description"),
                                Hidden = ctype.GetPropertyValue<bool>("Hidden")
                            };

                            res.ContentTypes.Add(ct);
                        }
                    }
                }
                else if (listProperty.Name == "Fields")
                {
                    res.Fields = new List<EntityListFieldInfo>();
                    IEnumerable fields = listPropertyValue as IEnumerable;
                    
                    if (fields != null)
                    {
                        foreach (var field in fields)
                        {
                            EntityListFieldInfo ef = new EntityListFieldInfo
                            {
                                Id = field.GetPropertyValue<Guid>("Id"),
                                Title = field.GetPropertyValue<string>("Title"),
                                InternalName = field.GetPropertyValue<string>("InternalName"),
                                //FieldType = field.GetPropertyValue<SPFieldType>("FieldType"),
                                //AllowMultipleValues = field.GetPropertyValue<bool>("AllowMultipleValues"),
                                //Choices = field.GetPropertyValue<IEnumerable>("Choices"),
                                //FillInChoice = field.GetPropertyValue<bool>("FillInChoice"),
                                Hidden = field.GetPropertyValue<bool>("Hidden"),
                                IsCalculated = field.GetPropertyValue<bool>("IsCalculated"),
                                ReadOnlyField = field.GetPropertyValue<bool>("ReadOnlyField"),
                                Required = field.GetPropertyValue<bool>("Required"),
                                Description = field.GetPropertyValue<string>("Description"),
                                //LookupDisplayColumn = field.GetPropertyValue<string>("LookupDisplayColumn"),
                                //LookupList = field.GetPropertyValue<string>("LookupList"),
                                //PrimaryFieldId = field.GetPropertyValue<string>("PrimaryFieldId")
                            };

                            res.Fields.Add(ef);
                        }
                    }
                }
                else
                {
                    PropertyInfo property = typeof(EntityListMetaData).GetProperty(listProperty.Name);
                    property.SetValue(res, listPropertyValue, null);
                }
            }
            return res;
        }
    }
}