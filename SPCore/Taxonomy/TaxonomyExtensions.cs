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
            foreach (Group group in groupCollection.Where(group => group.Name == name))
            {
                return group;
            }
            throw new ArgumentOutOfRangeException("name", name, "Could not find the taxonomy group");
        }

        public static Term GetByName(this TermCollection termSets, string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Term set name cannot be empty", "name");
            }
            foreach (var termSet in termSets.Where(termSet => termSet.Name.Equals(name, StringComparison.InvariantCulture)))
            {
                return termSet;
            }
            throw new ArgumentOutOfRangeException("name", name, "Could not find the term set");
        }

        public static Term GetByName(this TermSetItem termSet, string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Term set name cannot be empty", "name");
            }

            Term term = termSet is TermSet
                            ? (termSet as TermSet).GetTerms(name, true, StringMatchOption.ExactMatch, 1, false).FirstOrDefault()
                            : termSet.Terms.GetByName(name);
            return term;
        }

        public static TermSet GetByName(this TermSetCollection termSets, string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Term set name cannot be empty", "name");
            }
            foreach (var termSet in termSets.Where(termSet => termSet.Name == name))
            {
                return termSet;
            }
            throw new ArgumentOutOfRangeException("name", name, "Could not find the term set");
        }
    }
}
