using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SharePoint.Taxonomy;
using Group = Microsoft.SharePoint.Taxonomy.Group;
using GroupCollection = Microsoft.SharePoint.Taxonomy.GroupCollection;

namespace SPCore.Taxonomy
{
    public class ManagedMetadataManager
    {
        private const string FirstLine = "\"TermSetName\",\"TermSetDescription\",\"LCID\",\"AvailableForTagging\",\"TermDescription\",\"Level1\",\"Level2\",\"Level3\",\"Level4\",\"Level5\",\"Level6\",\"Level7\"";

        private readonly TermStore _termStore;
        private readonly string _groupName;
        private bool _allTermsAdded;
        private string _errorMessage;

        public ManagedMetadataManager(TermStore termStore, string groupName)
        {
            if (termStore == null) throw new ArgumentNullException("termStore");

            Delimiter = ",";
            UseDefaultImporter = false;
            Encoding = Encoding.UTF8;
            _termStore = termStore;
            _groupName = groupName;
        }

        /// <summary>
        /// The delimiter used to separate values (must be overridden)
        /// </summary>
        protected string Delimiter { get; set; }

        protected Encoding Encoding { get; set; }

        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

        public bool UseDefaultImporter { get; set; }

        public void ImportTermSet(StreamReader reader, bool ignoreExistingGroup, bool isOpenForTermCreation)
        {
            Group thisGroup = GetGroup(ignoreExistingGroup);

            if (UseDefaultImporter)
            {
                //Get Import Manager
                ImportManager manager = thisGroup.TermStore.GetImportManager();
                manager.ImportTermSet(thisGroup, reader, out _allTermsAdded, out _errorMessage);
            }
            else
            {
                string line;
                StringBuilder sb = new StringBuilder();

                while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                {
                    if (!line.Equals(FirstLine))
                        sb.AppendLine(line);
                }

                Hashtable parsed = ParseContent(sb.ToString(), Delimiter);
                var entities = GetEntities(parsed).ToArray();
                ImportTermSet(entities, thisGroup, isOpenForTermCreation);
            }
        }

        public void ImportTermSet(string filePath, bool ignoreExistingGroup, bool isOpenForTermCreation)
        {
            using (StreamReader reader = new StreamReader(filePath, Encoding, true))
            {
                ImportTermSet(reader, ignoreExistingGroup, isOpenForTermCreation);
                reader.Close();
            }
        }

        protected void ImportTermSet(IEnumerable<ManagedMetadataEntity> entities, Group termGroup, bool isOpenForTermCreation)
        {
            IEnumerable<ManagedMetadataEntity> termSetEntities = from e in entities
                                                                 where !string.IsNullOrEmpty(e.TermSetName)
                                                                 select e;

            // create term sets
            foreach (ManagedMetadataEntity termSetEntity in termSetEntities)
            {
                if (termSetEntity == null) { continue; }

                int lcid = termSetEntity.LCID.HasValue ? termSetEntity.LCID.Value : 1033;
                TermSet termSet = _termStore.GetTermSets(termSetEntity.TermSetName, lcid).FirstOrDefault();

                if (termSet != null)
                {
                    _termStore.WorkingLanguage = lcid; //set the working language to the language we want to add
                    termSet.Name = termSetEntity.TermSetName; //Set the language specific name
                }
                else
                {
                    termSet = termGroup.CreateTermSet(termSetEntity.TermSetName, Guid.NewGuid(), lcid);
                }

                termSet.Description = termSetEntity.TermSetDescription;
                termSet.IsAvailableForTagging = termSetEntity.AvailableForTagging;
                termSet.IsOpenForTermCreation = isOpenForTermCreation;

                var termLevels = new List<IEnumerable<ManagedMetadataEntity>>();

                #region First Level terms Query

                //TODO: add logic to identify parent term set.. and modify spreadsheet appropriately...
                IEnumerable<ManagedMetadataEntity> firstLevelTerms = from e in entities
                                                                     where (!String.IsNullOrEmpty(e.Level1Term) && String.IsNullOrEmpty(e.Level2Term)
                                                                            && String.IsNullOrEmpty(e.Level3Term) && String.IsNullOrEmpty(e.Level4Term)
                                                                            && String.IsNullOrEmpty(e.Level5Term) && string.IsNullOrEmpty(e.Level6Term)
                                                                            && String.IsNullOrEmpty(e.Level7Term) && e.LCID == termSetEntity.LCID)
                                                                     select e;

                termLevels.Add(firstLevelTerms);

                #endregion

                //Set custom order to match import
                //termSet.CustomSortOrder = String.Join(":", firstLevelTerms.Select(x => x.Level1Term).ToArray());
                _termStore.CommitAll();

                #region Second Level Terms Query

                var secondLevelTerms = from e in entities
                                       where (!String.IsNullOrEmpty(e.Level1Term) && !String.IsNullOrEmpty(e.Level2Term)
                                              && String.IsNullOrEmpty(e.Level3Term) && String.IsNullOrEmpty(e.Level4Term)
                                              && String.IsNullOrEmpty(e.Level5Term) && string.IsNullOrEmpty(e.Level6Term)
                                              && String.IsNullOrEmpty(e.Level7Term) && e.LCID == termSetEntity.LCID)
                                       select e;

                termLevels.Add(secondLevelTerms);

                #endregion

                #region Third Level Terms Query

                var thirdLevelTerms = from e in entities
                                      where (!String.IsNullOrEmpty(e.Level1Term) && !String.IsNullOrEmpty(e.Level2Term)
                                             && !String.IsNullOrEmpty(e.Level3Term) && String.IsNullOrEmpty(e.Level4Term)
                                             && String.IsNullOrEmpty(e.Level5Term) && string.IsNullOrEmpty(e.Level6Term)
                                             && String.IsNullOrEmpty(e.Level7Term) && e.LCID == termSetEntity.LCID)
                                      select e;

                termLevels.Add(thirdLevelTerms);

                #endregion

                #region Fourth Level Terms Query

                var fourthLevelTerms = from e in entities
                                       where (!String.IsNullOrEmpty(e.Level1Term) && !String.IsNullOrEmpty(e.Level2Term)
                                              && !String.IsNullOrEmpty(e.Level3Term) && !String.IsNullOrEmpty(e.Level4Term)
                                              && String.IsNullOrEmpty(e.Level5Term) && String.IsNullOrEmpty(e.Level6Term)
                                              && String.IsNullOrEmpty(e.Level7Term) && e.LCID == termSetEntity.LCID)
                                       select e;

                termLevels.Add(fourthLevelTerms);

                #endregion

                #region Fifth Level Terms Query

                var fifthLevelTerms = from e in entities
                                      where (!String.IsNullOrEmpty(e.Level1Term) && !String.IsNullOrEmpty(e.Level2Term)
                                             && !String.IsNullOrEmpty(e.Level3Term) && !String.IsNullOrEmpty(e.Level4Term)
                                             && !String.IsNullOrEmpty(e.Level5Term) && String.IsNullOrEmpty(e.Level6Term)
                                             && String.IsNullOrEmpty(e.Level7Term) && e.LCID == termSetEntity.LCID)
                                      select e;

                termLevels.Add(fifthLevelTerms);

                #endregion

                #region Sixth Level Terms Query

                var sixthLevelTerms = from e in entities
                                      where (!String.IsNullOrEmpty(e.Level1Term) && !String.IsNullOrEmpty(e.Level2Term)
                                             && !String.IsNullOrEmpty(e.Level3Term) && !String.IsNullOrEmpty(e.Level4Term)
                                             && !String.IsNullOrEmpty(e.Level5Term) && !String.IsNullOrEmpty(e.Level6Term)
                                             && String.IsNullOrEmpty(e.Level7Term) && e.LCID == termSetEntity.LCID)
                                      select e;

                termLevels.Add(sixthLevelTerms);

                #endregion

                #region Seventh Level Terms Query

                IEnumerable<ManagedMetadataEntity> seventhLevelTerms = from e in entities
                                                                       where (!String.IsNullOrEmpty(e.Level1Term) && !String.IsNullOrEmpty(e.Level2Term)
                                                                              && !String.IsNullOrEmpty(e.Level3Term) && !String.IsNullOrEmpty(e.Level4Term)
                                                                              && !String.IsNullOrEmpty(e.Level5Term) && !String.IsNullOrEmpty(e.Level6Term)
                                                                              && !String.IsNullOrEmpty(e.Level7Term) && e.LCID == termSetEntity.LCID)
                                                                       select e;

                termLevels.Add(seventhLevelTerms);

                #endregion

                foreach (ManagedMetadataEntity termEntity in firstLevelTerms)
                {
                    UpdateTerm(termSet, termEntity, termLevels, 1);
                }

                _termStore.CommitAll();
            }
        }

        protected virtual IEnumerable<ManagedMetadataEntity> GetEntities(Hashtable parsed)
        {
            var entities = (from List<string> values in parsed.Values select GetEntity(values));
            return entities;
        }

        private Group GetGroup(bool ignoreExistingGroup)
        {
            //Get TermStore Groups
            GroupCollection groups = _termStore.Groups;
            //Find group that we want to Import to
            Group thisGroup = groups.GetByName(_groupName);

            //Check that group exist
            if (thisGroup != null)
            {
                if (!ignoreExistingGroup)
                {
                    //Get all termset from that group
                    TermSetCollection termSets = thisGroup.TermSets;
                    //For each termset, delete it
                    foreach (TermSet termSet in termSets)
                    {
                        termSet.Delete();
                    }

                    //save all changes to TermStore
                    _termStore.CommitAll();
                }
            }
            //If group doesn't exist, create it
            else
            {
                thisGroup = _termStore.CreateGroup(_groupName);
            }

            return thisGroup;
        }

        private static ManagedMetadataEntity GetEntity(IEnumerable<string> values)
        {
            if (values == null)
            {
                return null;
            }

            int count = values.Count();
            ManagedMetadataEntity entity = new ManagedMetadataEntity();

            if (count > 0)
            {
                try
                {
                    entity.TermSetName = values.ElementAt(0);

                    if (count > 1)
                    {
                        entity.TermSetDescription = values.ElementAt(1);

                        if (count > 2)
                        {
                            int lcid;

                            if (int.TryParse(values.ElementAt(2), out lcid))
                            {
                                entity.LCID = entity.LCID == default(int) ? 1033 : lcid;
                            }
                            if (count > 3)
                            {
                                bool availableForTagging;
                                if (bool.TryParse(values.ElementAt(3), out availableForTagging))
                                {
                                    entity.AvailableForTagging = availableForTagging;
                                }

                                if (count > 4)
                                {
                                    entity.TermDescription = values.ElementAt(4);
                                    if (count > 5)
                                    {
                                        entity.Level1Term = values.ElementAt(5);

                                        if (count > 6)
                                        {
                                            entity.Level2Term = values.ElementAt(6);
                                        }
                                        if (count > 7)
                                        {
                                            entity.Level3Term = values.ElementAt(7);
                                        }
                                        if (count > 8)
                                        {
                                            entity.Level4Term = values.ElementAt(8);
                                        }
                                        if (count > 9)
                                        {
                                            entity.Level5Term = values.ElementAt(9);
                                        }
                                        if (count > 10)
                                        {
                                            entity.Level6Term = values.ElementAt(10);
                                        }
                                        if (count > 11)
                                        {
                                            entity.Level7Term = values.ElementAt(11);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }

            return entity;
        }

        private static Hashtable ParseContent(string content, string delimiter)
        {
            Hashtable hash = new Hashtable();

            if (string.IsNullOrEmpty(content))
            {
                return hash;
            }

            Regex rowSplitter = new Regex("[^\"\r\n]*(\r\n|\n|$)|(([^\"\r\n]*)(\"[^\"]*\")([^\"\r\n]*))*(\r\n|\n|$)");
            MatchCollection rowMatches = rowSplitter.Matches(content);

            int i = 0;
            foreach (Match rowMatch in rowMatches)
            {
                if (string.IsNullOrEmpty(rowMatch.Value)) continue;

                Regex cellSplitter = new Regex(string.Format("(?<Value>\"(?:[^\"]|\"\")*\"|[^{0}\r\n]*?)(?<Delimiter>{0}|\r\n|\n|$)", Regex.Escape(delimiter)));
                MatchCollection cellMatches = cellSplitter.Matches(rowMatch.Value);

                bool finished = false;
                List<string> values = new List<string>();
                foreach (Match cellMatch in cellMatches)
                {
                    if (!finished)
                    {
                        string value = cellMatch.Groups["Value"].Value;
                        value = value.Replace("\"", "");
                        values.Add(value);
                    }

                    finished = string.IsNullOrEmpty(cellMatch.Groups["Delimiter"].Value) || cellMatch.Groups["Delimiter"].Value == "\r\n"
                               || cellMatch.Groups["Delimiter"].Value == "\n";
                }

                if (values.Count > 0)
                {
                    hash.Add(i++, values);
                }
            }

            return hash;
        }

        private static void UpdateTerm(TermSetItem termSet, ManagedMetadataEntity entity, IEnumerable<IEnumerable<ManagedMetadataEntity>> termLevels, int currLevel)
        {
            int lcid = entity.LCID.HasValue ? entity.LCID.Value : 1033;
            string termName = TaxonomyItem.NormalizeName(entity.GetTermLevel(currLevel));

            if (string.IsNullOrEmpty(termName))
            {
                return;
            }

            Term term = termSet is TermSet
                            ? (termSet as TermSet).GetTerms(termName, lcid, true, StringMatchOption.ExactMatch, 1, false).
                                  FirstOrDefault()
                            : termSet.Terms.FirstOrDefault(t => t.Name.Equals(termName, StringComparison.InvariantCulture));

            if (term == null)
            {
                //try
                //{
                term = termSet.CreateTerm(termName, lcid, Guid.NewGuid());
                term.IsAvailableForTagging = entity.AvailableForTagging;
                //}
                //catch (Exception ex)
                //{
                //}
            }

            LabelCollection allLabels = term.GetAllLabels(lcid);

            if (allLabels.Count == 0 || !allLabels.Select(x => x.Value).Contains(termName))
            {
                //try
                //{
                term.CreateLabel(termName, lcid, true);
                term.SetDescription(entity.TermDescription, lcid);
                //}
                //catch (Exception ex)
                //{
                //}
            }

            if (termLevels == null) { return; }
            if (currLevel >= termLevels.Count()) return;

            IEnumerable<ManagedMetadataEntity> childList =
                termLevels.ElementAt(currLevel).Where(
                    t => t.HasSameLevelTerm(currLevel, entity));

            foreach (ManagedMetadataEntity childEntity in childList)
            {
                UpdateTerm(term, childEntity, termLevels, currLevel + 1);
            }
        }
    }
}
