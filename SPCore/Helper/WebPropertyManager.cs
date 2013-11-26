using System;
using Microsoft.SharePoint;

namespace SPCore.Helper
{
    public class WebPropertyManager
    {
        private readonly SPWeb _web;
        private bool _modified;

        public WebPropertyManager(SPWeb web)
        {
            if (web == null) throw new ArgumentNullException("web");
            _web = web;
        }

        public void Write(string propertyName, string propertyValue)
        {
            if (_web.Properties.ContainsKey(propertyName))
            {
                if (_web.Properties[propertyName] != propertyValue)
                {
                    _web.Properties[propertyName] = propertyValue;
                    _modified = true;
                }
            }
            else
            {
                _web.Properties.Add(propertyName, propertyValue);
                _modified = true;
            }
        }

        public void Delete(string propertyName)
        {
            if (_web.Properties.ContainsKey(propertyName))
            {
                _web.Properties[propertyName] = null;
                //_web.Properties.Remove(propertyName);
                _modified = true;
            }
        }

        public string Read(string propertyName)
        {
            return _web.Properties.ContainsKey(propertyName) ? _web.Properties[propertyName] : string.Empty;
        }

        public void Commit()
        {
            if(_modified)
            {
                _web.Properties.Update();
                _modified = false;
            }
        }
    }
}
