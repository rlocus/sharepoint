//using System;
//using System.IO;
//using System.Text;
//using System.Web.UI;
//using System.Web.UI.WebControls.WebParts;
//using Microsoft.SharePoint;

//namespace SPCore.Base
//{
//    public abstract class BaseVisualWebPart<T> : WebPart, IBaseVisualWebPart
//       where T : BaseVisualWebPartUserControl
//    {
//        #region Private Fields

//        private readonly StringBuilder _errorOutput = new StringBuilder();
//        private bool _abortProcessing;

//        #endregion

//        #region Public Properties

//        public abstract string AscxPath { get; }
//        public BaseVisualWebPartUserControl WebPartUserControl { get; private set; }

//        #endregion

//        #region Event Handlers

//        protected override void CreateChildControls()
//        {
//            try
//            {
//                WebPartUserControl = (T)Page.LoadControl(AscxPath);

//                if (WebPartUserControl != null)
//                {
//                    WebPartUserControl.Initialize(this);
//                    Controls.Add(WebPartUserControl);
//                }
//            }
//            catch (Exception ex)
//            {
//                HandleException(ex);
//            }

//            base.CreateChildControls();
//        }

//        protected override void OnLoad(EventArgs e)
//        {
//            base.OnLoad(e);
//            EnsureChildControls();
//        }

//        protected override void Render(HtmlTextWriter writer)
//        {
//            StringBuilder tempOutput = new StringBuilder();

//            if (!_abortProcessing)
//            {
//                HtmlTextWriter tempWriter = new HtmlTextWriter(new StringWriter(tempOutput));

//                try
//                {
//                    base.Render(tempWriter);
//                }
//                catch (Exception ex)
//                {
//                    HandleException(ex);
//                }
//            }

//            writer.Write(_abortProcessing ? _errorOutput.ToString() : tempOutput.ToString());
//        }

//        #endregion

//        #region Exception handling section

//        public void HandleException(Exception ex)
//        {
//            _abortProcessing = true;

//            using (StringWriter sw = new StringWriter(_errorOutput))
//            {
//                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
//                {
//                    HandleException(ex.Message, ex.StackTrace, htw);
//                }
//            }
//        }

//        protected virtual void HandleException(string errorMessage, string errorTrace, HtmlTextWriter writer)
//        {
//            string messageHtml = SPContext.Current.Web.CurrentUser.IsSiteAdmin
//                                    ? string.Format(
//                                                "<div><div class=\"ms-error\">{0}</div><br/><div>{1}</div></div>",
//                                                errorMessage,
//                                                errorTrace)
//                                    : "<div class=\"ms-error\">An error has occurred. Please contact your system administrator.</div>";
//            writer.Write(messageHtml);
//        }

//        #endregion

//    }
//}
