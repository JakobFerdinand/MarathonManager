using Core.Models;
using System;

namespace UI.RunnerManagement.Models
{
    internal class ImmutableRunner
    {
        public ImmutableRunner(
            int id
            , int? startnumber
            , Gender gender
            , string firstname
            , string lastname
            , string sportsClub
            , string city
            , string email
            , int yearOfBirth
            , string chipId
            , DateTime? timeAtDestination
            , TimeSpan? runningTime
            , ImmutableCategory category)
        {
            Id = id;
            Startnumber = startnumber;
            Gender = gender;
            Firstname = firstname;
            Lastname = lastname;
            SportsClub = sportsClub;
            City = city;
            Email = email;
            YearOfBirth = yearOfBirth;
            ChipId = chipId;
            TimeAtDestination = timeAtDestination;
            RunningTime = runningTime;
            Category = category;
        }

        public int Id { get; }
        public int? Startnumber { get; }
        public Gender Gender { get; }
        public string Firstname { get; }
        public string Lastname { get; }
        public string SportsClub { get; }
        public string City { get; }
        public string Email { get; }
        public int YearOfBirth { get; }
        public string ChipId { get; }

        public DateTime? TimeAtDestination { get; }
        public TimeSpan? RunningTime { get; }

        public ImmutableCategory Category { get; }
    }

    internal static partial class Extensions
    {
        public static ImmutableRunner ToImmutable(this Runner @this)
            => new ImmutableRunner(
                @this.Id,
                @this.Startnumber,
                @this.Gender,
                @this.Firstname,
                @this.Lastname,
                @this.SportsClub,
                @this.City,
                @this.Email,
                @this.YearOfBirth,
                @this.ChipId,
                @this.TimeAtDestination,
                @this.RunningTime,
                @this.Category.ToImmutable());
    }
}
