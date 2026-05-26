namespace _10xPV.Services.Csv.Contracts;

public static class CsvErrorCodes
{
    public const string EmptyFile = "CSV_EMPTY_FILE";
    public const string HeaderMissing = "CSV_HEADER_MISSING";
    public const string HeaderInvalid = "CSV_HEADER_INVALID";
    public const string RowFieldRequired = "CSV_ROW_FIELD_REQUIRED";
    public const string RowDateTimeInvalid = "CSV_ROW_DATETIME_INVALID";
    public const string RowNumberInvalid = "CSV_ROW_NUMBER_INVALID";
    public const string RowNumberNanOrInf = "CSV_ROW_NUMBER_NAN_OR_INF";
}