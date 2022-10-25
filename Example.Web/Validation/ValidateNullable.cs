using System.Runtime.CompilerServices;

namespace Example.Web.Validation;

public static class ValidateNullable
{
    public class ValidationFailedException : Exception
    {
        public ValidationFailedException(IEnumerable<string> fieldNames)
            : base($"Filed {string.Join(",", fieldNames)} required but it is empty")
        {
        }
    }

    public static T GetOrThrow<T>(Func<Context, T> factory)
    {
        var context = new Context();
        var res = factory(context);
        context.ThrowIfHasErrors();
        return res;
    }

    public class Context
    {
        private readonly List<string> _names = new();

        public T Get<T>(T? t, [CallerArgumentExpression("t")] string name = "") where T : struct
        {
            if (t != null)
            {
                return t.Value;
            }

            _names.Add(name);
            return new T();
        }

        public string Get(string? t, [CallerArgumentExpression("t")] string name = "")
        {
            if (t != null)
            {
                return t;
            }

            _names.Add(name);
            return string.Empty;
        }

        public string GetNotEmpty(string? t, [CallerArgumentExpression("t")] string name = "")
        {
            if (!string.IsNullOrEmpty(t))
            {
                return t;
            }

            _names.Add(name);
            return string.Empty;
        }

        public TOut GetEnum<T, TOut>(
            T? t,
            [CallerArgumentExpression("t")] string name = "")
            where T : struct, Enum
            where TOut : struct, Enum
        {
            if (t.HasValue)
            {
                var v = t.Value;
                var res = Unsafe.As<T, TOut>(ref v);
                if (Enum.IsDefined(res))
                {
                    return res;
                }
            }

            _names.Add(name);
            return default;
        }

        public TOut Get<T, TOut>(
            T? t,
            Func<T, Context, TOut> map,
            [CallerArgumentExpression("t")] string name = "") where TOut : notnull
        {
            if (t != null)
            {
                var context = new Context();
                var result = Execute(
                    map,
                    t,
                    context,
                    name
                );
                if (context._names.Count != 0)
                {
                    foreach (var innerName in context._names)
                    {
                        _names.Add($"{name}.{innerName}");
                    }
                }

                return result;
            }

            _names.Add(name);
            return default!;
        }

        public TOut Get<TOut>(
            string? t,
            Func<string, Context, TOut> map,
            [CallerArgumentExpression("t")] string name = "") where TOut : notnull
        {
            if (!string.IsNullOrEmpty(t))
            {
                var context = new Context();
                var result = Execute(
                    map,
                    t,
                    context,
                    name
                );
                if (context._names.Count != 0)
                {
                    foreach (var innerName in context._names)
                    {
                        _names.Add($"{name}.{innerName}");
                    }
                }

                return result;
            }

            _names.Add(name);
            return default!;
        }

        private static TOut Execute<TOut, T>(
            Func<T, Context, TOut> map,
            T t,
            Context context,
            string name)
        {
            try
            {
                return map(t, context);
            }
            catch (Exception)
            {
                context._names.Add(name);
                return default!;
            }
        }

        public TOut? GetOrDefault<T, TOut>(
            T? t,
            Func<T, Context, TOut> map,
            [CallerArgumentExpression("t")] string name = "") where TOut : class
        {
            if (t != null)
            {
                var context = new Context();
                var result = Execute(
                    map,
                    t,
                    context,
                    name
                );
                if (context._names.Count != 0)
                {
                    foreach (var innerName in context._names)
                    {
                        _names.Add($"{name}.{innerName}");
                    }
                }

                return result;
            }

            return null;
        }

        public TOut[] GetAndMapOrDefault<T, TOut>(
            IEnumerable<T>? t,
            Func<T, Context, TOut> map,
            [CallerArgumentExpression("t")] string name = "") where TOut : class
        {
            if (t != null)
            {
                var result = new List<TOut>();
                foreach (var (item, index) in t.Select((item, index) => (item, index)))
                {
                    var context = new Context();
                    var outItem = Execute(
                        map,
                        item,
                        context,
                        name
                    );
                    if (context._names.Count != 0)
                    {
                        foreach (var innerName in context._names)
                        {
                            _names.Add($"{name}[{index}].{innerName}");
                        }
                    }
                    else
                    {
                        result.Add(outItem);
                    }
                }

                return result.ToArray();
            }

            return Array.Empty<TOut>();
        }

        public void ThrowIfHasErrors()
        {
            if (_names.Count != 0)
            {
                throw new ValidationFailedException(_names);
            }
        }
    }
}