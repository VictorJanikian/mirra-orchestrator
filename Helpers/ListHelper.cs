namespace Mirra_Orchestrator.Helpers
{
    public static class ListHelper
    {
        public static bool IsNullOrEmpty<T>(this ICollection<T>? collection)
        {
            return collection == null || collection.Count == 0;
        }
    }
}
