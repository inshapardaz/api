// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectMapper.cs" company="Inshapardaz">
//   Muhammad Umer Farooq
// </copyright>
// <summary>
//   Object mapper class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using AutoMapper;

namespace Inshapardaz.Helpers
{
    /// <summary>
    /// Object mapper class
    /// </summary>
    public static class ObjectMapper
    {
        /// <summary>
        /// Maps the specified source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>Mapped object of type TDestination</returns>
        [DebuggerStepThrough]
        public static TDestination Map<TSource, TDestination>(this TSource source) where TSource : class
        {
            return Mapper.Map<TSource, TDestination>(source);
        }

        /// <summary>
        /// Maps the specified source to destination
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>Mapped object of type TDestination</returns>
        [DebuggerStepThrough]
        public static TDestination Map<TSource, TDestination>(this TSource source, TDestination target) where TSource : class
        {
            return Mapper.Map(source, target);
        }
    }
}