namespace Actors
{
    public class RoadInfo
    {
        public string RoadId { get; }
        public int SectionLengthInKm { get; }
        public int MaxAllowedSpeedInKmh { get; }
        public int LegalCorrectionInKmh { get; }

        public RoadInfo(string roadId, int sectionLengthInKm,
            int maxAllowedSpeedInKmh, int legalCorrectionInKmh)
        {
            RoadId = roadId;
            SectionLengthInKm = sectionLengthInKm;
            MaxAllowedSpeedInKmh = maxAllowedSpeedInKmh;
            LegalCorrectionInKmh = legalCorrectionInKmh;
        }
    }
}