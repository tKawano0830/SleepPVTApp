namespace SleepPvtTracker.Core.Exceptions;

//memo:UI側でシステムエラーと明確に判別するために専用の例外を設けておく

public class DomainException : Exception
{
    /// <summary>
    /// ビジネスルール違反エラー
    /// </summary>
    /// <param name="message"></param>
    public DomainException(string message) : base(message) { }
}

public class EntityNotFoundException : Exception
{
    /// <summary>
    /// エンティティが見つからない場合のエラー
    /// </summary>
    /// <param name="message"></param>
    public EntityNotFoundException(string message) : base(message) { }
}