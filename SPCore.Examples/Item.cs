using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Linq;
using SPCore.Linq;

namespace SPCore.Examples
{
    [DerivedEntityClass(Type = typeof(Employee))]
    [DerivedEntityClass(Type = typeof(Department))]
    [DerivedEntityClass(Type = typeof(Branch))]
    [DerivedEntityClass(Type = typeof(Company))]
    public class Item : EntityItem
    {
    }
}
