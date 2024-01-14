using DriveImport.Common.Dtos;
using YANLib;

namespace DriveImport.Api.Mappers;

public sealed class SheetsContentMapper
{
    public static List<SheetsContentDto> MapFromRangeData(IList<IList<object>> values)
    {
        var items = new List<SheetsContentDto>();

        foreach (var value in values)
        {
            items.Add(new()
            {
                Id = value[0].ToString(),
                Name = value[1].ToString(),
                Age = value[2].ToInt()
            });
        }

        return items;
    }
    public static IList<IList<object>> MapToRangeData(SheetsContentDto dto) => new List<IList<object>>
    {
        new List<object>()
        {
            dto.Id ?? string.Empty,
            dto.Name ?? string.Empty,
            dto.Age
        }
    };
}
