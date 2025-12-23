namespace Scribbly.Cubby;

public interface ICubbyClient
{
    Task Lock(CancellationToken token);
}