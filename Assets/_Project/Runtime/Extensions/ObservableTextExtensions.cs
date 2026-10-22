using System;
using R3;
using TMPro;

namespace Project.Runtime.Extensions
{
    /// <summary>
    /// Расширяет наблюдаемые последовательности привязкой значений к текстовым компонентам.
    /// </summary>
    internal static class ObservableTextExtensions
    {
        /// <summary>
        /// Создаёт привязку текста с выбором значения, исключением повторов и необязательным форматированием.
        /// </summary>
        /// <returns>Подписка, которую освобождает владелец привязки.</returns>
        internal static IDisposable BindToText<TSource, TValue>(this Observable<TSource> source,
            TMP_Text target,
            Func<TSource, TValue> selector,
            Func<TValue, string> formatter = null)
        {
            var values = source.Select(selector).DistinctUntilChanged();
            return formatter == null
                ? values.SubscribeToText(target)
                : values.SubscribeToText(target, formatter);
        }
    }
}