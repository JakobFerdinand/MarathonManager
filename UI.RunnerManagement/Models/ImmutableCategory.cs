using Core.Models;
using System;

namespace UI.RunnerManagement.Models
{
    internal class ImmutableCategory
    {
        public ImmutableCategory(
            int id
            , string name
            , DateTime? starttime)
        {
            Name = name;
            Starttime = starttime;
        }

        public int Id { get; }
        public string Name { get; }
        public DateTime? Starttime { get; }
    }

    internal static partial class Extensions
    { 
        public static ImmutableCategory ToImmutable(this Category @this)
            => new ImmutableCategory(
                @this.Id,
                @this.Name,
                @this.Starttime);
    }
}