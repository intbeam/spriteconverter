namespace SpriteConverter;

public interface IDistance<TSelf> where TSelf : IDistance<TSelf>
{
    float Distance(TSelf other);
}