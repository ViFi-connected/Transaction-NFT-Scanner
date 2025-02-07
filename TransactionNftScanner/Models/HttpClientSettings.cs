public class HttpClientSettings
{
    public HttpClientConfig Blackfrost { get; set; }
    public HttpClientConfig Ipfs { get; set; }
}

public class HttpClientConfig
{
    public string BaseAddress { get; set; }
    public string ProjectId { get; set; }
    public int Timeout { get; set; }
}
