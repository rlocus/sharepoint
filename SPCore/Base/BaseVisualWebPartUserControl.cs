//using System;
//using System.Web.UI;
//using System.Web.UI.WebControls.WebParts;

//namespace SPCore.Base
//{
//    public abstract class BaseVisualWebPartUserControl : UserControl
//    {
//        /// <summary>
//        /// Associated Web Part
//        /// </summary>
//        public IBaseVisualWebPart WebPart { get; private set; }

//        public string Title { get; private set; }

//        public void Initialize(IBaseVisualWebPart webPart)
//        {
//            if (webPart == null) throw new ArgumentNullException("webPart");

//            WebPart = webPart;
//            InitializeSettings(webPart as WebPart);
//        }

//        protected virtual void InitializeSettings(WebPart webPart)
//        {
//            Title = webPart.Title;
//        }
//    }
//}
