using System;
using System.Globalization;
using System.Linq.Expressions;
using Microsoft.SharePoint;
using SPCore.Formula.Base.Interfaces;
using Basic = SPCore.Formula.Elements.Basic;

namespace SPCore.Formula
{
    ////Examples:
    ////=IF(Column1<=Column2, "OK", "Not OK")
    //SPFormulaBuilder.CreateFormula(new If(
    //                                       new Expression(
    //                                             () =>
    //                                                 new ColumnReference("Column1") <=
    //                                                 new ColumnReference("Column2")
    //                                              ),
    //                                        new StringLiteral("OK"),
    //                                        new StringLiteral("Not OK")
    //                                        ));

    ////=DATE(YEAR(Column1)+Column2,MONTH(Column1),DAY(Column1))
    //SPFormulaBuilder.CreateFormula(new Date(
    //                                   new Expression(
    //                                       () =>
    //                                       new Year(new ColumnReference("Column1")) +
    //                                       new ColumnReference("Column2")
    //                                       ), new Month(new ColumnReference("Column1")),
    //                                   new Day(new ColumnReference("Column1"))
    //                                   ));

    ////=Column1+Column2+Column3
    //SPFormulaBuilder.CreateFormula(
    //               () =>
    //               new ColumnReference("Column1") + new ColumnReference("Column2") + new ColumnReference("Column3"));

    public class SPFormulaBuilder
    {
        /// <summary>
        /// Do not allow to create instances of this type
        /// </summary>
        private SPFormulaBuilder()
        {
        }

        /// <summary>
        /// Used to create: "={Text}"
        /// Note: Formula class cannot be used directly. Instead of instantiating Formula class, use this method.
        /// </summary>
        public static string CreateFormula<T>(T element, bool useEnvironmentCulture = false)
            where T : Base.Element, IValueType, IElementType
        {
            return new Basic.Formula<T>(element) { UseEnvironmentCulture = useEnvironmentCulture }.ToString();
        }

        public static string CreateFormula<T>(T element, CultureInfo culture)
            where T : Base.Element, IValueType, IElementType
        {
            return new Basic.Formula<T>(element, culture).ToString();
        }

        /// <summary>
        /// Used to create: "={Text}"
        /// Note: Formula class cannot be used directly. Instead of instantiating Formula class, use this method.
        /// </summary>
        public static string CreateFormula(Expression<Func<string>> expression, bool useEnvironmentCulture = false)
        {
            return new Basic.Formula<Basic.Expression>(expression) { UseEnvironmentCulture = useEnvironmentCulture }.ToString();
        }

        public static string CreateFormula(Expression<Func<string>> expression, CultureInfo culture)
        {
            return new Basic.Formula<Basic.Expression>(expression, culture).ToString();
        }

        public static void SetFormula<T>(SPField field, T element, bool useEnvironmentCulture = false)
             where T : Base.Element, IValueType, IElementType
        {
            field.DefaultFormula = CreateFormula(element, useEnvironmentCulture);
        }

        public static void SetFormula<T>(SPField field, T element, CultureInfo culture)
             where T : Base.Element, IValueType, IElementType
        {
            field.DefaultFormula = CreateFormula(element, culture);
        }

        public static void SetFormula(SPField field, Expression<Func<string>> expression, bool useEnvironmentCulture = false)
        {
            field.DefaultFormula = CreateFormula(expression, useEnvironmentCulture);
        }

        public static void SetFormula(SPField field, Expression<Func<string>> expression, CultureInfo culture)
        {
            field.DefaultFormula = CreateFormula(expression, culture);
        }
    }
}
