using System;
using System.ComponentModel;

namespace SPCore.BusinessData
{
    public abstract class BaseEntity : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static readonly PropertyChangingEventArgs EmptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        ///<summary>
        ///</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        ///<summary>
        ///</summary>
        public event PropertyChangingEventHandler PropertyChanging;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanging()
        {
            if (null != PropertyChanging)
            {
                PropertyChanging(this, EmptyChangingEventArgs);
            }
        }
    }
}
