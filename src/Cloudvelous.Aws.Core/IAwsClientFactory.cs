namespace Cloudvelous.Aws.Core;

/// <summary>
/// Factory interface for creating AWS service clients
/// </summary>
/// <typeparam name="TClient">The AWS client type</typeparam>
public interface IAwsClientFactory<TClient> where TClient : class
{
    /// <summary>
    /// Creates a configured AWS client instance
    /// </summary>
    /// <returns>A configured AWS client</returns>
    TClient CreateClient();
}
