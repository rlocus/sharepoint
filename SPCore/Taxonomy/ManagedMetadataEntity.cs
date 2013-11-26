using System;

namespace SPCore.Taxonomy
{
    public class ManagedMetadataEntity
    {
        public string TermSetName { get; set; }
        public string TermSetDescription { get; set; }
        public int? LCID { get; set; }
        public bool AvailableForTagging { get; set; }
        public string TermDescription { get; set; }
        public string Level1Term { get; set; }
        public string Level2Term { get; set; }
        public string Level3Term { get; set; }
        public string Level4Term { get; set; }
        public string Level5Term { get; set; }
        public string Level6Term { get; set; }
        public string Level7Term { get; set; }

        public bool HasSameLevelTerm(int termLevel, ManagedMetadataEntity entity)
        {
            var levelTerms = new[]
                                      {
                                          Level1Term, Level2Term, Level3Term, Level4Term, Level5Term, Level6Term,
                                          Level7Term
                                      };

            for (int i = 0; i < Math.Min(termLevel, levelTerms.Length); i++)
            {
                string levelTerm1 = (levelTerms[i] ?? string.Empty).Trim();
                string levelTerm2 = GetTermLevel(entity, i + 1);

                if (!levelTerm1.Equals(levelTerm2))
                {
                    return false;
                }
            }

            return true;
        }

        public string GetTermLevel(int termLevel)
        {
            return GetTermLevel(this, termLevel);
        }

        private static string GetTermLevel(ManagedMetadataEntity entity, int termLevel)
        {
            switch (termLevel)
            {
                case 1:
                    return (entity.Level1Term ?? string.Empty).Trim();
                case 2:
                    return (entity.Level2Term ?? string.Empty).Trim();
                case 3:
                    return (entity.Level3Term ?? string.Empty).Trim();
                case 4:
                    return (entity.Level4Term ?? string.Empty).Trim();
                case 5:
                    return (entity.Level5Term ?? string.Empty).Trim();
                case 6:
                    return (entity.Level6Term ?? string.Empty).Trim();
                case 7:
                    return (entity.Level7Term ?? string.Empty).Trim();
                default:
                    return string.Empty;
            }
        }

    }
}
