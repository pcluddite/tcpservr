// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using Tbasic.Runtime;

namespace Tbasic.Libraries
{
    internal class SystemLibrary : Library
    {
        public SystemLibrary()
        {
            Add("GetMonth", GetMonth);
            Add("GetDay", GetDay);
            Add("GetDayOfWeek", GetDayOfWeek);
            Add("GetYear", GetYear);
            Add("GetHour", GetHour);
            Add("GetMinute", GetMinute);
            Add("GetSecond", GetSecond);
            Add("GetMillisecond", GetMillisecond);
        }

        private object GetMonth(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(1);
            return DateTime.Now.Month;
        }

        private object GetDay(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(1);
            return DateTime.Now.Day;
        }

        private object GetDayOfWeek(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(1);
            return (int)DateTime.Now.DayOfWeek;
        }

        private object GetYear(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(1);
            return DateTime.Now.Year;
        }

        private object GetHour(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(1);
            return DateTime.Now.Hour;
        }

        private object GetMinute(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(1);
            return DateTime.Now.Minute;
        }

        private object GetSecond(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(1);
            return DateTime.Now.Second;
        }

        private object GetMillisecond(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(1);
            return DateTime.Now.Millisecond;
        }
    }
}