using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;

namespace SPCore.Taxonomy
{
    public static class TaxonomyHelper
    {
        public static TaxonomyFieldValue GetTaxonomyFieldValue(SPSite site, Guid sspId, Guid termSetId, string value, bool addIfDoesNotExist/*, out bool newTermAdded*/)
        {
            bool newTermAdded;

            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            TaxonomySession taxonomySession = new TaxonomySession(site);
            TermStore termStore = taxonomySession.TermStores[sspId];
            TermSet termSet = termStore.GetTermSet(termSetId);

            TaxonomyFieldValue val = GetTaxonomyFieldValue(termSet, value, addIfDoesNotExist, out newTermAdded);

            if (newTermAdded)
            {
                termStore.CommitAll();
            }

            return val;
        }

        public static TaxonomyFieldValue GetTaxonomyFieldValue(TermSet termSet, string value, bool addIfDoesNotExist, out bool newTermAdded)
        {
            string termVal = TaxonomyItem.NormalizeName((value ?? string.Empty).Trim()); //ReplaceIllegalCharacters((value ?? string.Empty).Trim());
            Term term = null;
            newTermAdded = false;

            if (termSet != null)
            {
                if (!string.IsNullOrEmpty(termVal))
                {
                    term = termSet.GetTerms(termVal, termSet.TermStore.DefaultLanguage, true,
                                                 StringMatchOption.ExactMatch, 1, false).FirstOrDefault();
                }

                if (term == null && termSet.IsOpenForTermCreation && addIfDoesNotExist)
                {
                    if (!string.IsNullOrEmpty(termVal))
                    {
                        term = termSet.CreateTerm(termVal, termSet.TermStore.DefaultLanguage);
                        newTermAdded = true;
                        //termSet.TermStore.CommitAll();
                    }
                }

                if (term != null)
                {
                    string termValue = string.Concat(term.GetDefaultLabel(termSet.TermStore.DefaultLanguage),
                                                          TaxonomyField.TaxonomyGuidLabelDelimiter,
                                                          term.Id.ToString());
                    return TaxonomyFieldControl.GetTaxonomyValue(termValue);
                }
            }

            return null;
        }

        public static TaxonomyFieldValueCollection GetTaxonomyFieldValues(SPSite site, Guid sspId, Guid termSetId, IEnumerable<string> values, bool addIfDoesNotExist/*, out bool newTermsAdded*/)
        {
            bool newTermsAdded;
            TaxonomySession taxonomySession = new TaxonomySession(site);
            TermStore termStore = taxonomySession.TermStores[sspId];
            TermSet termSet = termStore.GetTermSet(termSetId);

            TaxonomyFieldValueCollection val = GetTaxonomyFieldValues(termSet, values, addIfDoesNotExist, out newTermsAdded);

            if (newTermsAdded)
            {
                termStore.CommitAll();
            }

            return val;
        }

        public static TaxonomyFieldValueCollection GetTaxonomyFieldValues(TermSet termSet, IEnumerable<string> values, bool addIfDoesNotExist, out bool newTermsAdded)
        {
            TaxonomyFieldValueCollection termValues = TaxonomyFieldControl.GetTaxonomyCollection("");
            newTermsAdded = false;

            if (values != null && values.Count() > 0)
            {
                bool[] newTermAddedResult = new bool[values.Count()];
                termValues.AddRange(values.Where(termValue => termValue != null)
                                        .Select((value, i) =>
                                                    {
                                                        bool newTermAdded;
                                                        TaxonomyFieldValue val = GetTaxonomyFieldValue(termSet,
                                                                                                       value,
                                                                                                       addIfDoesNotExist,
                                                                                                       out newTermAdded);
                                                        newTermAddedResult[i] = newTermAdded;
                                                        return val;
                                                    }));
                newTermsAdded = newTermAddedResult.Any(newTermAdded => newTermAdded);
            }

            return termValues;
        }

        public static TaxonomyFieldValue GetTaxonomyFieldValue(SPSite site, TaxonomyField field, string value, bool addIfDoesNotExist/*, out bool newTermAdded*/)
        {
            TaxonomyFieldValue taxonomyFieldValue = new TaxonomyFieldValue(field);
            TaxonomyFieldValue tempValue = GetTaxonomyFieldValue(site, field.SspId, field.TermSetId, value, addIfDoesNotExist/*, out newTermAdded*/);

            if (tempValue != null)
            {
                taxonomyFieldValue.PopulateFromLabelGuidPair(tempValue.ToString());
            }

            return taxonomyFieldValue;
        }

        public static TaxonomyFieldValueCollection GetTaxonomyFieldValues(SPSite site, TaxonomyField field, IEnumerable<string> values, bool addIfDoesNotExist/*, out bool newTermsAdded*/)
        {
            if (values == null) throw new ArgumentNullException("values");

            TaxonomySession taxonomySession = new TaxonomySession(site);
            TermStore termStore = taxonomySession.TermStores[field.SspId];
            TermSet termSet = termStore.GetTermSet(field.TermSetId);

            TaxonomyFieldValueCollection termValues = new TaxonomyFieldValueCollection(field);
            bool newTermsAdded = false;

            if (values != null && values.Count() > 0)
            {
                bool[] newTermAddedResult = new bool[values.Count()];
                termValues.AddRange(
                    values.Where(termValue => termValue != null)
                        .Select((value, i) =>
                                    {
                                        bool newTermAdded;
                                        TaxonomyFieldValue val = GetTaxonomyFieldValue(termSet,
                                                                                       value,
                                                                                       addIfDoesNotExist,
                                                                                       out newTermAdded);
                                        newTermAddedResult[i] = newTermAdded;
                                        return val;
                                    }));
                newTermsAdded = newTermAddedResult.Any(newTermAdded => newTermAdded);
            }

            if (newTermsAdded)
            {
                termStore.CommitAll();
            }

            return termValues;
        }

        public static void SetDefaultValue(SPSite site, TaxonomyField field, string defaultValue, bool addIfDoesNotExist/*, out bool newTermAdded*/)
        {
            if (field.AllowMultipleValues)
            {
                TaxonomyFieldValueCollection oTaxonomyFieldValues = GetTaxonomyFieldValues(site, field, new[] { defaultValue }, addIfDoesNotExist/*, out newTermAdded*/);

                string validatedString = field.GetValidatedString(oTaxonomyFieldValues);

                if (field.DefaultValue != validatedString)
                {
                    field.DefaultValue = validatedString;
                    field.Update();
                }
            }
            else
            {
                TaxonomyFieldValue oTaxonomyFieldValue = GetTaxonomyFieldValue(site, field, defaultValue, addIfDoesNotExist/*, out newTermAdded*/);

                //string validatedString = field.GetValidatedString(oTaxonomyFieldValue);

                if (field.DefaultValue != oTaxonomyFieldValue.ValidatedString)
                {
                    field.DefaultValue = oTaxonomyFieldValue.ValidatedString;
                    field.Update();
                }
            }
        }

        public static void SetDefaultValue(SPSite site, TaxonomyField field, string[] defaultValue, bool addIfDoesNotExist/*, out bool newTermAdded*/)
        {
            if (defaultValue == null || defaultValue.Length == 0)
            {
                //newTermAdded = false;
                return;
            }

            if (field.AllowMultipleValues)
            {
                TaxonomyFieldValueCollection oTaxonomyFieldValues = GetTaxonomyFieldValues(site, field, defaultValue, addIfDoesNotExist/*, out newTermAdded*/);

                string validatedString = field.GetValidatedString(oTaxonomyFieldValues);

                if (field.DefaultValue != validatedString)
                {
                    field.DefaultValue = validatedString;
                    field.Update();
                }
            }
            else
            {
                SetDefaultValue(site, field, defaultValue[0], addIfDoesNotExist/*, out newTermAdded*/);
            }
        }

        public static string ReplaceIllegalCharacters(string termLabel)
        {
            return termLabel.
                Replace("\t", " ").
                Replace(";", ",").
                Replace("\"", "\uFF02").
                Replace("<", "\uFF1C").
                Replace(">", "\uFF1E").
                Replace("|", "\uFF5C");
        }

        public static TaxonomyFieldValue GetTaxonomyFieldValue(SPSite site, TaxonomyField field, string value, out List<int> wssIds)
        {
            TaxonomySession taxonomySession = new TaxonomySession(site);
            TermStore termStore = taxonomySession.TermStores[field.SspId];
            TermSet termSet = termStore.GetTermSet(field.TermSetId);

            wssIds = new List<int>();
            TaxonomyFieldValue taxonomyFieldValue = TaxonomyFieldControl.GetTaxonomyValue(value);

            wssIds.AddRange(TaxonomyField.GetWssIdsOfTerm(site, termStore.Id, termSet.Id,
                                                          new Guid(taxonomyFieldValue.TermGuid), true, 100));

            return taxonomyFieldValue;
        }

        public static TaxonomyFieldValueCollection GetTaxonomyFieldValues(SPSite site, TaxonomyField field, string values, out List<int> wssIds)
        {
            if (values == null) throw new ArgumentNullException("values");

            TaxonomySession taxonomySession = new TaxonomySession(site);
            TermStore termStore = taxonomySession.TermStores[field.SspId];
            TermSet termSet = termStore.GetTermSet(field.TermSetId);

            TaxonomyFieldValueCollection termValues = new TaxonomyFieldValueCollection(field);
            termValues.AddRange(TaxonomyFieldControl.GetTaxonomyCollection(values));

            wssIds = new List<int>();
            foreach (TaxonomyFieldValue termValue in termValues)
            {
                wssIds.AddRange(TaxonomyField.GetWssIdsOfTerm(site, termStore.Id, termSet.Id,
                                                              new Guid(termValue.TermGuid), false, 500));
            }

            return termValues;
        }

        public static void ConnectTaxonomyField(TaxonomyField field, SPSite site, string termStoreName, string termGroup, string termSetName, bool isOpen = false, bool createValuesInEditForm = false)
        {
            if (site == null) { return; }

            TaxonomySession session = new TaxonomySession(site);
            ConnectTaxonomyField(field, session, termStoreName, termGroup, termSetName, isOpen, createValuesInEditForm);
        }

        public static void ConnectTaxonomyField(TaxonomyField field, SPSite site, string termGroup, string termSetName, bool isOpen = false, bool createValuesInEditForm = false)
        {
            if (site == null) { return; }

            TaxonomySession session = new TaxonomySession(site);
            ConnectTaxonomyField(field, session, termGroup, termSetName, isOpen, createValuesInEditForm);
        }

        public static void ConnectTaxonomyField(TaxonomyField field, TaxonomySession session, string termStoreName, string termGroup, string termSetName, bool isOpen = false, bool createValuesInEditForm = false)
        {
            TermStore termStore = session.TermStores.SingleOrDefault(ts => string.Equals(ts.Name, termStoreName));
            ConnectTaxonomyField(field, session, termStore, termGroup, termSetName, isOpen, createValuesInEditForm);
        }

        public static void ConnectTaxonomyField(TaxonomyField field, TaxonomySession session, string termGroup, string termSetName, bool isOpen = false, bool createValuesInEditForm = false)
        {
            TermStore termStore;

            if (field != null && field.SspId != default(Guid))
            {
                termStore = session.TermStores.SingleOrDefault(ts => ts.Id == field.SspId) ?? session.DefaultKeywordsTermStore;
            }
            else
            {
                termStore = session.DefaultKeywordsTermStore;
            }

            ConnectTaxonomyField(field, session, termStore, termGroup, termSetName, isOpen, createValuesInEditForm);
        }

        public static void ConnectTaxonomyField(TaxonomyField field, TaxonomySession session, TermStore termStore, string termGroup, string termSetName, bool isOpen = false, bool createValuesInEditForm = false)
        {
            if (field == null || session == null || termStore == null) { return; }

            Group group = termStore.Groups.GetByName(termGroup);

            if (group != null)
            {
                TermSet termSet = group.TermSets.GetByName(termSetName);
                // connect the field to the specified term
                if (termSet != null)
                {
                    field.SspId = termSet.TermStore.Id;
                    field.TermSetId = termSet.Id;
                    field.Open = isOpen && termSet.IsOpenForTermCreation;
                    field.CreateValuesInEditForm = field.Open && createValuesInEditForm;
                }
            }

            field.TargetTemplate = string.Empty;
            field.AnchorId = Guid.Empty;
            field.Update();
        }

    }
}
