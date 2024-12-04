namespace Monads;

public readonly struct OptionResult<T>
{
    private readonly T? value = default;
    private readonly Error error;

    public bool HasValue => value is Unit || !EqualityComparer<T>.Default.Equals(value, default);
    public bool HasError => !EqualityComparer<Error>.Default.Equals(error, default);

    private OptionResult(T value)
    {
        this.value = value;
    }

    private OptionResult(None _)
    {
        value = default;
    }

    private OptionResult(Error error)
    {
        this.error = error;
    }

    /// <summary>
    /// Executa a função correspondente ao estado do objeto
    /// </summary>
    /// <typeparam name="TOut">Tipo de saída</typeparam>
    /// <param name="some">Função a ser executada caso haja valor</param>
    /// <param name="none">Função a ser executada caso NÃO haja valor</param>
    /// <param name="error">Função a ser executada em caso de erro</param>
    /// <returns></returns>
    public TOut Match<TOut>(Func<T, TOut> some, Func<TOut> none, Func<Error, TOut> error)
        => (HasValue, HasError) switch
        {
            (_, true) => error(this.error),
            (true, _) => some(value!),
            _ => none()
        };

    /// <summary>
    /// Mapeia um valor para outro, através da aplicação de uma função.
    /// </summary>
    /// <typeparam name="TOut">Tipo de saída</typeparam>
    /// <param name="func">Função a ser aplicada</param>
    /// <returns><![CDATA[OptionResult<TOut>]]></returns>
    public OptionResult<TOut> Then<TOut>(Func<T, TOut> func)
        => (HasValue, HasError) switch
        {
            (_, true) => error,
            (true, _) => Try(value, func!),
            _ => None.Value
        };

    /// <summary>
    /// Mapeia um valor para outro, através da aplicação de uma função.
    /// </summary>
    /// <typeparam name="TOut">Tipo de saída</typeparam>
    /// <param name="func">Função a ser aplicada</param>
    /// <returns><![CDATA[OptionResult<TOut>]]></returns>
    public OptionResult<TOut> Then<TOut>(Func<T, OptionResult<TOut>> func)
        => (HasValue, HasError) switch
        {
            (_, true) => error,
            (true, _) => Try(value, func!),
            _ => None.Value
        };

    /// <summary>
    /// Envolve uma função em um bloco try-catch.
    /// </summary>
    private OptionResult<TOut> Try<TIn, TOut>(TIn? value, Func<TIn?, TOut> function, Action<Error>? errorHandler = null)
    {
        try
        {
            return function(value);
        }
        catch (Exception ex)
        {
            var error = new Error(ex.Message, ex);
            errorHandler?.Invoke(error);

            return error;
        }
    }

    /// <summary>
    /// Envolve uma função em um bloco try-catch.
    /// </summary>
    private OptionResult<TOut> Try<TIn, TOut>(TIn? value, Func<TIn?, OptionResult<TOut>> function, Action<Error>? errorHandler = null)
    {
        try
        {
            return function(value);
        }
        catch (Exception ex)
        {
            var error = new Error(ex.Message, ex);
            errorHandler?.Invoke(error);

            return error;
        }
    }

    public static implicit operator OptionResult<T>(T value) => new(value);
    public static implicit operator OptionResult<T>(None none) => new(none);
    public static implicit operator OptionResult<T>(Error error) => new(error);
}

/// <summary>
/// Tipo que denota a ausência de valor
/// </summary>
public readonly struct None
{
    public static None Value => new();
}

/// <summary>
/// Tipo usado para métodos que não retornam valor (substituto ao tipo void)
/// </summary>
public readonly struct Unit
{
    public static Unit Value => new();
}

/// <summary>
/// Informações sobre o erro
/// </summary>
public readonly struct Error
{
    /// <summary>
    /// Mensagem de erro
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Exceção ocorrida (opcional)
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Informações adicionais sobre o erro (opcional)
    /// </summary>
    public object? ErrorData { get; }

    public Error(string message, Exception? ex = null, object? errorData = null)
    {
        Message = message;
        Exception = ex;
        ErrorData = errorData;
    }
}