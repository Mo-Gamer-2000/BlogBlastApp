/*
 * This class defines the PagedResult record, which represents a generic paged result containing records and total count.
 */

namespace BlogBlast.Models
{
    // Represents a generic paged result containing records and total count
    public record PagedResult<TResult>(TResult[] Records, int TotalCount);
}