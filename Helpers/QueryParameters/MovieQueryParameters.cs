namespace MyApp.Helpers.QueryParameters;

public record MovieQueryParameters
{
    /// <summary>
    /// Номер сторінки
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Кількість елементів на сторінці
    /// </summary>
    public int Size { get; set; } = 2;

    /// <summary>
    /// Пошук
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Жанр
    /// </summary>
    public string? Genre { get; set; }

    /// <summary>
    /// Мінімальний рік
    /// </summary>
    public int? MinYear { get; set; }

    /// <summary>
    /// Максимальний рік
    /// </summary>
    public int? MaxYear { get; set; }

    /// <summary>
    /// Сортування Приклад "title_asc", "title_desc", "year_asc", "year_desc"
    /// </summary>
    public string? Sort { get; set; }
   
}
