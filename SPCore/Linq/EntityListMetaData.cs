using System;
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
        public IEnumerable<EntityListContentTypeInfo> ContentTypes { get; set; }
        public string Description { get; set; }
        public bool EnableAttachments { get; set; }
        public IEnumerable<EntityListFieldInfo> Fields { get; set; }
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

            var propNames = typeof(EntityListMetaData).GetProperties().Select(p => p.Name).ToList();
            Type entityType = typeof(EntityList<T>);

            FieldInfo listField = entityType.GetField("list", BindingFlags.NonPublic | BindingFlags.Instance);

            if (listField != null)
            {
                var listValue = listField.GetValue(entityList);
                Type listType = listValue.GetType();
                PropertyInfo[] listProperties = listType.GetProperties();

                foreach (PropertyInfo listProperty in listProperties)
                {
                    if (!propNames.Contains(listProperty.Name))
                    {
                        continue;
                    }

                    var listPropertyValue = listProperty.GetValue(listValue, null);

                    if (listProperty.Name == "ContentTypes")
                    {
                        IEnumerable ctypes = listPropertyValue as IEnumerable;

                        if (ctypes != null)
                        {
                            res.ContentTypes = ctypes.Cast<object>().Select(ctype => new EntityListContentTypeInfo
                            {
                                Id = ctype.GetPropertyValue<string>("Id"),
                                Name = ctype.GetPropertyValue<string>("Name"),
                                Description = ctype.GetPropertyValue<string>("Description"),
                                Hidden = ctype.GetPropertyValue<bool>("Hidden")
                            });
                        }
                    }
                    else if (listProperty.Name == "Fields")
                    {
                        res.Fields = new List<EntityListFieldInfo>();
                        IEnumerable fields = listPropertyValue as IEnumerable;

                        if (fields != null)
                        {
                            res.Fields = fields.Cast<object>().Select(field => new EntityListFieldInfo
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
                            });
                        }
                    }
                        //else if (listProperty.Name == "List")
                        //{

                        //}
                    else
                    {
                        PropertyInfo property = typeof (EntityListMetaData).GetProperty(listProperty.Name);

                        if (property != null)
                        {
                            property.SetValue(res, listPropertyValue, null);
                        }
                    }
                }
            }

            return res;
        }
    }
}
