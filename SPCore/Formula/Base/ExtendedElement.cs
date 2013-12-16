﻿using System;
using SPCore.Formula.Base.Interfaces;
using SPCore.Formula.Elements.Conditional;

namespace SPCore.Formula.Base
{
    public abstract class ExtendedElement : Element, IConditionalType
    {
        #region + Operator
        public static string operator +(String e1, ExtendedElement e2)
        {
            return String.Format("{0}{1}{2}", e1, "+", e2);
        }

        public static string operator +(ExtendedElement e1, String e2)
        {
            return String.Format("{0}{1}{2}", e1, "+", e2);
        }

        public static string operator +(ExtendedElement e1, ExtendedElement e2)
        {
            return String.Format("{0}{1}{2}", e1, "+", e2);
        }
        #endregion

        #region - Operator

        public static string operator -(String e1, ExtendedElement e2)
        {
            return String.Format("{0}{1}{2}", e1, "-", e2);
        }

        public static string operator -(ExtendedElement e1, String e2)
        {
            return String.Format("{0}{1}{2}", e1, "-", e2);
        }

        public static string operator -(ExtendedElement e1, ExtendedElement e2)
        {
            return String.Format("{0}{1}{2}", e1, "-", e2);
        }

        #endregion

        #region * Operator

        public static string operator *(String e1, ExtendedElement e2)
        {
            return String.Format("{0}{1}{2}", e1, "*", e2);
        }

        public static string operator *(ExtendedElement e1, String e2)
        {
            return String.Format("{0}{1}{2}", e1, "*", e2);
        }

        public static string operator *(ExtendedElement e1, ExtendedElement e2)
        {
            return String.Format("{0}{1}{2}", e1, "*", e2);
        }

        #endregion

        #region / Operator

        public static string operator /(String e1, ExtendedElement e2)
        {
            return String.Format("{0}{1}{2}", e1, "/", e2);
        }

        public static string operator /(ExtendedElement e1, String e2)
        {
            return String.Format("{0}{1}{2}", e1, "/", e2);
        }

        public static string operator /(ExtendedElement e1, ExtendedElement e2)
        {
            return String.Format("{0}{1}{2}", e1, "/", e2);
        }

        #endregion

        #region & Operator

        public static string operator &(String e1, ExtendedElement e2)
        {
            return String.Format("{0}{1}{2}", e1, "&", e2);
        }

        public static string operator &(ExtendedElement e1, String e2)
        {
            return String.Format("{0}{1}{2}", e1, "&", e2);
        }

        public static string operator &(ExtendedElement e1, ExtendedElement e2)
        {
            return String.Format("{0}{1}{2}", e1, "&", e2);
        }

        #endregion

        #region ^ Operator

        public static string operator ^(String e1, ExtendedElement e2)
        {
            return String.Format("{0}{1}{2}", e1, "^", e2);
        }

        public static string operator ^(ExtendedElement e1, String e2)
        {
            return String.Format("{0}{1}{2}", e1, "^", e2);
        }

        public static string operator ^(ExtendedElement e1, ExtendedElement e2)
        {
            return String.Format("{0}{1}{2}", e1, "^", e2);
        }

        #endregion

        public static string operator >(ExtendedElement e1, ExtendedElement e2)
        {
            return String.Format("{0}{1}{2}", e1, ">", e2);
        }

        public static string operator <(ExtendedElement e1, ExtendedElement e2)
        {
            return String.Format("{0}{1}{2}", e1, "<", e2);
        }

        public static string operator >=(ExtendedElement e1, ExtendedElement e2)
        {
            return String.Format("{0}{1}{2}", e1, ">=", e2);
        }

        public static string operator <=(ExtendedElement e1, ExtendedElement e2)
        {
            return String.Format("{0}{1}{2}", e1, "<=", e2);
        }

        public static string operator ==(ExtendedElement e1, ExtendedElement e2)
        {
            return String.Format("{0}{1}{2}", e1, "=", e2);
        }

        public static string operator !=(ExtendedElement e1, ExtendedElement e2)
        {
            return new Not(() => e1 == e2).ToString();
        }

        public static string operator !(ExtendedElement e)
        {
            return new Not(e).ToString();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}
