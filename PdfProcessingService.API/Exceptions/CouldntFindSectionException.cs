namespace PdfProcessingService.API.Exceptions
{
    public class CouldntFindSectionException : Exception
    {
        private static string GetMessage(string sectionName)
        {
            return $"Couldn't find {sectionName} section.";            
        }

        public CouldntFindSectionException(string sectionName) 
            : base(GetMessage(sectionName))
        {            
        }
    }
}
