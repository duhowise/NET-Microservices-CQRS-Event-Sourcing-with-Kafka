namespace Post.Common.Options;

public class OpenTelemetryConfig
{
    public string ServiceName { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public string Protocol { get; set; }
}