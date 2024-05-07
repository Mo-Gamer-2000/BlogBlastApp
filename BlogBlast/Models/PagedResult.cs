namespace BlogBlast.Models;
// Definition of a generic paged result containing records and total count
public record PagedResult<TResult>(TResult[] Records, int TotalCount);
