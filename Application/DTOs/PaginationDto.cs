namespace SDTechnicalAssessment.Application.DTOs
{
    // This class contains pagination information
    // returned to the API client.
    public class PaginationDto<T>
    {
        // The actual data for the current page.
        public List<T> Data { get; set; } = new();

        // Current page number.
        public int PageNumber { get; set; }

        // Number of records requested per page.
        public int PageSize { get; set; }

        // Total number of records available in the database.
        public int TotalRecords { get; set; }

        // Total number of pages available.
        public int TotalPages { get; set; }
    }
}