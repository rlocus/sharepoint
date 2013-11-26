//using System;
//using System.Data;
//using Microsoft.SharePoint;
//using Microsoft.SharePoint.Utilities;

//namespace SPCore
//{
//    public class SPPagedDataSource
//    {
//        #region [ Constants ]
//        // below 2 string format are good if you have orderby in your caml query.
//        const string NextPageWithOrderString = "Paged=TRUE&p_FSObjType=0&p_{0}={1}&p_ID={2}";
//        const string PreviousPageWithOrderString = "Paged=TRUE&p_FSObjType=0&PagedPrev=TRUE&p_{0}={1}&p_ID={2}";
//        ////below 2 string format are good if you dont have orderby in your caml query.
//        const string NextPageString = "Paged=TRUE&p_ID={0}";
//        const string PreviousPageString = "Paged=TRUE&PagedPrev=TRUE&p_ID={0}";
//        #endregion

//        #region [ Properties ]
//        public DataTable DataTable { get; private set; }

//        public SPListItemCollectionPosition ItemCollPosition { get; private set; }

//        public SPListItem FirstItem { get; private set; }

//        public SPListItem LastItem { get; private set; }

//        public int ItemsCount { get; private set; }

//        public DataRow this[int index]
//        {
//            get
//            {
//                return DataTable != null && DataTable.Rows.Count > 0 ? DataTable.Rows[index] : null;
//            }
//        }

//        #endregion

//        #region [ Constructors ]
//        public SPPagedDataSource(SPListItemCollection itemColl)
//        {
//            if (itemColl == null) { return; }

//            ItemCollPosition = itemColl.ListItemCollectionPosition;

//            if (itemColl.Count > 0)
//            {
//                FirstItem = itemColl[0];
//                LastItem = itemColl[itemColl.Count - 1];
//                DataTable = itemColl.GetDataTable();
//                ItemsCount = itemColl.Count;
//            }

//        }

//        #endregion

//        #region [ Public methods ]
//        public string GetPreviousPagingInfo()
//        {
//            return GetPreviousPagingInfo(null);
//        }

//        public string GetPreviousPagingInfo(SPField sortBy)
//        {
//            if (FirstItem != null)
//            {
//                if (sortBy != null)
//                {
//                    string filterValue = GetFilterValue(FirstItem[sortBy.InternalName], sortBy.FieldValueType);
//                    return string.Format(PreviousPageWithOrderString, sortBy.InternalName, filterValue, FirstItem.ID);
//                }

//                return string.Format(PreviousPageString, FirstItem.ID);
//            }

//            return string.Empty;
//        }

//        public string GetNextPagingInfo()
//        {
//            return GetNextPagingInfo(null);
//        }

//        public string GetNextPagingInfo(SPField sortBy)
//        {
//            if (LastItem != null)
//            {
//                if (sortBy != null)
//                {
//                    string filterValue = GetFilterValue(LastItem[sortBy.InternalName], sortBy.FieldValueType);
//                    return string.Format(NextPageWithOrderString, sortBy.InternalName, filterValue, LastItem.ID);
//                }

//                return string.Format(NextPageString, LastItem.ID);
//            }

//            return string.Empty;
//        }
//        #endregion

//        #region [ Private methods ]
//        private string GetFilterValue(object filterFieldValue, Type filterFieldType)
//        {
//            string filterValue = string.Empty;

//            if (filterFieldValue != null)
//            {
//                if (filterFieldType == typeof(DateTime))
//                {
//                    filterValue = SPEncode.UrlEncode(((DateTime)filterFieldValue).ToUniversalTime().ToString("yyyyMMdd hh:mm:ss"));
//                }
//                else
//                {
//                    SPEncode.UrlEncode((string)filterFieldValue);
//                }
//            }

//            return filterValue;
//        }
//        #endregion
//    }
//}
