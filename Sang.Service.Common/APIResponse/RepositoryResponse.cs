namespace Sang.Service.Common.ApiResponse
{
    public class RepositoryResponse
    {
        public object Data { get; set; }
        public ErrorType ErrorType { get; set; }
        public string? ErrorMessage { get; set; }
        //public IEnumerable<string> ErrorMessage { get; set; }
    }
}
