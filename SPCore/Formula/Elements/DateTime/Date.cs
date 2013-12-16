using SPCore.Formula.Base;
using SPCore.Formula.Base.Attributes;
using SPCore.Formula.Base.Interfaces;

namespace SPCore.Formula.Elements.DateTime
{
    public class Date : ExtendedElement, IValueType
    {
        [RequiredParameter]
        public IValueType Year;

        [RequiredParameter]
        public IValueType Month;

        [RequiredParameter]
        public IValueType Day;

        protected override string Template
        {
            get 
            {
                return "DATE({Year}" + SectionSeparator + "{Month}" + SectionSeparator + "{Day})";
            }
        }

        /// <summary>
        /// Used to create: DATE({Year},{Month},{Day})
        /// </summary>
        public Date(IValueType year, IValueType month, IValueType day)
        {
            this.Year = year;
            this.Month = month;
            this.Day = day;
        }        
    }
}
