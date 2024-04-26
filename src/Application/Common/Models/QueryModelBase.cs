namespace Application.Common.Models
{
    public abstract class QueryModelBase
    {
        public decimal ReturnCode { get; set; } = -1;
        public string ReturnMsg { get; set; } = string.Empty;
    }

    public abstract class QueryModelListBase<T> : QueryModelBase
    {
        public List<T> Data { get; set; } = new List<T>();
    }
}
