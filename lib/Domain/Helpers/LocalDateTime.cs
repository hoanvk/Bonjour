
namespace Bonjour.Domain.Helpers;

public class LocalDateTime : ValueObject
{
    private DateTime? datetime;

    public LocalDateTime(DateTime? datetime)
    {
        this.datetime = datetime;
    }

    public string Format()
    {
        return datetime.HasValue
        ? datetime.Value.ToString("yyyy-MM-dd HH:mm:ss")
        : string.Empty;
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return datetime;
    }
}