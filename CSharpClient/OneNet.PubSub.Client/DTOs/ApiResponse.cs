namespace OneNet.PubSub.Client.DTOs
{
    internal class ApiResponse<T>
    {
        public int Status { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
    }

    internal class ApiResponse
    {
        public int Status { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }
    }
}