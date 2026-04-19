namespace Mirra_Orchestrator.Helpers
{
    public static class CronTimezoneHelper
    {
        public static string ConvertUtcToTimezone(string cronExpression, string timeZoneId)
        {
            if (string.IsNullOrWhiteSpace(timeZoneId))
                throw new ArgumentException("Timezone is required.");

            TimeZoneInfo tz;
            try
            {
                tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
                throw new ArgumentException($"Invalid timezone: '{timeZoneId}'.");
            }

            var offset = tz.BaseUtcOffset;
            if (offset == TimeSpan.Zero) return cronExpression;

            var fields = cronExpression.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (fields.Length != 5)
                throw new ArgumentException("Cron expression must have 5 fields.");

            int utcMinute = int.Parse(fields[0]);
            int offsetTotalMinutes = (int)offset.TotalMinutes;

            var utcHours = ExpandField(fields[1], 0, 23);

            var localHours = new SortedSet<int>();
            int? consistentDayShift = null;
            int localMinute = 0;

            foreach (int utcHour in utcHours)
            {
                localMinute = calculateLocalMinutesAndHours(utcMinute, offsetTotalMinutes, localHours, ref consistentDayShift, utcHour);
            }

            string localHourField = CompressField(localHours.ToList());

            string dayOfWeekField = fields[4];
            string dayOfMonthField = fields[2];

            if (consistentDayShift.HasValue && consistentDayShift.Value != 0)
            {
                dayOfWeekField = calculateLocalDayOfWeek(consistentDayShift, dayOfWeekField);

                dayOfMonthField = calculateLocalDayOfMonth(consistentDayShift, dayOfMonthField);
            }

            return $"{localMinute} {localHourField} {dayOfMonthField} {fields[3]} {dayOfWeekField}";
        }

        private static string calculateLocalDayOfMonth(int? consistentDayShift, string dayOfMonthField)
        {
            if (dayOfMonthField != "*" && dayOfMonthField != "?")
            {
                var utcDays = ExpandField(dayOfMonthField, 1, 31);
                var localDays = new SortedSet<int>();
                foreach (int day in utcDays)
                {
                    int shifted = day + consistentDayShift.Value;
                    if (shifted < 1) shifted += 31;
                    if (shifted > 31) shifted -= 31;
                    localDays.Add(shifted);
                }
                dayOfMonthField = CompressField(localDays.ToList());
            }

            return dayOfMonthField;
        }

        private static string calculateLocalDayOfWeek(int? consistentDayShift, string dayOfWeekField)
        {
            if (dayOfWeekField != "*" && dayOfWeekField != "?")
            {
                var utcDays = ExpandField(dayOfWeekField, 0, 6);
                var localDays = new SortedSet<int>();
                foreach (int day in utcDays)
                {
                    int shifted = (day + consistentDayShift.Value % 7 + 7) % 7;
                    localDays.Add(shifted);
                }
                dayOfWeekField = CompressField(localDays.ToList());
            }

            return dayOfWeekField;
        }

        private static int calculateLocalMinutesAndHours(int utcMinute, int offsetTotalMinutes, SortedSet<int> localHours, ref int? consistentDayShift, int utcHour)
        {
            int totalLocalMinutes = utcHour * 60 + utcMinute + offsetTotalMinutes;

            int dayShift = 0;
            while (totalLocalMinutes < 0) { totalLocalMinutes += 1440; dayShift--; }
            while (totalLocalMinutes >= 1440) { totalLocalMinutes -= 1440; dayShift++; }

            int localMinute = totalLocalMinutes % 60;
            localHours.Add(totalLocalMinutes / 60);

            if (consistentDayShift == null)
                consistentDayShift = dayShift;
            else if (consistentDayShift != dayShift)
                throw new ArgumentException(
                    "The combination of this cron expression and timezone results in a day boundary split that cannot be represented as a single local cron expression. Please simplify the hour field.");
            return localMinute;
        }

        private static SortedSet<int> ExpandField(string field, int min, int max)
        {
            var values = new SortedSet<int>();

            if (field == "*" || field == "?")
            {
                for (int i = min; i <= max; i++)
                    values.Add(i);
                return values;
            }

            foreach (var part in field.Split(','))
            {
                if (part.Contains('/'))
                {
                    var stepParts = part.Split('/');
                    int step = int.Parse(stepParts[1]);
                    int start = stepParts[0] == "*" ? min : int.Parse(stepParts[0]);
                    for (int i = start; i <= max; i += step)
                        values.Add(i);
                }
                else if (part.Contains('-'))
                {
                    var rangeParts = part.Split('-');
                    int start = int.Parse(rangeParts[0]);
                    int end = int.Parse(rangeParts[1]);
                    for (int i = start; i <= end; i++)
                        values.Add(i);
                }
                else if (int.TryParse(part, out int value))
                {
                    values.Add(value);
                }
            }

            return values;
        }

        private static string CompressField(List<int> values)
        {
            if (values.Count == 0) return "*";

            var parts = new List<string>();
            int i = 0;

            while (i < values.Count)
            {
                int start = values[i];
                int end = start;

                while (i + 1 < values.Count && values[i + 1] == end + 1)
                {
                    end = values[++i];
                }

                if (start == end)
                    parts.Add(start.ToString());
                else if (end == start + 1)
                {
                    parts.Add(start.ToString());
                    parts.Add(end.ToString());
                }
                else
                    parts.Add($"{start}-{end}");

                i++;
            }

            return string.Join(",", parts);
        }
    }
}
