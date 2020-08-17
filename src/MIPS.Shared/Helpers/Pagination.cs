namespace MIPS.Shared.Helpers
{
    public static class Pagination
    {
        public static (int index, int offset) GetTwoLevelIndex(int entry, int width)
        {
            int offset = entry % width;
            int index = entry / width;

            return (index, offset);
        }
    }
}
