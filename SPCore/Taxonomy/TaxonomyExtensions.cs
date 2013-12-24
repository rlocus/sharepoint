using System;
using System.Linq;
using Microsoft.SharePoint.Taxonomy;

namespace SPCore.Taxonomy
{
    public static class TaxonomyExtensions
    {
        public static Group GetByName(this GroupCollection groupCollection, string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Taxonomy group name cannot be empty", "name");
            }

            return groupCollection.SingleOrDefault(group => string.Equals(group.Name, name));
        }

        public static Term GetByName(this TermCollection termSets, string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Term set name cannot be empty", "name");
            }
            return termSets.SingleOrDefault(termSet => string.Equals(termSet.Name, name));
        }

        public static Term GetByName(this TermSetItem termSet, string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Term set name cannot be empty", "name");
            }

            Term term = termSet is TermSet
                            ? (termSet as TermSet).GetTerms(name, true, StringMatchOption.ExactMatch, 1, false).SingleOrDefault()
                            : termSet.Terms.GetByName(name);
            return term;
        }

        public static TermSet GetByName(this TermSetCollection termSets, string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Term set name cannot be empty", "name");
            }
            return termSets.SingleOrDefault(termSet => string.Equals(termSet.Name, name));
        }
    }
}
